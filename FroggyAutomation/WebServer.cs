#region License
// Copyright (c) 2012, David Bennett. All rights reserved.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
// MA 02110-1301  USA
#endregion
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace FroggyAutomation
{
    /// <summary>
    /// The web server basic class.
    /// </summary>
    public class WebServer
    {
        public event Action<HttpListenerContext, string> ProcessRequest;

        private static readonly ILog log = LogManager.GetLogger(typeof(WebServer).Name);
        private readonly HttpListener listener;
        private readonly Thread listenerThread;
        private readonly Thread[] workers;
        private readonly ManualResetEvent stop;
        private readonly ManualResetEvent ready;
        private readonly string basePath;
        private readonly Queue<HttpListenerContext> queue;

        /// <summary>
        /// Creates a new web server with the specific number of threads.
        /// </summary>
        /// <param name="maxThreads">The maximum number of threads</param>
        public WebServer(int maxThreads, string basePath)
        {
            this.basePath = basePath;
            workers = new Thread[maxThreads];
            queue = new Queue<HttpListenerContext>();
            stop = new ManualResetEvent(false);
            ready = new ManualResetEvent(false);
            listener = new HttpListener();
            listenerThread = new Thread(HandleRequests);
        }

        /// <summary>
        /// Starts the server with the specific bind prefixes
        /// </summary>
        /// <param name="bindprefixes">The prefixes to bind</param>
        public void Start(string bindprefix, int webSocketPort)
        {
            log.InfoFormat("Starting web server with {0}", bindprefix);
            listener.Prefixes.Add(bindprefix);
            listener.Start();
            listenerThread.Start();

            for (int i = 0; i < workers.Length; i++)
            {
                workers[i] = new Thread(Worker);
                workers[i].Start();
            }
        }

        /// <summary>
        /// Disposes the server and gets rid of the http listener.
        /// </summary>
        public void Dispose()
        {
            Stop();
            queue.Clear();
            ready.Dispose();
        }

        /// <summary>
        /// Stops the server from running.
        /// </summary>
        public void Stop()
        {
            stop.Set();
            listenerThread.Join();
            foreach (Thread worker in workers)
            {
                worker.Join();
            }
            listener.Stop();
            log.InfoFormat("Stoping web server");
        }

        /// <summary>
        /// Handles a request from the system.
        /// </summary>
        private void HandleRequests()
        {
            while (listener.IsListening)
            {
                var context = listener.BeginGetContext(ContextReady, null);

                if (0 == WaitHandle.WaitAny(new[] { stop, context.AsyncWaitHandle }))
                    return;
            }
        }

        /// <summary>
        /// Called when there is some data to read on one of the listeners.
        /// </summary>
        /// <param name="ar">The results from the system</param>
        private void ContextReady(IAsyncResult ar)
        {
            try
            {
                lock (queue)
                {
                    queue.Enqueue(listener.EndGetContext(ar));
                    ready.Set();
                }
            }
            catch { return; }
        }

        /// <summary>
        /// Worker thread to spool up and deal with the data.
        /// </summary>
        private void Worker()
        {
            WaitHandle[] wait = new[] { ready, stop };
            while (0 == WaitHandle.WaitAny(wait))
            {
                HttpListenerContext context;
                lock (queue)
                {
                    if (queue.Count > 0)
                        context = queue.Dequeue();
                    else
                    {
                        ready.Reset();
                        continue;
                    }
                }
                // Handle the request and do something exciting!
                try
                {
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;
                    // Find the path from the request
                    string url = request.Url.LocalPath.TrimStart('/');
                    try
                    {
                        string file = Path.Combine(basePath, url);
                        log.DebugFormat("Found file {0} from {1}", file, url);
                        if (File.Exists(file))
                        {
                            switch (Path.GetExtension(file))
                            {
                                case ".html":
                                    response.ContentType = System.Net.Mime.MediaTypeNames.Text.Html;
                                    break;
                                case ".css":
                                case ".js":
                                    response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
                                    break;
                                case ".png":
                                    response.ContentType = "image/png";
                                    break;
                                case ".jpg":
                                    response.ContentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                                    break;
                            }
                            FileStream input = File.OpenRead(file);
                            input.CopyTo(response.OutputStream);
                            response.StatusCode = 200;
                            response.Close();
                        }
                        else
                        {
                            response.StatusCode = 404;
                        }
                    }
                    catch (Exception eh)
                    {
                        log.ErrorFormat("Exception handling response {0} {1} {2}", request.Url.PathAndQuery, eh.Message, eh.StackTrace);
                    }

                    response.Close();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Exception handling response {0} {1}", ex.Message, ex.StackTrace);
                }
            }
        }
    }
}

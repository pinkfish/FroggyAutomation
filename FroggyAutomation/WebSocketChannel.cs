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
using Alchemy;
using Alchemy.Classes;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FroggyAutomation
{
    /// <summary>
    /// A web socket channel to talk to the frontend.
    /// </summary>
    public class WebSocketChannel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebSocketChannel).Name);

        public delegate void WebCallback(UserContext context, object data);

        private readonly WebSocketServer server;
        private readonly ConcurrentDictionary<Type, WebCallback> callbacks;

        /// <summary>
        /// Creates a web socket server on the specific port.
        /// </summary>
        /// <param name="port">The port to wait on</param>
        public WebSocketChannel(int port)
        {
            server = new WebSocketServer(port, IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnect = OnConnect,
                OnConnected = OnConnected,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)
            };
            server.Start();
        }

        public void Send(object data)
        {
        }

        /// <summary>
        /// Adds a callback for the specified type.
        /// </summary>
        /// <param name="type">The type to callback on when deserialized</param>
        /// <param name="callback">The callback to call</param>
        public void AddCallback(Type type, WebCallback callback)
        {
            callbacks[type] = callback;
        }

        private void OnDisconnect(Alchemy.Classes.UserContext context)
        {
            log.InfoFormat("Disconnected {0}", context.ClientAddress.ToString());
        }

        private void OnConnected(Alchemy.Classes.UserContext context)
        {
            log.InfoFormat("Connected {0}", context.ClientAddress.ToString());
        }

        private void OnConnect(Alchemy.Classes.UserContext context)
        {
            log.InfoFormat("Connect {0}", context.ClientAddress.ToString());
        }

        /// <summary>
        /// Sending a message to the client.
        /// </summary>
        /// <param name="context"></param>
        private void OnSend(Alchemy.Classes.UserContext context)
        {
            log.InfoFormat("Send {0}", context.ClientAddress.ToString());
        }

        /// <summary>
        /// Receive a message from the system and do something useful with it.
        /// </summary>
        /// <param name="context"></param>
        private void OnReceive(Alchemy.Classes.UserContext context)
        {
            try
            {
                string json = context.DataFrame.ToString();
                object obj = JsonConvert.DeserializeObject(json);
                if (callbacks.ContainsKey(obj.GetType()))
                {
                    CallContext callContext = new CallContext { Context = context, Data = obj, Callback = callbacks[obj.GetType()] };
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.QueueCallback), callContext);
                }
            }
            catch (Exception e)
            {
                ErrorResponse errorResponse = new ErrorResponse
                {
                    OriginalData = new { e.Message }
                };
                context.Send(JsonConvert.SerializeObject(errorResponse));
            }
        }
        
        /// <summary>
        /// The queue callback.
        /// </summary>
        /// <param name="data">Data to use in the callback</param>
        private void QueueCallback(object data)
        {
            CallContext context = (CallContext)data;
            context.Callback(context.Context, context.Data);
        }

        /// <summary>
        /// Defines the response object to send back to the client
        /// </summary>
        public class ErrorResponse
        {
            public dynamic OriginalData { get; set; }
        }

        internal class CallContext
        {
            internal UserContext Context { get; set; }
            internal WebCallback Callback { get; set; }
            internal object Data { get;  set; }
        }
    }
}

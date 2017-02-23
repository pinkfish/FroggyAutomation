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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FroggyAutomation.Database;
using System.IO;
using log4net;

namespace FroggyAutomation
{
    /// <summary>
    ///  Basic class that starts the whole system up.
    /// </summary>
    public class FroggyAutomation
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebServer).Name);
        private FileDatabase database;
        private Plugins plugins;
        private WebServer server;

        static void Main(string[] args)
        {
            FroggyAutomation froggy = new FroggyAutomation();
            froggy.Start();
            return;
        }

        public void Start()
        {
            log.DebugFormat("Starting up the log system.");
            // Load the plugins first, then then database.
            plugins = new Plugins(BuildString(Properties.Settings.Default.PluginDirectory));
            // Startup the database.
            database = new FileDatabase(BuildString(Properties.Settings.Default.SerializedData));
            // Startup the web server.
            server = new WebServer(10, BuildString(Properties.Settings.Default.WebData));
            String prefix = String.Format("http://+:{0}/", Properties.Settings.Default.WebPort);
            server.Start(prefix, Properties.Settings.Default.WebPort);
            System.Threading.Thread.Sleep(200000);
        }

        private string BuildString(string input)
        {
            StringBuilder builder = new StringBuilder(input);
            foreach (Environment.SpecialFolder folder in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                builder.Replace("{" + folder.ToString() + "}", Environment.GetFolderPath(folder));
            }
            builder.Replace("{ApplicationDirectory}", Environment.CurrentDirectory);
            return builder.ToString();
        }
    }
}

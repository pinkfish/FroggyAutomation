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
using FroggyAutomation.Database;
using FroggyPlugin;
using FroggyPlugin.Data;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly:AddinRoot("FroggyAutomation", "1.0")]
namespace FroggyAutomation
{
    /// <summary>
    /// The automation system.
    /// </summary>
    public class Plugins
    {
        private Plugin[] plugins;
        private PluginInterface system;

        /// <summary>
        /// Setup the automation system.
        /// </summary>
        /// <param name="directory">The directory the plugins are in</param>
        public Plugins(string directory)
        {
            AddinManager.Initialize(directory);
            AddinManager.Registry.Update();
            plugins = AddinManager.GetExtensionObjects<Plugin>();
        }

        /// <summary>
        /// Initialize all the plugsin with the data loaded from the database.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="config"></param>
        /// <param name="devices"></param>
        public void InitializePlugins(Database.Database db, Configuration config, IList<BasicDevice> devices)
        {
            this.system = new PluginInterface(new PluginDeviceInterface(db));

            foreach (Plugin plugin in plugins)
            {
                // If we have config, then track it.\
                PluginConfig pluginConfig = db.GetPluginConfig(plugin.GetType());
                if (config != null)
                {
                    plugin.Configuration = pluginConfig;
                }
                plugin.Startup(system);
            }
        }
    }
}

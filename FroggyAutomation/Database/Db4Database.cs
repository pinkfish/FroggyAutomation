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
using Db4objects.Db4o;
using FroggyPlugin.Data;
using FroggyPlugin;

namespace FroggyAutomation.Database
{
    internal class Db4Database : Database
    {
        private readonly IObjectContainer db;

        /// <summary>
        /// Creates a new database.
        /// </summary>
        /// <param name="filename">The filename to use for the database</param>
        public Db4Database(string filename)
        {
            db = Db4oFactory.OpenFile(filename);
        }

        /// <summary>
        /// The system configuration.
        /// </summary>
        public Configuration Config
        {
            get
            {
                IList<Configuration> set = db.Query<Configuration>(typeof(Configuration));
                if (set.Count > 0)
                {
                    return set.First();
                }
                return new Configuration();
            }
            set
            {
                IList<Configuration> set = db.Query<Configuration>(typeof(Configuration));
                if (set.Count > 0)
                {
                    Configuration update = set.First();
                    update.Merge(value);
                    db.Store(update);
                }
                else
                {
                    db.Store(value);
                }
            }
        }

        /// <summary>
        /// All the known devices.
        /// </summary>
        public IEnumerable<BasicDevice> Devices
        {
            get
            {
                return db.Query<BasicDevice>(typeof(BasicDevice));
            }
        }

        /// <summary>
        /// Adds the device into the system.
        /// </summary>
        /// <param name="device">The device to add in</param>
        public void AddOrUpdateDevice(BasicDevice device)
        {
            IList<BasicDevice> items = db.Query<BasicDevice>(delegate(BasicDevice input) { return input.Address.Equals(device.Address); });
            if (items.Count > 0)
            {
                BasicDevice update = items.First();
                update.Merge(device);
                db.Store(update);
            }
            else
            {
                db.Store(device);
            }
        }

        /// <summary>
        /// Finds the device in system.
        /// </summary>
        /// <param name="device">The device to find</param>
        public BasicDevice FindDevice(DeviceReference reference)
        {
            IList<BasicDevice> items = db.Query<BasicDevice>(delegate(BasicDevice input) { return input.Address.Equals(reference); });
            if (items.Count > 0)
            {
                return items.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Find the config for the specific type of plugin.
        /// </summary>
        /// <param name="type">Type of the plugin</param>
        /// <returns>The config object</returns>
        public PluginConfig GetPluginConfig(Type type)
        {
            return null;
        }

        /// <summary>
        /// Stores the plugin config into the database for this specific plugin.
        /// </summary>
        /// <param name="cofig">The config o use</param>
        /// <param name="type">Type of the data</param>
        public void SetPluginConfig(PluginConfig cofig, Type type)
        {
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            db.Dispose();
        }
    }
}

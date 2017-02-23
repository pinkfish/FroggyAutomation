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
using FroggyPlugin;
using FroggyPlugin.Data;
using FroggyPlugin.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FroggyAutomation
{
    /// <summary>
    /// The class to deal with all the plugins and updates into the main system.
    /// </summary>
    internal class PluginDeviceInterface : DevicesSystem
    {
        private Database.Database db;

        /// <summary>
        /// Create a new interface for the devices to the plugin.
        /// </summary>
        /// <param name="db">The database to use for data</param>
        public PluginDeviceInterface(Database.Database db)
        {
            this.db = db;
        }

        public BasicDevice FindDevice(FroggyPlugin.Data.DeviceReference reference)
        {
            return db.FindDevice(reference);
        }

        public IEnumerable<BasicDevice> Devices
        {
            get { return db.Devices; }
        }

        public void AddDevice(BasicDevice device)
        {
            db.AddOrUpdateDevice(device);
        }

        public void UpdateDevice(DeviceUpdate update)
        {
            throw new NotImplementedException();
        }
    }
}

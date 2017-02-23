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
using System.Threading.Tasks;

namespace FroggyPlugin.Data
{
    /// <summary>
    /// How to reference devices in the system.
    /// </summary>
    [Serializable]
    public class DeviceReference
    {
        /// <summary>
        /// The new device reference.
        /// </summary>
        /// <param name="address">Address of the device</param>
        /// <param name="pluginName">Plugin the device is associated with</param>
        public DeviceReference(string address, string pluginName)
        {
            this.Address = address;
            this.PluginName = pluginName;
        }
        /// <summary>
        /// The id of the device to lookup.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// The name of the plugin this is associated with.
        /// </summary>
        public string PluginName { get; set; }
    }
}

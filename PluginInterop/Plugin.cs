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
using FroggyPlugin.Events;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FroggyPlugin
{
    /// <summary>
    /// The plugin interface for froggy automation.  This is what you implement to be a plugin.
    /// </summary>
    [TypeExtensionPoint]
    public interface Plugin : IDisposable
    {
        /// <summary>
        /// The configuration for this plugin.  It will set set before the start method is called.
        /// THis needs to be serializable since it will be saved into a database.
        /// </summary>
        PluginConfig Configuration { get; set; }

        /// <summary>
        /// Starts up the plugin with the specific Froggy Automation specific settings.
        /// </summary>
        /// <param name="callbacks">The callbacks to use</param>
        /// <returns>true if we successful started up</returns>
        bool Startup(FroggySystem callbacks);

        /// <summary>
        /// The device update. This is called if this plugin owns a device.
        /// </summary>
        /// <param name="update">The device to update with the type of update</param>
        void UpdateDevice(DeviceUpdate update);
    }
}

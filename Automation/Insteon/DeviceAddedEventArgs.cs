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
using Automation.Insteon.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation.Insteon
{
    /// <summary>
    /// Used when a device changes in the system.
    /// </summary>
    public class DeviceAddedEventArgs : EventArgs
    {
        private DeviceBase device;

        /// <summary>
        /// The event args to use.
        /// </summary>
        /// <param name="device">the changed device</param>
        public DeviceAddedEventArgs(DeviceBase device)
        {
            this.device = device;
        }

        /// <summary>
        /// The device that changed.
        /// </summary>
        public DeviceBase Device
        {
            get
            {
                return device;
            }
        }
    }
}

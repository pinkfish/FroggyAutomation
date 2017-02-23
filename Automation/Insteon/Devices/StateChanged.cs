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

namespace Automation.Insteon.Devices
{
    /// <summary>
    /// Class that is used to track a state change.
    /// </summary>
    public class StateChanged
    {
        /// <summary>
        /// The specific piece of state that changed.
        /// </summary>
        public enum StateThatChanged
        {
            TurnedOn,
            TurnedOff,
            Dim,
            Bright,
            CoolingMode,
            HeatingMode,
            HighHumidity,
            LowHumidity
        }
        private readonly DeviceBase device;
        private readonly int group;
        private readonly StateThatChanged change;

        /// <summary>
        /// Creates a new state change object.
        /// </summary>
        /// <param name="device">The device that changed</param>
        /// <param name="group">The group it changed in</param>
        /// <param name="change">The type of change</param>
        public StateChanged(DeviceBase device, int group, StateThatChanged change)
        {
            this.device = device;
            this.group = group;
            this.change = change;
        }

        /// <summary>
        /// The device that changed.
        /// </summary>
        public DeviceBase Device { get { return device; } }
        /// <summary>
        /// The group on the device that changed.
        /// </summary>
        public int Group { get { return group; } }
        /// <summary>
        /// The state on the device that changed.
        /// </summary>
        public StateThatChanged Change { get { return change; } }
    }
}

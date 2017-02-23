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
using FroggyPlugin.Data;

namespace FroggyPlugin.Devices
{
    /// <summary>
    /// A device that has a level from 0-100, has a percentage value.  This would
    /// cover dimmers, and anything else that has a percentage level as it's basic
    /// control.
    /// </summary>
    public class PercentLevelDevice : BasicDevice
    {
        enum StateIndexes
        {
            Percent
        }

        /// <summary>
        /// Create a new percentage level device.
        /// </summary>
        public PercentLevelDevice()
        {
            SetState(StateIndexes.Percent.ToString(), new DeviceState<int>(DeviceStateBase.DeviceStateType.PercentageRange, "Percent", "Basic"));
        }

        /// <summary>
        /// The on level of the device.
        /// </summary>
        public int Level
        {
            get
            {
                DeviceState<int> state = (DeviceState<int>)GetState(StateIndexes.Percent.ToString());
                return state.Value;
            }
            set
            {
                DeviceState<int> state = (DeviceState<int>)GetState(StateIndexes.Percent.ToString());
                state.Value = value;
            }
        }
    }
}

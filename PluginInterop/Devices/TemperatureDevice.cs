﻿#region License
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
    /// The basis of a device that deals with temperature.  Thermostats are based off this
    /// as would things like a hookup to weather underground.
    /// </summary>
    public class TemperatureDevice : BasicDevice
    {
        enum StateIndexes
        {
            Temperature
        }

        /// <summary>
        /// Create a new percentage level device.
        /// </summary>
        public TemperatureDevice()
        {
            SetState(StateIndexes.Temperature.ToString(), new DeviceState<int>(DeviceStateBase.DeviceStateType.Temperature, "Percent", "Basic"));
        }

        /// <summary>
        /// The temperature the device.
        /// </summary>
        public int Level
        {
            get
            {
                DeviceState<int> state = (DeviceState<int>)GetState(StateIndexes.Temperature.ToString());
                return state.Value;
            }
            set
            {
                DeviceState<int> state = (DeviceState<int>)GetState(StateIndexes.Temperature.ToString());
                state.Value = value;
            }
        }
    }
}

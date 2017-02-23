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
    /// Basic thermostat device to use in the system.
    /// </summary>
    public class Thermostat : TemperatureDevice
    {
        /// <summary>
        /// Basic modes for all thermostats.  Well, ones we worry about for automation anyway.
        /// </summary>
        public enum ThermostatMode {
            Auto,
            Heat,
            Cool,
            Off,
            Program // Follow it's build in program.
        }

       
        enum StateIndexes
        {
            Heat = 0,
            Cool = 1,
            Mode = 2,
            Temperature = 3,
            Humidity = 4,
            FanOn = 5
        }

        /// <summary>
        /// Create a new thermostat.
        /// </summary>
        /// <param name="numExtraStates"></param>
        public Thermostat()
            : base()
        {
            // Setup the basic states.
            SetState(StateIndexes.Heat.ToString(), new DeviceState<int>(DeviceStateBase.DeviceStateType.Temperature, "Heat", "Basic"));
            SetState(StateIndexes.Cool.ToString(), new DeviceState<int>(DeviceStateBase.DeviceStateType.Temperature, "Cool", "Basic"));
            SetState(StateIndexes.Humidity.ToString(), new DeviceState<int>(DeviceStateBase.DeviceStateType.PercentageRange, "Humidity", "Basic"));
            SetState(StateIndexes.FanOn.ToString(), new DeviceState<bool>(DeviceStateBase.DeviceStateType.Boolean, "Fan On", "Basic"));
            SetState(StateIndexes.Mode.ToString(), new DeviceState<ThermostatMode>(DeviceState<ThermostatMode>.DeviceStateType.Enum, "Mode", "Basic"));
        }

        /// <summary>
        /// The current temperature as a limit in the heat mode.
        /// When heat is turned on.
        /// </summary>
        public int Heat
        {
            get
            {
                DeviceState<int> state = (DeviceState<int>)GetState(StateIndexes.Heat.ToString());
                return state.Value;
            }
            set
            {
                DeviceState<int> state = (DeviceState<int>)GetState(StateIndexes.Heat.ToString());
                state.Value = value;
            }
        }
        /// <summary>
        /// The current temperate s a limit in the cool mode, when the
        /// cooling is turned on.
        /// </summary>
        public int Cool 
        {
            get
            {
                DeviceState<int> state = (DeviceState<int>)GetState(StateIndexes.Cool.ToString());
                return state.Value;
            }
            set
            {
                DeviceState<int> state = (DeviceState<int>)GetState(StateIndexes.Cool.ToString());
                state.Value = value;
            }
        }
        /// <summary>
        /// The mode of the thermostat.
        /// </summary>
        public ThermostatMode Mode
        {
            get
            {
                DeviceState<ThermostatMode> state = (DeviceState<ThermostatMode>)GetState(StateIndexes.Mode.ToString());
                return state.Value;
            }
            set
            {
                DeviceState<ThermostatMode> state = (DeviceState<ThermostatMode>)GetState(StateIndexes.Mode.ToString());
                state.Value = value;
            }
        }

        /// <summary>
        /// Current humidity.
        /// </summary>
        public int Humidity
        {
            get
            {
                DeviceState<int> state = (DeviceState<int>)GetState(StateIndexes.Humidity.ToString());
                return state.Value;
            }
            set
            {
                DeviceState<int> state = (DeviceState<int>)GetState(StateIndexes.Humidity.ToString());
                state.Value = value;
            }
        }

        /// <summary>
        /// If the fan is currently turned on.
        /// </summary>
        public bool FanOn
        {
            get
            {
                DeviceState<bool> state = (DeviceState<bool>)GetState(StateIndexes.FanOn.ToString());
                return state.Value;
            }
            set
            {
                DeviceState<bool> state = (DeviceState<bool>)GetState(StateIndexes.FanOn.ToString());
                state.Value = value;
            }
        }

    }
}

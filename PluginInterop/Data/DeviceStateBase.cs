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

namespace FroggyPlugin.Data
{
    /// <summary>
    /// The basic device state to deal with in the devices.
    /// </summary>
    [Serializable]
    public abstract class DeviceStateBase
    {
        /// <summary>
        /// The type of the state.
        /// </summary>
        public enum DeviceStateType
        {
            OnOff,
            PercentageRange,
            Temperature,
            Code,
            Boolean,
            Enum
        }

        /// Create a new device state with the specified typed.
        /// </summary>
        /// <param name="state">The start device state</param>
        /// <param name="name">The name of the state, this is used for display purposes</param>
        /// <param name="group">The group the state is in, this is used for display purposes</param>
        public DeviceStateBase(DeviceStateType state, string name, string group)
        {
            this.StateType = state;
            this.Name = name;
        }

        /// <summary>
        /// The type of date in this specific piece of state.
        /// </summary>
        public DeviceStateType StateType { get; set; }

        /// <summary>
        /// Name of the state to show in the ux.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The group the state fits into, for example there should be a specific group for the plugin.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Anything the plugin needs to store to connect this state to it's devices.
        /// </summary>
        public object AutomationData { get; set; }

        /// <summary>
        /// The data about the value that changed.
        /// </summary>
        /// <param name="sender">Which state changed</param>
        /// <param name="args">The details of the change</param>
        public delegate void ValueChangedHandler(object sender, DeviceStateValueChangedEventArgs args);

        /// <summary>
        /// Triggered when the value changes.
        /// </summary>
        public event ValueChangedHandler ValueChanged;

        protected void FireValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, new DeviceStateValueChangedEventArgs(this));
            }
        }
    }
}

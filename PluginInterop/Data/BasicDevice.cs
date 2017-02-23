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
    /// The basic device for all cross-interface stuff.
    /// </summary>
    [Serializable]
    public class BasicDevice
    {
        private DeviceReference reference;
        private object automationData;
        private Dictionary<string, DeviceStateBase> states = new Dictionary<string, DeviceStateBase>();

        /// <summary>
        /// Create a new device with the specified number of states associated with it.
        /// </summary>
        /// <param name="numOfStates"></param>
        public BasicDevice()
        {
        }

        /// <summary>
        /// The address of this device.
        /// </summary>
        public DeviceReference Address { get { return reference; } set { reference = value; } }

        /// <summary>
        /// Name of the device.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The on levels array.
        /// </summary>
        public Dictionary<string, DeviceStateBase>.ValueCollection States { get { return states.Values; } }

        /// <summary>
        /// Set the data for this group on this device.  For lights this is the on level, for
        /// thermostats this is the temperature, etc.
        /// </summary>
        /// <param name="button">The group to change the level for</param>
        /// <param name="onLevel">The new data</param>
        public void SetState(string stateId, DeviceStateBase onLevel)
        {
            // Update the events.
            if (states.ContainsKey(stateId))
            {
                states[stateId].ValueChanged -= BasicDevice_ValueChanged;
            }
            states[stateId] = onLevel;
            states[stateId].ValueChanged += BasicDevice_ValueChanged;
        }

        /// <summary>
        /// The state ids present in this device.
        /// </summary>
        /// <returns>The list of state ids</returns>
        public Dictionary<string, DeviceStateBase>.KeyCollection GetStateIds()
        {
            return states.Keys;
        }


        private void BasicDevice_ValueChanged(object sender, DeviceStateValueChangedEventArgs args)
        {
            // Rethrow the evenbt from this device.
            if (ValueChanged != null)
            {
                args.Device = this;
                ValueChanged(sender, args);
            }
        }

        /// <summary>
        /// Get the data associated with the specific group/button.
        /// </summary>
        /// <param name="group">The group to get the data for</param>
        /// <returns>The data associated with the group</returns>
        public DeviceStateBase GetState(string stateId)
        {
            return states[stateId];
        }

        /// <summary>
        /// Automation specific data, this is parsed by the various systems differently.  This cn be used to store
        /// system specific data like the brightness of leds, or the load sensing state of the items.
        /// </summary>
        public object AutomationData { get { return automationData; } set { automationData = value; } }

        /// <summary>
        /// Merges in the new device to this one.
        /// </summary>
        /// <param name="device"></param>
        public void Merge(BasicDevice device)
        {
            this.states = device.states;
            this.AutomationData = device.AutomationData;
            this.Name = device.Name;
            this.Address = device.Address;
        }

        /// </summary>
        /// <param name="sender">Which state changed</param>
        /// <param name="args">The details of the change</param>
        public delegate void ValueChangedHandler(object sender, DeviceStateValueChangedEventArgs args);

        /// <summary>
        /// Triggered when the value changes.
        /// </summary>
        public event ValueChangedHandler ValueChanged;
    }
}

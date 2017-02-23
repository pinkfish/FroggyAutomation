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
    /// The state of the device, this is only the value part of the state.
    /// </summary>
    public class DeviceState<T> : DeviceStateBase where T : IComparable
    {
        T stateValue;

        /// Create a new device state with the specified type.
        /// </summary>
        /// <param name="state">The start device state</param>
        public DeviceState(DeviceStateType state, string name, string group)
            : base(state, name, group)
        {
        }

        /// <summary>
        /// The current value.
        /// </summary>
        public T Value
        {
            get
            {
                return this.stateValue;
            }
            set
            {
                if (stateValue == null && value == null)
                {
                    // no changed.
                    return;
                }
                if (stateValue == null)
                {
                    stateValue = value;
                    FireValueChanged();
                }
                else if (stateValue.CompareTo(value) != 0)
                {
                    stateValue = value;
                    FireValueChanged();
                }
            }
        }
    }
}

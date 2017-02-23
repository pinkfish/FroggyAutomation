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
using Automation.Insteon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Insteon.Devices
{
    /// <summary>
    /// A dimmable light with all the dimmable stuff on it.
    /// </summary>
    public class DimmingLight : OnOffDevice
    {
        public DimmingLight(InsteonCommunication coms, DeviceId id, byte category, byte subcategory) : base(coms, id, category, subcategory)
        {
        }

        /// <summary>
        /// Turns the light on and sets the ramp level at the same time.
        /// </summary>
        /// <param name="onLevel">0-255 is the range</param>
        /// <param name="ramp">0-255 is the range</param>
        public Messages.PowerLineModemMessage.MessageResponse SetLightOnWithRamp(int onLevel, int ramp)
        {
            ramp = (ramp - 1) / 2;
            onLevel = onLevel / 16;
            byte cmd2 = (byte)((onLevel << 4) | (ramp & 0xf));
            return SendStandardCommand(InsteonPacket.Command.LightOnWithRamp, cmd2);
        }

        /// <summary>
        /// Turns the light off and sets the ramp level at the same time.
        /// </summary>
        /// <param name="ramp">0-255 is the range</param>
        public Messages.PowerLineModemMessage.MessageResponse SetLightOffWithRamp(int ramp)
        {
            ramp = (ramp - 1) / 2;
            byte cmd2 = (byte)(ramp & 0xf);
            return SendStandardCommand(InsteonPacket.Command.LightOffWithRamp, cmd2);
        }
    }
}

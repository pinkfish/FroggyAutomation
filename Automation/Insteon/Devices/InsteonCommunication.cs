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
using Automation.Insteon.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Insteon.Devices
{
    /// <summary>
    /// Communicates with the insteon world.
    /// </summary>
    public interface InsteonCommunication
    {
        /// <summary>
        /// Sends a direct packet to an insteon endpoint, with a response.
        /// </summary>
        /// <param name="packet">packet to send</param>
        /// <param name="response">response packet</param>
        /// <returns>If the send was successful</returns>
        PowerLineModemMessage.MessageResponse SendDirectInsteonPacket(InsteonPacket packet, out InsteonPacket response);
    }
}

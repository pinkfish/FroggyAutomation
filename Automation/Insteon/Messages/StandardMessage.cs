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

namespace Automation.Insteon.Messages
{
    /// <summary>
    /// The standard message, insteon packet plus the basic message byte.
    /// </summary>
    public class StandardMessage : PowerLineModemMessage
    {
        /// <summary>
        /// Create a new standard message from the insteon input packet.
        /// </summary>
        /// <param name="message">The message id to create</param>
        /// <param name="packet">the insteon packet to create it with</param>
        public StandardMessage(Message message, InsteonPacket packet)
            : base(message)
        {
            Packet = packet;
        }

        /// <summary>
        /// Creates a new standard message from the specified data.
        /// </summary>
        /// <param name="message">The message id to create</param>
        /// <param name="data">The data to create it with</param>
        public StandardMessage(Message message, byte[] data) : base(message)
        {
            Packet = new InsteonPacket(data);
        }

        /// <summary>
        /// A nice new packet to process.
        /// </summary>
        internal InsteonPacket Packet { get; set; }

        /// <summary>
        /// How big is the response, this is used for processing.
        /// </summary>
        public static int ResponseSize
        {
            get
            {
                return 7;
            }
        }

        public override byte[] PayloadByteArray()
        {
            return Packet.ToByteArray();
        }
    }
}

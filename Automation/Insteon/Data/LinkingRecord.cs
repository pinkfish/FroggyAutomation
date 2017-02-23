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

namespace Automation.Insteon.Data
{
    /// <summary>
    ///  The linking record returned from the modem.
    /// </summary>
    public class LinkingRecord
    {
        private byte group;
        private byte flags;
        private DeviceId id;
        private byte linkData1;
        private byte linkData2;
        private byte linkData3;

        /// <summary>
        /// Creates the record based onthe data from the modem.
        /// </summary>
        /// <param name="data">the data from the modem</param>
        public LinkingRecord(byte[] data)
        {
            flags = data[0];
            group = data[1];
            id = new DeviceId(data[2], data[3], data[4]);
            linkData1 = data[5];
            linkData2 = data[6];
            linkData3 = data[7];
        }

        public byte Group { get { return group; } }
        public byte Flags { get { return flags; } }
        public DeviceId Address { get { return id; } }
        public byte LinkData1 { get { return linkData1; } }
        public byte LinkData2 { get { return linkData2; } }
        public byte LinkData3 { get { return linkData3; } }
    }
}

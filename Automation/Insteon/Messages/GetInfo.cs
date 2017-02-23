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
    public class GetInfo : PowerLineModemMessage
    {
        private DeviceId id;
        private byte category;
        private byte subcategory;
        private byte firmwareVersion;

        public GetInfo()
            : base(Message.GetImInfo)
        {
        }

        public DeviceId Id { get { return id; } }
        public byte Category { get { return category; } }
        public byte Subcategory { get { return subcategory; } }
        public byte FirmwareVersion { get { return firmwareVersion; } }

        protected override int VerifyExtraResponseData(byte[] data, int pos)
        {
            id = new DeviceId(data[0], data[1], data[2]);
            category = data[3];
            subcategory = data[4];
            firmwareVersion = data[5];
            return 6;
        }

        public override byte[] PayloadByteArray()
        {
            return new byte[0];
        }

        public static int ResponseSize { get { return 7; } }
    }
}

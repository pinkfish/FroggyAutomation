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
    /// Represents a device id in the insteon system.
    /// </summary>
    public class DeviceId
    {
        private byte[] address;

        /// <summary>
        /// Creates a device id from a 3 length data array.
        /// </summary>
        /// <param name="address"></param>
        public DeviceId(byte[] address)
        {
            if (address.Length != 3)
            {
                throw new ArgumentException("Device id must be three numbers");
            }
            this.address = address;
        }

        /// <summary>
        /// Creates a device id from the three bits of the device id.
        /// </summary>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        /// <param name="address3"></param>
        public DeviceId(byte address1, byte address2, byte address3)
        {
            this.address = new byte[3];
            this.address[0] = address1;
            this.address[1] = address2;
            this.address[2] = address3;
        }

        /// <summary>
        /// Creates a device id from a dotted string address.
        /// </summary>
        /// <param name="address"></param>
        public DeviceId(string address)
        {
            this.address = ParseFromString(address);
        }

        /// <summary>
        /// The address in the byte format.
        /// </summary>
        public byte[] Address
        {
            get
            {
                return address;
            }
        }

        /// <summary>
        /// Convert the byte data to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0:X}.{1:X}.{2:X}", address[0], address[1], address[2]);
        }

        /// <summary>
        /// Parses the address from a string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private byte[] ParseFromString(string str)
        {
            string[] bits = str.Split('.');
            byte[] byteAddress = new byte[bits.Length];
            if (bits.Length != 3)
            {
                throw new ArgumentException("Device id must be of the format x.y.z");
            }
            for (int i = 0; i < byteAddress.Length; i++)
            {
                byteAddress[i] = Byte.Parse(bits[i], System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            return byteAddress;
        }

        public override bool Equals(object obj)
        {
            if (obj is DeviceId)
            {
                DeviceId id = (DeviceId)obj;
                return id.Address[0] == this.Address[0] &&
                    id.Address[1] == this.Address[1] &&
                    id.Address[2] == this.Address[2];
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (this.Address[0] << 16) | (this.Address[1] << 8) | this.Address[2];
        }
    }
}

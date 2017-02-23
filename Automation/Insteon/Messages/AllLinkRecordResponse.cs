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
    /// The all link response with all the data in it.
    /// </summary>
    public class AllLinkRecordResponse : PowerLineModemMessage
    {
        private LinkingRecord record;

        public AllLinkRecordResponse(Message message, byte[] data) : base(message)
        {
            this.record = new LinkingRecord(data);
        }

        public LinkingRecord Record { get { return record; } }

        public static int ResponseSize
        {
            get { return 8; }
        }

        public override byte[] PayloadByteArray()
        {
            // Not used for this.
            throw new NotImplementedException();
        }
    }
}

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
    public class GetConfiguration : PowerLineModemMessage
    {
        private const byte CONFIG_DISABLE_AUTO_LINKING = 0x80;
        private const byte CONFIG_MONITOR_MODE = 0x40;
        private const byte CONFIG_MANUAL_LED_CONTROL = 0x20;
        private const byte CONFIG_DISABLE_RS232_DEADMAN = 0x10;

        private Configuration config = new Configuration();

        public GetConfiguration() : base(Message.GetImConfiguration)
        {
        }

        public Configuration Configuration { get { return config; } }

        protected override int VerifyExtraResponseData(byte[] data, int pos)
        {
            // Pull the data out of the response and update the send packet.
            config.DisableAutomaticLinking = (data[pos] & CONFIG_DISABLE_AUTO_LINKING) != 0;
            config.MonitorMode = (data[pos] & CONFIG_MONITOR_MODE) != 0;
            config.ManualLEDOperation = (data[pos] & CONFIG_MANUAL_LED_CONTROL) != 0;
            config.Deadman = (data[pos] & CONFIG_DISABLE_RS232_DEADMAN) != 0;
            return 3;
        }


        public static int ResponseSize { get { return 4; } }

        public override byte[] PayloadByteArray()
        {
            return new byte[0];
        }
    }
}

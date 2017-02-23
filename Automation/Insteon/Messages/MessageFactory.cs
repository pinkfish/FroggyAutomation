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

namespace Automation.Insteon.Messages
{
    /// <summary>
    /// Turns an incoming insteon message into a class.
    /// </summary>
    public class MessageFactory
    {
        /// <summary>
        /// Creates the message and fills in the data.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static PowerLineModemMessage GetReceivedMessage(PowerLineModemMessage.Message message, byte[] data)
        {
            switch (message)
            {
                case PowerLineModemMessage.Message.StandardMessageReceived:
                    return new StandardMessage(message, data);
                case PowerLineModemMessage.Message.ExtendedMessageReceived:
                    return new ExtendedMessage(message, data);
                case PowerLineModemMessage.Message.AllLinkRecordResponse:
                    return new AllLinkRecordResponse(message, data);
            }
            return null;
        }

        /// <summary>
        /// Figures out how big this message needs to be.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int SizeOfMessage(PowerLineModemMessage.Message message, PowerLineModemMessage sending)
        {
            switch (message)
            {
                case PowerLineModemMessage.Message.StandardMessageReceived:
                    return 9;
                case PowerLineModemMessage.Message.ExtendedMessageReceived:
                    return 23;
                case PowerLineModemMessage.Message.GetImConfiguration:
                    return GetConfiguration.ResponseSize;
                case PowerLineModemMessage.Message.GetImInfo:
                    return GetInfo.ResponseSize;
                case PowerLineModemMessage.Message.AllLinkRecordResponse:
                    return AllLinkRecordResponse.ResponseSize;
                case PowerLineModemMessage.Message.SendInsteonMessage:
                    if (sending != null)
                    {
                        StandardMessage standard = (StandardMessage)sending;
                        if (standard.Packet.Flags.ExtendedMessage)
                        {
                            return ExtendedMessage.ResponseSize;
                        }
                        return StandardMessage.ResponseSize;
                    }
                    return 0;
                default:
                    return BasicMessage.ResponseSize;
            }
            return 0;
        }
    }
}

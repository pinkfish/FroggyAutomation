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
    /// The message to send to the powerline modem.  This wraps the insteon messages
    /// in a useful way.
    /// </summary>
    public abstract class PowerLineModemMessage
    {
        public enum Message : byte
        {
            StandardMessageReceived = 0x50,
            ExtendedMessageReceived = 0x51,
            X10MessageReceived = 0x52,
            AllLinkingCompleted = 0x53,
            ButtonEventReported = 0x54,
            UserResetDetected = 0x55,
            AllLinkCleanupFailureReport = 0x56,
            AllLinkRecordResponse = 0x57,
            AllLinkCleanupStatusReport = 0x58,
            GetImInfo = 0x60,
            SendAllLinkCommand = 0x61,
            SendInsteonMessage = 0x62,
            SendX10 = 0x63,
            StartAllLinking = 0x64,
            CancelAllLinking = 0x65,
            SetHostDeviceCategory = 0x66,
            ResetTheIm = 0x67,
            SetInsteonAckMessageByte = 0x68,
            GetFirstAllLinkRecord = 0x69,
            GetNextAllLinkRecord = 0x6A,
            SetImConfiguration = 0x6B,
            GetAllLinkRecordForSender = 0x6C,
            LedOn = 0x6D,
            LedOff = 0x6E,
            ManageAllLinkRecord = 0x6F,
            SetInsteonNakMessageByte = 0x70,
            SetInsteonAckMessageTwoBytes = 0x71,
            RFSleep = 0x72,
            GetImConfiguration = 0x73
        }

        /// <summary>
        /// The response from the message.
        /// </summary>
        public enum MessageResponse
        {
            Ack,
            Nack,
            Invalid,
            Unknown
        }

        internal const byte ACK = 0x6;
        internal const byte NACK = 0x15;

        /// <summary>
        /// Creates a message with the message type.
        /// </summary>
        /// <param name="message"></param>
        public PowerLineModemMessage(Message message)
        {
            MessageType = message;
            this.HasResponse = false;
        }

        /// <summary>
        /// The message type.
        /// </summary>
        public Message MessageType { get; set; }
        /// <summary>
        /// If we have processed a response.
        /// </summary>
        public bool HasResponse { get; set; }
        /// <summary>
        /// If the response was an ack.
        /// </summary>
        public byte ResponseAck { get; set; }

        /// <summary>
        /// Verifies the response, looking at the data and parsing any needed parts of it 
        /// back into the message.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public MessageResponse VerifyResponse(byte[] data)
        {
            if (this.HasResponse)
            {
                throw new InvalidOperationException("Already received response for " + MessageType);
            }
            byte[] arr = PayloadByteArray();
            if (arr.Length > data.Length)
            {
                return MessageResponse.Invalid;
            }
            bool verified = true;
            for (int i = 0; i < arr.Length; i++)
            {
                verified = verified && (arr[i] == data[i]);
            }
            if (!verified)
            {
                return MessageResponse.Invalid;
            }
            int pos = arr.Length;
            pos += VerifyExtraResponseData(data, pos);
            if (data[pos] == ACK)
            {
                return MessageResponse.Ack;
            }
            if (data[pos] == NACK)
            {
                return MessageResponse.Nack;
            }
            this.HasResponse = true;
            this.ResponseAck = data[pos];
            return MessageResponse.Unknown;
        }

        /// <summary>
        /// The payload data to send.  Must be overriden in derived classes.
        /// </summary>
        /// <returns></returns>
        public abstract byte[] PayloadByteArray();

        /// <summary>
        /// Verifies any extra response data, where the response has extra bits to deal with.
        /// </summary>
        /// <param name="data">The data to process</param>
        /// <param name="pos">Where we are in the data</param>
        /// <returns>The number of bytes processed</returns>
        protected virtual int VerifyExtraResponseData(byte[] data, int pos)
        {
            return 0;
        }
    }
}

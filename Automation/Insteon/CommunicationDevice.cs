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
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Automation.Insteon
{
    /// <summary>
    /// Wraps the communication with the serial port, dealing with the insteon packets themselves.
    /// </summary>
    public class CommunicationDevice : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommunicationDevice).Name);

        enum ReadState
        {
            START,
            MESSAGE_TYPE,
            MESSAGE_DATA
        }

        private SerialPort port;
        private PowerLineModemMessage sending;
        private ReadState state;
        private PowerLineModemMessage.MessageResponse sendingResponse;
        private PowerLineModemMessage.Message message;
        private AutoResetEvent receivedAck = new AutoResetEvent(false);
        private int readData;
        private byte[] data;

        private const byte START_OF_MESSAGE = 0x2;

        /// <summary>
        /// Create the communication device for dealing with the power line mode.
        /// </summary>
        /// <param name="port">The port to use</param>
        public CommunicationDevice(SerialPort port)
        {
            OpenPort(port);
        }

        /// <summary>
        /// Opens the specific port.
        /// </summary>
        /// <param name="port"></param>
        public void OpenPort(SerialPort port)
        {
            if (this.port != null)
            {
                this.port.Dispose();
            }
            this.port = port;
            this.port.Open();
            this.port.DiscardInBuffer();
            this.port.DiscardOutBuffer();
            this.port.DataReceived += port_DataReceived;
        }

        /// <summary>
        /// Sends a command, verifies the ack.
        /// </summary>
        /// <param name="cmd1"></param>
        /// <param name="cmd2"></param>
        public PowerLineModemMessage.MessageResponse SendCommand(PowerLineModemMessage message)
        {
            // Only one message sending at a time.
            log.Debug("Start  <<");
            try
            {
                lock (this)
                {
                    // Lock access around the port when sending/receiving.
                    lock (port)
                    {
                        // Clear input first.
                        port_DataReceived(this, null);

                        byte[] preamble = new byte[2];
                        preamble[0] = START_OF_MESSAGE;
                        preamble[1] = (byte)message.MessageType;
                        byte[] payload = message.PayloadByteArray();
                        sending = message;
                        port.Write(preamble, 0, preamble.Length);
                        port.Write(payload, 0, payload.Length);

                        log.DebugFormat("  Output [{0}-{1}]", BitConverter.ToString(preamble), BitConverter.ToString(payload));
                    }
                    return WaitForResponse(message);
                }
            }
            finally
            {
                log.Debug(">> End");
            }
        }


        private void ReadMessages(byte[] input, int size)
        {
            List<PowerLineModemMessage> receivedMessages = new List<PowerLineModemMessage>();

            lock (port)
            {
                string strDebug = "Input: [";
                // First we look for a 0x2, which they all start with.
                for (int i = 0; i < size; i++)
                {
                    int data = input[i];
                    strDebug += String.Format("{0:x2}-", data, readData, (this.data == null ? 0 :this.data.Length), state.ToString());
                    switch (state)
                    {
                        case ReadState.START:
                            if (data == START_OF_MESSAGE)
                            {
                                state = ReadState.MESSAGE_TYPE;
                            }
                            break;
                        case ReadState.MESSAGE_TYPE:
                            // This next one is the message type.
                            message = (PowerLineModemMessage.Message)data;
                            state = ReadState.MESSAGE_DATA;
                            this.data = new byte[MessageFactory.SizeOfMessage(message, sending)];
                            readData = 0;
                            break;
                        case ReadState.MESSAGE_DATA:
                            if (readData < this.data.Length)
                            {
                                this.data[readData++] = (byte)data;
                            }
                            if (readData >= this.data.Length)
                            {
                                // If this is a response, update the request with the response data.
                                if (sending != null && message == sending.MessageType)
                                {
                                    sendingResponse = sending.VerifyResponse(this.data);
                                    sending = null;
                                    receivedAck.Set();
                                }
                                else
                                {
                                    // Create the received message.
                                    PowerLineModemMessage packet = MessageFactory.GetReceivedMessage(message, this.data);
                                    if (packet != null)
                                    {
                                        receivedMessages.Add(packet);
                                    }
                                }
                                state = ReadState.START;
                            }
                            break;
                    }
                }
                log.Debug(strDebug + "]");
            }
            if (ReceivedMessage != null)
            {
                // Switch context so we don't end up deadlocked.
                ThreadPool.QueueUserWorkItem(delegate
                {
                    foreach (PowerLineModemMessage mess in receivedMessages)
                    {
                        ReceivedMessage(this, new RecievedMessageEventArgs(mess));
                    }
                });
            }
        }

        private PowerLineModemMessage.MessageResponse WaitForResponse(PowerLineModemMessage message)
        {
            // See if we have some data.
            if (receivedAck.WaitOne(2000))
            {
                return sendingResponse;
            }
            else
            {
                return PowerLineModemMessage.MessageResponse.Unknown;
            }
        }

        private int depth = 0;
        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                log.Debug("Started Receive " + (depth++));
                int count = port.BytesToRead;
                byte[] buffer = new byte[count];
                port.Read(buffer, 0, count);
                ReadMessages(buffer, count);
            }
            finally
            {
                log.Debug("End Receive " + (--depth));
            }
        }

        public delegate void ReceivedMessageHandler(object sender, RecievedMessageEventArgs args);

        /// <summary>
        /// Called when a new message is received (not in response to a message sent).
        /// </summary>
        public event ReceivedMessageHandler ReceivedMessage;

        /// <summary>
        /// Disposes the port.
        /// </summary>
        public void Dispose()
        {
            this.port.Dispose();
            this.message = PowerLineModemMessage.Message.UserResetDetected;
            this.port = null;
            this.readData = 0;
            this.receivedAck.Dispose();
            this.receivedAck = null;
            this.sending = null;
            this.sendingResponse = PowerLineModemMessage.MessageResponse.Unknown;
            this.state = ReadState.START;
            this.data = null;
            this.depth = 0;
        }
    }
}

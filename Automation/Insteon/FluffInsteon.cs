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
using Automation.Insteon.Devices;
using Automation.Insteon.Messages;
using log4net;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Automation.Insteon
{
    /// <summary>
    /// Main basic system with all the setup for the fluff system.
    /// </summary>
    public class FluffInsteon : InsteonCommunication, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FluffInsteon).Name);

        private CommunicationDevice port;
        private PowerLineModule plm;
        private Dictionary<DeviceId, LinkingRecord> linkedDevices = new Dictionary<DeviceId, LinkingRecord>();
        private Dictionary<DeviceId, DeviceBase> devices = new Dictionary<DeviceId,DeviceBase>();
        private AutoResetEvent linkingEvent;
        private InsteonPacket sendingPacket;
        private InsteonPacket receivedPacket;
        private DeviceFactory deviceFactory;
        private DeviceProvider provider;
        private bool connected;

        /// <summary>
        /// The basic system object, sets up the connection to the serial port.
        /// </summary>
        /// <param name="portName"></param>
        public FluffInsteon(string portName, DeviceProvider provider)
        {
            this.port = new CommunicationDevice(new SerialPort(portName, 19000, Parity.None, 8, StopBits.One));
            this.port.ReceivedMessage += port_ReceivedMessage;
            this.linkingEvent = new AutoResetEvent(false);
            this.deviceFactory = new DeviceFactory(this);
            this.provider = provider;
            log.Info("Started FluffInsteon up");
        }

        /// <summary>
        /// Reads all the devices from the PLM and stores the mapping locally.  Along with the
        /// basic info and config of the plm.
        /// </summary>
        public bool Startup()
        {
            GetInfo info = new GetInfo();
            PowerLineModemMessage.MessageResponse response = port.SendCommand(info);
            if (response == PowerLineModemMessage.MessageResponse.Ack)
            {
                plm = new PowerLineModule(this, info.Id, info.Category, info.Subcategory, info.FirmwareVersion);
                // Ask for the config and info about the modem.
                GetConfiguration config = new GetConfiguration();
                response = port.SendCommand(config);
                if (response == PowerLineModemMessage.MessageResponse.Ack)
                {
                    plm.Config = config.Configuration;
                    this.ReadAllLinks();
                    this.SetupAllDevices();
                    connected = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            this.linkingEvent.Dispose();
            this.port.Dispose();
            this.devices.Clear();
            this.devices = null;
            this.deviceFactory = null;
            this.linkedDevices = null;
            this.linkingEvent = null;
            this.plm = null;
            this.port = null;
            this.provider = null;
            this.receivedPacket = null;
            this.sendingPacket = null;
            this.connected = false;
        }

        /// <summary>
        /// Returns true if the system is up and running.
        /// </summary>
        public bool Conncted { get { return connected; } }

        /// <summary>
        /// Reads the links/devices from the plm.
        /// </summary>
        private void ReadAllLinks()
        {
            this.linkedDevices = new Dictionary<DeviceId, LinkingRecord>();
            List<DeviceId> ids = new List<DeviceId>();
            BasicMessage firstAllLink = new BasicMessage(PowerLineModemMessage.Message.GetFirstAllLinkRecord);
            if (port.SendCommand(firstAllLink) == PowerLineModemMessage.MessageResponse.Ack)
            {
                // Wait to receive the response.
                this.linkingEvent.WaitOne();
                BasicMessage nextAllLink = new BasicMessage(PowerLineModemMessage.Message.GetNextAllLinkRecord);
                while (port.SendCommand(nextAllLink) == PowerLineModemMessage.MessageResponse.Ack)
                {
                    // Wait to receive the response.
                    this.linkingEvent.WaitOne();
                }
            }
        }

        /// <summary>
        /// Query the type from all the known devices.
        /// </summary>
        private void SetupAllDevices()
        {
            foreach (LinkingRecord record in this.linkedDevices.Values)
            {
                if (provider != null)
                {
                    Tuple<byte, byte> categories = provider.GetDeviceCategory(record.Address);
                    if (categories != null)
                    {
                        AddDevice(record.Address, categories.Item1, categories.Item2);
                        continue;
                    }
                }
                InsteonPacket packet = new InsteonPacket(record.Address, InsteonPacket.Command.IdRequest);
                InsteonPacket response;
                if (SendDirectInsteonPacket(packet, out response) == PowerLineModemMessage.MessageResponse.Ack)
                {
                    log.InfoFormat("Id Request Acked {0}", response.FromAddress);
                }
                else
                {
                    log.InfoFormat("Id Request Failed {0}", record.Address);
                }
            }
        }

        /// <summary>
        /// Return all the linked devices know to this modem.
        /// </summary>
        public Dictionary<DeviceId, LinkingRecord> LinkedDevices { get { return this.linkedDevices; } }

        /// <summary>
        /// Find a device in the list of devices.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool FindDevice(string address, out DeviceBase device)
        {
            return FindDevice(new DeviceId(address), out device);
        }

        /// <summary>
        /// Finds a device based on the byte address.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool FindDevice(DeviceId address,out DeviceBase device)
        {
            return devices.TryGetValue(address, out device);
        }

        /// <summary>
        /// The power line module, has the config and info about the connection to the insteon world.
        /// </summary>
        public PowerLineModule PowerLineModule { get { return plm; } }

        /// <summary>
        /// Sends the specified direct message and waits for a response.
        /// </summary>
        /// <param name="packet">Packet to send</param>
        /// <param name="response">The response from the device</param>
        /// <returns>the result of the call</returns>
        public PowerLineModemMessage.MessageResponse SendDirectInsteonPacket(InsteonPacket packet, out InsteonPacket response)
        {
            // Only one packet at a time.
            lock (this)
            {
                if (packet.Flags.MessageType != InsteonPacket.InsteonFlags.MessageTypeEnum.Direct)
                {
                    response = null;
                    return PowerLineModemMessage.MessageResponse.Invalid;
                }
                // Send only one direct packet at a time.
                PowerLineModemMessage.MessageResponse messageResponse;
                StandardMessage message = new StandardMessage(PowerLineModemMessage.Message.SendInsteonMessage, packet);
                this.sendingPacket = packet;
                messageResponse = port.SendCommand(message);
                if (messageResponse == PowerLineModemMessage.MessageResponse.Ack)
                {
                    // We should get an ack back for this message.
                    if (this.linkingEvent.WaitOne(5000))
                    {
                        // See if we got back an ack or nack.
                        if (this.receivedPacket.Flags.MessageType == InsteonPacket.InsteonFlags.MessageTypeEnum.NackDirect)
                        {
                            messageResponse = PowerLineModemMessage.MessageResponse.Nack;
                        }
                        response = this.receivedPacket;
                    }
                    else
                    {
                        response = null;
                        return PowerLineModemMessage.MessageResponse.Unknown;
                    }
                }
                else
                {
                    response = null;
                }
                return messageResponse;
            }
        }

        private void AddDevice(DeviceId id, byte category, byte subcategory)
        {
            DeviceBase device = this.deviceFactory.CreateDevice(id, category, subcategory);
            this.devices[device.Address] = device;
            log.InfoFormat("Added device {0} called {1}", device.Address.ToString(), device.DeviceName);
            if (DeviceAdded != null)
            {
                DeviceAdded(this, new DeviceAddedEventArgs(device));
            }
        }

        /// <summary>
        /// Called when a message comes in from the modem.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void port_ReceivedMessage(object sender, RecievedMessageEventArgs args)
        {
            switch (args.Message.MessageType)
            {
                case PowerLineModemMessage.Message.AllLinkRecordResponse:
                    // If we get this, then query the device for more details.
                    AllLinkRecordResponse response = (AllLinkRecordResponse)args.Message;
                    this.linkedDevices[response.Record.Address] = response.Record;
                    this.linkingEvent.Set();
                    break;
                case PowerLineModemMessage.Message.StandardMessageReceived:
                    StandardMessage standardMessage = (StandardMessage)args.Message;
                    if (standardMessage.Packet.FromAddress.Equals(this.sendingPacket.FromAddress))
                    {
                        if (standardMessage.Packet.Flags.MessageType == InsteonPacket.InsteonFlags.MessageTypeEnum.AckDirect ||
                            standardMessage.Packet.Flags.MessageType == InsteonPacket.InsteonFlags.MessageTypeEnum.NackDirect)
                        {
                            this.receivedPacket = standardMessage.Packet;
                            this.linkingEvent.Set();
                            break;
                        }
                    }
                    // Find the device and send the packet there.
                    if (this.devices.ContainsKey(standardMessage.Packet.FromAddress))
                    {
                        DeviceBase device = this.devices[standardMessage.Packet.FromAddress];
                        StateChanged changed = device.handleInsteonPacket(standardMessage.Packet);
                        if (changed != null)
                        {
                            if (DeviceChanged != null)
                            {
                                DeviceChanged(this, new DeviceChangedEventArgs(changed));
                            }
                        }
                    }
                    else
                    {
                        // If this is a broadcast set button pressed, add into the device list.
                        if (standardMessage.Packet.Flags.MessageType == InsteonPacket.InsteonFlags.MessageTypeEnum.Broadcast &&
                            standardMessage.Packet.Command1 == (byte)InsteonPacket.BroadcastCommand.SetButtonPressed)
                        {
                            byte category = standardMessage.Packet.ToAddress.Address[0];
                            byte subCategory = standardMessage.Packet.ToAddress.Address[1];
                            AddDevice(standardMessage.Packet.FromAddress, category, subCategory);
                        }
                    }
                    break;
                case PowerLineModemMessage.Message.ExtendedMessageReceived:
                    ExtendedMessage extendedMessage = (ExtendedMessage)args.Message;
                    if (extendedMessage.Packet.FromAddress.Equals(this.sendingPacket.FromAddress))
                    {
                        if (extendedMessage.Packet.Flags.MessageType == InsteonPacket.InsteonFlags.MessageTypeEnum.AckDirect ||
                            extendedMessage.Packet.Flags.MessageType == InsteonPacket.InsteonFlags.MessageTypeEnum.NackDirect)
                        {
                            this.receivedPacket = extendedMessage.Packet;
                            this.linkingEvent.Set();
                            break;
                        }
                    }
                    // Find the device and send the packet there.
                    if (this.devices.ContainsKey(extendedMessage.Packet.FromAddress))
                    {
                        DeviceBase device = this.devices[extendedMessage.Packet.FromAddress];
                        StateChanged changed = device.handleInsteonPacket(extendedMessage.Packet);
                        if (changed != null)
                        {
                            if (DeviceChanged != null)
                            {
                                DeviceChanged(this, new DeviceChangedEventArgs(changed));
                            }
                        }
                    }
                    break;
            }
        }

        public delegate void DeviceChangedHandler(object sender, DeviceChangedEventArgs args);
        public delegate void DeviceAddedHandler(object sender, DeviceAddedEventArgs args);

        /// <summary>
        /// Called when a device changes.
        /// </summary>
        public event DeviceChangedHandler DeviceChanged;

        /// <summary>
        /// Called when a device is added (found).
        /// </summary>
        public event DeviceAddedHandler DeviceAdded;
    }
}

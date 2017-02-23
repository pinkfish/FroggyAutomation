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

namespace Automation.Insteon.Devices
{
    /// <summary>
    /// The base device for all insteon systems.
    /// </summary>
    public class DeviceBase
    {
        private readonly byte category;
        private readonly byte subcategory;
        private readonly DeviceId deviceId;
        private readonly InsteonCommunication coms;
        private string deviceName;

        #region DeviceCategoryLookup
        internal class DeviceCategoryData
        {
            internal DeviceCategoryData(string defaultName, Dictionary<byte, string> subCategories)
            {
                DefaultName = defaultName;
                SubCategories = subCategories;
            }
            internal string DefaultName { get; set; }
            internal Dictionary<byte, string> SubCategories { get; set; }
        }
        private static readonly Dictionary<byte, DeviceCategoryData> DeviceSubcategoryLookup
            = new Dictionary<byte, DeviceCategoryData>()
            {
                { 0x00, new DeviceCategoryData("Unknown Control Device {0:x}", new Dictionary<byte,string>() 
                    { 
                        {0x00, "Unknown"},
                        {0x04, "ControlLinc [2430]"},
                        {0x05, "RemoteLinc [2440]"},
                        {0x06, "Icon Tabletop Controller [2830]"},
                        {0x08, "EZBridge/EZServer"},
                        {0x09, "SignalLinc RF Signal Enhancer [2442]"},
                        {0x0A, "Balboa Instrument's Poolux LCD Controller"},
                        {0x0B, "Access Point [2443]"},
                        {0x0C, "IES Color Touchscreen"},
                        {0x0D, "SmartLabs KeyFOB"}
                    }) },
                { 0x01, new DeviceCategoryData("Unknown Dimmer {0:x}", new Dictionary<byte,string>() 
                    { 
                        {0x00, "LampLinc V2 [2456D3]"},
                        {0x01, "SwitchLinc V2 Dimmer 600W [2476D]"},
                        {0x02, "In-LineLinc Dimmer [2475D]"},
                        {0x03, "Icon Switch Dimmer [2876D]"},
                        {0x04, "SwitchLinc V2 Dimmer 1000W [2476DH]"},
                        {0x05, "KeypadLinc Dimmer Countdown Timer [2484DWH8]"},
                        {0x06, "LampLinc 2-Pin [2456D2]"},
                        {0x07, "Icon LampLinc V2 2-Pin [2856D2]"},
                        {0x08, "SwitchLinc Dimmer Count-down Timer [2484DWH8]"},
                        {0x09, "KeypadLinc Dimmer [2486D]"},
                        {0x0A, "Icon In-Wall Controller [2886D]"},
                        {0x0B, "Access Point LampLinc [2458D3]"},
                        {0x0C, "KeypadLinc Dimmer - 8-Button defaulted mode [2486DWH8]"},
                        {0x0D, "SocketLinc [2454D]"},
                        {0x0E, "LampLinc Dimmer, Dual-Band [2457D3]"},
                        {0x13, "ICON SwitchLinc Dimmer for Lixar/Bell Canada [2676D-B]"},
                        {0x17, "ToggleLinc Dimmer [2466D]"},
                        {0x18, "Icon SL Dimmer Inline Companion [2474D]"},
                        {0x19, "SwitchLinc 800W"},
                        {0x1A, "In-LineLinc Dimmer with Sense [2475D2]"},
                        {0x1B, "KeypadLinc 6-button Dimmer [2486DWH6]"},
                        {0x1C, "KeypadLinc 8-button Dimmer [2486DWH8]"},
                        {0x1D, "SwitchLinc Dimmer 1200W [2476D]"},
                        {0x20, "SwitchLinc Dimmer (Dual Band) [2477D]"}
                    }) },
                { 0x02, new DeviceCategoryData("Unknown Switching Light {0:x}", new Dictionary<byte,string>() 
                    { 
                        {0x05, "KeypadLinc Relay - 8-Button defaulted mode [2486SWH8]"},
                        {0x06, "Outdoor ApplianceLinc [2456S3E]"},
                        {0x07, "TimerLinc [2456ST3]"},
                        {0x08, "OutletLinc [2473S]"},
                        {0x09, "ApplianceLinc [2456S3]"},
                        {0x0A, "SwitchLinc Relay [2476S]"},
                        {0x0B, "Icon On Off Switch [2876S]"},
                        {0x0C, "Icon Appliance Adapter [2856S3]"},
                        {0x0D, "ToggleLinc Relay [2466S]"},
                        {0x0E, "SwitchLinc Relay Countdown Timer [2476ST]"},
                        {0x0F, "KeypadLinc On/Off Switch [2486SWH6]"},
                        {0x10, "In-LineLinc Relay [2475D]"},
                        {0x11, "EZSwitch30 (240V, 30A load controller)"},
                        {0x12, "Icon SL Relay Inline Companion"},
                        {0x13, "ICON SwitchLinc Relay for Lixar/Bell Canada [2676R-B]"},
                        {0x14, "In-LineLinc Relay with Sense [2475S2]"},
                        {0x15, "SwitchLinc Relay with Sense [2476SS]"},
                        {0x16, "SwitchLinc Relay with Sense [2476S2]"},
                        {0x1C, "SwitchLinc Relay [2476S]"},
                        {0x1E, "KeypadLinc 6-button Relay Dual Band [2487S]" },
                        {0x1F, "Inline SwitchLinc Relay Dual Band [2475SDB]" }
                    }) },
                { 0x03, new DeviceCategoryData("Unknown interface {0:x}", new Dictionary<byte,string>() 
                    { 
                        {0x01, "PowerLinc Serial [2414S]"},
                        {0x02, "PowerLinc USB [2414U]"},
                        {0x03, "Icon PowerLinc Serial [2814S]"},
                        {0x04, "Icon PowerLinx USB [2814U]"},
                        {0x05, "SmartLabs PowerLinc Modem Serial [2412S]"},
                        {0x06, "SmartLabs IR to Insteon Interface [2411R]"},
                        {0x07, "SmartLabs IRLinc - IR Transmitter Interface [2411T]"},
                        {0x08, "SmartLabs Bi-Directional IR -Insteon Interface"},
                        {0x09, "SmartLabs RF Developer's Board [2600RF]"},
                        {0x0A, "SmartLabs PowerLinc Modem Ethernet [2412E]"},
                        {0x0B, "SmartLabs PowerLinc Modem USB [2412U]"},
                        {0x0C, "SmartLabs PLM Alert Serial"},
                        {0x0D, "SimpleHomeNet EZX 10RF"},
                        {0x0E, "X10 TW-523/PSC05 Translator"},
                        {0x0F, "EZX10IR (X10 IR receiver, Insteon controller and IR distribution hub)"},
                        {0x10, "SmartLinc 2412N INSTEON Central Controller"},
                        {0x11, "PowerLinc - Serial (Dual Band) [2413S]"},
                        {0x12, "RF Modem Card"},
                        {0x13, "PowerLinc USB - HouseLinc 2 enabled [2412UH]"},
                        {0x14, "PowerLinc Serial - HouseLinc 2 enabled [2412SH]"},
                        {0x15, "PowerLinc - USB (Dual Band) [2413U]"}
                    }) },
                { 0x04, new DeviceCategoryData("Unknown Irrigation {0:x}", new Dictionary<byte,string>() 
                    { 
                        {0x00, "Compacta EZRain Sprinkler Controller"}
                    }) },
                { 0x05, new DeviceCategoryData("Unknown Climate Control {0:x}", new Dictionary<byte,string>() 
                    { 
                        {0x00, "Broan SMSC080 Exhaust Fan"},
                        {0x01, "Compacta EZTherm"},
                        {0x02, "Broan SMSC110 Exhaust Fan"},
                        {0x03, "INSTEON Thermostat Adapter [2441V]"},
                        {0x04, "Compacta EZThermx Thermostat"},
                        {0x05, "Broan, Venmar, BEST Rangehoods"},
                        {0x06, "Broan SmartSense Make-up Damper"}
                    }) },
                { 0x06, new DeviceCategoryData("Unknonw Pool/Spa Control {0:x}", new Dictionary<byte,string>() 
                    { 
                        {0x00, "Compacta EZPool"},
                        {0x01, "Low-end pool controller (Temp. Eng. Project name)"},
                        {0x02, "Mid-Range pool controller (Temp. Eng. Project name)"},
                        {0x03, "Next Generation pool controller (Temp. Eng. Project name)"}
                    }) },
                { 0x07, new DeviceCategoryData("Unknown home entertainment {0:x}", new Dictionary<byte,string>() 
                    { 
                        {0x00, "IOLinc [2450]"},
                        {0x01, "Compacta EZSns1W Sensor Interface Module"},
                        {0x02, "Compacta EZIO8T I/O Module"},
                        {0x03, "Compacta EZIO2X4 #5010D INSTEON / X10 Input/Output Module"},
                        {0x04, "Compacta EZIO8SA I/O Module"},
                        {0x05, "Compacta EZSnsRF #5010E RF Receiver Interface Module for Dakota Alerts Products"},
                        {0x06, "Compacta EZISnsRf Sensor Interface Module"},
                        {0x07, "EZIO6I (6 inputs)"},
                        {0x08, "EZIO4O (4 relay outputs)"}
                    }) },
                { 0x09, new DeviceCategoryData("Unknown Power Control", new Dictionary<byte,string>() 
                    { 
                        {0x00, "Compacta EZEnergy"},
                        {0x01, "OnSitePro Leak Detector"},
                        {0x02, "OnsitePro Control Valve"},
                        {0x03, "Energy Inc. TED 5000 Single Phase Measuring Transmitting Unit (MTU)"},
                        {0x04, "Energy Inc. TED 5000 Gateway - USB"},
                        {0x05, "Energy Inc. TED 5000 Gateway - Ethernet"},
                        {0x06, "Energy Inc. TED 3000 Three Phase Measuring Transmitting Unit (MTU)"},
                    }) },
                { 0x0E, new DeviceCategoryData("Unknown Window Coverings {0:x}", new Dictionary<byte,string>() 
                    { 
                        {0x00, "Somfy Drape Controller RF Bridge"}
                    }) },
                { 0x0F, new DeviceCategoryData("Unknown Access Control {0:x}", new Dictionary<byte,string>() 
                    { 
                        {0x00, "Weiland Doors' Central Drive and Controller"},
                        {0x01, "Weiland Doors' Secondary Central Drive"},
                        {0x02, "Weiland Doors' Assist Drive"},
                        {0x03, "Weiland Doors' Elevation Drive"}
                    }) },
                { 0x10, new DeviceCategoryData("Unknown Security/Health Safety {0:x}", new Dictionary<byte,string>() 
                    { 
                        {0x00, "First Alert ONELink RF to Insteon Bridge"},
                        {0x01, "Motion Sensor [2420M]"},
                        {0x02, "TriggerLinc - INSTEON Open / Close Sensor [2421]"}
                    }) }
            };
        #endregion

        /// <summary>
        /// All the basic parts of the device, initialized here.
        /// </summary>
        /// <param name="coms"></param>
        /// <param name="deviceId"></param>
        /// <param name="category"></param>
        /// <param name="subcategory"></param>
        public DeviceBase(InsteonCommunication coms, DeviceId deviceId, byte category, byte subcategory)
        {
            this.coms = coms;
            this.deviceId = deviceId;
            this.category = category;
            this.subcategory = subcategory;
            if (DeviceSubcategoryLookup.ContainsKey(category))
            {
                DeviceCategoryData data = DeviceSubcategoryLookup[category];
                if (data.SubCategories.ContainsKey(subcategory))
                {
                    this.deviceName = data.SubCategories[subcategory];
                }
                else
                {
                    this.deviceName = String.Format(data.DefaultName, subcategory);
                }
            }
            else
            {
                this.deviceName = String.Format("Unknown Device {0:x} sub: {1:x}", category, subcategory);
            }
        }

        /// <summary>
        /// The name of the device.
        /// </summary>
        public string DeviceName { get { return deviceName; } }
        /// <summary>
        /// The category of the device.
        /// </summary>
        public byte Category { get { return category; } }
        /// <summary>
        /// The sub category of the device.
        /// </summary>
        public byte Subcategory { get { return subcategory; } }
        /// <summary>
        /// The address of the device.
        /// </summary>
        public DeviceId Address { get { return deviceId; } }
        /// <summary>
        /// The object to use to talk to the insteon system.
        /// </summary>
        internal InsteonCommunication Insteon { get { return this.coms; } }

        #region Sending Commands
        /// <summary>
        /// Sends a standard command off to do stuff.
        /// </summary>
        /// <param name="cmd">The command to send</param>
        /// <param name="command2">The second part of the command to update</param>
        /// <returns></returns>
        protected Messages.PowerLineModemMessage.MessageResponse SendStandardCommand(InsteonPacket.Command cmd, byte command2)
        {
            InsteonPacket packet = new InsteonPacket(this.deviceId, cmd);
            packet.Command2 |= command2;
            InsteonPacket response;
            return coms.SendDirectInsteonPacket(packet, out response);
        }

        /// <summary>
        /// Sends an extended command to the specified device.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Messages.PowerLineModemMessage.MessageResponse SendExtendedCommand(InsteonPacket.ExtendedCommand cmd, byte[] data)
        {
            return this.SendExtendedCommand(cmd, 0, data);
        }

        /// <summary>
        /// Sends an extended command off to do stuff.
        /// </summary>
        /// <param name="cmd">The command to send</param>
        /// <param name="command2">The second part of the command to update</param>
        /// <returns></returns>
        protected Messages.PowerLineModemMessage.MessageResponse SendExtendedCommand(InsteonPacket.ExtendedCommand cmd, byte command2, byte[] data)
        {
            InsteonPacket packet = new InsteonPacket(this.deviceId, cmd, command2, data);
            InsteonPacket response;
            return coms.SendDirectInsteonPacket(packet, out response);
        }

        /// <summary>
        /// Sends an extended command off to do stuff.
        /// </summary>
        /// <param name="cmd">The command to send</param>
        /// <param name="command2">The second part of the command to update</param>
        /// <returns></returns>
        protected Messages.PowerLineModemMessage.MessageResponse SendExtendedCommandWithResponse(InsteonPacket.ExtendedCommand cmd, byte command2, byte[] data, out InsteonPacket response)
        {
            InsteonPacket packet = new InsteonPacket(this.deviceId, cmd, command2, data);
            return coms.SendDirectInsteonPacket(packet, out response);
        }

        /// <summary>
        /// Sends a standard command off to do stuff.
        /// </summary>
        /// <param name="cmd">The command to send</param>
        /// <param name="command2">The second part of the command to update</param>
        /// <returns></returns>
        protected Messages.PowerLineModemMessage.MessageResponse SendStandardCommandWithResponse(InsteonPacket.Command cmd, byte command2, out byte outCommand1, out byte outCommand2)
        {
            InsteonPacket packet = new InsteonPacket(this.deviceId, cmd);
            packet.Command2 |= command2;
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = coms.SendDirectInsteonPacket(packet, out response);
            if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
            {
                outCommand1 = response.Command1;
                outCommand2 = response.Command2;
            }
            else
            {
                outCommand1 = 0;
                outCommand2 = 0;
            }
            return ret;
        }

        /// <summary>
        /// Sends a standard command off to do stuff.
        /// </summary>
        /// <param name="cmd">The command to send</param>
        /// <param name="command2">The second part of the command to update</param>
        /// <returns></returns>
        protected Messages.PowerLineModemMessage.MessageResponse SendStandardCommandWithResponse(InsteonPacket.Command cmd, byte command2, out byte outCommand2)
        {
            InsteonPacket packet = new InsteonPacket(this.deviceId, cmd);
            packet.Command2 |= command2;
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = coms.SendDirectInsteonPacket(packet, out response);
            if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
            {
                outCommand2 = response.Command2;
            }
            else
            {
                outCommand2 = 0;
            }
            return ret;
        }
        #endregion

        /// <summary>
        /// Handle an insteon packet from somewhere else.
        /// </summary>
        /// <param name="packet">the incoming packet</param>
        /// <returns>true if something changed</returns>
        internal virtual StateChanged handleInsteonPacket(InsteonPacket packet)
        {
            return null;
        }

        #region Operating Flags
        private OperatingFlagsData operatingFlags;
        /// <summary>
        /// The operating flags for the class.
        /// </summary>
        public class OperatingFlagsData
        {
            private bool programLock;
            private bool ledOnDuringTransmit;
            private bool resumeDim;
            private bool ledOrBacklight;
            private byte data;
            private DeviceBase device;

            private const byte PROGRAM_LOCK = 0x80;
            private const byte LED_ON_DURING_TRANSMIT = 0x40;
            private const byte RESUME_DIM = 0x20;
            private const byte LED_OR_BACKLIGHT = 0x08;
            private const byte MASK = 0xe8;
            private byte cmd2;

            public OperatingFlagsData(DeviceBase device, byte data)
            {
                this.data = data;
                this.device = device;
            }

            public bool ProgramLock
            {
                get { return programLock; }
                set
                {
                    programLock = value;
                    ChangeData();
                }
            }

            public bool LedOnDuringTransmit
            {
                get { return ledOnDuringTransmit; }
                set
                {
                    ledOnDuringTransmit = value;
                    ChangeData();
                }
            }
            public bool ResumeDim
            {
                get { return resumeDim; }
                set
                {
                    resumeDim = value;
                    ChangeData();
                }
            }
            public bool LedOrBacklight
            {
                get { return ledOrBacklight; }
                set
                {
                    ledOrBacklight = value;
                    ChangeData();
                }
            }

            private void ChangeData()
            {
                byte sendData = data;
                sendData = (byte)(sendData & MASK);
                if (this.programLock)
                {
                    sendData |= PROGRAM_LOCK;
                }
                if (this.ledOnDuringTransmit)
                {
                    sendData |= LED_ON_DURING_TRANSMIT;
                }
                if (this.resumeDim)
                {
                    sendData |= RESUME_DIM;
                }
                if (this.ledOrBacklight)
                {
                    sendData |= LED_OR_BACKLIGHT;
                }
                device.SendStandardCommand(InsteonPacket.Command.GetOperatingFlags, sendData);
            }
        }

        /// <summary>
        /// The operating flags for the device.
        /// </summary>
        public OperatingFlagsData OperatingFlags
        {
            get
            {
                byte cmd2;
                SendStandardCommandWithResponse(InsteonPacket.Command.GetOperatingFlags, 0, out cmd2);
                this.operatingFlags = new OperatingFlagsData(this, cmd2);
                return this.operatingFlags;
            }
        }
        #endregion

        #region Device Text String
        private string textString;
        /// <summary>
        /// Gets the text string from the device.
        /// </summary>
        /// <param name="outTextString"></param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse GetTextString(out string outTextString)
        {
            outTextString = textString;
            if (textString == null)
            {
                InsteonPacket packet = new InsteonPacket(this.deviceId, InsteonPacket.Command.ProductDataRequest, 0x2);
                packet.Flags.ExtendedMessage = true;
                packet.UserData = new byte[14];
                InsteonPacket response;
                Messages.PowerLineModemMessage.MessageResponse ret = coms.SendDirectInsteonPacket(packet, out response);
                if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
                {
                    // Response should be an extended message.
                    if (response.Flags.ExtendedMessage)
                    {
                        int i;
                        for (i = 0; i < response.UserData.Length; i++)
                        {
                            if (response.UserData[i] == 0)
                            {
                                break;
                            }
                        }
                        textString = System.Text.Encoding.ASCII.GetString(response.UserData, 0, i);
                        outTextString = textString;
                        return ret;
                    }
                }
                return ret;
            }
            return Messages.PowerLineModemMessage.MessageResponse.Ack;
        }
        #endregion
    }
}

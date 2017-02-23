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
    /// Format of an insteon packet.
    /// </summary>
    public class InsteonPacket
    {
        private DeviceId fromAddress;
        private DeviceId toAddress;
        private InsteonFlags flags;
        private byte cmd1;
        private byte cmd2;
        private byte[] userData;

        #region Commands
        /// <summary>
        /// Commands, first and second commands in the top 8 bits first command, 
        /// bottom 8 bits second command.
        /// </summary>
        public enum Command : byte
        {
            AssignToGroup = 0x01,
            DeleteFromGroup = 0x02,
            ProductDataRequest = 0x03,
            EnterLinkingMode = 0x09,
            EnterUnlinkingMode = 0x0A,
            GetInsteonEngineVersion = 0x0D,
            Ping = 0x0f,
            IdRequest = 0x10,
            LightOn = 0x11,
            LightOnFast = 0x12,
            LightOff = 0x13,
            LightOffFast = 0x14,
            Bright = 0x15,
            Dim = 0x16,
            StartManualChange = 0x17,
            StopManualChange = 0x18,
            LightStatusRequest = 0x19,
            StatusReport = 0x1A,
            ReadLastLevel = 0x1B,
            SetLastLevel = 0x1C,
            ReadPresetLevel = 0x1D,
            SetPresetLevel = 0x1E,
            GetOperatingFlags = 0x1F,
            SetOperatingFlags = 0x20,
            DeleteGroupX10Address = 0x21,
            LoadOff = 0x22,
            LoadOn = 0x23,
            DoReadEE = 0x24,
            LevelPoke = 0x25,
            RatePoke = 0x26,
            CurrentStatus = 0x27,
            SetAddressMSB = 0x28,
            Poke = 0x29,
            PokeExtended = 0x2A,
            Peek = 0x2B,
            PeekInternal = 0x2C,
            PokeInternal = 0x2D,
            LightOnWithRamp = 0x2E,
            LightOffWithRamp = 0x2F,
            PoolOn = 0x50,
            PoolOff = 0x51,
            PoolTemperatureUp = 0x52,
            PoolTemperatureDown = 0x53,
            PoolControl = 0x54,
            ThermostatTemperatureUp = 0x68,
            ThermostatTemperatureDown = 0x69,
            ThermostatGetZoneInformation = 0x6A,
            ThermostatControl = 0x6B,
            ThermostatSetCoolSetpoint = 0x6C,
            ThermostatSetHeatSetpoint = 0x6D,
            AssignToCompanionGroup = 0x81,
        }

        public enum ExtendedCommand
        {
            FxUsernameResponse = 0x0301,
            SetDeviceTextString = 0x0303,
            SetAllLinkCommandAlias = 0x0304,
            SetAllLinkCommandAlisExtended = 0x0305,
            BlockDataTransferFailure = 0x2A00,
            BlockDataTransferCompleteOneByte = 0x2A01,
            BlockDataTransferCompleteTwoBytes = 0x2A02,
            BlockDataTransferCompleteThreeBytes = 0x2A03,
            BlockDataTransferCompleteFourBytes = 0x2A04,
            BlockDataTransferCompleteFiveBytes = 0x2A05,
            BlockDataTransferCompleteSixBytes = 0x2A06,
            BlockDataTransferCompleteSevenBytes = 0x2A07,
            BlockDataTransferCompleteEightBytes = 0x2A08,
            BlockDataTransferCompleteNineBytes = 0x2A09,
            BlockDataTransferCompleteTenBytes = 0x2A0A,
            BlockDataTransferCompleteElevenBytes = 0x2A0B,
            BlockDataTransferCompleteTwelveBytes = 0x2A0C,
            BlockDataTransferContinues = 0x2A0D,
            BlockDataTransferRequest = 0x2AFF,
            ExtendedGetSet = 0x2E00,
            ReadWriteAllLinkDatabase = 0x2F00,
            TriggerAllLinkCommand = 0x3000,
            SetSpinklerProgram = 0x4000,
            GetSpinklerProgramResponse = 0x4100,
            IOSetSensorNominal = 0x4B00,
            IOAlarmDataResponse = 0x4C00,
            PoolSetDeviceTemperature = 0x5000,
            PoolSetDeviveHysteresis = 0x5001,
            ThermostatZoneTemperatureUp = 0x6800,
            ThermostatZoneTemperatureDown = 0x6900,
            ThermostatSetZoneCoolSetpoint = 0x6C00,
            ThemostatSetZoneHeatSetpoint = 0x6D00,
        }

        public enum BroadcastCommand : byte
        {
            Announce = 0x0,
            SetButtonPressed = 0x1,
            StatusChanged = 0x2,
            SetButtonPressedMaster = 0x3,
            DebugReport = 0x49
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new standard length command.
        /// </summary>
        /// <param name="id">id to send to</param>
        /// <param name="cmd">Standard command to send</param>
        public InsteonPacket(DeviceId id, Command cmd)
            : this(id, cmd, 0)
        {
        }

        /// <summary>
        /// Create a new standard length command.
        /// </summary>
        /// <param name="id">id to sent to</param>
        /// <param name="cmd">cmd1 bit</param>
        /// <param name="cmd2">cmd2 bit</param>
        public InsteonPacket(DeviceId id, Command cmd, byte cmd2)
        {
            Command1 = (byte)cmd;
            Command2 = cmd2;
            FromAddress = id;
            ToAddress = id;
            flags = new InsteonFlags();
            flags.MessageType = InsteonFlags.MessageTypeEnum.Direct;
            flags.MaxHops = 0x3;
            flags.HopsLeft = 0x3;
        }

        /// <summary>
        /// Creates an insteon packet with an extended command.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cmd"></param>
        public InsteonPacket(DeviceId id, ExtendedCommand cmd, byte cmd2, byte[] data)
        {
            Command1 = (byte)((int)cmd >> 8);
            Command2 = (byte)(((int)cmd & 0xff) | cmd2);
            UserData = data;
            FromAddress = id;
            ToAddress = id;
            flags = new InsteonFlags();
            flags.MessageType = InsteonFlags.MessageTypeEnum.Direct;
            flags.ExtendedMessage = true;
            flags.MaxHops = 0x3;
            flags.HopsLeft = 0x3;
        }

        /// <summary>
        /// Create from an input byte array from the power line modem.
        /// </summary>
        /// <param name="data">The input data</param>
        public InsteonPacket(byte[] data)
        {
            FromAddress = new DeviceId(data[0], data[1], data[2]);
            ToAddress = new DeviceId(data[3], data[4], data[5]);
            Flags = new InsteonFlags();
            Flags.FromByte(data[6]);
            Command1 = data[7];
            Command2 = data[8];
            UserData = new byte[data.Length - 9];
            Array.Copy(data, 9, UserData, 0, data.Length - 9);
        }
        #endregion

        /// <summary>
        /// The address the packet is from.
        /// </summary>
        public DeviceId FromAddress { get { return fromAddress; } set { this.fromAddress = value; } }
        /// <summary>
        /// The address the packet is to.
        /// </summary>
        public DeviceId ToAddress { get { return toAddress; } set { this.toAddress = value; } }
        /// <summary>
        /// The command.  
        /// </summary>
        public byte Command1 { get { return cmd1; } set { this.cmd1 = value; } }
        /// <summary>
        /// The command.  
        /// </summary>
        public Command StandardCommand { get { return (Command)cmd1; } }
        /// <summary>
        /// The extended command.  
        /// </summary>
        public ExtendedCommand ExtendedCommandData { get { return (ExtendedCommand)((cmd1 << 8) | cmd2); } }
        /// <summary>
        /// Extra data associated with the command.
        /// </summary>
        public byte Command2 { get { return cmd2; } set { this.cmd2 = value; } }
        /// <summary>
        /// User data in an extended format.
        /// </summary>
        public byte[] UserData { get { return userData; } set { userData = value; } }

        /// <summary>
        /// The composed command, based on the parts of command1 and command2.
        /// </summary>
        public Command ComposedCommand {
            get
            {
                return (Command)(((int)cmd1 << 8) | cmd2);
            }
        }

        /// <summary>
        /// Converts this a byte array to send to the modem.
        /// </summary>
        /// <returns>The byte array in the modem format.</returns>
        public byte[] ToByteArray()
        {
            byte[] data;
            if (Flags.ExtendedMessage)
            {
                data = new byte[20];
                UserData.CopyTo(data, 6);
            }
            else
            {
                data = new byte[6];
            }
            data[0] = FromAddress.Address[0];
            data[1] = FromAddress.Address[1];
            data[2] = FromAddress.Address[2];
            data[3] = Flags.ToByte();
            data[4] = (byte)Command1;
            data[5] = Command2;
            return data;
        }

        #region Flags
        public InsteonFlags Flags { get { return flags; } set { this.flags = value; } }

        public class InsteonFlags
        {
            public enum MessageTypeEnum : byte
            {
                Direct = 0x0,
                Broadcast = 0x4,
                GroupBroadcast = 0x6,
                GroupCleanupDirect = 0x2,
                AckDirect = 0x1,
                NackDirect = 0x5,
                AckGroupCleanupDirect = 0x3,
                NackGroupCleanupDirect = 0x7
            }
            public MessageTypeEnum MessageType { get; set; }
            public bool ExtendedMessage { get; set; }
            public byte HopsLeft { get; set; }
            public byte MaxHops { get; set; }

            public byte ToByte()
            {
                byte retValue = (byte)((byte)(MessageType) << 5);
                retValue |= (byte)(ExtendedMessage ? 0x10 : 0);
                retValue |= (byte)((HopsLeft & 0x3) << 2);
                retValue |= (byte)((MaxHops & 0x3) << 0);
                return retValue;
            }

            public void FromByte(byte data)
            {
                MessageType = (MessageTypeEnum)((data >> 5) & 0x7);
                ExtendedMessage = (data & 0x10) != 0;
                HopsLeft = (byte)((data >> 2) & 0x3);
                MaxHops = (byte)((data >> 0) & 0x3);
            }
        }
        #endregion
    }
}

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
    /// Basic device for all devices that can be turned on and off.
    /// </summary>
    public class OnOffDevice : DeviceBase
    {
        private bool hasStatus;
        private byte onLevel;
        private byte rampRate;

        /// <summary>
        /// The constructor for the on/off device.
        /// </summary>
        /// <param name="cms"></param>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <param name="subcategory"></param>
        public OnOffDevice(InsteonCommunication coms, DeviceId id, byte category, byte subcategory)
            : base(coms, id, category, subcategory)
        {
            this.hasStatus = false;
            this.onLevel = 0;
        }

        /// <summary>
        /// If we have the status from the device.
        /// </summary>
        public bool HasStatus { get { return hasStatus; } }

        /// <summary>
        /// Turns the light on quickly.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse setLightOnFast(int level)
        {
            // Need to get the status.
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.LightOnFast, (byte)level);
            if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
            {
                this.hasStatus = true;
                this.onLevel = (byte)level;
            }
            return ret;
        }

        /// <summary>
        /// Turns the light off quickly.
        /// </summary>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse setLightOffFast()
        {
            // Need to get the status.
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.LightOffFast, (byte)0);
            if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
            {
                this.hasStatus = true;
                this.onLevel = 0;
            }
            return ret;
        }

        /// <summary>
        /// Brightens one step.
        /// </summary>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse Brighten()
        {
            // Need to get the status.
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.Bright, (byte)0);
            if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
            {
                if (this.hasStatus)
                {
                    int newLevel = this.onLevel + (256 / 32);
                    if (newLevel > 0xff)
                    {
                        newLevel = 0xff;
                    }
                    this.onLevel = (byte)newLevel;
                }
            }
            return ret;
        }

        /// <summary>
        /// Dims one step.
        /// </summary>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse Dim()
        {
            // Need to get the status.
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.Dim, (byte)0);
            if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
            {
                if (this.hasStatus)
                {
                    int newLevel = this.onLevel - (256 / 32);
                    if (newLevel < 0)
                    {
                        newLevel = 0;
                    }
                    this.onLevel = (byte)newLevel;
                }
            }
            return ret;
        }

        /// <summary>
        /// The current on level of the device.
        /// </summary>
        public int OnLevel
        {
            get
            {
                if (HasStatus)
                {
                    return this.onLevel;
                }
                // Need to get the status.
                byte data;
                Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommandWithResponse(InsteonPacket.Command.LightStatusRequest, (byte)0, out data);
                if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
                {
                    this.hasStatus = true;
                    this.onLevel = data;
                }
                return this.onLevel;
            }
            set
            {
                Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.LightOn, (byte)value);
                if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
                {
                    this.onLevel = (byte)value;
                    this.hasStatus = true;
                }
            }
        }

        /// <summary>
        /// The ramp rate for the device.
        /// </summary>
        public int RampRate
        {
            set
            {
                Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.RatePoke, (byte)value);
                if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
                {
                    this.rampRate = (byte)value;
                }
            }
            get
            {
                return this.rampRate;
            }
        }

        internal override StateChanged handleInsteonPacket(InsteonPacket packet)
        {
            StateChanged changed = null;
            switch (packet.Flags.MessageType)
            {
                case InsteonPacket.InsteonFlags.MessageTypeEnum.GroupBroadcast:
                    InsteonPacket.Command cmd = packet.StandardCommand;
                    switch (cmd)
                    {
                        case InsteonPacket.Command.LightOn:
                        case InsteonPacket.Command.LightOnFast:
                            this.onLevel = 0xff;
                            changed = new StateChanged(this, packet.ToAddress.Address[2], StateChanged.StateThatChanged.TurnedOn);
                            break;
                        case InsteonPacket.Command.LightOff:
                        case InsteonPacket.Command.LightOffFast:
                            this.onLevel = 0;
                            changed = new StateChanged(this, packet.ToAddress.Address[2], StateChanged.StateThatChanged.TurnedOff);
                            break;
                        case InsteonPacket.Command.Dim:
                            changed = new StateChanged(this, packet.ToAddress.Address[2], StateChanged.StateThatChanged.Dim);
                            break;
                        case InsteonPacket.Command.Bright:
                            changed = new StateChanged(this, packet.ToAddress.Address[2], StateChanged.StateThatChanged.Bright);
                            break;
                    }
                    break;
                case InsteonPacket.InsteonFlags.MessageTypeEnum.GroupCleanupDirect:
                    break;
            }
            if (changed == null)
            {
                return base.handleInsteonPacket(packet);
            }
            return changed;
        }
    }
}

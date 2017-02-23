using FluffInsteon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluffInsteon.Devices
{
    /// <summary>
    /// The pool and spa device.
    /// </summary>
    public class PoolAndSpaDevice : DeviceBase
    {
        public enum PoolDevice
        {
            Pool = 1,
            Spa = 2,
            Heat = 3,
            Pump = 4
        }
        public enum PoolMode
        {
            Pool = 0,
            Spa = 1
        }
        private enum PoolControlCommand
        {
            LoadInitializationValues = 0,
            LoadEEPROMFromRAM = 1,
            GetPoolMode = 2,
            GetAmbientTemperature = 3,
            GetWaterTemperature = 4,
            GetPh = 5
        }
        private HashSet<PoolDevice> poolDeviceOn = new HashSet<PoolDevice>();

        public PoolAndSpaDevice(InsteonCommunication coms, DeviceId id, byte category, byte subcategory)
            : base(coms, id, category, subcategory)
        {
        }

        /// <summary>
        /// Turns the specific pool device on.
        /// </summary>
        /// <param name="device">The pool device to turn on</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse TurnPoolDeviceOn(PoolDevice device)
        {
            // Need to get the status.
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.PoolOn, (byte)device);
            if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
            {
                poolDeviceOn.Add(device);
            }
            return ret;
        }

        /// <summary>
        /// Turns the specific pool device off.
        /// </summary>
        /// <param name="device">The pool device to turn off</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse TurnPoolDeviceOff(PoolDevice device)
        {
            // Need to get the status.
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.PoolOff, (byte)device);
            if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
            {
                poolDeviceOn.Remove(device);
            }
            return ret;
        }

        /// <summary>
        /// Change the temperatureof the pool up a few degrees.
        /// </summary>
        /// <param name="changeBy">Number of degrees to change the pool temperature by</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse PoolTemperatureUp(byte changeBy)
        {
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.PoolTemperatureUp, changeBy);
            return ret;
        }

        /// <summary>
        /// Change the temperature of the pool down by a few degrees.
        /// </summary>
        /// <param name="changeBy">Number of degrees to change by</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse PoolTemperatureDown(byte changeBy)
        {
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.PoolTemperatureDown, changeBy);
            return ret;
        }

        /// <summary>
        /// Gets the mode of the pool (spa or pool).
        /// </summary>
        /// <param name="mode">The mode of the pool</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse GetPoolMode(out PoolMode mode)
        {
            byte response;
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommandWithResponse(InsteonPacket.Command.PoolControl, (byte)PoolControlCommand.GetPoolMode, out response);
            mode = (PoolMode)response;
            return ret;
        }

        /// <summary>
        /// Gets the ambient temperature of the air around the pool.
        /// </summary>
        /// <param name="temperature">The temperature of the air around the spa</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse GetAmbientTemperature(out byte temperature)
        {
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommandWithResponse(InsteonPacket.Command.PoolControl, (byte)PoolControlCommand.GetAmbientTemperature, out temperature);
            return ret;
        }

        /// <summary>
        /// Gets the temperature of the water in the pool or spa.
        /// </summary>
        /// <param name="temperature">The water temperature</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse GetWaterTemperature(out byte temperature)
        {
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommandWithResponse(InsteonPacket.Command.PoolControl, (byte)PoolControlCommand.GetWaterTemperature, out temperature);
            return ret;
        }

        /// <summary>
        /// Gets the ph of the water.
        /// </summary>
        /// <param name="ph">The ph of the water in pool or spa</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse GetPh(out byte ph)
        {
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommandWithResponse(InsteonPacket.Command.PoolControl, (byte)PoolControlCommand.GetWaterTemperature, out ph);
            return ret;
        }
    }
}

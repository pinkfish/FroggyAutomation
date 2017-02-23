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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FroggyPlugin.Data;
using System.Runtime.Serialization;
using log4net;

namespace FroggyAutomation.Database
{
    /// <summary>
    /// Persists the whole thing to a file on the disk.
    /// </summary>
    internal class FileDatabase : Database
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FileDatabase).Name);
        private string filename;
        private FileDatabaseData data;
        BinaryFormatter serializer = new BinaryFormatter();
  
        public FileDatabase(string filename)
        {
            this.filename = filename;
            ReadFile();
        }

        private void ReadFile()
        {
            if (File.Exists(filename))
            {
                FileStream output = new FileStream(filename, FileMode.OpenOrCreate);
                try
                {
                    object data = serializer.Deserialize(output);
                    if (data is FileDatabaseData)
                    {
                        this.data = (FileDatabaseData)data;
                    }
                }
                catch (SerializationException e)
                {
                    log.WarnFormat("Failed to deserialize from the file {0} - {1}", filename, e);
                }
                output.Close();
            }
            else
            {
                this.data = new FileDatabaseData();
            }
        }

        private void WriteFile()
        {
            FileStream output = new FileStream(filename, FileMode.Truncate);
            serializer.Serialize(output, data);
            output.Close();
        }

        public Configuration Config
        {
            get
            {
                return data.Config;
            }
            set
            {
                data.Config = value;
                WriteFile();
            }
        }

        public IEnumerable<FroggyPlugin.Data.BasicDevice> Devices
        {
            get { return data.Devices.Values; }
        }

        public void AddOrUpdateDevice(FroggyPlugin.Data.BasicDevice device)
        {
            data.Devices[device.Address] = device;
            WriteFile();
        }

        public FroggyPlugin.Data.BasicDevice FindDevice(FroggyPlugin.Data.DeviceReference reference)
        {
            if (data.Devices.ContainsKey(reference))
            {
                return data.Devices[reference];
            }
            return null;
        }

        public FroggyPlugin.PluginConfig GetPluginConfig(Type type)
        {
            return data.PluginConfig[type];
        }

        public void SetPluginConfig(FroggyPlugin.PluginConfig config, Type type)
        {
            data.PluginConfig[type] = config;
            WriteFile();
        }

        /// <summary>
        /// Dispose the system.
        /// </summary>
        public void Dispose()
        {
            data = null;
            filename = null;
            serializer = null;
        }
    }
}

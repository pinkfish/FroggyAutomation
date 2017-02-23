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

namespace FroggyAutomation.Database
{
    /// <summary>
    /// Configuration for the class.
    /// </summary>
    [Serializable]
    public class Configuration
    {
        /// <summary>
        /// Creates an empty config file.
        /// </summary>
        public Configuration()
        {
            this.WebServerPort = Properties.Settings.Default.WebPort;
        }

        /// <summary>
        /// The port the web server runs on.
        /// </summary>
        public int WebServerPort { get; set; }

        /// <summary>
        /// Merge in the data from the other object.
        /// </summary>
        /// <param name="config">The object to merge in</param>
        public void Merge(Configuration config)
        {
            this.WebServerPort = config.WebServerPort;
        }
    }
}

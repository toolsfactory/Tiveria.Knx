﻿/*
    Tiveria.Knx - a .Net Core base KNX library
    Copyright (c) 2018 M. Geissler

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU General Public License cover the whole
    combination.
*/

using System;
using Tiveria.Common.IO;
using Tiveria.Knx.IP.Structures;
using Tiveria.Knx.IP.Utils;
using Tiveria.Knx.Exceptions;

namespace Tiveria.Knx.IP.ServiceTypes
{
    /// <summary>
    /// <code>
    /// +--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 |
    /// +--------+--------+--------+--------+
    /// | Channel|Reserved| HPAI            |
    /// | ID     | 0x00   | Control Endpoint|
    /// +--------+--------+-----------------+
    /// </code>
    /// </summary>
    public class DisconnectRequest : ServiceTypeBase
    {
        #region private fields
        private Hpai _controlEndpoint;
        private byte _channelId;
        #endregion

        #region public properties
        public Hpai ControlEndpoint => _controlEndpoint;
        public byte ChannelId => _channelId;
        #endregion

        #region constructors
        protected DisconnectRequest() : base(ServiceTypeIdentifier.DISCONNECT_REQ)
        { }

        public DisconnectRequest(byte channelId, Hpai controlEndpoint)
            : this()
        {
            if (controlEndpoint == null)
                throw new ArgumentNullException("Controlendpoint");
            _controlEndpoint = controlEndpoint;
            _channelId = channelId;
        }

        protected DisconnectRequest(IndividualEndianessBinaryReader br)
            : this()
        {
            ParseChannelId(br);
            SkipReserved(br);
            ParseHpai(br);
        }
        #endregion

        #region parsing
        private void ParseChannelId(IndividualEndianessBinaryReader br)
        {
            _channelId = br.ReadByte();
        }

        private void ParseHpai(IndividualEndianessBinaryReader br)
        {
            _controlEndpoint = Hpai.Parse(br);
        }
        #endregion

        public override void WriteToByteArray(byte[] buffer, int offset = 0)
        {
            base.WriteToByteArray(buffer, offset);
            buffer[offset] = ChannelId;
            buffer[offset + 1] = 0x00; //Reserved;
            _controlEndpoint.WriteToByteArray(buffer, offset + 2);
        }
    }
    /// <summary>
    /// <code>
    /// +--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 |
    /// +--------+--------+--------+--------+
    /// | Channel|Reserved| HPAI            |
    /// | ID     | 0x00   | Control Endpoint|
    /// +--------+--------+-----------------+
    /// </code>
    /// </summary>
    public class DisconnectResponse : ServiceTypeBase, IServiceType
    {
        #region private fields
        private ErrorCodes _status;
        private byte _channelId;
        #endregion

        #region public properties
        public ErrorCodes Status => _status;
        public byte ChannelId => _channelId;
        #endregion

        #region constructors
        protected DisconnectResponse() : base(ServiceTypeIdentifier.DISCONNECT_RES)
        { }

        public DisconnectResponse(byte channelId, ErrorCodes status)
            : this()
        {
            _status = status;
            _channelId = channelId;
        }

        protected DisconnectResponse(IndividualEndianessBinaryReader br)
            : this()
        {
            ParseChannelId(br);
            ParseStatus(br);
        }
        #endregion

        #region parsing
        private void ParseChannelId(IndividualEndianessBinaryReader br)
        {
            _channelId = br.ReadByte();
        }

        private void ParseStatus(IndividualEndianessBinaryReader br)
        {
            var status = br.ReadByte();
            if (!Enum.IsDefined(typeof(ErrorCodes), status))
                throw BufferFieldException.TypeUnknown("Status", status);
            _status = (ErrorCodes)status;
        }
        #endregion

        public override void WriteToByteArray(byte[] buffer, int offset = 0)
        {
            base.WriteToByteArray(buffer, offset);
            buffer[offset] = ChannelId;
            buffer[offset + 1] = (byte)_status;
        }
    }
}

using H9e.Core.Compress;
using System;
using System.Collections.Generic;
using System.Text;

namespace H9e.Tcp.Packet {

    public interface IH9eTcpPacket {
        byte[] ToBytes();
    }

}

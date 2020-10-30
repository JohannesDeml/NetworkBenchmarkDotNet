using System;
using System.Collections.Generic;
using System.Text;

namespace ElfhildNet
{
    public static class NetConstants
    {
        //can be tuned
        public const int WindowSize = 8000;
        public const int HalfWindowSize = WindowSize / 2;

        public const ushort MaxSequence = 32768;
        public const ushort HalfMaxSequence = MaxSequence / 2;

        //protocol
        internal const int ProtocolId = 11;
        internal const int MaxUdpHeaderSize = 68;

        internal static readonly int[] PossibleMtu =
        {
            576  - MaxUdpHeaderSize, //minimal
            1232 - MaxUdpHeaderSize,
            1460 - MaxUdpHeaderSize, //google cloud
            1472 - MaxUdpHeaderSize, //VPN
            1492 - MaxUdpHeaderSize, //Ethernet with LLC and SNAP, PPPoE (RFC 1042)
            1500 - MaxUdpHeaderSize,  //Ethernet II (RFC 1191),
            3000 - MaxUdpHeaderSize
        };

        internal static readonly int MaxPacketSize = PossibleMtu[PossibleMtu.Length - 1];

        //peer specific
        public const byte MaxConnectionNumber = 4;

        public const int PacketPoolSize = 1000;
    }
}

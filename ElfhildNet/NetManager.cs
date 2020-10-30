using System;
using System.Collections.Generic;
using System.Text;
using NanoSockets;

namespace ElfhildNet
{
    public enum PacketType
    {
        Unreliable,
        Reliable,
        ConnectRequest,
        ConnectionDenied,
        Disconnected,
        Ping,
        Pong,
        Ack,
        MtuRequest,
        MtuResponse,
    }

    public sealed class NetManager
    {
        private Connection First;
        private Socket UdpSocketv4;
        private readonly Dictionary<Address, Connection> Connections = new Dictionary<Address, Connection>(new AddressComparer());
        public bool IsRunning
        {
            get
            {
                return isRunning && First != null;
            }
        }

        bool isRunning = true;

        private class AddressComparer : IEqualityComparer<Address>
        {
            public bool Equals(Address x, Address y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(Address obj)
            {
                return obj.GetHashCode();
            }
        }

        public event Action<Func<Connection>, Action, string> ConnectionRequestEvent;

        public void Poll()
        {
            Address address = new Address();

            ByteBuffer buffer = ByteBuffer.Allocate();

            while (UDP.Poll(UdpSocketv4, 0) > 0)
            {
                int count = 0;

                try
                {
                    if ((count = UDP.Receive(UdpSocketv4, ref address, buffer.data, buffer.data.Length)) > 0)
                    {
                        buffer.size = count;
                        buffer.position = 0;

                        PacketType type = (PacketType)(buffer.GetByte());

                        switch (type)
                        {
                            case PacketType.ConnectRequest:
                                if (!Connections.ContainsKey(address))
                                {
                                    string token = buffer.GetString();

                                    Connection conn = new Connection()
                                    {
                                        RemoteEndPoint = address,
                                        UdpSocketv4 = UdpSocketv4,
                                        State = ConnectionState.Connected
                                    };

                                    Connections.Add(address, conn);

                                    ConnectionRequestEvent?.Invoke(() =>
                                    {
                                        conn.Next = First;

                                        First = conn;

                                        conn.PushAck(1);

                                        return conn;
                                    }, () =>
                                    {
                                        ByteBuffer response = ByteBuffer.Allocate();

                                        response.Put((byte)PacketType.ConnectionDenied);

                                        UDP.Send(UdpSocketv4, ref address, response.data, response.position);

                                        ByteBuffer.Deallocate(response);
                                    }, token);
                                }
                                break;
                            case PacketType.Disconnected:
                                {
                                    Connection conn;

                                    if (Connections.TryGetValue(address, out conn))
                                    {
                                        switch (conn.State)
                                        {
                                            case ConnectionState.Connecting:
                                            case ConnectionState.Connected:
                                                conn.OnDisconnect();
                                                break;
                                        }
                                    }
                                }
                                break;
                            default:
                                {
                                    Connection conn;

                                    if (Connections.TryGetValue(address, out conn))
                                    {
                                        switch (conn.State)
                                        {
                                            case ConnectionState.Connecting:
                                                conn.State = ConnectionState.Connected;
                                                conn.ProcessPacket(type, buffer);
                                                break;
                                            case ConnectionState.Connected:
                                                conn.ProcessPacket(type, buffer);
                                                break;
                                        }
                                    }
                                }
                                break;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Connection conn;

                    if (Connections.TryGetValue(address, out conn))
                    {
                        switch (conn.State)
                        {
                            case ConnectionState.Connecting:
                            case ConnectionState.Connected:
                                conn.OnDisconnect();
                                break;
                        }
                    }
                }
            }

            ByteBuffer.Deallocate(buffer);
        }

        public void Update(float delta)
        {
        begin:
            if (First != null)
            {
                if (First.Update(delta))
                {
                    Connections.Remove(First.RemoteEndPoint);

                    First = First.Next;

                    goto begin;
                }

                Connection before = First;

                Connection conn = First.Next;

                while (conn != null)
                {
                    if (conn.Update(delta))
                    {
                        before.Next = conn.Next;

                        Connections.Remove(conn.RemoteEndPoint);
                    }

                    before = conn;

                    conn = conn.Next;
                }
            }
        }

        public Connection Connect(string address, int port, string token)
        {
            UdpSocketv4 = UDP.Create(512 * 1024, 512 * 1024);

            Address connectionAddress = new Address();

            connectionAddress.Port = (ushort)port;

            UDP.SetIP(ref connectionAddress, address);

            Connection conn = new Connection()
            {
                RemoteEndPoint = connectionAddress,
                UdpSocketv4 = UdpSocketv4,
                State = ConnectionState.Connecting
            };

            First = conn;

            Connections.Add(connectionAddress, First);

            UDP.Connect(UdpSocketv4, ref connectionAddress);

            var connectionPacket = ByteBuffer.Allocate();

            connectionPacket.Put((byte)PacketType.ConnectRequest);

            connectionPacket.Put(token);

            conn.ReliablePackets.Add(1, connectionPacket);

            return conn;
        }

        public void Start(int port)
        {
            Address listenAddress = new Address();

            listenAddress.Port = (ushort)port;

            UdpSocketv4 = UDP.Create(512 * 1024, 512 * 1024);

            UDP.SetIP(ref listenAddress, "::0");

            UDP.Bind(UdpSocketv4, ref listenAddress);
        }

        public void Stop()
        {
            isRunning = false;

            if (First != null)
            {
                First.Disconnect();

                Connection conn = First.Next;

                while (conn != null)
                {
                    conn.Disconnect();

                    conn = conn.Next;
                }
            }
        }
    }
}

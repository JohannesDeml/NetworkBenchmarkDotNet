using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NanoSockets;

namespace ElfhildNet
{
    public enum ConnectionState
    {
        Connecting,
        Connected,
        Disconnected
    }

    public class Connection
    {

        internal ByteBuffer ReliableBuffer;
        internal ByteBuffer UnreliableBuffer;
        internal ByteBuffer AckBuffer;

        public ByteBuffer Current;

        public Address RemoteEndPoint;
        public Socket UdpSocketv4;

        internal Dictionary<short, ByteBuffer> ReliablePackets = new Dictionary<short, ByteBuffer>();

        private short Sequence = 1;
        private BitArray Window = new BitArray(NetConstants.WindowSize, false);

        private float timeout = 5.0f;
        private float reliableTimeout = 0.1f;
        private float pingTimeout = 1.0f;
        private DateTime pingSentAt = DateTime.UtcNow;

        private int Rtt = 50;
        /*

        private float mtuTimeout = 1.0f;
        private int mtuCheckAttempts;

        private const int MaxMtuCheckAttempts = 8;
        */

        private const int Mtu = 1232 - 68;

        public int Ping
        {
            get
            {
                return Rtt / 2;
            }
        }

        private Stack<KeyValuePair<short, ByteBuffer>> FrameReliablePackets = new Stack<KeyValuePair<short, ByteBuffer>>();

        public Connection Next;

        public event Action Disconnected;
        public event Action<ByteBuffer> PacketReceived;


        public ConnectionState State;

        public void BeginReliable()
        {
#if DEBUG
            if (Current != null) throw new NotSupportedException();
#endif

            if (ReliableBuffer == null)
            {
                ReliableBuffer = ByteBuffer.Allocate();

                Sequence = (short)((Sequence + 1) % NetConstants.WindowSize);

                ReliableBuffer.Put((byte)PacketType.Reliable);
                ReliableBuffer.Put(Sequence);

                FrameReliablePackets.Push(new KeyValuePair<short, ByteBuffer>(Sequence, ReliableBuffer));
            }

            Current = ReliableBuffer;
        }

        public void BeginReliableImportant()
        {
#if DEBUG
            if (Current != null) throw new NotSupportedException();
#endif
            Current = ByteBuffer.Allocate();

            Sequence = (short)((Sequence + 1) % NetConstants.WindowSize);

            Current.Put((byte)PacketType.Reliable);
            Current.Put(Sequence);

            FrameReliablePackets.Push(new KeyValuePair<short, ByteBuffer>(Sequence, Current));
        }

        public void PushAck(short sequence)
        {
            if (AckBuffer == null)
            {
                AckBuffer = ByteBuffer.Allocate();
                AckBuffer.Put((byte)PacketType.Ack);
            }

            AckBuffer.Put(sequence);

            if (AckBuffer.position > (Mtu - 150))
            {
                Send(AckBuffer);

                ByteBuffer.Deallocate(AckBuffer);

                AckBuffer = null;
            }
        }

        public void EndReliable()
        {
#if DEBUG
            if (Current != ReliableBuffer) throw new NotSupportedException();
#endif


            if (ReliableBuffer.position > (Mtu - 150))
            {
                ReliableBuffer = null;
            }


            Current = null;
        }


        public void EndReliableImportant()
        {
#if DEBUG
            if (Current == ReliableBuffer || Current == UnreliableBuffer || Current == null) throw new NotSupportedException();
#endif

            Current = null;
        }

        public void BeginUnreliable()
        {
#if DEBUG
            if (Current != null) throw new NotSupportedException();
#endif
            if (UnreliableBuffer == null)
            {
                UnreliableBuffer = ByteBuffer.Allocate();

                UnreliableBuffer.Put((byte)PacketType.Unreliable);
            }

            Current = UnreliableBuffer;
        }

        private void Send(ByteBuffer buffer)
        {
            UDP.Send(UdpSocketv4, ref RemoteEndPoint, buffer.data, buffer.position);
        }

        public void EndUnreliable()
        {
#if DEBUG
            if (Current != UnreliableBuffer) throw new NotSupportedException();
#endif

            if (UnreliableBuffer.position > (Mtu - 150))
            {

                Send(UnreliableBuffer);

                ByteBuffer.Deallocate(UnreliableBuffer);

                UnreliableBuffer = null;
            }

            Current = null;
        }

        public void ProcessPacket(PacketType type,  ByteBuffer buffer)
        {
            timeout = 5.0f;

            switch (type)
            {
                case PacketType.Pong:
                    {
                        Rtt = (Rtt + (int)(DateTime.UtcNow - pingSentAt).Milliseconds) / 2;
                    }
                    break;
                case PacketType.Ping:
                    {
                        ByteBuffer bbufer = ByteBuffer.Allocate();

                        bbufer.Put((byte)PacketType.Pong);

                        Send(bbufer);

                        ByteBuffer.Deallocate(bbufer);
                    }
                    break;/*
                case PacketType.MtuResponse:
                    {
                        int idx = buffer.GetInt();
                        bool success = buffer.GetBool();

                        if (success)
                        {
                            if (idx > MtuPosition)
                            {

                                if (idx == NetConstants.PossibleMtu.Length - 1)
                                {
                                    mtuCheckAttempts = MaxMtuCheckAttempts;
                                }
                                else
                                {
                                    mtuTimeout = 1.0f;
                                    mtuCheckAttempts = 0;
                                    MtuPosition = idx;
                                }

                                Mtu = NetConstants.PossibleMtu[idx];

                                Console.WriteLine("Mtu: " + Mtu);
                            }
                        }
                        else
                        {
                            mtuCheckAttempts = MaxMtuCheckAttempts;
                        }
                    }
                    break;
                case PacketType.MtuRequest:
                    {
                        int idx = buffer.GetInt();

                        if(idx < NetConstants.PossibleMtu.Length)
                        {
                            buffer.position += (NetConstants.PossibleMtu[idx] - 9);

                            if(buffer.GetInt() != idx)
                            {
                                ByteBuffer bbufer = ByteBuffer.Allocate();

                                bbufer.Put((byte)PacketType.MtuResponse);
                                bbufer.Put(idx);
                                bbufer.Put(false);

                                Send(bbufer);

                                ByteBuffer.Deallocate(bbufer);
                            }
                            else
                            {
                                ByteBuffer bbufer = ByteBuffer.Allocate();

                                bbufer.Put((byte)PacketType.MtuResponse);
                                bbufer.Put(idx);
                                bbufer.Put(true);

                                Send(bbufer);

                                ByteBuffer.Deallocate(bbufer);
                            }
                        }
                        else
                        {
                            ByteBuffer bbufer = ByteBuffer.Allocate();

                            bbufer.Put((byte)PacketType.MtuResponse);
                            bbufer.Put(idx);
                            bbufer.Put(false);

                            Send(bbufer);

                            ByteBuffer.Deallocate(bbufer);
                        }

                    }
                    break;
                    */
                case PacketType.Ack:
                    while (buffer.HasData)
                    {
                        short sequence = buffer.GetShort();

                        ByteBuffer bbuffer;

#if UNITY_5_3_OR_NEWER
                        if (ReliablePackets.TryGetValue(sequence, out bbuffer))
                        {
                            ReliablePackets.Remove(sequence);
                            ByteBuffer.Deallocate(bbuffer);
          
                        }
#else

                        if (ReliablePackets.Remove(sequence, out bbuffer))
                        {
                            ByteBuffer.Deallocate(bbuffer);
                        }
#endif
                    }
                    break;
                case PacketType.Reliable:
                    {
                        short sequence = buffer.GetShort();

                        if (!Window.Get(sequence))
                        {
                            Window.Set(sequence, true);

                            Window.Set(Math.Abs((sequence - NetConstants.HalfWindowSize) % NetConstants.WindowSize), false);

                            PushAck(sequence);

                            PacketReceived?.Invoke(buffer);
                        }
                    }
                    break;
                case PacketType.Unreliable:
                    {
                        PacketReceived?.Invoke(buffer);
                    }
                    break;
            }
        }

        public bool Update(float delta)
        {
            if (State == ConnectionState.Disconnected)
                return true;

            timeout -= delta;

            if (timeout <= 0)
            {
                OnDisconnect();

                return true;
            }

            pingTimeout -= delta;

            if (pingTimeout <= 0f)
            {
                ByteBuffer pingBuffer = ByteBuffer.Allocate();

                pingBuffer.Put((byte)PacketType.Ping);

                Send(pingBuffer);

                ByteBuffer.Deallocate(pingBuffer);

                pingTimeout = 1.5f;

                pingSentAt = DateTime.UtcNow;
            }

            /*

            if (mtuCheckAttempts < MaxMtuCheckAttempts)
            {
                mtuTimeout -= delta;

                if(mtuTimeout <= 0f)
                {
                    mtuCheckAttempts += 1;

                    ByteBuffer mtuBuffer = ByteBuffer.Allocate();

                    mtuBuffer.Put((byte)PacketType.MtuRequest);

                    mtuBuffer.Put((int)MtuPosition + 1);

                    mtuBuffer.position += (NetConstants.PossibleMtu[MtuPosition + 1] - 9);

                    mtuBuffer.Put((int)MtuPosition + 1);

                    Send(mtuBuffer);

                    ByteBuffer.Deallocate(mtuBuffer);

                    mtuTimeout = 1.0f;
                }
            }

            */

#if DEBUG
            if (ReliablePackets.Count > 200)
            {
                Console.WriteLine("Reliable Packet Count is too High: " + ReliablePackets.Count);
            }
#endif   

            if (reliableTimeout <= 0)
            {
                foreach (ByteBuffer buffer in ReliablePackets.Values)
                {
                    Send(buffer);
                }

                reliableTimeout = 0.1f;
            }
            else
            {
                reliableTimeout -= delta;
            }

            if (ReliableBuffer != null)
            {
                ReliableBuffer = null;
            }

            if (UnreliableBuffer != null)
            {
                Send(UnreliableBuffer);

                ByteBuffer.Deallocate(UnreliableBuffer);

                UnreliableBuffer = null;
            }

            if (AckBuffer != null)
            {
                Send(AckBuffer);

                ByteBuffer.Deallocate(AckBuffer);

                AckBuffer = null;
            }

            while(FrameReliablePackets.Count > 0)
            {
                KeyValuePair<short, ByteBuffer> kv = FrameReliablePackets.Pop();

                Send(kv.Value);

                ReliablePackets.Add(kv.Key, kv.Value);
            }

            return false;
        }

        public void Disconnect()
        {
            ByteBuffer response = ByteBuffer.Allocate();

            response.Put((byte)PacketType.Disconnected);

            Send(response);

            ByteBuffer.Deallocate(response);

            OnDisconnect();
        }

        internal void OnDisconnect()
        {
            State = ConnectionState.Disconnected;

            foreach(ByteBuffer buffer in ReliablePackets.Values)
            {
                ByteBuffer.Deallocate(buffer);
            }

            ReliablePackets.Clear();

            while (FrameReliablePackets.Count > 0)
            {
                KeyValuePair<short, ByteBuffer> kv = FrameReliablePackets.Pop();

                ByteBuffer.Deallocate(kv.Value);
            }

            Disconnected?.Invoke();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ElfhildNet
{
    public class ByteBuffer
    {
        static ByteBuffer Pool;
        static int PoolSize;
		static object locker = new object();

        public static ByteBuffer Allocate()
        {
			lock (locker)
			{
				if (Pool == null)
				{
					return new ByteBuffer();
				}
				else
				{
					ByteBuffer val = Pool;

					Pool = Pool.Link;

					val.Link = null;

					PoolSize--;

					return val;
				}
			}
        }

        public bool HasData
        {
            get { return position < size; }
        }

        public void Reset()
        {
            position = 0;
            size = 0;
        }

        public static void Deallocate(ByteBuffer buffer)
        {
			lock (locker)
			{
				if (PoolSize < 5000)
				{
					PoolSize++;

					buffer.Reset();

					buffer.Link = Pool;

					Pool = buffer;
				}
			}
        }

        private ByteBuffer()
        {
            data = new byte[1500];
        }

        public byte[] data;
        public int position;
        public int size;


        ByteBuffer Link;

        public void ResizeIfNeed(int newSize)
        {
            int len = data.Length;
            if (len < newSize)
            {
               while (len < newSize)
                    len *= 2;
                Array.Resize(ref data, len);
            }
        }


        public void Put(float value)
        {

                ResizeIfNeed(position + 4);
            FastBitConverter.GetBytes(data, position, value);
            position += 4;
        }

        public void Put(double value)
        {

                ResizeIfNeed(position + 8);
            FastBitConverter.GetBytes(data, position, value);
            position += 8;
        }

        public void Put(long value)
        {
            ResizeIfNeed(position + 8);
            FastBitConverter.GetBytes(data, position, value);
            position += 8;
        }

        public void Put(ulong value)
        {
            ResizeIfNeed(position + 8);
            FastBitConverter.GetBytes(data, position, value);
            position += 8;
        }

        public void Put(int value)
        {

                ResizeIfNeed(position + 4);
            FastBitConverter.GetBytes(data, position, value);
            position += 4;
        }

        public void Put(uint value)
        {

                ResizeIfNeed(position + 4);
            FastBitConverter.GetBytes(data, position, value);
            position += 4;
        }

        public void Put(char value)
        {

                ResizeIfNeed(position + 2);
            FastBitConverter.GetBytes(data, position, value);
            position += 2;
        }

        public void Put(ushort value)
        {

                ResizeIfNeed(position + 2);
            FastBitConverter.GetBytes(data, position, value);
            position += 2;
        }

        public void Put(short value)
        {

                ResizeIfNeed(position + 2);
            FastBitConverter.GetBytes(data, position, value);
            position += 2;
        }

        public void Put(sbyte value)
        {

                ResizeIfNeed(position + 1);
            data[position] = (byte)value;
            position++;
        }

        public void Put(byte value)
        {

                ResizeIfNeed(position + 1);
            data[position] = value;
            position++;
        }

        public void Put(byte[] data, int offset, int length)
        {

                ResizeIfNeed(position + length);
            Buffer.BlockCopy(data, offset, data, position, length);
            position += length;
        }

        public void Put(byte[] data)
        {
           ResizeIfNeed(position + data.Length);
            Buffer.BlockCopy(data, 0, data, position, data.Length);
            position += data.Length;
        }

        public void PutSBytesWithLength(sbyte[] data, int offset, int length)
        {

            ResizeIfNeed(position + length + 4);
            FastBitConverter.GetBytes(this.data, position, length);
            Buffer.BlockCopy(data, offset, data, position + 4, length);
            position += length + 4;
        }

        public void PutSBytesWithLength(sbyte[] data)
        {

            ResizeIfNeed(position + data.Length + 4);
            FastBitConverter.GetBytes(this.data, position, data.Length);
            Buffer.BlockCopy(data, 0, data, position + 4, data.Length);
            position += data.Length + 4;
        }

        public void PutBytesWithLength(byte[] data, int offset, int length)
        {
            FastBitConverter.GetBytes(this.data, position, length);
            Buffer.BlockCopy(data, offset, this.data, position + 4, length);
            position += length + 4;
        }

		public void PutBytes(byte[] data, int offset, int length)
		{
			Buffer.BlockCopy(data, offset, this.data, position, length);
			this.position += length;
		}

		public void PutBytesWithLength(byte[] data)
        {
            ResizeIfNeed(position + data.Length + 4);
            FastBitConverter.GetBytes(this.data, position, data.Length);
            Buffer.BlockCopy(data, 0, this.data, position + 4, data.Length);
            position += data.Length + 4;
        }

        public void Put(bool value)
        {

                ResizeIfNeed(position + 1);
            data[position] = (byte)(value ? 1 : 0);
            position++;
        }

        private void PutArray(Array arr, int sz)
        {
            ushort length = arr == null ? (ushort)0 : (ushort)arr.Length;
            sz *= length;

                ResizeIfNeed(position + sz + 2);
            FastBitConverter.GetBytes(data, position, length);
            if (arr != null)
                Buffer.BlockCopy(arr, 0, data, position + 2, sz);
            position += sz + 2;
        }

        public void PutArray(float[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(double[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(long[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(ulong[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(int[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(uint[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(ushort[] value)
        {
            PutArray(value, 2);
        }

        public void PutArray(short[] value)
        {
            PutArray(value, 2);
        }

        public void PutArray(bool[] value)
        {
            PutArray(value, 1);
        }

        public void PutArray(string[] value)
        {
            ushort len = value == null ? (ushort)0 : (ushort)value.Length;
            Put(len);
            for (int i = 0; i < len; i++)
                Put(value[i]);
        }

        public void PutArray(string[] value, int maxLength)
        {
            ushort len = value == null ? (ushort)0 : (ushort)value.Length;
            Put(len);
            for (int i = 0; i < len; i++)
                Put(value[i], maxLength);
        }

        public void Put(IPEndPoint endPoint)
        {
            Put(endPoint.Address.ToString());
            Put(endPoint.Port);
        }

        public void Put(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Put(0);
                return;
            }

            //put bytes count
            int bytesCount = Encoding.UTF8.GetByteCount(value);

                ResizeIfNeed(position + bytesCount + 4);
            Put(bytesCount);

            //put string
            Encoding.UTF8.GetBytes(value, 0, value.Length, data, position);
            position += bytesCount;
        }

        public void Put(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                Put(0);
                return;
            }

            int length = value.Length > maxLength ? maxLength : value.Length;
            //calculate max count
            int bytesCount = Encoding.UTF8.GetByteCount(value);

                ResizeIfNeed(position + bytesCount + 4);

            //put bytes count
            Put(bytesCount);

            //put string
            Encoding.UTF8.GetBytes(value, 0, length, data, position);

            position += bytesCount;
        }

        public byte GetByte()
        {
            byte res = this.data[this.position];
            this.position += 1;
            return res;
        }

        public sbyte GetSByte()
        {
            var b = (sbyte)this.data[this.position];
            this.position++;
            return b;
        }

        public bool[] GetBoolArray()
        {
            ushort size = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            var arr = new bool[size];
            Buffer.BlockCopy(this.data, this.position, arr, 0, size);
            this.position += size;
            return arr;
        }

        public ushort[] GetUShortArray()
        {
            ushort size = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            var arr = new ushort[size];
            Buffer.BlockCopy(this.data, this.position, arr, 0, size * 2);
            this.position += size * 2;
            return arr;
        }

        public short[] GetShortArray()
        {
            ushort size = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            var arr = new short[size];
            Buffer.BlockCopy(this.data, this.position, arr, 0, size * 2);
            this.position += size * 2;
            return arr;
        }

        public long[] GetLongArray()
        {
            ushort size = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            var arr = new long[size];
            Buffer.BlockCopy(this.data, this.position, arr, 0, size * 8);
            this.position += size * 8;
            return arr;
        }



        public ulong[] GetULongArray()
        {
            ushort size = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            var arr = new ulong[size];
            Buffer.BlockCopy(this.data, this.position, arr, 0, size * 8);
            this.position += size * 8;
            return arr;
        }

        public int[] GetIntArray()
        {
            ushort size = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            var arr = new int[size];
            Buffer.BlockCopy(this.data, this.position, arr, 0, size * 4);
            this.position += size * 4;
            return arr;
        }

        public uint[] GetUIntArray()
        {
            ushort size = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            var arr = new uint[size];
            Buffer.BlockCopy(this.data, this.position, arr, 0, size * 4);
            this.position += size * 4;
            return arr;
        }

        public float[] GetFloatArray()
        {
            ushort size = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            var arr = new float[size];
            Buffer.BlockCopy(this.data, this.position, arr, 0, size * 4);
            this.position += size * 4;
            return arr;
        }

        public double[] GetDoubleArray()
        {
            ushort size = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            var arr = new double[size];
            Buffer.BlockCopy(this.data, this.position, arr, 0, size * 8);
            this.position += size * 8;
            return arr;
        }

        public string[] GetStringArray()
        {
            ushort size = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            var arr = new string[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetString();
            }
            return arr;
        }

        public string[] GetStringArray(int maxStringLength)
        {
            ushort size = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            var arr = new string[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetString(maxStringLength);
            }
            return arr;
        }

        public bool GetBool()
        {
            bool res = this.data[this.position] > 0;
            this.position += 1;
            return res;
        }

        public char GetChar()
        {
            char result = BitConverter.ToChar(this.data, this.position);
            this.position += 2;
            return result;
        }

        public ushort GetUShort()
        {
            ushort result = BitConverter.ToUInt16(this.data, this.position);
            this.position += 2;
            return result;
        }

        public short GetShort()
        {
            short result = BitConverter.ToInt16(this.data, this.position);
            this.position += 2;
            return result;
        }

        public long GetLong()
        {
            long result = BitConverter.ToInt64(this.data, this.position);
            this.position += 8;
            return result;
        }

        public ulong GetULong()
        {
            ulong result = BitConverter.ToUInt64(this.data, this.position);
            this.position += 8;
            return result;
        }

        public int GetInt()
        {
            int result = BitConverter.ToInt32(this.data, this.position);
            this.position += 4;
            return result;
        }

        public uint GetUInt()
        {
            uint result = BitConverter.ToUInt32(this.data, this.position);
            this.position += 4;
            return result;
        }

        public float GetFloat()
        {
            float result = BitConverter.ToSingle(this.data, this.position);
            this.position += 4;
            return result;
        }

        public double GetDouble()
        {
            double result = BitConverter.ToDouble(this.data, this.position);
            this.position += 8;
            return result;
        }
        public string GetString(int maxLength)
        {
            int bytesCount = GetInt();
            if (bytesCount <= 0 || bytesCount > maxLength * 2)
            {
                return string.Empty;
            }

            int charCount = Encoding.UTF8.GetCharCount(this.data, this.position, bytesCount);
            if (charCount > maxLength)
            {
                return string.Empty;
            }

            string result = Encoding.UTF8.GetString(this.data, this.position, bytesCount);
            this.position += bytesCount;
            return result;
        }

        public string GetString()
        {
            int bytesCount = GetInt();
            if (bytesCount <= 0 || bytesCount > (size - position))
            {
                return string.Empty;
            }

            string result = Encoding.UTF8.GetString(this.data, this.position, bytesCount);
            this.position += bytesCount;
            return result;
        }

        public void GetBytes(byte[] destination, int start, int count)
        {
            Buffer.BlockCopy(this.data, this.position, destination, start, count);
            this.position += count;
        }

        public void GetBytes(byte[] destination, int count)
        {
            Buffer.BlockCopy(this.data, this.position, destination, 0, count);
            this.position += count;
        }

        public sbyte[] GetSBytesWithLength()
        {
            int length = GetInt();
            sbyte[] outgoingData = new sbyte[length];
            Buffer.BlockCopy(this.data, this.position, outgoingData, 0, length);
            this.position += length;
            return outgoingData;
        }

        public byte[] GetBytesWithLength()
        {
            int length = GetInt();
            byte[] outgoingData = new byte[length];
            Buffer.BlockCopy(this.data, this.position, outgoingData, 0, length);
            this.position += length;
            return outgoingData;
        }

    }
}

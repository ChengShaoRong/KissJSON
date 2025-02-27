/*
 * KissJson : Keep It Simple Stupid JSON
 * Copyright © 2022-2025 RongRong. All right reserved.
 */
using System.Text;
using System;

namespace CSharpLike
{
    /// <summary>
    /// Stream for read and write
    /// </summary>
    internal class CSL_Stream
    {
        /// <summary>
        /// Get buff of this stream object
        /// </summary>
        /// <returns>byte array</returns>
        public byte[] GetBuff()
        {
            byte[] ret = new byte[pos];
            if (pos > 0)
                Array.Copy(buff, ret, pos);
            return ret;
        }
        /// <summary>
        /// Current buff
        /// </summary>
        public byte[] buff;
        /// <summary>
        /// Current position of the buff
        /// </summary>
        public int pos;
        /// <summary>
        /// Constructor with buff
        /// </summary>
        /// <param name="buff">Specific buff</param>
        public CSL_Stream(byte[] buff)
        {
            this.buff = buff;
            pos = 0;
            maxSize = buff.Length;
        }
        /// <summary>
        /// Constructor with size
        /// </summary>
        /// <param name="maxSize">Specific buff size</param>
        public CSL_Stream(int maxSize = 1024 * 1024)
            : this(new byte[maxSize])
        {
        }
        /// <summary>
        /// Max size of buff
        /// </summary>
        public int maxSize;
        void CheckSize(int add)
        {
            if (pos + add > maxSize)
            {
                maxSize *= 2;
                while (pos + add > maxSize)
                    maxSize *= 2;
                Array.Resize(ref buff, maxSize);
            }
        }
        /// <summary>
        /// Write a string value to stream
        /// </summary>
        /// <param name="value">String value to be write into stream.</param>
        public void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Write((int)0);
            }
            else
            {
                byte[] temp = Encoding.UTF8.GetBytes(value);
                Write(temp.Length);
                Array.Copy(temp, 0, buff, pos, temp.Length);
                pos += temp.Length;
            }
        }
        /// <summary>
        /// Read a string from stream
        /// </summary>
        /// <param name="value">String value read from stream.</param>
        public void Read(out string value)
        {
            Read(out int length);
            if (length > 0)
            {
                value = Encoding.UTF8.GetString(buff, pos, length);
                pos += length;
            }
            else
                value = "";
        }
        /// <summary>
        /// Write a boolean value to stream
        /// </summary>
        /// <param name="value">Boolean value to be write into stream.</param>
        public void Write(bool value)
        {
            CheckSize(1);
            buff[pos++] = (byte)(value ? 1 : 0);
        }
        /// <summary>
        /// Read a boolean from stream
        /// </summary>
        /// <param name="value">Boolean value read from stream.</param>
        public void Read(out bool value)
        {
            value = buff[pos] > 0;
            pos++;
        }
        /// <summary>
        /// Write a nullable boolean value to stream
        /// </summary>
        /// <param name="value">Nullable boolean value to be write into stream.</param>
        public void Write(bool? value)
        {
            if (value.HasValue)
            {
                CheckSize(2);
                buff[pos++] = 1;
                buff[pos++] = (byte)(value.Value ? 1 : 0);
            }
            else
            {
                CheckSize(1);
                buff[pos++] = 0;
            }
        }
        /// <summary>
        /// Read a nullable boolean from stream
        /// </summary>
        /// <param name="value">Nullable boolean value read from stream.</param>
        public void Read(out bool? value)
        {
            if (buff[pos++] > 0)
            {
                value = buff[pos++] > 0;
            }
            else
                value = null;
        }
        /// <summary>
        /// Write a integer value to stream
        /// </summary>
        /// <param name="value">Integer value to be write into stream.</param>
        public void Write(int value)
        {
            CheckSize(4);
            Array.Copy(BitConverter.GetBytes(value), 0, buff, pos, 4);
            pos += 4;
        }
        /// <summary>
        /// Read a integer from stream
        /// </summary>
        /// <param name="value">Integer value read from stream.</param>
        public void Read(out int value)
        {
            value = BitConverter.ToInt32(buff, pos);
            pos += 4;
        }
        /// <summary>
        /// Write a nullable integer value to stream
        /// </summary>
        /// <param name="value">Integer value to be write into stream.</param>
        public void Write(int? value)
        {
            if (value.HasValue)
            {
                CheckSize(5);
                buff[pos++] = 1;
                Array.Copy(BitConverter.GetBytes(value.Value), 0, buff, pos, 4);
                pos += 4;
            }
            else
            {
                CheckSize(1);
                buff[pos++] = 0;
            }
        }
        /// <summary>
        /// Read a integer from stream
        /// </summary>
        /// <param name="value">Nullable integer value read from stream.</param>
        public void Read(out int? value)
        {
            if (buff[pos++] > 0)
            {
                value = BitConverter.ToInt32(buff, pos);
                pos += 4;
            }
            else
                value = null;
        }
        /// <summary>
        /// Write a long integer value to stream
        /// </summary>
        /// <param name="value">Long integer value to be write into stream.</param>
        public void Write(long value)
        {
            CheckSize(8);
            Array.Copy(BitConverter.GetBytes(value), 0, buff, pos, 8);
            pos += 8;
        }
        /// <summary>
        /// Read a long integer from stream
        /// </summary>
        /// <param name="value">Long integer value read from stream.</param>
        public void Read(out long value)
        {
            value = BitConverter.ToInt64(buff, pos);
            pos += 8;
        }
        /// <summary>
        /// Write a nullable long integer value to stream
        /// </summary>
        /// <param name="value">Nullable long integer value to be write into stream.</param>
        public void Write(long? value)
        {
            if (value.HasValue)
            {
                CheckSize(9);
                buff[pos++] = 1;
                Array.Copy(BitConverter.GetBytes(value.Value), 0, buff, pos, 8);
                pos += 8;
            }
            else
            {
                CheckSize(1);
                buff[pos++] = 0;
            }
        }
        /// <summary>
        /// Read a nullable long integer from stream
        /// </summary>
        /// <param name="value">Nullable long integer value read from stream.</param>
        public void Read(out long? value)
        {
            if (buff[pos++] > 0)
            {
                value = BitConverter.ToInt64(buff, pos);
                pos += 8;
            }
            else
                value = null;
        }
        /// <summary>
        /// Write a ulong integer value to stream
        /// </summary>
        /// <param name="value">ULong integer value to be write into stream.</param>
        public void Write(ulong value)
        {
            CheckSize(8);
            Array.Copy(BitConverter.GetBytes(value), 0, buff, pos, 8);
            pos += 8;
        }
        /// <summary>
        /// Read a ulong integer from stream
        /// </summary>
        /// <param name="value">ULong integer value read from stream.</param>
        public void Read(out ulong value)
        {
            value = BitConverter.ToUInt64(buff, pos);
            pos += 8;
        }
        /// <summary>
        /// Write a nullable ulong integer value to stream
        /// </summary>
        /// <param name="value">Nullable ulong integer value to be write into stream.</param>
        public void Write(ulong? value)
        {
            if (value.HasValue)
            {
                CheckSize(9);
                buff[pos++] = 1;
                Array.Copy(BitConverter.GetBytes(value.Value), 0, buff, pos, 8);
                pos += 8;
            }
            else
            {
                CheckSize(1);
                buff[pos++] = 0;
            }
        }
        /// <summary>
        /// Read a nullable ulong integer from stream
        /// </summary>
        /// <param name="value">Nullable ulong integer value read from stream.</param>
        public void Read(out ulong? value)
        {
            if (buff[pos++] > 0)
            {
                value = BitConverter.ToUInt64(buff, pos);
                pos += 8;
            }
            else
                value = null;
        }
        /// <summary>
        /// Write a double value to stream
        /// </summary>
        /// <param name="value">Double value to be write into stream.</param>
        public void Write(double value)
        {
            CheckSize(8);
            Array.Copy(BitConverter.GetBytes(value), 0, buff, pos, 8);
            pos += 8;
        }
        /// <summary>
        /// Read a double from string
        /// </summary>
        /// <param name="value">Double value read from stream.</param>
        public void Read(out double value)
        {
            value = BitConverter.ToDouble(buff, pos);
            pos += 8;
        }
        /// <summary>
        /// Write a nullable double value to stream
        /// </summary>
        /// <param name="value">Nullable ulong integer value to be write into stream.</param>
        public void Write(double? value)
        {
            if (value.HasValue)
            {
                CheckSize(9);
                buff[pos++] = 1;
                Array.Copy(BitConverter.GetBytes(value.Value), 0, buff, pos, 8);
                pos += 8;
            }
            else
            {
                CheckSize(1);
                buff[pos++] = 0;
            }
        }
        /// <summary>
        /// Read a nullable double from stream
        /// </summary>
        /// <param name="value">Nullable double value read from stream.</param>
        public void Read(out double? value)
        {
            if (buff[pos++] > 0)
            {
                value = BitConverter.ToDouble(buff, pos);
                pos += 8;
            }
            else
                value = null;
        }
        /// <summary>
        /// Read a byte from string
        /// </summary>
        /// <param name="value">Byte value read from stream.</param>
        public void Write(byte value)
        {
            CheckSize(1);
            buff[pos++] = value;
        }
        /// <summary>
        /// Read a byte from string
        /// </summary>
        /// <param name="value">Byte value read from stream.</param>
        public void Read(out byte value)
        {
            value = buff[pos++];
        }
    }
}
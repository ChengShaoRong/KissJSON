/*
 * KissJson : Keep It Simple Stupid JSON
 * Copyright © 2022-2026 RongRong. All right reserved.
 */
using System.Text;
using System;

namespace CSharpLike
{
    /// <summary>
    /// Read/write streams dedicated to binary JSON files
    /// <br/><br/>Chinese:<br/>二进制JSON文件专用的读写流
    /// </summary>
    internal class CSL_Stream
    {
        /// <summary>
        /// Get buff of this stream object
        /// <br/><br/>Chinese:<br/>获取数据
        /// </summary>
        /// <returns>byte array<br/><br/>Chinese:<br/>字节数组</returns>
        public byte[] GetBuff()
        {
            byte[] ret = new byte[pos];
            if (pos > 0)
                Array.Copy(buff, ret, pos);
            return ret;
        }
        /// <summary>
        /// Current buff
        /// <br/><br/>Chinese:<br/>缓冲区
        /// </summary>
        public byte[] buff;
        /// <summary>
        /// Current position of the buff
        /// <br/><br/>Chinese:<br/>当前读写的位置
        /// </summary>
        public int pos;
        /// <summary>
        /// Constructor with buff
        /// <br/><br/>Chinese:<br/>带数据的构造函数
        /// </summary>
        /// <param name="buff">Specific buff<br/><br/>Chinese:<br/>指定的二进制数据</param>
        public CSL_Stream(byte[] buff)
        {
            this.buff = buff;
            pos = 0;
            maxSize = buff.Length;
        }
        /// <summary>
        /// Constructor with size
        /// <br/><br/>Chinese:<br/>带指定缓存区大小的构造函数
        /// </summary>
        /// <param name="maxSize">Specific buff size, default is 1MB<br/><br/>Chinese:<br/>指定的缓存区大小的,默认1MB</param>
        public CSL_Stream(int maxSize = 1024 * 1024)
            : this(new byte[maxSize])
        {
        }
        /// <summary>
        /// Max size of buff
        /// <br/><br/>Chinese:<br/>当前缓存区的最大值
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
        /// <br/><br/>Chinese:<br/>向数据流写入字符串
        /// </summary>
        /// <param name="value">String value to be write into stream.<br/><br/>Chinese:<br/>待写入的字符串</param>
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
        /// <br/><br/>Chinese:<br/>从数据流读取字符串
        /// </summary>
        /// <param name="value">String value read from stream.<br/><br/>Chinese:<br/>读取的字符串</param>
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
        /// <br/><br/>Chinese:<br/>向数据流写入布尔值
        /// </summary>
        /// <param name="value">Boolean value to be write into stream..<br/><br/>Chinese:<br/>待写入的布尔值</param>
        public void Write(bool value)
        {
            CheckSize(1);
            buff[pos++] = (byte)(value ? 1 : 0);
        }
        /// <summary>
        /// Read a boolean from stream
        /// <br/><br/>Chinese:<br/>从数据流读取布尔值
        /// </summary>
        /// <param name="value">Boolean value read from stream.<br/><br/>Chinese:<br/>读取的布尔值</param>
        public void Read(out bool value)
        {
            value = buff[pos] > 0;
            pos++;
        }
        /// <summary>
        /// Write a nullable boolean value to stream
        /// <br/><br/>Chinese:<br/>向数据流写入可空布尔值
        /// </summary>
        /// <param name="value">Nullable boolean value to be write into stream.<br/><br/>Chinese:<br/>待写入的可空布尔值</param>
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
        /// <br/><br/>Chinese:<br/>从数据流读取可空布尔值
        /// </summary>
        /// <param name="value">Nullable boolean value read from stream..<br/><br/>Chinese:<br/>读取的可空布尔值</param>
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
        /// <br/><br/>Chinese:<br/>向数据流写入整数
        /// </summary>
        /// <param name="value">Integer value to be write into stream.<br/><br/>Chinese:<br/>待写入的整数</param>
        public void Write(int value)
        {
            CheckSize(4);
            Array.Copy(BitConverter.GetBytes(value), 0, buff, pos, 4);
            pos += 4;
        }
        /// <summary>
        /// Read a integer from stream
        /// <br/><br/>Chinese:<br/>从数据流读取整数
        /// </summary>
        /// <param name="value">Integer value read from stream.<br/><br/>Chinese:<br/>读取的整数</param>
        public void Read(out int value)
        {
            value = BitConverter.ToInt32(buff, pos);
            pos += 4;
        }
        /// <summary>
        /// Write a nullable integer value to stream
        /// <br/><br/>Chinese:<br/>向数据流写入可空整数
        /// </summary>
        /// <param name="value">Integer value to be write into stream.<br/><br/>Chinese:<br/>待写入的可空整数</param>
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
        /// <br/><br/>Chinese:<br/>从数据流读取可空整数
        /// </summary>
        /// <param name="value">Nullable integer value read from stream.<br/><br/>Chinese:<br/>读取的可空整数</param>
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
        /// <br/><br/>Chinese:<br/>向数据流写入长整数
        /// </summary>
        /// <param name="value">Long integer value to be write into stream.<br/><br/>Chinese:<br/>待写入的长整数</param>
        public void Write(long value)
        {
            CheckSize(8);
            Array.Copy(BitConverter.GetBytes(value), 0, buff, pos, 8);
            pos += 8;
        }
        /// <summary>
        /// Read a long integer from stream
        /// <br/><br/>Chinese:<br/>从数据流读取长整数
        /// </summary>
        /// <param name="value">Long integer value read from stream.<br/><br/>Chinese:<br/>读取的长整数</param>
        public void Read(out long value)
        {
            value = BitConverter.ToInt64(buff, pos);
            pos += 8;
        }
        /// <summary>
        /// Write a nullable long integer value to stream
        /// <br/><br/>Chinese:<br/>向数据流写入可空长整数
        /// </summary>
        /// <param name="value">Nullable long integer value to be write into stream<br/><br/>Chinese:<br/>待写入的可空长整数.</param>
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
        /// <br/><br/>Chinese:<br/>从数据流读取可空长整数
        /// </summary>
        /// <param name="value">Nullable long integer value read from stream.<br/><br/>Chinese:<br/>读取的可空长整数</param>
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
        /// <br/><br/>Chinese:<br/>向数据流写入无符号长整数
        /// </summary>
        /// <param name="value">ULong integer value to be write into stream.<br/><br/>Chinese:<br/>待写入的无符号长整数</param>
        public void Write(ulong value)
        {
            CheckSize(8);
            Array.Copy(BitConverter.GetBytes(value), 0, buff, pos, 8);
            pos += 8;
        }
        /// <summary>
        /// Read a ulong integer from stream
        /// <br/><br/>Chinese:<br/>从数据流读取无符号长整数
        /// </summary>
        /// <param name="value">ULong integer value read from stream.<br/><br/>Chinese:<br/>读取的无符号长整数</param>
        public void Read(out ulong value)
        {
            value = BitConverter.ToUInt64(buff, pos);
            pos += 8;
        }
        /// <summary>
        /// Write a nullable ulong integer value to stream
        /// <br/><br/>Chinese:<br/>向数据流写入可空无符号长整数
        /// </summary>
        /// <param name="value">Nullable ulong integer value to be write into stream.<br/><br/>Chinese:<br/>待写入的可空无符号长整数</param>
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
        /// <br/><br/>Chinese:<br/>从数据流读取可空无符号长整数
        /// </summary>
        /// <param name="value">Nullable ulong integer value read from stream.<br/><br/>Chinese:<br/>读取的可空无符号长整数</param>
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
        /// Write a decimal value to stream
        /// <br/><br/>Chinese:<br/>向数据流写入十进制数
        /// </summary>
        /// <param name="value">Decimal value to be write into stream.<br/><br/>Chinese:<br/>待写入的十进制数</param>
        public void Write(decimal value)
        {
            CheckSize(16);
            int[] ints = decimal.GetBits(value);
            Array.Copy(BitConverter.GetBytes(ints[0]), 0, buff, pos, 4);
            Array.Copy(BitConverter.GetBytes(ints[1]), 0, buff, pos + 4, 4);
            Array.Copy(BitConverter.GetBytes(ints[2]), 0, buff, pos + 8, 4);
            Array.Copy(BitConverter.GetBytes(ints[3]), 0, buff, pos + 12, 4);
            pos += 16;
        }
        /// <summary>
        /// Read a decimal from stream
        /// <br/><br/>Chinese:<br/>从数据流读取十进制数
        /// </summary>
        /// <param name="value">Decimal value read from stream.<br/><br/>Chinese:<br/>读取的十进制数</param>
        public void Read(out decimal value)
        {
            int[] ints = new int[4];
            ints[0] = BitConverter.ToInt32(buff, pos);
            ints[1] = BitConverter.ToInt32(buff, pos + 4);
            ints[2] = BitConverter.ToInt32(buff, pos + 8);
            ints[3] = BitConverter.ToInt32(buff, pos + 12);
            value = new decimal(ints);
            pos += 16;
        }
        /// <summary>
        /// Write a double value to stream
        /// <br/><br/>Chinese:<br/>向数据流写入双精度浮点数
        /// </summary>
        /// <param name="value">Double value to be write into stream.<br/><br/>Chinese:<br/>待写入的双精度浮点数</param>
        public void Write(double value)
        {
            CheckSize(8);
            Array.Copy(BitConverter.GetBytes(value), 0, buff, pos, 8);
            pos += 8;
        }
        /// <summary>
        /// Read a double from stream
        /// <br/><br/>Chinese:<br/>从数据流读取双精度浮点数
        /// </summary>
        /// <param name="value">Double value read from stream.<br/><br/>Chinese:<br/>读取的双精度浮点数</param>
        public void Read(out double value)
        {
            value = BitConverter.ToDouble(buff, pos);
            pos += 8;
        }
        /// <summary>
        /// Write a nullable double value to stream
        /// <br/><br/>Chinese:<br/>向数据流写入可空双精度浮点数
        /// </summary>
        /// <param name="value">Nullable ulong integer value to be write into stream.<br/><br/>Chinese:<br/>待写入的可空双精度浮点数</param>
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
        /// <br/><br/>Chinese:<br/>从数据流读取可空双精度浮点数
        /// </summary>
        /// <param name="value">Nullable double value read from stream.<br/><br/>Chinese:<br/>读取的可空双精度浮点数</param>
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
        /// Write a byte value to stream
        /// <br/><br/>Chinese:<br/>向数据流写入字节
        /// </summary>
        /// <param name="value">Byte value to be write into stream.<br/><br/>Chinese:<br/>待写入的字节</param>
        public void Write(byte value)
        {
            CheckSize(1);
            buff[pos++] = value;
        }
        /// <summary>
        /// Read a byte from stream
        /// <br/><br/>Chinese:<br/>从数据流读取字节
        /// </summary>
        /// <param name="value">Byte value read from stream.<br/><br/>Chinese:<br/>读取的字节</param>
        public void Read(out byte value)
        {
            value = buff[pos++];
        }
    }
}
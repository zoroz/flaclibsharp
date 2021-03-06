using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Helpers {

    /// <summary>
    /// A helper class for parsing byes and bits to actual numbers.
    /// </summary>
    /// <remarks>Currently always operates with Big-Endian numbers (because this was created for FLAC parsing which uses big-endian by default).</remarks>
    public static class BinaryDataHelper {

        /// <summary>
        /// From a given data array, get a subset of items in a deep copied array.
        /// </summary>
        /// <param name="data">The input data (won't be altered)</param>
        /// <param name="offset">Where in the input data to start copying files.</param>
        /// <param name="length">The amount of bytes to copy.</param>
        /// <returns>A new array with a copy of the subset of data.</returns>
        public static byte[] GetDataSubset(byte[] data, int offset, int length) {
            byte[] newData = new byte[length];
            Array.Copy(data, offset, newData, 0, length);
            return newData;
        }

        /// <summary>
        /// From the data, reads an unsigned 16 bit integer starting from the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the integer, in bytes.</param>
        /// <returns>The number that was read.</returns>
        public static UInt16 GetUInt16(byte[] data, int byteOffset) {
            return (UInt16)GetUInt64(data, byteOffset, 16);
        }

        /// <summary>
        /// From the data, reads an unsigned 24 bit integer starting from the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the integer, in bytes.</param>
        /// <returns>The number that was read (it reads 24 bits, but the actual type will be a 32 bit integer).</returns>
        public static UInt32 GetUInt24(byte[] data, int byteOffset) {
            return (UInt32)GetUInt64(data, byteOffset, 24);
        }

        /// <summary>
        /// From the data, reads an unsigned 32 bit integer starting from the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the integer, in bytes.</param>
        /// <returns>The number that was read.</returns>
        public static UInt32 GetUInt32(byte[] data, int byteOffset) {
            return (UInt32)GetUInt64(data, byteOffset, 32);
        }

        /// <summary>
        /// From the data, reads an unsigned 64 bit integer starting from the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the integer, in bytes.</param>
        /// <returns>The number that was read.</returns>
        public static UInt64 GetUInt64(byte[] data, int byteOffset) {
            return (UInt64)GetUInt64(data, byteOffset, 64);
        }

        /// <summary>
        /// From the data, reads boolean at the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the boolean, in bytes.</param>
        /// <param name="bitOffset">In the found byte, defines the bit that will represent the boolean.</param>
        /// <returns>The number that was read.</returns>
        public static bool GetBoolean(byte[] data, int byteOffset, byte bitOffset) {
            return (GetUInt64(data, byteOffset, 1, bitOffset) == 1);
        }

        /// <summary>
        /// From the data, reads an integer value starting from the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the integer, in bytes.</param>
        /// <param name="bitCount">How many bits to read (16, 32, or something arbitrary but less than or equal to 64)</param>
        /// <returns>The number that was read.</returns>
        public static UInt64 GetUInt64(byte[] data, int byteOffset, int bitCount) {
            return GetUInt64(data, byteOffset, bitCount, 0);
        }

        /// <summary>
        /// Allows you to interpret part of a byte array as a number.
        /// </summary>
        /// <param name="data">The source data.</param>
        /// <param name="byteOffset">Where in the data to start reading (offset in bytes)</param>
        /// <param name="bitCount">How many bits to read (16, 32, or something arbitrary but less than or equal to 64)</param>
        /// <param name="bitOffset">In the first byte, at which bit to start reading the data from.</param>
        /// <remarks>Always assumes Big-Endian in the data store.</remarks>
        /// <returns></returns>
        public static UInt64 GetUInt64(byte[] data, int byteOffset, int bitCount, byte bitOffset) {
            UInt64 result = 0;

            // Check input
            if (bitCount > 64)
            {
                throw new ArgumentOutOfRangeException("bitCount", bitCount, "Should be a value lower than or equal to 64.");
            }
            
            if (bitOffset > 8)
            {
                throw new ArgumentOutOfRangeException("bitOffset", bitOffset, "Should be a value between 0 and 7.");
            }

            // Total amount of bits to read (the rest is masked)
            int totalBitCount = bitCount + bitOffset; 
            
            // byteCount = Math.Ceiling(totalBitCount / 8) = How many bytes we'll be reading in total (maximum 8)
            byte byteCount = (byte)(totalBitCount >> 3); // totalBitCount / 8
            if(totalBitCount % 8 > 0) {
                byteCount += 1;
            } // Math.Ceiling

            // Check if we won't read more bytes than there are available.
            if (byteCount > (data.Length - byteOffset))
            {
                throw new ArgumentOutOfRangeException("Provided arguments would require reading outside of the data array upper bounds.");
            }

            // The first byte needs to be masked with the bitOffset, as we might not read the first few bits
            result = (byte)(((data[byteOffset] << bitOffset) & 0xFF) >> bitOffset);

            // If we have more than 1 byte we'll read these in one by one
            for (int i = 1; i < byteCount; i++) {
                result = (result << 8) + data[byteOffset + i];
            }

            // Bits masked at the end of the number (because we don't want to read up until the full last byte)
            byte maskedBitCount = (byte)((byteCount << 3) - totalBitCount); // (byteCount * 8) - totalBitCount
            result = result >> maskedBitCount;

            return result;
        }

#region GetBytes

        /// <summary>
        /// Converts the given value to a big-endian byte stream.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static byte[] GetBytesUInt16(UInt16 value)
        {
            return GetBytes(value, 2);
        }

        /// <summary>
        /// Converts the given value to a big-endian byte stream.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static byte[] GetBytesUInt32(UInt32 value)
        {
            return GetBytes(value, 4);
        }

        /// <summary>
        /// Converts the given value to a big-endian byte stream.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static byte[] GetBytesUInt64(UInt64 value)
        {
            return GetBytes(value, 8);
        }

        /// <summary>
        /// Converts the given value to a big-endian byte stream.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="size">How many bytes to convert (the rest will be ignored in the value).</param>
        /// <returns></returns>
        public static byte[] GetBytes(UInt64 value, int size)
        {
            byte[] result = new byte[size];

            for (int i = size - 1; i >= 0; i--)
            {
                result[i] = (byte)(value & 0xFF);
                value = value >> 8;
            }

            return result;
        }

        /// <summary>
        /// Will return an array of ascii printable characters (values 0x20 to 0x7e) and will pad with zero's so that the array of bytes is always of a given size.
        /// </summary>
        /// <param name="value">The text to convert to bytes.</param>
        /// <param name="size">The total length of the resulting byte array.</param>
        /// <returns></returns>
        public static byte[] GetPaddedAsciiBytes(string value, int size)
        {
            byte[] result = new byte[size];
            byte[] data = Encoding.ASCII.GetBytes(value);
            for (int i = 0; i < result.Length && i < data.Length; i++)
            {
                if (data[i] >= 0x20 && data[i] <= 0x7e)
                {
                    result[i] = data[i];
                }
            }

            return result;
        }

#endregion

        /// <summary>
        /// For a given array of bytes, switch the endiannes of the length-bytes starting at byteOffset.
        /// </summary>
        /// <param name="data">The source data.</param>
        /// <param name="byteOffset">Where to start switching the endianness.</param>
        /// <param name="length">How many bytes to switch.</param>
        /// <returns></returns>
        public static byte[] SwitchEndianness(byte[] data, int byteOffset, int length)
        {
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[length - (i + 1)] = data[i + byteOffset];
            }

            return result;
        }

    }
}

/*  Copyright (c) 2011 Roeland Moors
 
    Permission is hereby granted, free of charge, to any person obtaining a 
    copy of this software and associated documentation files (the "Software"), 
    to deal in the Software without restriction, including without limitation 
    the rights to use, copy, modify, merge, publish, distribute, sublicense, 
    and/or sell copies of the Software, and to permit persons to whom the 
    Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included 
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
    DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
    OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ads.Client.Common;

namespace Ads.Client.Helpers
{
    internal static class GenericHelper
    {
        /// <summary>
        /// Get length in bytes from a valuetype (not for lists, custom types, ...)
        /// </summary>
        /// <typeparam name="T">ValueType</typeparam>
        /// <returns></returns>
        public static uint GetByteLengthFromType<T>(uint defaultStringLength) where T : IConvertible
        {
            uint length = 0;
            TypeCode typeCode = Type.GetTypeCode(typeof(T));

            switch (typeCode)
            {
                case TypeCode.Boolean: length = 1; break;
                case TypeCode.Byte: length = 1; break;
                case TypeCode.Int16: length = 2; break;
                case TypeCode.Int32: length = 4; break;
                case TypeCode.Int64: length = 8; break;
                case TypeCode.UInt16: length = 2; break;
                case TypeCode.UInt32: length = 4; break;
                case TypeCode.UInt64: length = 8; break;
                case TypeCode.Single: length = 4; break;
                case TypeCode.Double: length = 8; break;
                case TypeCode.String: length = defaultStringLength; break; 
                case TypeCode.Object:
                    if (Type.Equals(typeof(T), typeof(Date))) length = 8;
                    if (Type.Equals(typeof(T), typeof(Time))) length = 8;
                    break;                
            }

            if (length == 0) throw new AdsException(String.Format("Function GetByteLengthFromType doesn't support this type ({0}) yet!", typeCode.ToString()));

            return length;
        }

        /// <summary>
        /// Convert byte array to generic valuetype (not for strings, lists, custom types, ...)
        /// </summary>
        /// <typeparam name="T">ValueType</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetResultFromBytes<T>(byte[] value) where T : IConvertible
        {
            object v = null;

            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean: v = value[0]; break;
                case TypeCode.Byte: v = value[0]; break;
                case TypeCode.Int16: v = BitConverter.ToInt16(value, 0); break;
                case TypeCode.Int32: v = BitConverter.ToInt32(value, 0); break;
                case TypeCode.Int64: v = BitConverter.ToInt64(value, 0); break;
                case TypeCode.UInt16: v = BitConverter.ToUInt16(value, 0); break;
                case TypeCode.UInt32: v = BitConverter.ToUInt32(value, 0); break;
                case TypeCode.UInt64: v = BitConverter.ToUInt64(value, 0); break;
                case TypeCode.Single: v = BitConverter.ToSingle(value, 0); break;
                case TypeCode.Double: v = BitConverter.ToDouble(value, 0); break;
                case TypeCode.String: v = ByteArrayHelper.ByteArrayToString(value); break; 
                case TypeCode.Object:
                    if (Type.Equals(typeof(Date), typeof(T))) v = new Date(BitConverter.ToUInt32(value, 0));
                    if (Type.Equals(typeof(Time), typeof(T))) v = new Time(BitConverter.ToUInt32(value, 0));
                    break;

            }

            if (v == null) throw new AdsException("Function GetResultFromBytes doesn't support this type yet!");

            return (T)Convert.ChangeType(v, Type.GetTypeCode(typeof(T)), null);
        }

        /// <summary>
        /// Convert ValueType to byte array
        /// </summary>
        /// <typeparam name="T">ValueType</typeparam>
        /// <param name="varValue">The value that needs conversion</param>
        /// <returns></returns>
        public static IEnumerable<byte> GetBytesFromType<T>(T varValue) where T : IConvertible
        {
            IEnumerable<byte> varValueBytes = null;

            switch (varValue.GetTypeCode())
            {
                case TypeCode.Boolean: varValueBytes = BitConverter.GetBytes(varValue.ToBoolean(null)); break;
                case TypeCode.Byte: varValueBytes = new byte[] { varValue.ToByte(null)}; break; 
                case TypeCode.Char: varValueBytes = BitConverter.GetBytes(varValue.ToChar(null)); break;
                case TypeCode.Int16: varValueBytes = BitConverter.GetBytes(varValue.ToInt16(null)); break;
                case TypeCode.Int32: varValueBytes = BitConverter.GetBytes(varValue.ToInt32(null)); break;
                case TypeCode.Int64: varValueBytes = BitConverter.GetBytes(varValue.ToInt64(null)); break;
                case TypeCode.UInt16: varValueBytes = BitConverter.GetBytes(varValue.ToUInt16(null)); break;
                case TypeCode.UInt32: varValueBytes = BitConverter.GetBytes(varValue.ToUInt32(null)); break;
                case TypeCode.UInt64: varValueBytes = BitConverter.GetBytes(varValue.ToUInt64(null)); break;
                case TypeCode.Single: varValueBytes = BitConverter.GetBytes(varValue.ToSingle(null)); break;
                case TypeCode.Double: varValueBytes = BitConverter.GetBytes(varValue.ToDouble(null)); break;
                case TypeCode.String: varValueBytes = varValue.ToString().ToAdsBytes(); break;
                case TypeCode.Object:
                    if (Type.Equals(typeof(Date), typeof(T))) varValueBytes = BitConverter.GetBytes(varValue.ToInt32(null));
                    if (Type.Equals(typeof(Time), typeof(T))) varValueBytes = BitConverter.GetBytes(varValue.ToInt32(null));
                    break;
            }

            if (varValueBytes == null) throw new AdsException("Function GetBytesFromType doesn't support this type yet!");

            return varValueBytes;
        }

    }
}

﻿/*  Copyright (c) 2011 Roeland Moors
 
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
using System.Reflection;

namespace Ads.Client.Helpers
{

    internal static class GenericHelper
    {
        /// <summary>
        /// Get length in bytes from a valuetype or AdsSerializable 
        /// </summary>
        /// <typeparam name="T">ValueType or AdsSerializable</typeparam>
        /// <returns></returns>
        public static uint GetByteLengthFromType<T>(uint defaultStringLength) 
        {
            if (typeof(IConvertible).IsAssignableFrom(typeof(T)))
            {
                var length = GetByteLengthFromConvertibleType(typeof(T), defaultStringLength);
                if (length == 0) throw new AdsException(String.Format("Function GetByteLengthFromType doesn't support this type ({0}) yet!", typeof(T).Name));
                return length;
            }
            else
            {
                if (AdsAttribute.IsAdsSerializable<T>())
                {
                    uint length = 0;
                    List<AdsAttribute> attributes = GetAdsAttributes(typeof(T), defaultStringLength);
                    attributes.ForEach(a => length += a.ByteSize);
                    return length;
                }
                else throw new AdsException(String.Format(TypeNotImplementedError, typeof(T).Name));
            }
        }

        /// <summary>
        /// Convert byte array to generic valuetype or AdsSerializable
        /// </summary>
        /// <typeparam name="T">ValueType or AdsSerializable</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetResultFromBytes<T>(byte[] value, uint defaultStringLength) 
        {
            if (typeof(IConvertible).IsAssignableFrom(typeof(T)))
            {
                object v = GetBytesFromConvertibleType(typeof(T),  value);
                if (v == null) throw new AdsException("Function GetResultFromBytes doesn't support this type yet!");
                return (T)Convert.ChangeType(v, Type.GetTypeCode(typeof(T)), null);
            }
            else
            {
                if (AdsAttribute.IsAdsSerializable<T>())
                {
                    var adsObj = (T)Activator.CreateInstance(typeof(T));
                    List<AdsAttribute> attributes = GetAdsAttributes(typeof(T), defaultStringLength);

                    uint pos = 0;
                    foreach (var attr in attributes)
                    {
                        byte[] valarray = new byte[attr.ByteSize];
                        Array.Copy(value, (int)pos, valarray, 0, (int)attr.ByteSize);
                        var type = attr.GetPropery().PropertyType;
                        object val = GetBytesFromConvertibleType(type, valarray);
                        attr.GetPropery().SetValue(adsObj, val, null);
                        pos += attr.ByteSize;
                    }

                    return adsObj;
                }
                else throw new AdsException(String.Format(TypeNotImplementedError, typeof(T).Name));
            }
        }

        /// <summary>
        /// Convert ValueType or AdsSerializable to byte array
        /// </summary>
        /// <typeparam name="T">ValueType or AdsSerializable</typeparam>
        /// <param name="varValue2">The value that needs conversion</param>
        /// <returns></returns>
        public static IEnumerable<byte> GetBytesFromType<T>(T varValue, uint defaultStringLength) 
        {
            List<byte> varValueBytes = null;

            if (typeof(IConvertible).IsAssignableFrom(typeof(T)))
            {
                varValueBytes = GetBytesFromConvertible(typeof(T), varValue as IConvertible, defaultStringLength).ToList();
            }
            else
            {
                if (AdsAttribute.IsAdsSerializable<T>())
                {
                    var totallength = GetByteLengthFromType<T>(defaultStringLength);
                    varValueBytes = new List<byte>((int)totallength);
                    List<AdsAttribute> attributes = GetAdsAttributes(typeof(T), defaultStringLength);
                    foreach (var attr in attributes)
                    {
                        var type = attr.GetPropery().PropertyType;
                        var val = attr.GetPropery().GetValue(varValue, null) as IConvertible;
                        var bytes = GetBytesFromConvertible(type, val, defaultStringLength);
                        if (bytes.Count() != attr.ByteSize)
                        {
                            if (bytes.Count() > attr.ByteSize)
                                bytes = bytes.Take((int)attr.ByteSize).ToList();
                        }
                        varValueBytes.AddRange(bytes);
                    }
                }
            }

            if (varValueBytes == null) throw new AdsException("Function GetBytesFromType doesn't support this type yet!");

            return varValueBytes;
        }

        private static IEnumerable<byte> GetBytesFromConvertible(Type type, IConvertible value, uint defaultStringLength)
        {
            IEnumerable<byte> varValueBytes = null;

            if (value == null) return null;

            switch (value.GetTypeCode())
            {  
                case TypeCode.Boolean: varValueBytes = BitConverter.GetBytes(value.ToBoolean(null)); break;
                case TypeCode.Byte: varValueBytes = new byte[] { value.ToByte(null) }; break;
                case TypeCode.Char: varValueBytes = BitConverter.GetBytes(value.ToChar(null)); break;
                case TypeCode.Int16: varValueBytes = BitConverter.GetBytes(value.ToInt16(null)); break;
                case TypeCode.Int32: varValueBytes = BitConverter.GetBytes(value.ToInt32(null)); break;
                case TypeCode.Int64: varValueBytes = BitConverter.GetBytes(value.ToInt64(null)); break;
                case TypeCode.UInt16: varValueBytes = BitConverter.GetBytes(value.ToUInt16(null)); break;
                case TypeCode.UInt32: varValueBytes = BitConverter.GetBytes(value.ToUInt32(null)); break;
                case TypeCode.UInt64: varValueBytes = BitConverter.GetBytes(value.ToUInt64(null)); break;
                case TypeCode.Single: varValueBytes = BitConverter.GetBytes(value.ToSingle(null)); break;
                case TypeCode.Double: varValueBytes = BitConverter.GetBytes(value.ToDouble(null)); break;
                case TypeCode.DateTime: varValueBytes = BitConverter.GetBytes(value.ToInt32(null)); break;
                case TypeCode.String: varValueBytes = value.ToString().ToAdsBytes(); break;
                case TypeCode.Object:
                    if (Type.Equals(typeof(Date), type)) varValueBytes = BitConverter.GetBytes(value.ToInt32(null));
                    if (Type.Equals(typeof(Time), type)) varValueBytes = BitConverter.GetBytes(value.ToInt32(null));
                    break;
            }

            return varValueBytes;
        }

        private static uint GetByteLengthFromConvertibleType(Type type, uint defaultStringLength)
        {
            uint length = 0;
            TypeCode typeCode = Type.GetTypeCode(type);

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
                case TypeCode.DateTime: length = 4; break;
                case TypeCode.Object:
                    if (Type.Equals(type, typeof(Date))) length = 4;
                    if (Type.Equals(type, typeof(Time))) length = 4;
                    break;
            }
            return length;
        }

        private static object GetBytesFromConvertibleType(Type type, byte[] value)
        {
            object v = null;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean: v = value[0]; break;
                case TypeCode.Byte: v = value[0]; break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    switch (value.Length)
                    {
                        case 2: v = BitConverter.ToInt16(value, 0); break;
                        case 4: v = BitConverter.ToInt32(value, 0); break;
                        case 8: v = BitConverter.ToInt64(value, 0); break;
                    }
                    break;
                case TypeCode.UInt16:
                case TypeCode.UInt32: 
                case TypeCode.UInt64:
                    switch (value.Length)
                    {
                        case 2: v = BitConverter.ToUInt16(value, 0); break;
                        case 4: v = BitConverter.ToUInt32(value, 0); break;
                        case 8: v = BitConverter.ToUInt64(value, 0); break;
                    }
                    break;               
                case TypeCode.Single: 
                case TypeCode.Double: 
                    switch (value.Length)
                    {
                        case 4: v = BitConverter.ToSingle(value, 0); break;
                        case 8: v = BitConverter.ToDouble(value, 0); break;
                    }
                    break; 
                case TypeCode.String: v = ByteArrayHelper.ByteArrayToString(value); break;
                case TypeCode.DateTime: v = ByteArrayHelper.ByteArrayToDateTime(value); break;
                case TypeCode.Object:
                    if (Type.Equals(typeof(Date), type)) v = new Date(BitConverter.ToUInt32(value, 0));
                    if (Type.Equals(typeof(Time), type)) v = new Time(BitConverter.ToUInt32(value, 0));
                    break;
                default:
                    v = null;
                    break;
            }

            return (v);
        }

        public static List<AdsAttribute> GetAdsAttributes(Type type, uint defaultStringLength)
        {
            List<AdsAttribute> attributes = new List<AdsAttribute>();

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var p in props)
            {
                AdsAttribute attr = AdsAttribute.GetAdsAttribute(p);
                if (attr != null)
                {
                    attr.SetProperty(p);
                    if (attr.ByteSize == 0)
                    {
                        attr.ByteSize = GetByteLengthFromConvertibleType(p.PropertyType, defaultStringLength);
                    }
                    attributes.Add(attr);
                }
            }

            attributes = attributes.OrderBy(a => a.Order).ToList();

            return attributes;
        }

        const string TypeNotImplementedError = "Type {0} must be IConvertible or has the AdsSerializable attribute!";
    }
}

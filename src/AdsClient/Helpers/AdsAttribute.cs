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
using System.Reflection;

namespace Ads.Client
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AdsAttribute : Attribute
    {
        public uint ByteSize;
        public uint Order;

        public AdsAttribute()
        {
            ByteSize = 0;
            Order = 99;
        }

        private PropertyInfo propertyInfo;

        public void SetProperty(PropertyInfo p)
        {
            propertyInfo = p;
        }

        public PropertyInfo GetPropery()
        {
            return propertyInfo;
        }


        public static AdsAttribute GetAdsAttribute(PropertyInfo p)
        {
            AdsAttribute attribute = null;
            var attributes = p.GetCustomAttributes(typeof(AdsAttribute), false);
            if ((attributes != null) && (attributes.Count() > 0))
            {
                attribute = (AdsAttribute)attributes[0];
            }
            return attribute;
        }

        public static bool IsAdsSerializable<T>()
        {
            return typeof(T).IsDefined(typeof(AdsSerializableAttribute), false);
        }

    }
}
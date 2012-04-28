using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Client.Helpers
{
    public class ByteArrayHelper
    {
        /// <summary>
        /// Convert byte array to value defined by TypeOfValue
        /// </summary>
        /// <param name="value">The byte array that needs conversion</param>
        /// <param name="TypeOfValue">The type of the result</param>
        /// <returns></returns>
        public static object ByteArrayToTypeValue(byte[] value, Type TypeOfValue)
        {
            if (Type.Equals(TypeOfValue, null)) return null;
            if (Type.Equals(TypeOfValue, typeof(byte[]))) return value;
            var method = typeof(GenericHelper).GetMethod("GetResultFromBytes");
            var generic = method.MakeGenericMethod(TypeOfValue);
            return generic.Invoke(null, new object[] { value });
        }

        public static string ByteArrayToString(byte[] value)
        {
            return string.Concat(value.Select(b => b <= 0x7f ? (char)b : '?').TakeWhile(b => b > 0));
        }

        public static string ByteArrayToTestString(byte[] value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte val in value)
            {
                if (sb.Length > 0) sb.Append(',');
                sb.Append(val.ToString());   
            }
            return sb.ToString();
        }
    }
}

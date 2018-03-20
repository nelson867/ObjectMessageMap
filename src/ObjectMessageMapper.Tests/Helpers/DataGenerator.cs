//    MIT License
//
//    Copyright(c) 2016
//    Scott Nelson
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.

using System;
using System.IO;
using System.Text;

namespace ObjectMessageMapper.Tests.Helpers
{
    public static class DataGenerator
    {
        private static readonly Random RandGen = new Random(Guid.NewGuid().GetHashCode());

        public static readonly char[] CharsAlphaLower = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        public static readonly char[] CharsAlphaUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        public static readonly char[] CharsAlpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
        public static readonly char[] CharsAlphaNumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        public static readonly char[] CharsAlphaNumericSymbol = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]\{}|;:'/?.>,<".ToCharArray();
        public static readonly char[] CharsPassword = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]\{}|;:'/?.>,<".ToCharArray();
        public static readonly char[] CharsBase64 = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=".ToCharArray();
        public static readonly string[] TopLevelDomainNames = { "com", "net", "org", "biz", "info", "gov", "edu", "wa.us" };
        public static readonly string[] UrlProtocols = { "http", "https" };

        public static string RandomString(int length, char[] charSet = null)
        {
            if (charSet == null)
            {
                charSet = CharsAlpha;
            }
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(charSet[RandGen.Next(0, charSet.Length - 1)]);
            }

            return sb.ToString();
        }

        public static int RandomInt(int min = 1, int max = int.MaxValue)
        {
            return RandGen.Next(min, max);
        }

        public static long RandomLongId()
        {
            return RandomLong(1, Int64.MaxValue);
        }

        public static double RandomDouble()
        {
            return RandGen.NextDouble();
        }

        public static decimal RandomDecimal()
        {
            return Convert.ToDecimal(RandGen.NextDouble());
        }

        public static bool RandomBool()
        {
            return Convert.ToBoolean(RandGen.Next(0, 2));
        }

        public static long RandomLong()
        {
            return RandomLong(long.MinValue, long.MaxValue);
        }
        public static long RandomLong(long min, long max)
        {
            return Math.Min(max, Math.Max(min, (long)((RandGen.NextDouble() * 2.0 - 1.0) * long.MaxValue)));

            //byte[] buf = new byte[8];
            //RandGen.NextBytes(buf);
            //return (Math.Abs(BitConverter.ToInt64(buf, 0) % (max - min)) + min);
        }

        public static decimal RandomPrice(decimal min, decimal max)
        {
            return ((decimal)RandomInt((int)(min * 100m), (int)(max * 100))) / 100;
        }

        public static string RandomString(int minLength, int maxLength, char[] charSet = null)
        {
            return RandomString(RandGen.Next(minLength, maxLength), charSet);
        }

        public static string RandomPassword(int minLength, int maxLength)
        {
            return RandomString(minLength, maxLength, CharsPassword);
        }

        public static string RandomWords(int numWords, int minLength, int maxLength)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < numWords; i++)
            {
                if (i > 0)
                {
                    sb.Append(" ");
                }
                sb.Append(CharsAlphaUpper[RandGen.Next(0, CharsAlphaUpper.Length - 1)]);
                sb.Append(RandomString(minLength - 1, maxLength - 1, CharsAlphaLower));
            }
            return sb.ToString();
        }

        public static T Random<T>(T[] items)
        {
            return items[RandGen.Next(0, items.Length - 1)];
        }

        public static string RandomEmail(string domain = null)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(RandomString(4, RandGen.Next(5, 20), CharsAlphaNumeric));
            sb.Append("@");

            if (string.IsNullOrWhiteSpace(domain))
            {
                sb.Append(RandomString(4, RandGen.Next(5, 20), CharsAlphaNumeric));
                sb.Append(".");
                sb.Append(Random(TopLevelDomainNames));
            }
            else
            {
                sb.Append(domain);
            }
            return sb.ToString();
        }

        public static byte[] ByteArray(int minBytes = 20, int maxBytes = 5000)
        {
            int length = RandGen.Next(100, 500);

            byte[] buffer = new byte[length];
            RandGen.NextBytes(buffer);

            return buffer;
        }

        public static MemoryStream MemoryStream(int minBytes = 20, int maxBytes = 5000)
        {
            return new MemoryStream(ByteArray(minBytes, maxBytes));
        }
    }
}

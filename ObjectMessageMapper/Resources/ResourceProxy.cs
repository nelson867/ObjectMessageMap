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
using System.Resources;

namespace ObjectMessageMapper.Resources
{
    /// <summary>
    /// static object used to get and set references to
    /// the resource manager.
    /// </summary>
    internal static class ResourceProxy
    {
        private static object lockObj = new object();
        private static ResourceManager _resourceManager;

        /// <summary>
        /// Property: The ResourceManager that stores all the resource strings.
        /// This must be set before attempting to get any messages that get
        /// their strings from the resource string table.
        /// </summary>
        public static ResourceManager ResourceManager
        {
            get { return _resourceManager; }
            set
            {
                lock (lockObj)
                {
                    if (_resourceManager == null)
                    {
                        _resourceManager = value;
                    }
                }
            }
        }


        /// <summary>
        /// Helper function to get a string given a key value string.
        /// </summary>
        /// <param name="key">The key value string</param>
        /// <returns>The string.</returns>
        public static string GetString(string key)
        {
            return ResourceManager.GetString(key);
        }


    }
}

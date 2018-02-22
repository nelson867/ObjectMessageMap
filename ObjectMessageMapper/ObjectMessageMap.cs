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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectMessageMapper.Interfaces;
using ObjectMessageMapper.Core;
using System.Resources;
using ObjectMessageMapper.Resources;
using ObjectMessageMapper.Core.Mappers;

namespace ObjectMessageMapper
{
    /// <summary>
    /// ObjectMessageMap:
    /// 
    /// Maps and creates user message strings based on configuration
    /// mapping between objects, types, and enum. Each mapping can be
    /// configured to retrieve a string from the resource string table
    /// or from the object itself and the format it based on values
    /// from the object.
    /// </summary>
    public static class ObjectMessageMap
    {
        private static SelectorMapper<dynamic> MainMapper { get; set; }
        private static object lockObj = new object();


        /// <summary>
        /// Configures the ObjectMessageMap to use the specified resource
        /// manager when retrieving messages from the resource file.
        /// </summary>
        /// <param name="resManager"></param>
        public static void SetResourceManager(ResourceManager resManager)
        {
            ResourceProxy.ResourceManager = resManager;
        }

        /// <summary>
        /// Configures the ObjectMessageMap for the givin type.
        /// </summary>
        /// <typeparam name="T">Type to configure</typeparam>
        /// <param name="configure">Lambda: All configuration methods.</param>
        public static void Configure<T>(Action<IConfigureChain<T>> configure)
        {
            if (MainMapper == null)
            {
                lock (lockObj)
                {
                    if (MainMapper == null)
                    {
                        MainMapper = new SelectorMapper<dynamic>(LookupStrategyEnum.InheritChain);
                    }
                }
            }

            MainMapper.Configure<T>(configure, LookupStrategyEnum.InheritChain);
        }

        /// <summary>
        /// Resets and Clears all current configuration. After calling
        /// Clear, you must configure again before attepting to
        /// retrieve a message.
        /// </summary>
        public static void Clear()
        {
            lock(lockObj)
            {
                MainMapper = null;
            }
        }

        /// <summary>
        /// Get the message mapped to the givin object.
        /// </summary>
        /// <param name="obj">Object to retrieve message for.</param>
        /// <returns>string: The message.</returns>
        public static string GetMessage(object obj)
        {
            return MainMapper.GetMessage(obj);
        }

        /// <summary>
        /// Get the message mapped to the givin type.
        /// </summary>
        /// <param name="type">The type to get the message for.</param>
        /// <returns>string: The message.</returns>
        public static string GetMessage(Type type)
        {
            return MainMapper.GetMessage(type);
        }

    }
}

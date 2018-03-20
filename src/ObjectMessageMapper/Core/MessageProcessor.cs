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
using ObjectMessageMapper.Interfaces;
using ObjectMessageMapper.Resources;

namespace ObjectMessageMapper.Core
{
    /// <summary>
    /// Configuration node that represents a UseMessage configuration.
    /// </summary>
    /// <typeparam name="T">Type of object mapped to the message.</typeparam>
    internal class MessageProcessor<T> : IMappedItem, IMessageData<T>
    {
        #region Properties
        private string ResourceKey { get; set; }
        private Func<T, string> MessageAccessor { get; set; }
        private List<Func<T, string>> DataItems { get; set; }
        #endregion

        #region Constructors
        public MessageProcessor(string resourceKey)
        {
            DataItems = new List<Func<T, string>>();
            ResourceKey = resourceKey;
        }

        public MessageProcessor(Func<T, string> msgAccessor)
        {
            DataItems = new List<Func<T, string>>();
            MessageAccessor = msgAccessor;
        }
        #endregion

        #region Fluent Interfaces

        /// <summary>
        /// Configures the message processor to format the string
        /// using the specified lambda accessor expression. The 
        /// lambda expression is given the object and is retrieves
        /// the value to use to format the string.
        /// </summary>
        /// <param name="valueAccessor">Lambda expression used to retrieve the value that's
        ///  used to format the string.</param>
        /// <returns>Fluent Interface</returns>
        public IMessageData<T> WithValue(Func<T, string> valueAccessor)
        {
            DataItems.Add(valueAccessor);
            return this;
        }

        #endregion

        #region Message Retrieval Interface Methods

        /// <summary>
        /// Gets the messages that's mapped to the specified object.
        /// </summary>
        /// <param name="obj">The object to get the message for.</param>
        /// <exception cref="FormatException">Thrown if the number of data items do not match the number of string formaters.</exception>
        /// <returns>string</returns>
        public string GetMessage(object obj)
        {
            string message = MessageAccessor != null ? MessageAccessor((T)obj) : ResourceProxy.GetString(ResourceKey);

            // If there are data items (WithValue), then
            // format the resource string with the given values.
            if (DataItems.Count > 0)
            {
                message = string.Format(message, DataItems.Select(accessor => accessor((T)obj)).ToArray<object>());
            }
            
            return message;
        }

        /// <summary>
        /// Gets the messages that's mapped to the specified type.
        /// </summary>
        /// <param name="type">The type to get the message for.</param>
        /// <returns>string</returns>
        public string GetMessage(Type type)
        {
            if (MessageAccessor != null)
            {
                T obj = (T)Activator.CreateInstance(type);
                return MessageAccessor(obj);
            }
            return ResourceProxy.GetString(ResourceKey);
        }

        #endregion

    }
}

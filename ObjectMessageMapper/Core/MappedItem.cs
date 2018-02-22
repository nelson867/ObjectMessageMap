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

namespace ObjectMessageMapper.Core
{
    internal abstract class MappedItem<T> : IMappedItem
    {
        public string TypeNameResourceKey { get; set; }
        public Func<T, string> TypeNameAccessor { get; set; }

        /// <summary>
        /// abstract method for GetMessage to call after processing.
        /// </summary>
        /// <param name="obj">The object to get the message for.</param>
        /// <returns>The string message</returns>
        protected abstract string GetMessageFromMapping(object obj);

        /// <summary>
        /// abstract method for GetMessage to call after processing.
        /// </summary>
        /// <param name="type">The type to get the type name for.</param>
        /// <returns>The type name</returns>
        protected abstract string GetMessageFromMapping(Type type);

        /// <summary>
        /// Private Helper Method that creates a MessageProcessor
        /// for the current settings.
        /// </summary>
        /// <returns></returns>
        private MessageProcessor<T> GetCurrentMessageProcessor()
        {
            if (TypeNameAccessor != null)
            {
                return new MessageProcessor<T>(TypeNameAccessor);   
            }
            if (!string.IsNullOrWhiteSpace(TypeNameResourceKey))
            {
                return new MessageProcessor<T>(TypeNameResourceKey);
            }
            return null;
        }


        /// <summary>
        /// Gets the messages that's mapped to the specified object.
        /// </summary>
        /// <param name="obj">The object to get the message for.</param>
        /// <returns>string</returns>
        public string GetMessage(object obj)
        {
            return GetMessageFromMapping(obj);
        }

        /// <summary>
        /// Gets the messages that's mapped to the specified type.
        /// </summary>
        /// <param name="type">The type to get the message for.</param>
        /// <returns>string</returns>
        public string GetMessage(Type type)
        {
            if (type == typeof(T))
            {
                MessageProcessor<T> msgProcessor = GetCurrentMessageProcessor();
                if (msgProcessor != null)
                {
                    return msgProcessor.GetMessage(type);
                }
            }
            return GetMessageFromMapping(type);
        }
    }
}

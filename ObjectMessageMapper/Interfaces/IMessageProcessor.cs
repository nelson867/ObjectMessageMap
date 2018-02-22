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

namespace ObjectMessageMapper.Interfaces
{
    public interface IMessageProcessor<T, TFluentType>
    {
        /// <summary>
        /// Declares that the message should come from the resources
        /// with the specified key.
        /// </summary>
        /// <param name="resourceKey">string: resource string table key.</param>
        /// <returns>Fluent Interface</returns>
        TFluentType UseMessage(string resourceKey);

        /// <summary>
        /// Declares that the message will be retrieved via a user specified lambda.
        /// </summary>
        /// <param name="accessor">Lambda expression that's given the mapped object and returns a string.</param>
        /// <returns>Fluent Interface</returns>
        TFluentType UseMessage(Func<T, string> accessor);
    }
}

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
    public interface IMessageData<T, TFluentType>
    {
        /// <summary>
        /// Configures the message processor to format the string
        /// using the specified lambda accessor expression. The 
        /// lambda expression is given the object and is retrieves
        /// the value to use to format the string.
        /// </summary>
        /// <param name="valueAccessor">Lambda expression used to retrieve the value that's
        ///  used to format the string.</param>
        /// <returns>Fluent Interface</returns>
        TFluentType WithValue(Func<T, string> valueAccessor);
    }
    public interface IMessageData<T> : IMessageData<T, IMessageData<T>>
    {
        
    }
}

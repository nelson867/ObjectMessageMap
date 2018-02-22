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
    public interface ISelectorMapper<T>
    {
        /// <summary>
        /// Declares a type to be configured.
        /// </summary>
        /// <typeparam name="TSub">Type to configure.</typeparam>
        /// <returns>Fluent Interface</returns>
        IForChain<TSub> For<TSub>() where TSub : T;

        /// <summary>
        /// Declares a type to be configured as well as providing
        /// an resource key for resource lookup.
        /// </summary>
        /// <typeparam name="TSub">Type to configure.</typeparam>
        /// <returns>Fluent Interface</returns>
        IForChain<TSub> For<TSub>(string typeNameResourceKey) where TSub : T;

        /// <summary>
        /// Declares a type to be configured as well as providing
        /// an accessor lambda that gets the types names.
        /// </summary>
        /// <typeparam name="TSub">Type to configure.</typeparam>
        /// <returns>Fluent Interface</returns>
        IForChain<TSub> For<TSub>(Func<TSub, string> accessor) where TSub : T;

        /// <summary>
        /// Declares a that the specified values is to use the
        /// following configuration.
        /// </summary>
        /// <param name="value">Value to map.</param>
        /// <returns>Fluent Interface</returns>
        IForChain<T> For(T value);
    }
}

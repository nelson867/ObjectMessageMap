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

namespace ObjectMessageMapper.Core.Mappers
{
    /// <summary>
    /// IMapper interfaces. Used to map objects and/or type to items.
    /// </summary>
    internal interface IMapper
    {
        /// <summary>
        /// Property: The search type.
        /// </summary>
        LookupStrategyEnum LookupStrategy { get; set; }

        /// <summary>
        /// Adds an item to the map and maps it to the value given.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="mappedItem">The item to map.</param>
        void Add(object value, IMappedItem mappedItem);

        /// <summary>
        /// Adds the item to the map and maps it to the given type.
        /// </summary>
        /// <typeparam name="TKey">The type to map the item to.</typeparam>
        /// <param name="mappedItem">The item that's mapped.</param>
        void Add<TKey>(IMappedItem mappedItem);

        /// <summary>
        /// Finds and returns the item based on the given object.
        /// </summary>
        /// <param name="item">The object to get the type from.</param>
        /// <returns>The item if found, null if not.</returns>
        IMappedItem Find(object item);

        /// <summary>
        /// Finds and returns the item based on the given type.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns>The item if found, null if not.</returns>
        IMappedItem Find(Type type);
    }
}

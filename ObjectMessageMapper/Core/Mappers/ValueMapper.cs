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

namespace ObjectMessageMapper.Core.Mappers
{
    /// <summary>
    /// Mapper that maps values like int, long, double, etc...
    /// </summary>
    /// <typeparam name="T">Type of values.</typeparam>
    internal class ValueMapper<T> : IMapper
    {
        public LookupStrategyEnum LookupStrategy { get; set; }

        private Dictionary<T, IMappedItem> Map { get; set; }

        public ValueMapper()
        {
            Map = new Dictionary<T, IMappedItem>();
        }

        /// <summary>
        /// Adds an item to the map and maps it to the value given.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="mappedItem">The item to map.</param>
        public void Add(object value, IMappedItem mappedItem)
        {
            Map.Add((T)value, mappedItem);
        }

        /// <summary>
        /// This should not be valid in this context. A value
        /// mapper should only be value Add and not the Type
        /// Add.
        /// </summary>
        /// <exception cref="InvalidOperationException">Always throws.</exception>
        public void Add<TKey>(IMappedItem mappedItem)
        {
            throw new InvalidOperationException("Mapping Types are not supported on value type mappings.");
        }

        /// <summary>
        /// Finds and returns the item based on the given value.
        /// </summary>
        /// <param name="item">The value to search for.</param>
        /// <returns>The item if found, null if not.</returns>
        /// <exception cref="InvalidOperationException">Throws if the input object is not the same type used to map.</exception>
        public IMappedItem Find(object item)
        {
            if (item.GetType() != typeof(T))
            {
                throw new InvalidOperationException("Item is not of type T");
            }
            if (Map.ContainsKey((T)item))
            {
                return Map[(T)item];
            }
            return null;
        }

        /// <summary>
        /// Not Supported. Just returns null if called.
        /// </summary>
        /// <returns>null</returns>
        public IMappedItem Find(Type type)
        {
            return null;
        }
    }
}

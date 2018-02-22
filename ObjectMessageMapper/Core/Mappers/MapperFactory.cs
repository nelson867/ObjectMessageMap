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
    /// Mapper Factory.
    /// </summary>
    internal class MapperFactory
    {
        /// <summary>
        /// Creates the appropriate mapper based on the type.
        /// </summary>
        /// <typeparam name="T">Type of objects to store.</typeparam>
        /// <param name="lookupStrategy">How to search for an item.</param>
        /// <returns>A mapper that implements the IMapper interface.</returns>
        public static IMapper GetMapper<T>(LookupStrategyEnum lookupStrategy = LookupStrategyEnum.ConcreteOnly)
        {
            Type type = typeof(T);

            //if (type.IsEnum) // || type == typeof(Enum))
            //{
            //    return new EnumMapper<T>();
            //}

            if (type.IsValueType)
            {
                return new ValueMapper<T>();
            }

            return new TypeMapper<T>(lookupStrategy);
        }
    }
}

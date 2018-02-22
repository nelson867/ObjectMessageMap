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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectMessageMapper.Core;
using ObjectMessageMapper.Core.Mappers;
using ObjectMessageMapper.Tests.Helpers;

namespace ObjectMessageMapper.Tests.UnitTests
{
    [TestClass]
    public class MapperTests
    {
        [TestMethod, TestCategory("Unit Test")]
        public void OMM_TypeMapper_AddWithObject()
        {
            TypeMapper<Exception> mapper = new TypeMapper<Exception>();
            Exception ex = new Exception("My Exception");
            mapper.Add(ex, new MessageProcessor<Exception>("RESOURCE_KEY"));

            Assert.IsNotNull(mapper.Find(ex));
        }

        [TestMethod, TestCategory("Unit Test")]
        public void OMM_ValueMapper_AddTypeNotSupported()
        {
            ValueMapper<int> mapper = new ValueMapper<int>();
            AssertExtension.Throws<InvalidOperationException>(() => mapper.Add<int>(new MessageProcessor<Exception>("RESOURCE_KEY")));
        }

        [TestMethod, TestCategory("Unit Test")]
        public void OMM_ValueMapper_FindTypeNotSupported()
        {
            ValueMapper<int> mapper = new ValueMapper<int>();
            double val = 1.0;
            AssertExtension.Throws<InvalidOperationException>(() => mapper.Find(val));
        }

        [TestMethod, TestCategory("Unit Test")]
        public void OMM_ValueMapper_Find_NotFound_ReturnsNull()
        {
            ValueMapper<int> mapper = new ValueMapper<int>();
            int val = 1;
            Assert.IsNull(mapper.Find(val));
        }

        [TestMethod, TestCategory("Unit Test")]
        public void OMM_ValueMapper_Find_WithType_ReturnsNull()
        {
            ValueMapper<int> mapper = new ValueMapper<int>();
            int val = 1;
            Assert.IsNull(mapper.Find(val.GetType()));
        }
    }
}

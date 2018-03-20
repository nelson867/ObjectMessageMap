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
using ObjectMessageMapper.Interfaces;

namespace ObjectMessageMapper.Core.Relayers
{
    /// <summary>
    /// This object is used as a relay object that chains the methods
    /// (the Fluent Interface) needed when UseMessage(...) is called
    /// from a chained conditional.
    /// </summary>
    /// <typeparam name="T">The configuring Type</typeparam>
    internal class ConditionalRelayMessage<T> : IConditionalUseMessage<T>
    {
        private SelectorConditional<T> Selector { get; set; }
        private MessageProcessor<T> MessageProcessor { get; set; }
        public ConditionalRelayMessage(SelectorConditional<T> selector, MessageProcessor<T> messageProcessor)
        {
            Selector = selector;
            MessageProcessor = messageProcessor;
        }

        /// <summary>
        /// Configures the message processor to format the string
        /// using the specified lambda accessor expression. The 
        /// lambda expression is given the object and is retrieves
        /// the value to use to format the string.
        /// </summary>
        /// <param name="valueAccessor">Lambda expression used to retrieve the value that's
        ///  used to format the string.</param>
        /// <returns>Fluent Interface</returns>
        public IConditionalUseMessage<T> WithValue(Func<T, string> valueAccessor)
        {
            MessageProcessor.WithValue(valueAccessor);
            return this;
        }

        /// <summary>
        /// Declares that the following configuration is to be
        /// used when the given condition is true.
        /// </summary>
        /// <param name="condition">Lambda to test the object</param>
        /// <returns>Fluent Interface</returns>
        public IWhenChain<T> When(Func<T, bool> condition)
        {
            return new ConditionalRelayWhen<T>(Selector, condition);
        }

        /// <summary>
        /// Declares that the following configuration is to
        /// be used when other type/values are not found.
        /// </summary>
        /// <returns>Fluent Interface</returns>
        public IDefaultChain<T> Default()
        {
            return new ConditionalRelayDefault<T>(Selector);
        }
    }
}

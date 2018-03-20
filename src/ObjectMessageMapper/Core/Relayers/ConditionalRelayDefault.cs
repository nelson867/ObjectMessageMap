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
    /// (the Fluent Interface) needed when Default(...) is called from
    /// a chained conditional. 
    /// </summary>
    /// <typeparam name="T">The configuring Type</typeparam>
    internal class ConditionalRelayDefault<T> : IDefaultChain<T>
    {
        private SelectorConditional<T> Selector { get; set; }

        public ConditionalRelayDefault(SelectorConditional<T> selector)
        {
            Selector = selector;
        }

        /// <summary>
        /// Declares that the message should come from the resources
        /// with the specified key.
        /// </summary>
        /// <param name="resourceKey">string: resource string table key.</param>
        /// <returns>Fluent Interface</returns>
        public IMessageData<T> UseMessage(string resourceKey)
        {
            MessageProcessor<T> messageProcessor = new MessageProcessor<T>(resourceKey);
            Selector.DefaultItem = messageProcessor;
            return messageProcessor;
        }

        /// <summary>
        /// Declares that the message will be retrieved via a user specified lambda.
        /// </summary>
        /// <param name="accessor">Lambda expression that's given the mapped object and returns a string.</param>
        /// <returns>Fluent Interface</returns>
        public IMessageData<T> UseMessage(Func<T, string> accessor)
        {
            MessageProcessor<T> messageProcessor = new MessageProcessor<T>(accessor);
            Selector.DefaultItem = messageProcessor;
            return messageProcessor;
        }

        /// <summary>
        /// Configures the current mapping using the current type value.
        /// </summary>
        /// <param name="configure">Lambda express used to configure the mapped type.</param>
        public void Configure(Action<IConfigureChain<T>> configure)
        {
            SelectorMapper<T> mapper = new SelectorMapper<T>();
            Selector.DefaultItem = mapper;
            configure(mapper);
        }

        /// <summary>
        /// Configures a mapping that is based on property value of the object.
        /// </summary>
        /// <typeparam name="TSub">Sub type to configure</typeparam>
        /// <param name="accessor">Lambda expression used to access the sub property.</param>
        /// <param name="configure">Lambda expression used to configure the mapping.</param>
        public void Configure<TSub>(Func<T, TSub> accessor, Action<IConfigureChain<TSub>> configure)
        {
            SelectorMapper<TSub> mapper = new SelectorMapperSub<T, TSub>(accessor);
            Selector.DefaultItem = mapper;
            configure(mapper);
        }
    }
}

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
    /// (the Fluent Interface) needed when Default(...) is called. 
    /// </summary>
    /// <typeparam name="T">The new configuring Type</typeparam>
    /// <typeparam name="TParent">The current configuring Type</typeparam>
    internal class SelectorRelayDefault<T, TParent> : IDefaultChain<T>
    {
        private SelectorMapper<TParent> Selector { get; set; }
        public SelectorRelayDefault(SelectorMapper<TParent> selectorMapper)
        {
            Selector = selectorMapper;
        }

        /// <summary>
        /// Creates a Message Processor that will retrieve a string from the
        /// resource file using the specified Key 
        /// Usages:
        ///    cfg.For().Default().UseMessage(...)
        ///    cfg.Default().UseMessage(...) 
        /// </summary>
        /// <param name="resourceKey">Key into the string table.</param>
        /// <returns>Fluent Interface</returns>
        public IMessageData<T> UseMessage(string resourceKey)
        {
            MessageProcessor<T> mappedItem = new MessageProcessor<T>(resourceKey);
            Selector.SetDefaultMappedItem(mappedItem);
            return mappedItem;
        }

        /// <summary>
        /// Creates a Message Processor that will retrieve a string using an
        /// accessor lambda the takes the current object and returns a string.
        /// Usages:
        ///    cfg.For().Default().UseMessage(...)
        ///    cfg.Default().UseMessage(...) 
        /// </summary>
        /// <param name="accessor">Lambda expression that takes the parent
        /// type and returns a string.</param>
        /// <returns>Fluent Interface</returns>
        public IMessageData<T> UseMessage(Func<T, string> accessor)
        {
            MessageProcessor<T> mappedItem = new MessageProcessor<T>(accessor);
            Selector.SetDefaultMappedItem(mappedItem);
            return mappedItem;
        }

        /// <summary>
        /// Used to create a sub configuration with a property accessor for the
        /// specified type.
        /// Usages:
        ///    cmd.For().Default().Configure(...)
        ///    cmd.Default().Configure(...)
        /// </summary>
        /// <typeparam name="TSub">The new subt type to configure.</typeparam>
        /// <param name="accessor">Lambda accessor that takes the current type
        /// object and returns the sub object type.</param>
        /// <param name="configure">Lambda expression that builds the configuration
        /// for the new type.</param>
        public void Configure<TSub>(Func<T, TSub> accessor, Action<IConfigureChain<TSub>> configure)
        {
            Selector.ConfigureDefault<TSub, T>(accessor, configure);
        }

        /// <summary>
        /// Used to create a sub configuration without a property accessor for the
        /// specified type.
        /// Usages:
        ///    cmd.For().Default().Configure(...)
        ///    cmd.Default().Configure(...)
        /// </summary>
        /// <param name="configure">Lambda expression that builds the configuration
        /// for the new type.</param>
        public void Configure(Action<IConfigureChain<T>> configure)
        {
            Selector.ConfigureDefault<T>(configure);
        }
    }
}

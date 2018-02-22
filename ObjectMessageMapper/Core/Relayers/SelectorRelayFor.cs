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
using ObjectMessageMapper.Core.Mappers;

namespace ObjectMessageMapper.Core.Relayers
{
    /// <summary>
    /// This object is used as a relay object that chains the methods
    /// (the Fluent Interface) needed when For(...) is called from a
    /// configuration method (SelectorMapper). 
    /// </summary>
    /// <typeparam name="T">The new configuring Type</typeparam>
    /// <typeparam name="TParent">The current configuring Type</typeparam>
    internal class SelectorRelayFor<T, TParent> : IForChain<T>
    {
        private SelectorMapper<TParent> Selector { get; set; }
        private T Value { get; set; }
        private bool ValueBased { get; set; }

        public string TypeNameResourceKey { get; set; }
        public Func<T, string> TypeNameAccessor { get; set; }

        public SelectorRelayFor(SelectorMapper<TParent> selectorMapper)
        {
            Selector = selectorMapper;
        }
        public SelectorRelayFor(SelectorMapper<TParent> selectorMapper, T value)
        {
            Selector = selectorMapper;
            Value = value;
            ValueBased = true;
        }

        #region Fluent Interface Methods

        /// <summary>
        /// Relay handler. Called from the SelectorMapper's For(...). This
        /// creates a conditional handler that will select a spcific
        /// configuration when a specifid condition is true.
        /// </summary>
        /// <param name="condition">Test delegate to test whether to execute action.</param>
        /// <returns>Fluent Interface</returns>
        public IWhenChain<T> When(Func<T, bool> condition)
        {
            SelectorConditional<T> selectCondition = new SelectorConditional<T>()
            {
                TypeNameResourceKey = TypeNameResourceKey,
                TypeNameAccessor = TypeNameAccessor
            };
            AddToSelectorMap(selectCondition);
            return selectCondition.When(condition);
        }

        /// <summary>
        /// Relay handler. Called from the SelectorMapper's For(...).
        /// This creates the default handler for when the type is
        /// not found.
        /// </summary>
        /// <returns></returns>
        public IDefaultChain<T> Default()
        {
            SelectorMapper<T> newSelector = null;
            Selector.Configure<T>(cfg => newSelector = (SelectorMapper<T>)cfg);
            return new SelectorRelayDefault<T, T>(newSelector);
        }

        /// <summary>
        /// Called cfg.For().UseMessage(...). Creates a Message Processor
        /// that will retrieve a string from the resource file using the
        /// specified Key 
        /// </summary>
        /// <param name="resourceKey">Key into the string table.</param>
        /// <returns>Fluent Interface</returns>
        public IMessageData<T> UseMessage(string resourceKey)
        {
            MessageProcessor<T> mappedItem = new MessageProcessor<T>(resourceKey);
            AddToSelectorMap(mappedItem);
            return mappedItem;
        }

        /// <summary>
        /// Called cfg.For().UseMessage(...). Creates a Message Processor
        /// that will retrieve a string using an accessor lambda the takes
        /// the current object and returns a string.
        /// </summary>
        /// <param name="accessor">Lambda expression that takes the parent type and returns a string.</param>
        /// <returns>Fluent Interface</returns>
        public IMessageData<T> UseMessage(Func<T, string> accessor)
        {
            MessageProcessor<T> mappedItem = new MessageProcessor<T>(accessor);
            AddToSelectorMap(mappedItem);
            return mappedItem;
        }

        /// <summary>
        /// Called cfg.For().Configure(...). Used to create a sub configuration
        /// with a property accessor for the specified type.
        /// </summary>
        /// <typeparam name="TSub">The new subt type to configure.</typeparam>
        /// <param name="accessor">Lambda accessor that takes the current type
        /// object and returns the sub object type.</param>
        /// <param name="configure">Lambda expression that builds the configuration
        /// for the new type.</param>
        public void Configure<TSub>(Func<T, TSub> accessor, Action<IConfigureChain<TSub>> configure)
        {
            // We are in the context of a For<type>(...).Configure<type2>() method and
            // therefore we have a ParentType, For Type, and now a new SubType. Basically
            // you have the following:
            //
            //   cfg.For<Type1>().Configure<Type2>(...)
            //
            // The cfg object carries the parent type (TParent),
            // the For carries the sub type (T),
            // and the Confgirure (this call), carries the third type (TSub).
            //
            // In this case, we create two mapper objects. The reason is because you
            // may have a TypeName specification which will go with the For<type> and
            // not the TSub type. Therefore we create a new SelectorMapper and
            // associate the TypeName accessors to that node and then create another
            // one for this configuration.

            SelectorMapper<T> mapper = null;
            Selector.Configure<T>(cfg => mapper = (SelectorMapper<T>)cfg, LookupStrategyEnum.InheritChain, TypeNameResourceKey, TypeNameAccessor);


            SelectorMapper<TSub> subMapper = new SelectorMapperSub<T, TSub>(accessor, LookupStrategyEnum.ConcreteOnly);
            mapper.Mapper.Add<T>(subMapper);
            configure(subMapper);
        }

        /// <summary>
        /// Configures the current mapping using the current type value.
        /// </summary>
        /// <param name="configure">Lambda express used to configure the mapped type.</param>
        public void Configure(Action<IConfigureChain<T>> configure)
        {
            Selector.Configure<T>(configure, LookupStrategyEnum.InheritChain, TypeNameResourceKey, TypeNameAccessor);
        }

        #endregion

        /// <summary>
        /// Private helper method for adding an item to the selector's mapper.
        /// It check if the item is Type based or Value based and calls
        /// the appropriate add method.
        /// </summary>
        /// <param name="item"></param>
        private void AddToSelectorMap(IMappedItem item)
        {
            if (ValueBased)
            {
                Selector.Mapper.Add(Value, item);
            }
            else
            {
                Selector.Mapper.Add<T>(item);
            }
        }
    }
}

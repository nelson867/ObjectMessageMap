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
using ObjectMessageMapper.Core.Relayers;
using ObjectMessageMapper.Core.Mappers;

namespace ObjectMessageMapper.Core
{
    /// <summary>
    /// Configuration node that maps object types to specified configurations.
    /// </summary>
    /// <typeparam name="T">Object type to configure.</typeparam>
    internal class SelectorMapper<T> : MappedItem<T>, IConfigureChain<T>
    {
        #region Properties

        internal IMapper Mapper { get; private set; }
        internal IMappedItem DefaultItem { get; set; }

        #endregion

        #region Constructor

        public SelectorMapper(LookupStrategyEnum lookupStrategy = LookupStrategyEnum.ConcreteOnly)
        {
            Mapper = MapperFactory.GetMapper<T>(lookupStrategy);
        }

        #endregion

        #region Fluent Interface Implementation

        /// <summary>
        /// Declares a type to be configured.
        /// </summary>
        /// <typeparam name="TSub">Type to configure.</typeparam>
        /// <returns>Fluent Interface</returns>
        public IForChain<TSub> For<TSub>() where TSub : T
        {
            return new SelectorRelayFor<TSub, T>(this);
        }

        /// <summary>
        /// Declares a type to be configured as well as providing
        /// an resource key for resource lookup.
        /// </summary>
        /// <typeparam name="TSub">Type to configure.</typeparam>
        /// <returns>Fluent Interface</returns>
        public IForChain<TSub> For<TSub>(string typeNameResourceKey) where TSub : T
        {
            return new SelectorRelayFor<TSub, T>(this) { TypeNameResourceKey = typeNameResourceKey };
        }

        /// <summary>
        /// Declares a type to be configured as well as providing
        /// an accessor lambda that gets the types names.
        /// </summary>
        /// <typeparam name="TSub">Type to configure.</typeparam>
        /// <returns>Fluent Interface</returns>
        public IForChain<TSub> For<TSub>(Func<TSub, string> accessor) where TSub : T
        {
            return new SelectorRelayFor<TSub, T>(this) { TypeNameAccessor = accessor };
        }

        /// <summary>
        /// Declares a that the specified values is to use the
        /// following configuration.
        /// </summary>
        /// <param name="value">Value to map.</param>
        /// <returns>Fluent Interface</returns>
        public IForChain<T> For(T value)
        {
            return new SelectorRelayFor<T, T>(this, value);
        }

        /// <summary>
        /// Declares that the following configuration is to
        /// be used when other type/values are not found.
        /// </summary>
        /// <returns>Fluent Interface</returns>
        public IDefaultChain<T> Default()
        {
            return new SelectorRelayDefault<T, T>(this);
        }

        #endregion

        #region Message Retrieval Interface Methods

        /// <summary>
        /// Gets the messages that's mapped to the specified object.
        /// </summary>
        /// <param name="obj">The object to get the message for.</param>
        /// <returns>string</returns>
        protected override string GetMessageFromMapping(object obj)
        {
            IMappedItem item = FindItem(obj);
            if (item != null)
            {
                return item.GetMessage(obj);
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the messages that's mapped to the specified type.
        /// </summary>
        /// <param name="type">The type to get the message for.</param>
        /// <returns>string</returns>
        protected override string GetMessageFromMapping(Type type)
        {
            IMappedItem item = FindItem(type);
            if (item != null)
            {
                return item.GetMessage(type);
            }
            return string.Empty;
        }

        #endregion

        #region public helper methods

        /// <summary>
        /// Configure using the current type. This is special because
        /// it has not accessor to get at the sub property type.
        /// </summary>
        /// <typeparam name="TSub">The new type</typeparam>
        /// <param name="configure">Lambda expression that configures the mapping</param>
        /// <param name="lookupStrategy">The mapper's lookup strategy.</param>
        public void Configure<TSub>(Action<IConfigureChain<TSub>> configure,
            LookupStrategyEnum lookupStrategy = LookupStrategyEnum.ConcreteOnly,
            string typeNameResourceKey = "",
            Func<TSub, string> TypeNameAccessor = null)
        {
            SelectorMapper<TSub> mapper = new SelectorMapper<TSub>(lookupStrategy)
            {
                TypeNameResourceKey = typeNameResourceKey,
                TypeNameAccessor = TypeNameAccessor
            };

            Mapper.Add<TSub>(mapper);
            configure(mapper);
        }

        /// <summary>
        /// Configures the Default handler.
        /// </summary>
        /// <typeparam name="TSub">The new type</typeparam>
        /// <typeparam name="TParent">the current parent type</typeparam>
        /// <param name="accessor">Lambda expression that access the property message lookup.</param>
        /// <param name="configure">Lambda expression that configures the mapping</param>
        public void ConfigureDefault<TSub>(Action<IConfigureChain<TSub>> configure,
            LookupStrategyEnum lookupStrategy = LookupStrategyEnum.ConcreteOnly)
        {
            SelectorMapper<TSub> mapper = new SelectorMapper<TSub>(lookupStrategy);
            DefaultItem = mapper;
            configure(mapper);
        }

        /// <summary>
        /// Configures the Default handler.
        /// </summary>
        /// <typeparam name="TSub">The new type</typeparam>
        /// <typeparam name="TParent">the current parent type</typeparam>
        /// <param name="accessor">Lambda expression that access the property message lookup.</param>
        /// <param name="configure">Lambda expression that configures the mapping</param>
        public void ConfigureDefault<TSub, TParent>(Func<TParent, TSub> accessor,
            Action<IConfigureChain<TSub>> configure,
            LookupStrategyEnum lookupStrategy = LookupStrategyEnum.ConcreteOnly)
        {
            SelectorMapper<TSub> mapper = new SelectorMapperSub<TParent, TSub>(accessor, lookupStrategy);
            DefaultItem = mapper;
            configure(mapper);
        }

        /// <summary>
        /// Helper Method that sets the default 
        /// </summary>
        /// <param name="mappedItem"></param>
        public void SetDefaultMappedItem(IMappedItem mappedItem)
        {
            DefaultItem = mappedItem;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Find the items to use for getting the message. If the object
        /// is not mapped, then it will return the default item. If the
        /// default item is not set, then it will retrun null;
        /// </summary>
        /// <param name="obj">Object/value to find</param>
        /// <returns>IMappedItem or null</returns>
        protected IMappedItem FindItem(object obj)
        {
            IMappedItem item = Mapper.Find(obj);

            if (item != null)
            {
                return item;
            }
            return DefaultItem;
        }

        /// <summary>
        /// Find the items to use for getting the message. If the object
        /// is not mapped, then it will return the default item. If the
        /// default item is not set, then it will retrun null;
        /// </summary>
        /// <param name="type">Object type to find</param>
        /// <returns>IMappedItem or null</returns>
        protected IMappedItem FindItem(Type type)
        {
            IMappedItem item = Mapper.Find(type);

            if (item != null)
            {
                return item;
            }
            return DefaultItem;
        }
        #endregion
    }
}

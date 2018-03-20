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
using System.Linq;
using ObjectMessageMapper.Interfaces;
using ObjectMessageMapper.Core.Helpers;
using ObjectMessageMapper.Core.Relayers;

namespace ObjectMessageMapper.Core
{
    /// <summary>
    /// Configuration node that represents a conditional configuration. "When(...)..."
    /// </summary>
    /// <typeparam name="T">Type of object mapped to the configuration.</typeparam>
    internal class SelectorConditional<T> : MappedItem<T>, ISelectorConditional<T>
    {
        #region Properties
        public IMappedItem DefaultItem { get; set; }
        private List<ConditionalItem<T>> Items { get; set; }
        #endregion

        #region Constructor
        public SelectorConditional()
        {
            Items = new List<ConditionalItem<T>>();
        }
        #endregion

        #region Fluent Interface Methods

        /// <summary>
        /// Declares that the following configuration is to be
        /// used when the given condition is true.
        /// </summary>
        /// <param name="condition">Lambda to test the object</param>
        /// <returns>Fluent Interface</returns>
        public IWhenChain<T> When(Func<T, bool> condition)
        {
            return new ConditionalRelayWhen<T>(this, condition);
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
            IMappedItem item = FindItem((T)obj);
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
            // If we get here, then the user is looking for a type name
            // but the configuration hasn't specified one yet. Therefore
            // we just return an empty string
            return string.Empty;
        }

        #endregion

        #region public Methods
        
        /// <summary>
        /// Helper method used to add a conditional configuration from 
        /// the relay object.
        /// </summary>
        /// <param name="condition">Lambda expression to use to test if the condition is true.</param>
        /// <param name="item">Item to map to the condition.</param>
        public void AddConditionItem(Func<T, bool> condition, IMappedItem item)
        {
            Items.Add(new ConditionalItem<T>()
            {
                ConditionTest = condition,
                Item = item
            });
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Helper method to find the correct item based on it's condition test.
        /// If none match the condition, then the default item is returned.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private IMappedItem FindItem(T obj)
        {
            ConditionalItem<T> item = Items.FirstOrDefault(entry => entry.ConditionTest(obj));
            if (item != null)
            {
                return item.Item;
            }
            return DefaultItem;
        }

        #endregion
    }
}

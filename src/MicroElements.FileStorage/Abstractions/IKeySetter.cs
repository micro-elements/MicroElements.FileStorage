// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Key setter strategy.
    /// Uses for setting key property.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IKeySetter<TValue>
    {
        /// <summary>
        /// Gets expression for Id property access.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression<Action<TValue, string>> SetIdExpression();

        /// <summary>
        /// Get Id property accessor function.
        /// </summary>
        /// <returns>Id function.</returns>
        Action<TValue, string> SetIdFunc();
    }
}
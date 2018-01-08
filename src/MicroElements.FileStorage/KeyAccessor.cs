// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage
{
    /// <summary>
    /// Key accessor implemented be expressions.
    /// </summary>
    /// <typeparam name="TValue">Entity type.</typeparam>
    public class KeyAccessor<TValue> : IKeyGetter<TValue>, IKeySetter<TValue>
    {
        private readonly Expression<Func<TValue, string>> _getKeyExpression;
        private readonly Expression<Action<TValue, string>> _setKeyExpression;
        private readonly Func<TValue, string> _idFunc;
        private readonly Action<TValue, string> _setIdFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAccessor{TValue}"/> class.
        /// </summary>
        /// <param name="getKeyExpression">Expression for get key.</param>
        /// <param name="setKeyExpression">Expression for set key.</param>
        public KeyAccessor([NotNull] Expression<Func<TValue, string>> getKeyExpression, [NotNull] Expression<Action<TValue, string>> setKeyExpression)
        {
            Check.NotNull(getKeyExpression, nameof(getKeyExpression));
            Check.NotNull(setKeyExpression, nameof(setKeyExpression));

            _getKeyExpression = getKeyExpression;
            _setKeyExpression = setKeyExpression;

            _idFunc = _getKeyExpression.Compile();
            _setIdFunc = _setKeyExpression.Compile();
        }

        /// <inheritdoc />
        public Expression<Func<TValue, string>> GetIdExpression() => _getKeyExpression;

        /// <inheritdoc />
        public Func<TValue, string> GetIdFunc() => _idFunc;

        /// <inheritdoc />
        public Expression<Action<TValue, string>> SetIdExpression() => _setKeyExpression;

        /// <inheritdoc />
        public Action<TValue, string> SetIdFunc() => _setIdFunc;
    }
}

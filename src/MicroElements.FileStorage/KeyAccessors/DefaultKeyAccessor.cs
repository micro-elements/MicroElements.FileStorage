// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Utils;

namespace MicroElements.FileStorage.KeyAccessors
{
    /// <summary>
    /// Default key getter strategy. Uses property 'Id' for access to entity key.
    /// <para>Caches expression and func.</para>
    /// </summary>
    /// <typeparam name="TValue">Entity type.</typeparam>
    public class DefaultKeyAccessor<TValue> : IKeyGetter<TValue>, IKeySetter<TValue>
    {
        /// <summary>
        /// Default instance if KeyAccessor for type <see cref="TValue"/>
        /// </summary>
        public static readonly DefaultKeyAccessor<TValue> Instance = new DefaultKeyAccessor<TValue>();

        private readonly Lazy<Expression<Func<TValue, string>>> _getIdExpression;
        private readonly Lazy<Expression<Action<TValue, string>>> _setIdExpression;
        private readonly Lazy<Func<TValue, string>> _idFunc;
        private readonly Lazy<Action<TValue, string>> _setIdFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultKeyAccessor{TValue}"/> class.
        /// <para>By default uses string property <c>Id</c></para>
        /// </summary>
        /// <param name="keyPropertyName">Name of <c>Id</c> property.</param>
        public DefaultKeyAccessor(string keyPropertyName = "Id")
        {
            // ReSharper disable ConvertClosureToMethodGroup
            _getIdExpression = new Lazy<Expression<Func<TValue, string>>>(() => ExpressionFactory.GetIdExpression<TValue>(keyPropertyName));
            _setIdExpression = new Lazy<Expression<Action<TValue, string>>>(() => ExpressionFactory.SetIdExpression<TValue>(keyPropertyName));
            _idFunc = new Lazy<Func<TValue, string>>(() => _getIdExpression.Value.Compile());
            _setIdFunc = new Lazy<Action<TValue, string>>(() => _setIdExpression.Value.Compile());
        }

        /// <inheritdoc />
        public Expression<Func<TValue, string>> GetIdExpression() => _getIdExpression.Value;

        /// <inheritdoc />
        public Func<TValue, string> GetIdFunc() => _idFunc.Value;

        /// <inheritdoc />
        public Expression<Action<TValue, string>> SetIdExpression() => _setIdExpression.Value;

        /// <inheritdoc />
        public Action<TValue, string> SetIdFunc() => _setIdFunc.Value;
    }
}

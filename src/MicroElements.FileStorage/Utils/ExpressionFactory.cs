using System;
using System.Linq.Expressions;
using System.Reflection;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Utils
{
    /// <summary>
    /// Factory of expressions.
    /// </summary>
    public static class ExpressionFactory
    {
        /// <summary>
        /// Gets Id property accessor expression.
        /// <para>Generates expression: <code>instance => instance.Id</code></para>
        /// </summary>
        /// <typeparam name="TValue">Entity type.</typeparam>
        /// <returns>Get Id property accessor expression.</returns>
        public static Expression<Func<TValue, string>> GetIdExpression<TValue>(string keyPropertyName = "Id")
        {
            //instance => instance.Id;
            ParameterExpression instance = Expression.Parameter(typeof(TValue), "instance");
            MemberExpression memberExpression = Expression.Property(instance, keyPropertyName);
            return Expression.Lambda<Func<TValue, string>>(memberExpression, instance);
        }

        /// <summary>
        /// Gets Id property accessor expression when Id is not <see cref="string"/>.
        /// <para>Generates expression: <code>instance => (string)Convert.ChangeType(instance.Id, typeof(string));</code></para>
        /// </summary>
        /// <remarks>Uses default type conversion by <see cref="Convert.ChangeType(object,System.Type)"/>.</remarks>
        /// <typeparam name="TValue">Entity type.</typeparam>
        /// <returns>Get Id property accessor expression.</returns>
        public static Expression<Func<TValue, string>> GetIdWithConvertExpression<TValue>(string keyPropertyName = "Id")
        {
            //instance => (string)Convert.ChangeType(instance.Id, typeof(string))
            ParameterExpression instance = Expression.Parameter(typeof(TValue), "instance");
            MemberExpression memberExpression = Expression.Property(instance, keyPropertyName);

            MethodInfo methodInfo = typeof(Convert).GetMethod(nameof(Convert.ChangeType), new[] { typeof(object), typeof(Type) });
            var changeType = Expression.Call(methodInfo, Expression.TypeAs(memberExpression, typeof(object)), Expression.Constant(typeof(string)));

            var idExpression = Expression.Lambda<Func<TValue, string>>(Expression.Convert(changeType, typeof(string)), instance);
            return idExpression;
        }

        /// <summary>
        /// Creates SetId property accessor.
        /// <para>Generates expression: <code>(instance, key) => instance.Id = key</code></para>
        /// </summary>
        /// <typeparam name="TValue">Entity type.</typeparam>
        /// <returns>Set Id property accessor expression.</returns>
        public static Expression<Action<TValue, string>> SetIdExpression<TValue>(string keyPropertyName = "Id")
        {
            //(instance, key) => instance.Id = key;
            ParameterExpression instance = Expression.Parameter(typeof(TValue), "instance");
            MemberExpression idProperty = Expression.Property(instance, keyPropertyName);
            ParameterExpression key = Expression.Parameter(typeof(string), "key");
            BinaryExpression binaryExpression = Expression.Assign(idProperty, key);
            return Expression.Lambda<Action<TValue, string>>(binaryExpression, instance, key);
        }

        /// <summary>
        /// Generates expression: <code>collection => collection.Add(item)</code> where <c>collection</c> is generic <see cref="IDocumentCollection"/>
        /// </summary>
        /// <typeparam name="TValue">Entity type.</typeparam>
        /// <returns></returns>
        public static Expression<Action<TValue>> AddExpression<TValue>()
        {
            //collection => collection.Add(item)
            var itemType = typeof(TValue);
            var collectionType = typeof(IDocumentCollection<>).MakeGenericType(itemType);
            ParameterExpression instance = Expression.Parameter(collectionType, "instance");
            ParameterExpression item = Expression.Parameter(itemType, "item");
            var callExpression = Expression.Call(instance, nameof(IDocumentCollection<object>.Add), new[] { itemType }, item);
            return Expression.Lambda<Action<TValue>>(callExpression, instance);
        }
    }
}
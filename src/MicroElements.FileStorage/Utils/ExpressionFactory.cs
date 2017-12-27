using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// <para>Generates expression: instance => instance.Id</para>
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
        /// Creates SetId property accessor.
        /// <para>Generates expression: (instance, key) => instance.Id = key</para>
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

        public static Expression<Action<TValue>> AddExpression<TValue>()
        {
            //IDocumentCollection<T> collection
            //collection => collection.Add(item)
            var itemType = typeof(TValue);
            var collectionType = typeof(IDocumentCollection<>).MakeGenericType(itemType);
            ParameterExpression instance = Expression.Parameter(collectionType, "instance");
            ParameterExpression item = Expression.Parameter(itemType, "item");
            var callExpression = Expression.Call(instance, "Add", new[] { itemType }, item);
            return Expression.Lambda<Action<TValue>>(callExpression, instance);
        }

        static Expression<Func<IEnumerable<T>, T>> CreateLambda<T>()
        {
            var source = Expression.Parameter(typeof(IEnumerable<T>), "source");

            var call = Expression.Call(typeof(Enumerable), "Last", new Type[] { typeof(T) }, source);

            return Expression.Lambda<Func<IEnumerable<T>, T>>(call, source);
        }

    }
}
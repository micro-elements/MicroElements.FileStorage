// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace MicroElements.FileStorage
{
    public static class ExpressionUtils
    {
        public static PropertyInfo GetProperty<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            var member = GetMemberExpression(expression).Member;
            var property = member as PropertyInfo;
            if (property == null)
            {
                throw new InvalidOperationException(string.Format("Member with Name '{0}' is not a property.", member.Name));
            }
            return property;
        }

        private static MemberExpression GetMemberExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", "expression");
            }

            return memberExpression;
        }

        public static Action<TEntity, TProperty> CreateSetter<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            PropertyInfo propertyInfo = ExpressionUtils.GetProperty(property);

            ParameterExpression instance = Expression.Parameter(typeof(TEntity), "instance");
            ParameterExpression parameter = Expression.Parameter(typeof(TProperty), "param");

            var body = Expression.Call(instance, propertyInfo.GetSetMethod(), parameter);
            var parameters = new ParameterExpression[] { instance, parameter };

            return Expression.Lambda<Action<TEntity, TProperty>>(body, parameters).Compile();
        }

        public static Func<TEntity, TProperty> CreateGetter<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            PropertyInfo propertyInfo = ExpressionUtils.GetProperty(property);

            ParameterExpression instance = Expression.Parameter(typeof(TEntity), "instance");

            var body = Expression.Call(instance, propertyInfo.GetGetMethod());
            var parameters = new ParameterExpression[] { instance };

            return Expression.Lambda<Func<TEntity, TProperty>>(body, parameters).Compile();
        }

        public static Func<TEntity> CreateDefaultConstructor<TEntity>()
        {
            var body = Expression.New(typeof(TEntity));
            var lambda = Expression.Lambda<Func<TEntity>>(body);

            return lambda.Compile();
        }

        public static Func<object> CreateInstance(Type classType, Type itemType, object arg)
        {
            //not tested
            var genericType = classType.MakeGenericType(itemType);
            var constructorInfo = genericType.GetConstructor(Array.Empty<Type>());
            var body = Expression.New(constructorInfo, Expression.Constant(arg));
            var lambda = Expression.Lambda<Func<object>>(body);
            return lambda.Compile();
        }

        //https://stackoverflow.com/questions/4432026/activator-createinstance-performance-alternative
        public delegate object ObjectActivator();

        public static ObjectActivator CreateCtor(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("type");
            }
            ConstructorInfo emptyConstructor = type.GetConstructor(Type.EmptyTypes);
            var dynamicMethod = new DynamicMethod("CreateInstance", type, Type.EmptyTypes, true);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Newobj, emptyConstructor);
            ilGenerator.Emit(OpCodes.Ret);
            return (ObjectActivator)dynamicMethod.CreateDelegate(typeof(ObjectActivator));
        }

        public static Expression<Func<IEnumerable<T>, T>> CreateLambda<T>()
        {
            return source => source.Last();
        }

        public static LambdaExpression Last(Type yourType)
        {
            var createMeth = typeof(ExpressionUtils).GetMethod("CreateLambda");
            LambdaExpression l = (LambdaExpression)createMeth.MakeGenericMethod(yourType).Invoke(null, new object[0]);
            return l;
        }

        //https://stackoverflow.com/questions/2850265/calling-a-generic-method-using-lambda-expressions-and-a-type-only-known-at-runt?rq=1
        public static LambdaExpression CreateLambda(Type type)
        {
            var source = Expression.Parameter(
                typeof(IEnumerable<>).MakeGenericType(type), "source");

            var call = Expression.Call(
                typeof(Enumerable), "Last", new Type[] { type }, source);

            return Expression.Lambda(call, source);
        }

    }
}

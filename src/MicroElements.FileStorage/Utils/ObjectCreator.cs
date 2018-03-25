// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace MicroElements.FileStorage.Utils
{
    /// <summary>
    /// Class that provides methods of creating types of objects using compiled lambda expressions.  
    /// This method of object creation is much faster than .Net Activator method when creating
    /// multiple objects.  It is much slower when creating a single object.  The user should use
    /// GetCreator to create a compiled lambda expression that can then be used to create multiple
    /// instances of that type.  The compiled lambda expression should be cached for re-use.
    /// Do not use this method for creating single objects.
    /// </summary>
    public static class ObjectCreator
    {
        /// <summary>
        /// A delegate that can be used to repeatedly create instances of type T using
        /// a specific constructor.  You cannot use the same delegate for different
        /// constructors.  You should create a delegate using GetCreator for each
        /// constructor that you need to call.
        /// </summary>
        /// <typeparam name="T">The type of object that this delegate will create.</typeparam>
        /// <param name="parameters">The parameters to create the object with.</param>
        public delegate T Creator<T>(params object[] parameters);

        #region Methods
        /// <summary>
        /// Static method that gets the number of constructors this type has.
        /// Uses reflection so should not be called repeatedly, cache the 
        /// result if need be.
        /// </summary>
        /// <typeparam name="T">The type of object for which the number of constructors is being queried.</typeparam>
        /// <returns>Number of constructors.</returns>
        public static int NumberOfConstructors<T>() where T : class
        {
            return typeof(T).GetConstructors().Length;
        }

        /// <summary>
        /// Static method that gets the parameters that a constructor takes.
        /// Uses reflection so should not be called repeatedly, cache the 
        /// result if need be.
        /// </summary>
        /// <typeparam name="T">The type of object for which the parameters of the constructor is being queried.</typeparam>
        /// <param name="constructorId">Id of the constructor to query.</param>
        /// <returns>Ordered array of types which can be zero-length.</returns>
        public static Type[] ConstructorParameters<T>(int constructorId) where T : class
        {
            // Get constructor information.
            ConstructorInfo[] constructors = typeof(T).GetConstructors();

            // Valid request?
            if (constructorId < 0 || constructorId >= constructors.Length)
            {
                throw new ArgumentException("This type does not contain a constructor for id: " + constructorId.ToString(), "constructorId");
            }

            // Get the parameters for this constructor.
            ParameterInfo[] paramsInfo = constructors[constructorId].GetParameters();

            // Result array.
            Type[] result = new Type[paramsInfo.Length];

            // Go through each one.
            for (int i = 0; i < paramsInfo.Length; ++i)
            {
                result[i] = paramsInfo[i].ParameterType;
            }

            // Done.
            return result;
        }

        /// <summary>
        /// Static method that gets a compiled lambda expression that can then be used
        /// to repeatedly create instances of that type using the constructor that
        /// the compiled lambda expression was created from.
        /// Uses the first constructor.
        /// </summary>
        /// <typeparam name="T">The type of object that the compiled lamda expression will create.</typeparam>
        /// <returns>A compiled lambda expression in the form of a delegate.</returns>
        /// <exception cref="System.ArgumentException">Thrown when T does not have a constructor.</exception>
        public static Creator<T> GetCreator<T>() where T : class
        {
            // Get constructor information.
            ConstructorInfo[] constructors = typeof(T).GetConstructors();

            // Valid request?
            if (constructors.Length == 0)
            {
                throw new ArgumentException("This type does not contain a constructor.");
            }

            // Create the compiled lambda expression
            return GetCreator<T>(constructors[0], ConstructorParameters<T>(0));
        }

        /// <summary>
        /// Static method that gets a compiled lambda expression that can then be used
        /// to repeatedly create instances of that type using the constructor that
        /// the compiled lambda expression was created from.
        /// Uses the specified constructor.
        /// </summary>
        /// <typeparam name="T">The type of object that the compiled lamda expression will create.</typeparam>
        /// <param name="constructorId">Id of the constructor to use.</param>
        /// <returns>A compiled lambda expression in the form of a delegate.</returns>
        /// <exception cref="System.ArgumentException">Thrown when T does not have a constructor at that id.</exception>
        public static Creator<T> GetCreator<T>(int constructorId) where T : class
        {
            // Get constructor information.
            ConstructorInfo[] constructors = typeof(T).GetConstructors();

            // Valid request?
            if (constructorId < 0 || constructorId >= constructors.Length)
            {
                throw new ArgumentException("This type does not contain a constructor for id: " + constructorId.ToString(), "constructorId");
            }

            // Create the compiled lambda expression
            return GetCreator<T>(constructors[constructorId], ConstructorParameters<T>(constructorId));
        }

        /// <summary>
        /// Static method that gets a compiled lambda expression that can then be used
        /// to repeatedly create instances of that type using the constructor that
        /// the compiled lambda expression was created from.
        /// Uses the constructor that matches the constructor parameters passed in.
        /// </summary>
        /// <typeparam name="T">The type of object that the compiled lamda expression will create.</typeparam>
        /// <param name="constructorParameters">An ordered array of the parameters the constructor takes.</param>
        /// <returns>A compiled lambda expression in the form of a delegate.</returns>
        /// <exception cref="System.ArgumentException">Thrown when T does not have a constructor that takes the
        /// passed in set of parameters.</exception>
        public static Creator<T> GetCreator<T>(Type[] constructorParameters) where T : class
        {
            // Get constructor information.
            ConstructorInfo[] constructors = typeof(T).GetConstructors();

            // Go through each constructor looking for a match.
            for (int i = 0; i < constructors.Length; ++i)
            {
                if (constructorParameters.SequenceEqual(ConstructorParameters<T>(i)))
                {
                    // We have a match.
                    return GetCreator<T>(constructors[i], constructorParameters);
                }
            }

            // No match.
            throw new ArgumentException("This type does not does not have a constructor that takes the passed in set of parameters.", "constructorParameters");
        }

        /// <summary>
        /// Static private method that generates the compiled lambda expression from
        /// information passed in.
        /// </summary>
        /// <typeparam name="T">The type of object that the compiled lamda expression will create.</typeparam>
        /// <param name="constructor">The constructor to be used.</param>
        /// <param name="constructorParameters">An ordered array of the parameters the constructor takes.</param>
        /// <returns>A compiled lambda expression.</returns>
        private static Creator<T> GetCreator<T>(ConstructorInfo constructor, Type[] constructorParameters)
        {
            // Create a single param of type object[].
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

            // Create a typed expression for each parameter in the constructor.
            Expression[] argsExpressions = new Expression[constructorParameters.Length];

            for (int i = 0; i < constructorParameters.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, constructorParameters[i]);
                argsExpressions[i] = paramCastExp;
            }

            // Make a NewExpression that calls the constructor with the args we just created.
            NewExpression newExpression = Expression.New(constructor, argsExpressions);

            // Create a lambda with the NewExpression as body and our param object[] as the argument.
            LambdaExpression lambda = Expression.Lambda(typeof(Creator<T>), newExpression, param);

            // Compile it.
            Creator<T> compiled = (Creator<T>)lambda.Compile();

            // Done.
            return compiled;
        }
        #endregion
    }
}

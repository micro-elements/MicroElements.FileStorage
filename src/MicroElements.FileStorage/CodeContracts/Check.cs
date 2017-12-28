using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace MicroElements.FileStorage.CodeContracts
{
    /// <summary>
    /// Code Contracts.
    /// Some stuff inspired by:
    /// https://github.com/rsdn/CodeJam/blob/master/Main/src/Assertions/Code.cs
    /// https://github.com/aspnet/EntityFrameworkCore/blob/dev/src/Shared/Check.cs
    /// </summary>
    [DebuggerStepThrough]
    public static class Check
    {
        /// <summary>
        /// Ensures that <paramref name="arg"/> != <c>null</c>
        /// </summary>
        /// <typeparam name="T">Type of the value. Auto-inferred in most cases</typeparam>
        /// <param name="arg">The argument.</param>
        /// <param name="argName">Name of the argument.</param>
        [ContractAnnotation("arg:null => halt")]
        [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
        [AssertionMethod]
        public static void NotNull<T>(
            [CanBeNull, NoEnumeration, AssertionCondition(AssertionConditionType.IS_NOT_NULL)] T arg,
            [NotNull, InvokerParameterName] string argName) where T : class
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }

        [ContractAnnotation("arg:null => halt")]
        [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
        [AssertionMethod]
        public static void NotEmpty<T>(
            [CanBeNull, NoEnumeration, AssertionCondition(AssertionConditionType.IS_FALSE)] ICollection<T> arg,
            [NotNull, InvokerParameterName] string argName)
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
            if (arg.Count == 0)
                throw new ArgumentException("collection must be not empty", argName);
        }

        [ContractAnnotation("arg:null => halt")]
        [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
        [AssertionMethod]
        public static void NotEmpty<T>(
            [CanBeNull, NoEnumeration, AssertionCondition(AssertionConditionType.IS_FALSE)] IReadOnlyCollection<T> arg,
            [NotNull, InvokerParameterName] string argName)
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
            if (arg.Count == 0)
                throw new ArgumentException("collection must be not empty", argName);
        }
    }
}

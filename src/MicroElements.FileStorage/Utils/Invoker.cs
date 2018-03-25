// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MicroElements.FileStorage.Utils
{
    public static class Invoker
    {
        public static Task Execute(Type genericParamType, object methodHost, string methodName, object arg = null)
        {
            var methodInfo = methodHost.GetType()
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .MakeGenericMethod(genericParamType);

            return (Task)methodInfo.Invoke(methodHost, arg != null ? new[] { arg } : null);
        }
    }
}

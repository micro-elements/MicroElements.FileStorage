// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace MicroElements.FileStorage.Functional
{
    public struct Some2<T> where T : class
    {
        public T Value { get; }

        public Some2(T value) : this()
        {
            Value = value ?? throw new ArgumentNullException(nameof(value), "Null value is not awailable");
        }

        public static implicit operator T(Some2<T> some) { return some.Value; }
        public static implicit operator Some2<T>(T value) { return new Some2<T>(value); }
    }
}
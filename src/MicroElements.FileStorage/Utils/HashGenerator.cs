// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Security.Cryptography;
using System.Text;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Utils
{
    public static class HashGenerator
    {
        private static readonly Lazy<MD5CryptoServiceProvider> Md5CryptoProvider = new Lazy<MD5CryptoServiceProvider>(() => new MD5CryptoServiceProvider());

        public static string Md5Hash<T>(this T entity, ISerializer serializer)
        {
            var serialized = serializer.Serialize(new object[] { entity }, typeof(T));
            var serializedBytes = Encoding.UTF8.GetBytes(serialized.Content);
            var hash = Md5CryptoProvider.Value.ComputeHash(serializedBytes);
            return AsText(hash);
        }

        public static string AsText(this byte[] hash, int predefinedLength = 32)
        {
            var stringBuilder = new StringBuilder(predefinedLength);
            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("X2"));
            }

            return stringBuilder.ToString();
        }
    }
}

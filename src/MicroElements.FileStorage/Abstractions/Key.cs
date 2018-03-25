// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using JetBrains.Annotations;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Key abstraction.
    /// </summary>
    public class Key
    {
        /// <summary>
        /// Key type.
        /// </summary>
        public KeyType KeyType { get; }

        /// <summary>
        /// Collection.
        /// Uses for <see cref="Abstractions.KeyType.Identity"/> and <see cref="Abstractions.KeyType.Semantic"/>
        /// </summary>
        public string Collection { get; }

        /// <summary>
        /// Value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class.
        /// </summary>
        /// <param name="keyType">Key type.</param>
        /// <param name="value">Key value.</param>
        /// <param name="collection">Optional collection name.</param>
        public Key(KeyType keyType, [NotNull] string value, [CanBeNull] string collection = null)
        {
            Check.NotNull(value, nameof(value));

            KeyType = keyType;
            Value = value;
            Collection = collection ?? string.Empty;
        }

        /// <summary>
        /// Full key in format {collectionName}/{id}.
        /// </summary>
        public string Formatted
        {
            get
            {
                if (KeyType == KeyType.UniqId)
                {
                    return Value;
                }

                if (!string.IsNullOrEmpty(Collection))
                {
                    return $"{Collection}/{Value}";
                }

                return Value;
            }
        }

        #region Equality and ToString

        protected bool Equals(Key other)
        {
            return KeyType == other.KeyType && string.Equals(Collection, other.Collection) && string.Equals(Value, other.Value);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Key)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)KeyType;
                hashCode = (hashCode * 397) ^ (Collection != null ? Collection.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(KeyType)}: {KeyType}, {nameof(Collection)}: {Collection}, {nameof(Value)}: {Value}";
        }

        #endregion
    }
}

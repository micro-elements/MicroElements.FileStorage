// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Key type.
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// Globally unique id.
        /// <para>Can be generated on client side.</para>
        /// <para>Examples: Guid, MongoId, Flake.</para> 
        /// </summary>
        UniqId = 0,

        /// <summary>
        /// Id that can be got from entity.
        /// <para>Not unique globally. Quazy unique for small data sets.</para> 
        /// <para>For example: full user name and birthdate.</para> 
        /// </summary>
        Semantic,

        /// <summary>
        /// Id unique for one collection. It's a sequence of ordinal numbers.
        /// </summary>
        Identity,

        /// <summary>
        /// Hash key. Represents state of entity.
        /// <para>Not unique globally.</para>
        /// <para>Same hash represents equivalent entities but its not truth in common.</para>
        /// </summary>
        Hash
    }
}

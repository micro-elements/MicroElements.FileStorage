// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.FileStorage
{
    // Most of thease issues can be found on githib:
    // https://github.com/micro-elements/MicroElements.FileStorage/issues

    // todo: StorageEngine Key limitations
    // todo: TKey
    // https://ravendb.net/docs/article-page/3.5/all/server/kb/document-key-generation
    // guid, semantic, identity
    // todo: special struct for key
    // todo: NewId or CombGuid

    // todo: create collection (data cache)
    // todo: create snapshot
    // todo: load from other format
    // todo: key work: full key, short, generation, max, guid
    // todo: metrics
    // todo: logging
    // todo: fullInMemory | lazyLoad
    // todo: readonly
    // todo: validation
    // todo: NewId generator
    // todo: full configuration verify
    // todo: security: sign, encrypt, hashing
    // todo: audit
    // todo: persistent sequence for IdentityKeyGenerator
    // todo: TTL for caching?
    // todo: reactive? observable?
    // todo: Addon rules: EveryDay (Time function), SizeLimit

    // todo: DocumentCollection: Add, Delete is lockable change operations. Need some like session or transaction log.

    /*
     Some investigation:
        https://github.com/RolandPheasant/DynamicData/blob/a848dd088d30181a0896a027fb33d5eb96669c33/DynamicData/Cache/Change.cs

    */

}

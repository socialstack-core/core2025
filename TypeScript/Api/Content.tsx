/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports

// Module
/*
*/
export type Content<uint> = Object & {
    type?: string,
    id: uint,
}

/*
*/
export type UserCreatedContent<uint> = Content<uint> & {
    userId: uint,
    createdUtc: Date,
    editedUtc: Date,
}

/*
*/
export type VersionedContent<uint> = UserCreatedContent<uint> & {
    revisionId?: uint,
    isDraft: boolean,
    publishDraftDate?: Date,
    revision: int,
}



/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, VersionedContent} from 'TypeScript/Api/ApiEndpoints'

// Module
export type User = VersionedContent<number> & {
    email?: string,
    emailOptOutFlags?: number,
    emailVerifyToken?: string,
    passwordHash?: string,
    loginAttempts?: number,
    loginRevokeCount?: number,
    role?: number,
    privateVerify?: number,
    featureRef?: string,
    avatarRef?: string,
    username?: string,
    revision?: number,
    userId?: number,
    createdUtc?: Date,
    editedUtc?: Date,
    id?: number,
    joinedUtc?: Date,
    isDraft?: boolean,
    type?: string,
}

export class UserApi extends AutoApi<User>{
    public constructor(){
        super('v1/user')
    }

}



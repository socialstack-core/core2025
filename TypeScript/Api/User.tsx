/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes, VersionedContent} from 'Api/ApiEndpoints'
import {Role, RoleIncludes} from './Role'

// Module
export class UserIncludes extends ApiIncludes{
    get userRole(): RoleIncludes {
        return new RoleIncludes(this.text, 'userRole')
    }

    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'CreatorUser')
    }

}

export type User = VersionedContent & {
    email?: string,
    emailOptOutFlags?: number,
    role?: number,
    featureRef?: string,
    avatarRef?: string,
    username?: string,
    localeId?: number,
    revision?: number,
    userId?: number,
    createdUtc?: Date,
    editedUtc?: Date,
    id?: number,
    joinedUtc?: Date,
    revisionId?: number,
    isDraft?: boolean,
    type?: string,
    userRole?: Role,
    creatorUser?: User,
}

export class UserApi extends AutoApi<User, UserIncludes>{
    public constructor(){
        super('v1/user')
    }

}

export default new UserApi();

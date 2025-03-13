/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {Role, RoleIncludes} from './Role'
import {getJson, ApiList} from 'UI/Functions/WebRequest'

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

    public resendVerificationEmail(): Promise<void> {
        return getJson(this.apiUrl + '/sendverifyemail', { })
    }

    public verifyUser(): Promise<void> {
        return getJson(this.apiUrl + '/verify/{userid}/{token}', { })
    }

    public self(): Promise<SessionResponse> {
        return getJson(this.apiUrl + '/self', { })
    }

    public logout(): Promise<SessionResponse> {
        return getJson(this.apiUrl + '/logout', { })
    }

    public login(): Promise<SessionResponse> {
        return getJson(this.apiUrl + '/login', { })
    }

    public impersonate(): Promise<SessionResponse> {
        return getJson(this.apiUrl + '/{id}/impersonate', { })
    }

    public unpersonate(): Promise<SessionResponse> {
        return getJson(this.apiUrl + '/unpersonate', { })
    }

}

export default new UserApi();

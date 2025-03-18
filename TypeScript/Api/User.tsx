/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {UserIncludes, RoleIncludes} from './Includes'
import {Role} from 'Api/Role'
import {getJson, ApiList} from 'UI/Functions/WebRequest'

// Module
/*
  A particular user account.
*/
export type User = VersionedContent & {
    email?: string,
    emailOptOutFlags?: number,
    firstName?: string,
    lastName?: string,
    fullName?: string,
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

/*
  Used when someone has forgot their password
*/
export type UserPasswordForgot = {
    email?: string,
}

/*
  Used when setting a password during user verification.
*/
export type OptionalPassword = {
    password?: string,
}

/*
  Used when logging in. This is fully defined by the password auth service(s) that you have available.
            The default one is Api.PasswordAuth
*/
export type UserLogin = {
    emailOrUsername?: string,
    password?: string,
}

/**
    Auto generated API for User
    Handles user account endpoints.
*/
export class UserApi extends AutoApi<User, UserIncludes>{
    /**
      This extends the AutoApi class, which provides CRUD functionality, any methods seen in are from custom endpoints in the controller

    */
    public constructor(){
        super('v1/user')
        this.includes = new UserIncludes();
    }

    /**
      POST /v1/user/sendverifyemail/
            Sends the user a new token to verify their email.

    */
    public resendVerificationEmail(body: UserPasswordForgot): Promise<void> {
        return getJson(`${this.apiUrl}/sendverifyemail`, { body })
    }

    /**
      POST /v1/user/verify/{userid}/{token}
            Attempts to verify the users email. If a password is supplied, the users password is also set.

    */
    public verifyUser(userid: number, token: string, newPassword: OptionalPassword): Promise<void> {
        return getJson(`${this.apiUrl}/verify/${userid}/${token}`, { body: {
        newPassword,
        }})
    }

    /**
      Gets the current context.

    */
    public self(): Promise<SessionResponse> {
        return getJson(`${this.apiUrl}/self`)
    }

    /**
      Logs out this user account.

    */
    public logout(): Promise<SessionResponse> {
        return getJson(`${this.apiUrl}/logout`)
    }

    /**
      POST /v1/user/login/
            Attempts to login. Returns either a Context or a LoginResult.

    */
    public login(body: UserLogin): Promise<SessionResponse> {
        return getJson(`${this.apiUrl}/login`, { body })
    }

    /**
      Impersonate a user by their ID. This is a hard cookie switch. You will loose all admin functionality to make the impersonation as accurate as possible.

    */
    public impersonate(id: number): Promise<SessionResponse> {
        return getJson(`${this.apiUrl}/${id}/impersonate`)
    }

    /**
      Reverses an impersonation.

    */
    public unpersonate(): Promise<SessionResponse> {
        return getJson(`${this.apiUrl}/unpersonate`)
    }

}

export default new UserApi();

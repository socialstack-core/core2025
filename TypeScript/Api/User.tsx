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
    emailOptOutFlags: uint,
    emailVerifyToken?: string,
    passwordHash?: string,
    loginAttempts: int,
    failedLoginTimeUtc?: Date,
    firstName?: string,
    lastName?: string,
    fullName?: string,
    loginRevokeCount: uint,
    role: uint,
    privateVerify: long,
    featureRef?: FileRef,
    avatarRef?: FileRef,
    username?: string,
    localeId?: uint,
    passwordReset?: string,
    joinedUtc: Date,
    userRole: Role,
    creatorUser: User,
}

/*
  A context constructed primarily from a cookie value. 
            Uses other locale hints such as Accept-Lang when the user doesn't specifically have one set in the cookie.
*/
export type Context = {
    roleId: uint,
    role?: Role,
    siteDomainId: uint,
    localeId: uint,
    userId: uint,
    user?: User,
    ignorePermissions: boolean,
    permitEditedUtcChange: boolean,
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
*/
export type UserLogin = {
    emailOrUsername?: string,
    password?: string,
}

/*
*/
export type LoginResult = {
    moreDetailRequired?: Record<string, string | number | boolean>,
    cookieName?: string,
    user?: User,
    success: boolean,
    loginData?: UserLogin,
}

/*
  A soft kind of failure which can occur when more info is required.
*/
export type LoginResultOrContext = {
    loginResult?: LoginResult,
    context?: SessionResponse,
}

/**
    Auto generated API for User
    Handles user account endpoints.
*/
export class UserApi extends AutoApi<User, UserIncludes>{
    /**
      This extends the AutoApi class, which provides CRUD functionality, any methods seen in are from custom endpoints in the controller

    */
    public constructor (){
        super('user')
        this.includes = new UserIncludes();
    }

    /**
      POST /v1/user/sendverifyemail/
            Sends the user a new token to verify their email.

    */
    public resendVerificationEmail = (context: SessionResponse, body: UserPasswordForgot): Promise<SessionResponse>  => {
        return getJson(this.apiUrl + '/sendverifyemail', body )
    }

    /**
      POST /v1/user/verify/{userid}/{token}
            Attempts to verify the users email. If a password is supplied, the users password is also set.

    */
    public verifyUser = (context: SessionResponse, userid: uint, token: string, newPassword: OptionalPassword): Promise<SessionResponse>  => {
        return getJson(this.apiUrl + '/verify/' + userid + '/' + token + '', 
        newPassword
        )
    }

    /**
      Gets the current context.

    */
    public self = (context: SessionResponse): Promise<SessionResponse>  => {
        return getJson(this.apiUrl + '/self')
    }

    /**
      Logs out this user account.

    */
    public logout = (httpContext: HttpContext, context: SessionResponse): Promise<SessionResponse>  => {
        return getJson(this.apiUrl + '/logout')
    }

    /**
      POST /v1/user/login/
            Attempts to login. Returns either a Context or a LoginResult.

    */
    public login = (httpContext: HttpContext, context: SessionResponse, body: UserLogin[]): Promise<LoginResultOrContext>  => {
        return getJson(this.apiUrl + '/login', body )
    }

    /**
      Impersonate a user by their ID. This is a hard cookie switch. You will loose all admin functionality to make the impersonation as accurate as possible.

    */
    public impersonate = (httpContext: HttpContext, context: SessionResponse, id: uint): Promise<SessionResponse>  => {
        return getJson(this.apiUrl + '/' + id + '/impersonate')
    }

    /**
      Reverses an impersonation.

    */
    public unpersonate = (httpContext: HttpContext): Promise<SessionResponse>  => {
        return getJson(this.apiUrl + '/unpersonate')
    }

}

export default new UserApi();

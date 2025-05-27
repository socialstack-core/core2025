/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { UserIncludes } from 'Api/Includes';

import { Role } from 'Api/Role';

// TYPES

/**
* This type was generated to reflect {User} (Api.Users.User)
**/
export type User = VersionedContent<uint> & {
    passwordReset?: string;
    joinedUtc?: Date | string | number;
    email?: string;
    emailOptOutFlags?: uint;
    emailVerifyToken?: string;
    passwordHash?: string;
    loginAttempts?: int;
    failedLoginTimeUtc?: Date | string | number;
    firstName?: string;
    lastName?: string;
    fullName?: string;
    lastVisitedUtc?: Date | string | number;
    loginRevokeCount?: uint;
    role?: uint;
    privateVerify?: long;
    featureRef?: string;
    avatarRef?: string;
    username?: string;
    localeId?: uint;
    // HasVirtualField() fields (2 in total)
    userRole?: Role;
    creatorUser?: User;
}

/**
* This type was generated to reflect {UserPasswordForgot} (Api.Users.UserPasswordForgot)
**/
export type UserPasswordForgot = {
    email?: string;
}

/**
* This type was generated to reflect {OptionalPassword} (Api.Users.OptionalPassword)
**/
export type OptionalPassword = {
    password?: string;
}

/**
* This type was generated to reflect {UserLogin} (Api.Users.UserLogin)
**/
export type UserLogin = {
    emailOrUsername?: string;
    password?: string;
}
// ENTITY CONTROLLER

export class UserApi extends AutoController<User,uint>{

    constructor(){
        super('/v1/user');
        this.includes = new UserIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Users.UserController}::{ResendVerificationEmail}
     * @url 'v1/user/sendverifyemail'
     */
    resendVerificationEmail = (setSession: (s: SessionResponse) => Session, body: UserPasswordForgot): Promise<Session> => {
        return getJson<SessionResponse>(this.apiUrl + '/sendverifyemail', body)
            .then(setSession)
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Users.UserController}::{VerifyUser}
     * @url 'v1/user/verify/{userid}/{token}'
     */
    verifyUser = (setSession: (s: SessionResponse) => Session, userid: uint, token: string, newPassword: OptionalPassword): Promise<Session> => {
        return getJson<SessionResponse>(this.apiUrl + '/verify/' + userid +'/' + token +'', newPassword)
            .then(setSession)
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Users.UserController}::{Self}
     * @url 'v1/user/self'
     */
    self = (setSession: (s: SessionResponse) => Session): Promise<Session> => {
        return getJson<SessionResponse>(this.apiUrl + '/self')
            .then(setSession)
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Users.UserController}::{Logout}
     * @url 'v1/user/logout'
     */
    logout = (setSession: (s: SessionResponse) => Session): Promise<Session> => {
        return getJson<SessionResponse>(this.apiUrl + '/logout')
            .then(setSession)
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Users.UserController}::{Login}
     * @url 'v1/user/login'
     */
    login = (body: UserLogin): Promise<void> => {
        return getJson<void>(this.apiUrl + '/login', body)
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Users.UserController}::{Impersonate}
     * @url 'v1/user/{id}/impersonate'
     */
    impersonate = (setSession: (s: SessionResponse) => Session, id: uint): Promise<Session> => {
        return getJson<SessionResponse>(this.apiUrl + '/' + id +'/impersonate')
            .then(setSession)
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Users.UserController}::{Unpersonate}
     * @url 'v1/user/unpersonate'
     */
    unpersonate = (setSession: (s: SessionResponse) => Session): Promise<Session> => {
        return getJson<SessionResponse>(this.apiUrl + '/unpersonate')
            .then(setSession)
    }

}

export default new UserApi();

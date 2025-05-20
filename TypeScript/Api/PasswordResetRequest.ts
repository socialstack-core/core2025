/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { PasswordResetRequestIncludes } from 'Api/Includes';

import { Role } from 'Api/Role';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {PasswordResetRequest} (Api.PasswordResetRequests.PasswordResetRequest)
**/
export type PasswordResetRequest = Content<uint> & {
    token?: string;
    isUsed?: boolean;
    email?: string;
    createdUtc?: Date | string | number;
    userId?: uint;
}

/**
* This type was generated to reflect {Context} (Api.Contexts.Context)
**/
export type SessionResponse = {
    roleId?: uint;
    role?: Role;
    localeId?: uint;
    userId?: uint;
    user?: User;
    ignorePermissions?: boolean;
    permitEditedUtcChange?: boolean;
}

/**
* This type was generated to reflect {NewPassword} (Api.PasswordResetRequests.NewPassword)
**/
export type NewPassword = {
    password?: string;
}

/**
* This type was generated to reflect {ResetToken} (Api.PasswordResetRequests.ResetToken)
**/
export type ResetToken = {
    token?: string;
    url?: string;
}
// ENTITY CONTROLLER

export class PasswordResetRequestApi extends AutoController<PasswordResetRequest,uint>{

    constructor(){
        super('/v1/passwordResetRequest');
        this.includes = new PasswordResetRequestIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.PasswordResetRequests.PasswordResetRequestController}::{CheckTokenExists}
     * @url 'v1/passwordResetRequest/token/' + token + ''
     */
    checkTokenExists = (token: string): Promise<Object> => {
        return getJson<Object>(this.apiUrl + '/token/' + token + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.PasswordResetRequests.PasswordResetRequestController}::{LoginWithToken}
     * @url 'v1/passwordResetRequest/login/' + token + ''
     */
    loginWithToken = (setSession: (s: SessionResponse) => Session, token: string, newPassword: NewPassword): Promise<SessionResponse> => {
        return getJson<SessionResponse>(this.apiUrl + '/login/' + token + '', newPassword);
    }

    /**
     * Generated from a .NET type.
     * @see {Api.PasswordResetRequests.PasswordResetRequestController}::{Generate}
     * @url 'v1/passwordResetRequest/' + id + '/generate'
     */
    generate = (id: uint): Promise<ResetToken> => {
        return getJson<ResetToken>(this.apiUrl + '/' + id + '/generate');
    }

}

export default new PasswordResetRequestApi();

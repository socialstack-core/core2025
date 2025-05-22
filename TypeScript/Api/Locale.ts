/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { LocaleIncludes } from 'Api/Includes';

import { Role } from 'Api/Role';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Locale} (Api.Translate.Locale)
**/
export type Locale = VersionedContent<uint> & {
    shortCode?: string;
    currencyCode?: string;
    name?: string;
    code?: string;
    flagIconRef?: string;
    aliases?: string;
    isDisabled?: boolean;
    isRedirected?: boolean;
    permanentRedirect?: boolean;
    rightToLeft?: boolean;
    pagePath?: string;
    domains?: string;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
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
// ENTITY CONTROLLER

export class LocaleApi extends AutoController<Locale,uint>{

    constructor(){
        super('/v1/locale');
        this.includes = new LocaleIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Translate.LocaleController}::{Set}
     * @url 'v1/locale/set/' + id + ''
     */
    set = (setSession: (s: SessionResponse) => Session, id: uint): Promise<SessionResponse> => {
        return getJson<SessionResponse>(this.apiUrl + '/set/' + id + '');
    }

}

export default new LocaleApi();

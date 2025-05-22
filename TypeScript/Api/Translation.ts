/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { TranslationIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Translation} (Api.Translate.Translation)
**/
export type Translation = VersionedContent<uint> & {
    module?: string;
    original?: string;
    translated?: string;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class TranslationApi extends AutoController<Translation,uint>{

    constructor(){
        super('/v1/translation');
        this.includes = new TranslationIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Translate.TranslationController}::{PrePopulate}
     * @url 'v1/translation/prepopulate'
     */
    prePopulate = (): Promise<Object> => {
        return getJson<Object>(this.apiUrl + '/prepopulate');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Translate.TranslationController}::{LoadPotFiles}
     * @url 'v1/translation/potfiles'
     */
    loadPotFiles = (): Promise<Object> => {
        return getJson<Object>(this.apiUrl + '/potfiles');
    }

}

export default new TranslationApi();

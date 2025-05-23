/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

import { ApiIncludes } from './Includes';
// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { CustomContentTypeIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {CustomContentTypeField} (Api.CustomContentTypes.CustomContentTypeField)
**/
export type CustomContentTypeField = VersionedContent<uint> & {
    customContentTypeId?: uint;
    defaultValue?: string;
    dataType?: string;
    linkedEntity?: string;
    name?: string;
    nickName?: string;
    localised?: boolean;
    urlEncoded?: boolean;
    isHidden?: boolean;
    hideSeconds?: boolean;
    roundMinutes?: boolean;
    validation?: string;
    order?: uint;
    group?: string;
    optionsArePrices?: boolean;
    deleted?: boolean;
    // HasVirtualField() fields (2 in total)
    customContentType?: CustomContentType;
    creatorUser?: User;
}

/**
* This type was generated to reflect {CustomContentType} (Api.CustomContentTypes.CustomContentType)
**/
export type CustomContentType = VersionedContent<uint> & {
    fields?: CustomContentTypeField[][];
    name?: string;
    nickName?: string;
    summary?: string;
    iconRef?: string;
    deleted?: boolean;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}

/**
* This type was generated to reflect {TypeInfo} (Api.CustomContentTypes.CustomContentTypeController+TypeInfo)
**/
export type TypeInfo = {
    name?: string;
    value?: string;
}
// ENTITY CONTROLLER

export class CustomContentTypeApi extends AutoController<CustomContentType,uint>{

    constructor(){
        super('/v1/customcontenttype');
        this.includes = new CustomContentTypeIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.CustomContentTypes.CustomContentTypeController}::{GetAllTypes}
     * @url 'v1/customContentType/alltypes'
     */
    getAllTypes = (): Promise<string[]> => {
        return getJson<string[]>(this.apiUrl + '/alltypes');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.CustomContentTypes.CustomContentTypeController}::{GetAllTypesPlus}
     * @url 'v1/customContentType/allcustomtypesplus'
     */
    getAllTypesPlus = (): Promise<TypeInfo[]> => {
        return getJson<TypeInfo[]>(this.apiUrl + '/allcustomtypesplus');
    }

}

export default new CustomContentTypeApi();

/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { CustomContentTypeFieldIncludes } from 'Api/Includes';

import { CustomContentType } from 'Api/CustomContentType';

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
// ENTITY CONTROLLER

export class CustomContentTypeFieldApi extends AutoController<CustomContentTypeField,uint>{

    constructor(){
        super('/v1/customcontenttypefield');
        this.includes = new CustomContentTypeFieldIncludes();
    }

}

export default new CustomContentTypeFieldApi();

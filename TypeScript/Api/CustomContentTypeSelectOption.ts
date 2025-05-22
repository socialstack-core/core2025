/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { CustomContentTypeSelectOptionIncludes } from 'Api/Includes';

import { CustomContentTypeField } from 'Api/CustomContentType';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {CustomContentTypeSelectOption} (Api.CustomContentTypes.CustomContentTypeSelectOption)
**/
export type CustomContentTypeSelectOption = VersionedContent<uint> & {
    customContentTypeFieldId?: uint;
    value?: string;
    order?: uint;
    // HasVirtualField() fields (2 in total)
    customContentTypeField?: CustomContentTypeField;
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class CustomContentTypeSelectOptionApi extends AutoController<CustomContentTypeSelectOption,uint>{

    constructor(){
        super('/v1/customContentTypeSelectOption');
        this.includes = new CustomContentTypeSelectOptionIncludes();
    }

}

export default new CustomContentTypeSelectOptionApi();

/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ContentFieldAccessRuleIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {ContentFieldAccessRule} (Api.Permissions.ContentFieldAccessRule)
**/
export type ContentFieldAccessRule = VersionedContent<uint> & {
    entityName?: string;
    isVirtualType?: boolean;
    fieldName?: string;
    canRead?: string;
    canWrite?: string;
    roleId?: uint;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class ContentFieldAccessRuleApi extends AutoController<ContentFieldAccessRule,uint>{

    constructor(){
        super('/v1/contentfieldaccessrule');
        this.includes = new ContentFieldAccessRuleIncludes();
    }

}

export default new ContentFieldAccessRuleApi();

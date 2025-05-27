/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ComponentGroupIncludes } from 'Api/Includes';

import { Role } from 'Api/Role';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {ComponentGroup} (Api.Components.ComponentGroup)
**/
export type ComponentGroup = VersionedContent<uint> & {
    name?: string;
    allowedComponents?: string;
    // HasVirtualField() fields (2 in total)
    role?: Role;
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class ComponentGroupApi extends AutoController<ComponentGroup,uint>{

    constructor(){
        super('/v1/componentgroup');
        this.includes = new ComponentGroupIncludes();
    }

}

export default new ComponentGroupApi();

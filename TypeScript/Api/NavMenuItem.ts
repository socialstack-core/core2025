/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiIncludes } from './Includes';
// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { NavMenuItemIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {NavMenuItem} (Api.NavMenus.NavMenuItem)
**/
export type NavMenuItem = VersionedContent<uint> & {
    navMenuId?: uint;
    menuKey?: string;
    parentItemId?: uint;
    bodyJson?: string;
    target?: string;
    iconRef?: string;
    order?: int;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class NavMenuItemApi extends AutoController<NavMenuItem,uint>{

    constructor(){
        super('/v1/navmenuitem');
        this.includes = new NavMenuItemIncludes();
    }

}

export default new NavMenuItemApi();

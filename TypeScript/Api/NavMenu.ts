/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiIncludes } from './Includes';
// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { NavMenuIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {NavMenu} (Api.NavMenus.NavMenu)
**/
export type NavMenu = VersionedContent<uint> & {
    key?: string;
    name?: string;
    target?: string;
    order?: int;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class NavMenuApi extends AutoController<NavMenu,uint>{

    constructor(){
        super('/v1/navmenu');
        this.includes = new NavMenuIncludes();
    }

}

export default new NavMenuApi();

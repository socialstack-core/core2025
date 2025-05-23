/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiIncludes } from './Includes';
// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { AdminNavMenuItemIncludes } from 'Api/Includes';

// TYPES

/**
* This type was generated to reflect {AdminNavMenuItem} (Api.NavMenus.AdminNavMenuItem)
**/
export type AdminNavMenuItem = Content<uint> & {
    title?: string;
    target?: string;
    iconRef?: string;
    visibilityRuleJson?: string;
}
// ENTITY CONTROLLER

export class AdminNavMenuItemApi extends AutoController<AdminNavMenuItem,uint>{

    constructor(){
        super('/v1/adminnavmenuitem');
        this.includes = new AdminNavMenuItemIncludes();
    }

}

export default new AdminNavMenuItemApi();

/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

import { Role } from 'Api/Role';

import { User } from 'Api/User';

// ENUMS

export enum RoleGrantRuleType {
    Role,
    All,
    Feature,
    Single,
    Revoke,
    Important,
}

// TYPES

/**
* This type was generated to reflect {RoleGrantRule} (Api.Permissions.RoleGrantRule)
**/
export type RoleGrantRule = {
    ruleType: RoleGrantRuleType;
    filterQuery?: string;
    patterns?: string[][];
    sameAsRole?: Role;
}

/**
* This type was generated to reflect {GrantMeta} (Api.Permissions.GrantMeta)
**/
export type GrantMeta = {
    ruleDescription?: string;
    role?: Role;
}

/**
* This type was generated to reflect {PermissionMeta} (Api.Permissions.PermissionMeta)
**/
export type PermissionMeta = {
    key?: string;
    description?: string;
    grants?: GrantMeta[];
}

/**
* This type was generated to reflect {PermissionInformation} (Api.Permissions.PermissionInformation)
**/
export type PermissionInformation = {
    capabilities?: PermissionMeta[];
    roles?: Role[];
}
// NON-ENTITY CONTROLLERS

export class PermissionController {

   private apiUrl: string = '/v1/permission';

    /**
     * Generated from a .NET type.
     * @see {Api.Permissions.PermissionController}::{List}
     * @url '/list'
     */
    list = (): Promise<PermissionInformation> => {
        return getJson<PermissionInformation>(this.apiUrl + '/list')
    }

}

export default new PermissionController();

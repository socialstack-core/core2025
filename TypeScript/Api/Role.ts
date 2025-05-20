/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { RoleIncludes } from 'Api/Includes';

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
    patterns?: String[][];
    sameAsRole?: Role;
}

/**
* This type was generated to reflect {Role} (Api.Permissions.Role)
**/
export type Role = VersionedContent<uint> & {
    grantRules?: RoleGrantRule[][];
    name?: string;
    key?: string;
    canViewAdmin?: boolean;
    isComposite?: boolean;
    adminDashboardJson?: string;
    grantRuleJson?: string;
    inheritedRoleId?: uint;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class RoleApi extends AutoController<Role,uint>{

    constructor(){
        super('/v1/role');
        this.includes = new RoleIncludes();
    }

}

export default new RoleApi();

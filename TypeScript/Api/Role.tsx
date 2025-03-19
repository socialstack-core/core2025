/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {RoleIncludes, UserIncludes} from './Includes'
import {User} from 'Api/User'
import {getJson, ApiList} from 'UI/Functions/WebRequest'

// Module
/*
  A role which defines a set of capabilities to a user who is granted this particular role.
*/
export type Role = VersionedContent & {
    name?: string,
    key?: string,
    canViewAdmin: boolean,
    isComposite: boolean,
    adminDashboardJson?: string,
    grantRuleJson?: string,
    inheritedRoleId: int,
    revision: int,
    userId: int,
    createdUtc: Date,
    editedUtc: Date,
    id: int,
    revisionId?: int,
    isDraft: boolean,
    type?: string,
    creatorUser: User,
}

/**
    Auto generated API for Role
    Handles user role endpoints.
*/
export class RoleApi extends AutoApi<Role, RoleIncludes>{
    /**
      This extends the AutoApi class, which provides CRUD functionality, any methods seen in are from custom endpoints in the controller

    */
    public constructor (){
        super('role')
        this.includes = new RoleIncludes();
    }

}

export default new RoleApi();

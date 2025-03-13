/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {User, UserIncludes} from './User'
import {getJson, ApiList} from 'UI/Functions/WebRequest'

// Module
/**
    Allows custom chained includes inside the list & load methods.
*/
export class RoleIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'CreatorUser')
    }

}

/*
  A role which defines a set of capabilities to a user who is granted this particular role.
*/
export type Role = VersionedContent & {
    name?: string,
    key?: string,
    canViewAdmin?: boolean,
    isComposite?: boolean,
    adminDashboardJson?: string,
    grantRuleJson?: string,
    inheritedRoleId?: number,
    revision?: number,
    userId?: number,
    createdUtc?: Date,
    editedUtc?: Date,
    id?: number,
    revisionId?: number,
    isDraft?: boolean,
    type?: string,
    creatorUser?: User,
}

/**
    Auto generated API for Role
    Handles user role endpoints.
*/
export class RoleApi extends AutoApi<Role, RoleIncludes>{
    /**
      This extends the AutoApi class, which provides CRUD functionality, any methods seen in are from custom endpoints in the controller

    */
    public constructor(){
        super('v1/role')
    }

}

export default new RoleApi();

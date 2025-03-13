/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {User, UserIncludes} from './User'

// Module
export class RoleIncludes extends ApiIncludes{
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'CreatorUser')
    }

}

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

export class RoleApi extends AutoApi<Role, RoleIncludes>{
    public constructor(){
        super('v1/role')
    }

}

export default new RoleApi();

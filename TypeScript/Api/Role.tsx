/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, VersionedContent} from 'TypeScript/Api/ApiEndpoints'

// Module
export type Role = VersionedContent<number> & {
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
    isDraft?: boolean,
    type?: string,
}

export class RoleApi extends AutoApi<Role>{
    public constructor(){
        super('v1/role')
    }

}



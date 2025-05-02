/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {Role} from './Role'
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/*
*/
export type PermissionInformation = {
    capabilities?: PermissionMeta[],
    roles?: Role[],
}

/**
*/
export class PermissionApi{
    public apiUrl: string = 'permission';

    /**
      GET /v1/permission/list
            Returns meta about the list of available roles and their permission set.

    */
    public list = (): Promise<PermissionInformation>  => {
        return getJson<PermissionInformation>(this.apiUrl + '/list')
    }

}

/*
*/
export type PermissionMeta = {
    key?: string,
    description?: string,
    grants?: GrantMeta[],
}

export default new PermissionApi();

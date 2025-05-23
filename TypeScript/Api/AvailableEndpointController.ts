/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

// TYPES

/**
* This type was generated to reflect {Endpoint} (Api.AvailableEndpoints.Endpoint)
**/
export type Endpoint = {
    url?: string;
    summary?: string;
    urlFields?: string[];
    bodyFields?: string[];
    httpMethod?: string;
}

/**
* This type was generated to reflect {ContentType} (Api.AvailableEndpoints.ContentType)
**/
export type ContentType = {
    id?: int;
    name?: string;
}

/**
* This type was generated to reflect {ApiStructure} (Api.AvailableEndpoints.ApiStructure)
**/
export type ApiStructure = {
    endpoints?: Endpoint[][];
    contentTypes?: ContentType[][];
}
// NON-ENTITY CONTROLLERS

export class AvailableEndpointController {

   private apiUrl: string = '/v1';

    /**
     * Generated from a .NET type.
     * @see {Api.AvailableEndpoints.AvailableEndpointController}::{Uptime}
     * @url 'v1/uptime'
     */
    uptime = (): Promise<void> => {
        return getJson<void>(this.apiUrl + '/uptime')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.AvailableEndpoints.AvailableEndpointController}::{Get}
     * @url 'v1/'
     */
    get = (): Promise<ApiStructure> => {
        return getJson<ApiStructure>(this.apiUrl + '/')
    }

}

export default new AvailableEndpointController();

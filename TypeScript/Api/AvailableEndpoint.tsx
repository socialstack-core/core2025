/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/*
*/
export type ApiStructure = {
    endpoints?: Endpoint[],
    contentTypes?: ContentType[],
}

/**
*/
export class AvailableEndpointApi{
    public apiUrl: string = 'v1';

    /**
      Gets the time (in both ticks and as a timestamp) that the service last started at.

    */
    public uptime = (): Promise<string>  => {
        return getText(this.apiUrl + '/uptime')
    }

    /**
      GET /v1/
            Returns meta about what's available from this API. Includes endpoints and content types.

    */
    public get = (): Promise<ApiStructure>  => {
        return getJson<ApiStructure>(this.apiUrl)
    }

}

/*
*/
export type Endpoint = {
    url?: string,
    summary?: string,
    urlFields?: Record<string, Record<string, string | number | boolean>>,
    bodyFields?: Record<string, Record<string, string | number | boolean>>,
    httpMethod?: string,
}

/*
*/
export type ContentType = {
    id?: int,
    name?: string,
}

export default new AvailableEndpointApi();

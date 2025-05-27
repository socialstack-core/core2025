/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

// TYPES

/**
* This type was generated to reflect {PublicMessage} (Api.Startup.PublicMessage)
**/
export type PublicMessage = {
    message?: string;
    code?: string;
}

/**
* This type was generated to reflect {ServerIdentification} (Api.Startup.ServerIdentification)
**/
export type ServerIdentification = {
    id?: uint;
}

/**
* This type was generated to reflect {MonitoringQueryModel} (Api.Startup.MonitoringQueryModel)
**/
export type MonitoringQueryModel = {
    query?: string;
}

/**
* This type was generated to reflect {LogFilteringModel} (Api.Startup.LogFilteringModel)
**/
export type LogFilteringModel = {
    newerThan?: long;
    offset?: uint;
    pageSize?: uint;
    localOnly?: boolean;
    tag?: string;
}

/**
* This type was generated to reflect {IActionResult} (Microsoft.AspNetCore.Mvc.IActionResult)
**/
export type IActionResult = {
}

/**
* This type was generated to reflect {BufferPoolStatus} (Api.Startup.BufferPoolStatus)
**/
export type BufferPoolStatus = {
    writerCount?: int;
    bufferCount?: int;
    byteSize?: int;
}

/**
* This type was generated to reflect {MonitoringExecModel} (Api.Startup.MonitoringExecModel)
**/
export type MonitoringExecModel = {
    command?: string;
}

/**
* This type was generated to reflect {WebsocketClientInfo} (Api.Startup.WebsocketClientInfo)
**/
export type WebsocketClientInfo = {
    clients?: int;
}
// NON-ENTITY CONTROLLERS

export class StdOutController {

   private apiUrl: string = '/v1/monitoring';

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{V8Clear}
     * @url '/v8/clear'
     */
    v8Clear = (): Promise<void> => {
        return getJson<void>(this.apiUrl + '/v8/clear')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{UpdateCerts}
     * @url '/certs/update'
     */
    updateCerts = (): Promise<PublicMessage> => {
        return getJson<PublicMessage>(this.apiUrl + '/certs/update')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{UpdateWebserverConfig}
     * @url '/webserver/apply'
     */
    updateWebserverConfig = (): Promise<PublicMessage> => {
        return getJson<PublicMessage>(this.apiUrl + '/webserver/apply')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{WhoAmI}
     * @url '/whoami'
     */
    whoAmI = (): Promise<ServerIdentification | undefined> => {
        return getJson<ServerIdentification | undefined>(this.apiUrl + '/whoami')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{RunQuery}
     * @url '/query'
     */
    runQuery = (queryBody: MonitoringQueryModel): Promise<void> => {
        return getJson<void>(this.apiUrl + '/query', queryBody)
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{GetLog}
     * @url '/log'
     */
    getLog = (filtering: LogFilteringModel): Promise<void> => {
        return getJson<void>(this.apiUrl + '/log', filtering)
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{PlainTextBenchmark}
     * @url '/helloworld'
     */
    plainTextBenchmark = (): Promise<IActionResult> => {
        return getJson<IActionResult>(this.apiUrl + '/helloworld')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{BufferPoolStatus}
     * @url '/bufferpool/status'
     */
    bufferPoolStatus = (): Promise<BufferPoolStatus> => {
        return getJson<BufferPoolStatus>(this.apiUrl + '/bufferpool/status')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{BufferPoolClear}
     * @url '/bufferpool/clear'
     */
    bufferPoolClear = (): Promise<void> => {
        return getJson<void>(this.apiUrl + '/bufferpool/clear')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{GC}
     * @url '/gc'
     */
    gC = (): Promise<void> => {
        return getJson<void>(this.apiUrl + '/gc')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{Execute}
     * @url '/exec'
     */
    execute = (body: MonitoringExecModel): Promise<void> => {
        return getJson<void>(this.apiUrl + '/exec', body)
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{Halt}
     * @url '/halt'
     */
    halt = (): Promise<void> => {
        return getJson<void>(this.apiUrl + '/halt')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Startup.StdOutController}::{GetWsClientCount}
     * @url '/clients'
     */
    getWsClientCount = (): Promise<WebsocketClientInfo> => {
        return getJson<WebsocketClientInfo>(this.apiUrl + '/clients')
    }

}

export default new StdOutController();

/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {PublicMessage} from './PublicMessage'
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/*
*/
export type ServerIdentification = {
    id?: uint,
}

/*
*/
export type MonitoringQueryModel = {
    query?: string,
}

/*
*/
export type LogFilteringModel = {
    newerThan?: long,
    offset?: uint,
    pageSize?: uint,
    localOnly?: boolean,
    tag?: string,
}

/*
*/
export type HtmlCacheStatus = {
    locales?: HtmlCachedLocaleStatus[],
}

/*
*/
export type IActionResult = {
}

/*
*/
export type BufferPoolStatus = {
    writerCount?: int,
    bufferCount?: int,
    byteSize?: int,
}

/*
*/
export type MonitoringExecModel = {
    command?: string,
}

/*
*/
export type WebsocketClientInfo = {
    clients?: int,
}

/**
*/
export class StdOutApi{
    public apiUrl: string = 'monitoring';

    /**
      Attempts to purge V8 engines from the canvas renderer service.

    */
    public v8Clear = (): Promise<string>  => {
        return getText(this.apiUrl + '/v8/clear')
    }

    /**
      Triggers a certificate update (admin only).

    */
    public updateCerts = (): Promise<PublicMessage>  => {
        return getJson<PublicMessage>(this.apiUrl + '/certs/update')
    }

    /**
      Triggers a webserver config file update (admin only).

    */
    public updateWebserverConfig = (): Promise<PublicMessage>  => {
        return getJson<PublicMessage>(this.apiUrl + '/webserver/apply')
    }

    /**
      Indicates which server in a cluster this one is.

    */
    public whoAmI = (): Promise<ServerIdentification>  => {
        return getJson<ServerIdentification>(this.apiUrl + '/whoami')
    }

    /**
      Runs a query, returning the result set(s) as streaming JSON.
            Note that this will currently only work for MySQL database engines.

    */
    public runQuery = (queryBody: MonitoringQueryModel): Promise<string>  => {
        return getText(this.apiUrl + '/query', queryBody)
    }

    /**
      Gets the latest block of text from the stdout.

    */
    public getLog = (filtering: LogFilteringModel): Promise<string>  => {
        return getText(this.apiUrl + '/log', filtering)
    }

    /**
      Page cache status.

    */
    public htmlCache = (): Promise<HtmlCacheStatus>  => {
        return getJson<HtmlCacheStatus>(this.apiUrl + '/cachestatus/html')
    }

    /**
      Plaintext benchmark.

    */
    public plainTextBenchmark = (): Promise<IActionResult>  => {
        return getJson<IActionResult>(this.apiUrl + '/helloworld')
    }

    /**
      V8 status.

    */
    public bufferPoolStatus = (): Promise<BufferPoolStatus>  => {
        return getJson<BufferPoolStatus>(this.apiUrl + '/bufferpool/status')
    }

    /**
      Attempts to purge V8 engines from the canvas renderer service.

    */
    public bufferPoolClear = (): Promise<string>  => {
        return getText(this.apiUrl + '/bufferpool/clear')
    }

    /**
      Forces a GC run. Convenience for testing for memory leaks.

    */
    public gC = (): Promise<string>  => {
        return getText(this.apiUrl + '/gc')
    }

    /**
      Runs something on the command line. Super admin only (naturally).

    */
    public execute = (body: MonitoringExecModel): Promise<string>  => {
        return getText(this.apiUrl + '/exec', body)
    }

    /**
      Forces an application halt.

    */
    public halt = (): Promise<string>  => {
        return getText(this.apiUrl + '/halt')
    }

    /**
      Gets the latest number of websocket clients.

    */
    public getWsClientCount = (): Promise<WebsocketClientInfo>  => {
        return getJson<WebsocketClientInfo>(this.apiUrl + '/clients')
    }

}

/*
*/
export type HtmlCachedLocaleStatus = {
    localeId?: int,
    cachedPages?: HtmlCachedPageStatus[],
}

export default new StdOutApi();

/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/*
*/
export type MobilePageMeta = {
    apiHost?: string,
    localeId?: uint,
    cordova?: boolean,
    includePages?: boolean,
    customJs?: string,
}

/*
*/
export type FileContent = {
    mimeType?: string,
    rawBytes?: int[],
    lastModifiedUtc?: string,
    etag?: string,
    isCompressed?: boolean,
}

/**
*/
export class HtmlApi{
    public apiUrl: string = '';

    /**
      Lists all available static files.

    */
    public getMobileHtml = (mobileMeta: MobilePageMeta): Promise<string>  => {
        return getText(this.apiUrl + '/pack/static-assets/mobile-html', mobileMeta)
    }

    /**
      RTE config popup base HTML.

    */
    public getRteConfigPage = (): Promise<string>  => {
        return getText(this.apiUrl + '/pack/rte.html')
    }

    /**
      Gets or generates the robots.txt file.

    */
    public robots = (): Promise<FileContent>  => {
        return getJson<FileContent>(this.apiUrl + '/robots.txt')
    }

}

export default new HtmlApi();

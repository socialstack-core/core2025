/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
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

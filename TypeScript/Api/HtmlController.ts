/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

// TYPES

/**
* This type was generated to reflect {FileContent} (Api.Startup.Routing.FileContent)
**/
export type FileContent = {
    mimeType?: string;
    rawBytes?: byte[][];
    lastModifiedUtc?: string;
    etag?: string;
    isCompressed?: boolean;
}
// NON-ENTITY CONTROLLERS

export class HtmlController {

   private apiUrl: string = '/';

    /**
     * Generated from a .NET type.
     * @see {Api.Pages.HtmlController}::{GetRteConfigPage}
     * @url '/pack/rte.html'
     */
    getRteConfigPage = (): Promise<void> => {
        return getJson<void>(this.apiUrl + '/pack/rte.html')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Pages.HtmlController}::{Robots}
     * @url '/robots.txt'
     */
    robots = (): Promise<FileContent> => {
        return getJson<FileContent>(this.apiUrl + '/robots.txt')
    }

}

export default new HtmlController();

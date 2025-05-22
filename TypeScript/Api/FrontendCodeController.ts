/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

// TYPES

/**
* This type was generated to reflect {UIReloadResult} (Api.CanvasRenderer.UIReloadResult)
**/
export type UIReloadResult = {
    version?: long;
}

/**
* This type was generated to reflect {StaticFileInfo} (Api.CanvasRenderer.StaticFileInfo)
**/
export type StaticFileInfo = {
    size?: long;
    modifiedUtc?: ulong;
    ref?: string;
}

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

export class FrontendCodeController {

   private apiUrl: string = '/';

    /**
     * Generated from a .NET type.
     * @see {Api.CanvasRenderer.FrontendCodeController}::{Reload}
     * @url '/v1/monitoring/ui-reload'
     */
    reload = (): Promise<UIReloadResult> => {
        return getJson<UIReloadResult>(this.apiUrl + '/v1/monitoring/ui-reload');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.CanvasRenderer.FrontendCodeController}::{GetStaticFileList}
     * @url '/pack/static-assets/list.json'
     */
    getStaticFileList = (): Promise<StaticFileInfo[]> => {
        return getJson<StaticFileInfo[]>(this.apiUrl + '/pack/static-assets/list.json');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.CanvasRenderer.FrontendCodeController}::{GetTypeMeta}
     * @url '/pack/type-meta.json'
     */
    getTypeMeta = (): Promise<FileContent> => {
        return getJson<FileContent>(this.apiUrl + '/pack/type-meta.json');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.CanvasRenderer.FrontendCodeController}::{GetEmailMainJs}
     * @url '/pack/email-static/main.js?localeId=' + localeId + ''
     */
    getEmailMainJs = (localeId?: uint): Promise<FileContent> => {
        return getJson<FileContent>(this.apiUrl + '/pack/email-static/main.js?localeId=' + localeId + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.CanvasRenderer.FrontendCodeController}::{GetGlobalScss}
     * @url '/pack/scss/' + bundle + ''
     */
    getGlobalScss = (bundle: string): Promise<FileContent> => {
        return getJson<FileContent>(this.apiUrl + '/pack/scss/' + bundle + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.CanvasRenderer.FrontendCodeController}::{GetMainJs}
     * @url '/pack/main.js?localeId=' + localeId + ''
     */
    getMainJs = (localeId?: uint): Promise<FileContent> => {
        return getJson<FileContent>(this.apiUrl + '/pack/main.js?localeId=' + localeId + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.CanvasRenderer.FrontendCodeController}::{GetAdminMainJs}
     * @url '/en-admin/pack/main.js?localeId=' + localeId + ''
     */
    getAdminMainJs = (localeId?: uint): Promise<FileContent> => {
        return getJson<FileContent>(this.apiUrl + '/en-admin/pack/main.js?localeId=' + localeId + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.CanvasRenderer.FrontendCodeController}::{GetMainCss}
     * @url '/pack/main.css'
     */
    getMainCss = (): Promise<FileContent> => {
        return getJson<FileContent>(this.apiUrl + '/pack/main.css');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.CanvasRenderer.FrontendCodeController}::{GetAdminMainCss}
     * @url '/en-admin/pack/main.css'
     */
    getAdminMainCss = (): Promise<FileContent> => {
        return getJson<FileContent>(this.apiUrl + '/en-admin/pack/main.css');
    }

}

export default new FrontendCodeController();

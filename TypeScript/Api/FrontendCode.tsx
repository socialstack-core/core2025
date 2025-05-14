/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {FileContent} from './FileContent'
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/*
*/
export type UIReloadResult = {
    version?: long,
}

/*
*/
export type StaticFileInfo = {
    size?: long,
    modifiedUtc?: ulong,
    ref?: string,
}

/**
*/
export class FrontendCodeApi{
    public apiUrl: string = '';

    /**
      Reloads a prebuilt UI

    */
    public reload = (): Promise<UIReloadResult>  => {
        return getJson<UIReloadResult>(this.apiUrl + '//v1/monitoring/ui-reload')
    }

    /**
      Lists all available static files.

    */
    public getStaticFileList = (): Promise<StaticFileInfo[]>  => {
        return getJson<StaticFileInfo>(this.apiUrl + '//pack/static-assets/list.json')
    }

    /**
      The type metadata.

    */
    public getTypeMeta = (): Promise<FileContent>  => {
        return getJson<FileContent>(this.apiUrl + '//pack/type-meta.json')
    }

    /**
      Gets the email main.js file (site locale 1). The URL should be of the form /pack/email-static/main.js?loc=1&v=123123123123&h=ma83md83jd7hdur8
            Where loc is the locale ID, v is the original code build timestamp in ms, and h is the hash of the file.
            For convenience, ask FrontendCodeService for the url via GetMainJsUrl(Context context).

    */
    public getEmailMainJs = (localeId: uint = 1): Promise<FileContent>  => {
        return getJson<FileContent>(this.apiUrl + '//pack/email-static/main.js?localeId=' + localeId + '')
    }

    /**
      Gets global scss (debug dev builds only) so it can be seen. Bundle is e.g. "ui" or "admin".

    */
    public getGlobalScss = (bundle: string): Promise<FileContent>  => {
        return getJson<FileContent>(this.apiUrl + '//pack/scss/' + bundle + '')
    }

    /**
      Gets the main.js file (site locale 1). The URL should be of the form /pack/main.js?loc=1&v=123123123123&h=ma83md83jd7hdur8
            Where loc is the locale ID, v is the original code build timestamp in ms, and h is the hash of the file.
            For convenience, ask FrontendCodeService for the url via GetMainJsUrl(Context context).

    */
    public getMainJs = (localeId: uint = 1): Promise<FileContent>  => {
        return getJson<FileContent>(this.apiUrl + '//pack/main.js?localeId=' + localeId + '')
    }

    /**
      Gets the main.js file for the admin area (site locale 1). The URL should be of the form /en-admin/pack/main.js?loc=1&v=123123123123&h=ma83md83jd7hdur8
            Where loc is the locale ID, v is the original code build timestamp in ms, and h is the hash of the file.
            For convenience, ask FrontendCodeService for the url via GetMainJsUrl(Context context).

    */
    public getAdminMainJs = (localeId: uint = 1): Promise<FileContent>  => {
        return getJson<FileContent>(this.apiUrl + '//en-admin/pack/main.js?localeId=' + localeId + '')
    }

    /**
      Gets the main.css file for the ui (site locale 1). The URL should be of the form /pack/main.css?loc=1&v=123123123123&h=ma83md83jd7hdur8
            Where loc is the locale ID (currently unused), v is the original code build timestamp in ms, and h is the hash of the file.
            For convenience, ask FrontendCodeService for the url via GetMainJsUrl(Context context).

    */
    public getMainCss = (): Promise<FileContent>  => {
        return getJson<FileContent>(this.apiUrl + '//pack/main.css')
    }

    /**
      Gets the main.css file for the admin area (site locale 1). The URL should be of the form /en-admin/pack/main.css?loc=1&v=123123123123&h=ma83md83jd7hdur8
            Where loc is the locale ID (currently unused), v is the original code build timestamp in ms, and h is the hash of the file.
            For convenience, ask FrontendCodeService for the url via GetMainJsUrl(Context context).

    */
    public getAdminMainCss = (): Promise<FileContent>  => {
        return getJson<FileContent>(this.apiUrl + '//en-admin/pack/main.css')
    }

}

export default new FrontendCodeApi();

/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {FileContent} from './FileContent'
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/**
*/
export class DomainSiteMapApi{
    public apiUrl: string;

    /**
      Exposes the dynamic site map file

    */
    public siteMapXML = (): Promise<FileContent>  => {
        return getJson<FileContent>(this.apiUrl + '//sitemap.xml')
    }

}

export default new DomainSiteMapApi();

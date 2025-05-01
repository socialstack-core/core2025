/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/*
*/
export type AutoFormInfo = {
    supportsRevisions?: boolean,
    endpoint?: string,
    fields?: AutoFormField[],
}

/*
*/
export type AutoFormStructure = {
    forms?: AutoFormInfo[],
    contentTypes?: ContentType[],
}

/**
*/
export class AutoFormApi{
    public apiUrl: string = 'autoform';

    /**
      Gets the autoform info for a particular form by type and name. Type is usually content, component or config.
      @param {context} - Api.AvailableEndpoints.XmlDocMember
      @param {type} - Api.AvailableEndpoints.XmlDocMember
      @param {name} - Api.AvailableEndpoints.XmlDocMember

    */
    public get = (type: string, name: string): Promise<AutoFormInfo>  => {
        return getText(this.apiUrl + '/' + type + '/' + name + '')
    }

    /**
      GET /v1/autoform/all
            Returns meta about all content autoforms in this API.

    */
    public allContentForms = (): Promise<AutoFormStructure>  => {
        return getText(this.apiUrl + '/all')
    }

}

/*
*/
export type AutoFormField = {
    includable?: boolean,
    valueType?: string,
    module?: string,
    order?: uint,
    data?: Record<string, Record<string, string | number | boolean>>,
    tokeniseable?: boolean,
}

/*
*/
export type ContentType = {
    id?: int,
    name?: string,
}

export default new AutoFormApi();

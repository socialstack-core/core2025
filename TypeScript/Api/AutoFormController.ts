/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

import { ContentType } from 'Api/AvailableEndpointController';

// TYPES

/**
* This type was generated to reflect {AutoFormField} (Api.AutoForms.AutoFormField)
**/
export type AutoFormField = {
    includable?: boolean;
    valueType?: string;
    module?: string;
    order?: uint;
    data?: string[];
    tokeniseable?: boolean;
}

/**
* This type was generated to reflect {AutoFormInfo} (Api.AutoForms.AutoFormInfo)
**/
export type AutoFormInfo = {
    supportsRevisions?: boolean;
    endpoint?: string;
    fields?: AutoFormField[];
}

/**
* This type was generated to reflect {AutoFormStructure} (Api.AutoForms.AutoFormStructure)
**/
export type AutoFormStructure = {
    forms?: AutoFormInfo[];
    contentTypes: ContentType[];
}
// NON-ENTITY CONTROLLERS

export class AutoFormController {

   private apiUrl: string = '/v1/autoform';

    /**
     * Generated from a .NET type.
     * @see {Api.AutoForms.AutoFormController}::{Get}
     * @url 'v1/autoform/' + type + '/' + name + ''
     */
    get = (type: string, name: string): Promise<AutoFormInfo> => {
        return getJson<AutoFormInfo>(this.apiUrl + '/' + type + '/' + name + '')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.AutoForms.AutoFormController}::{AllContentForms}
     * @url 'v1/autoform/all'
     */
    allContentForms = (): Promise<AutoFormStructure> => {
        return getJson<AutoFormStructure>(this.apiUrl + '/all')
    }

}

export default new AutoFormController();

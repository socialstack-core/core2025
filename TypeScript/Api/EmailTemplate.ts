/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { EmailTemplateIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {EmailTemplate} (Api.Emails.EmailTemplate)
**/
export type EmailTemplate = VersionedContent<uint> & {
    key?: string;
    name?: string;
    subject?: string;
    bodyJson?: string;
    notes?: string;
    sendFrom?: string;
    emailType?: int;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}

/**
* This type was generated to reflect {EmailTestResponse} (Api.Emails.EmailTestResponse)
**/
export type EmailTestResponse = {
    sent?: boolean;
}

/**
* This type was generated to reflect {EmailTestRequest} (Api.Emails.EmailTestRequest)
**/
export type EmailTestRequest = {
    templateKey?: string;
    customData?: string;
}
// ENTITY CONTROLLER

export class EmailTemplateApi extends AutoController<EmailTemplate,uint>{

    constructor(){
        super('/v1/emailtemplate');
        this.includes = new EmailTemplateIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Emails.EmailTemplateController}::{TestEmail}
     * @url 'v1/emailtemplate/test'
     */
    testEmail = (mailTest: EmailTestRequest): Promise<EmailTestResponse> => {
        return getJson<EmailTestResponse>(this.apiUrl + '/test', mailTest);
    }

}

export default new EmailTemplateApi();

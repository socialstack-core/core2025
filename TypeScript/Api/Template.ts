/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiIncludes } from './Includes';
// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { TemplateIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Template} (Api.Templates.Template)
**/
export type Template = VersionedContent<uint> & {
    key?: string;
    title?: string;
    description?: string;
    templateParent?: uint;
    baseTemplate?: string;
    templateType?: uint;
    moduleGroups?: string;
    bodyJson?: string;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class TemplateApi extends AutoController<Template,uint>{

    constructor(){
        super('/v1/template');
        this.includes = new TemplateIncludes();
    }

}

export default new TemplateApi();

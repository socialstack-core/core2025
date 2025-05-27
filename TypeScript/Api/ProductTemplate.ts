/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ProductTemplateIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {ProductTemplate} (Api.Payments.ProductTemplate)
**/
export type ProductTemplate = VersionedContent<uint> & {
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class ProductTemplateApi extends AutoController<ProductTemplate,uint>{

    constructor(){
        super('/v1/producttemplate');
        this.includes = new ProductTemplateIncludes();
    }

}

export default new ProductTemplateApi();

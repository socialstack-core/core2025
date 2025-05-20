/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ProductAttributeIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {ProductAttribute} (Api.Payments.ProductAttribute)
**/
export type ProductAttribute = VersionedContent<uint> & {
    name?: string;
    productAttributeType?: int;
    units?: string;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class ProductAttributeApi extends AutoController<ProductAttribute,uint>{

    constructor(){
        super('/v1/productAttribute');
        this.includes = new ProductAttributeIncludes();
    }

}

export default new ProductAttributeApi();

/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ProductAttributeValueIncludes } from 'Api/Includes';

import { ProductAttribute } from 'Api/ProductAttribute';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {ProductAttributeValue} (Api.Payments.ProductAttributeValue)
**/
export type ProductAttributeValue = VersionedContent<uint> & {
    productAttributeId?: uint;
    value?: string;
    // HasVirtualField() fields (2 in total)
    attribute?: ProductAttribute;
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class ProductAttributeValueApi extends AutoController<ProductAttributeValue,uint>{

    constructor(){
        super('/v1/productAttributeValue');
        this.includes = new ProductAttributeValueIncludes();
    }

}

export default new ProductAttributeValueApi();

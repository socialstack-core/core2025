/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiIncludes } from './Includes';
// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ProductAttributeGroupIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {ProductAttributeGroup} (Api.Payments.ProductAttributeGroup)
**/
export type ProductAttributeGroup = VersionedContent<uint> & {
    parentGroupKey?: string;
    key?: string;
    parentGroupId?: uint;
    name?: string;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class ProductAttributeGroupApi extends AutoController<ProductAttributeGroup,uint>{

    constructor(){
        super('/v1/productattributegroup');
        this.includes = new ProductAttributeGroupIncludes();
    }

}

export default new ProductAttributeGroupApi();

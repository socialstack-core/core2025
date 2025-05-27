/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ProductAttributeIncludes } from 'Api/Includes';

import { ProductAttributeGroup } from 'Api/ProductAttributeGroup';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {ProductAttribute} (Api.Payments.ProductAttribute)
**/
export type ProductAttribute = VersionedContent<uint> & {
    productAttributeGroupKey?: string;
    name?: string;
    key?: string;
    productAttributeGroupId?: uint;
    productAttributeType?: int;
    rangeType?: int;
    multiple?: boolean;
    units?: string;
    // HasVirtualField() fields (2 in total)
    attributeGroup?: ProductAttributeGroup;
    creatorUser?: User;
}

/**
* This type was generated to reflect {RouterNodeMetadata} (Api.Startup.Routing.RouterNodeMetadata)
**/
export type RouterNodeMetadata = {
    type?: string;
    hasChildren?: boolean;
    fullRoute?: string;
    childKey?: string;
    name?: string;
    contentId?: ulong;
    editUrl?: string;
}

/**
* This type was generated to reflect {TreeNodeDetail} (Api.Pages.PageController+TreeNodeDetail)
**/
export type TreeNodeDetail = {
    children: RouterNodeMetadata[];
    self: RouterNodeMetadata;
}

/**
* This type was generated to reflect {AttributeTreeLocation} (Api.Payments.AttributeTreeLocation)
**/
export type AttributeTreeLocation = {
    path?: string;
}
// ENTITY CONTROLLER

export class ProductAttributeApi extends AutoController<ProductAttribute,uint>{

    constructor(){
        super('/v1/productattribute');
        this.includes = new ProductAttributeIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductAttributeController}::{GetTreeNode}
     * @url 'v1/productAttribute/tree'
     */
    getTreeNode = (location: AttributeTreeLocation): Promise<TreeNodeDetail | undefined> => {
        return getJson<TreeNodeDetail | undefined>(this.apiUrl + '/tree', location)
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductAttributeController}::{GetTreeNodePath}
     * @url 'v1/productAttribute/tree'
     */
    getTreeNodePath = (path: string): Promise<TreeNodeDetail | undefined> => {
        return getJson<TreeNodeDetail | undefined>(this.apiUrl + '/tree?path=' + path + '')
    }

}

export default new ProductAttributeApi();

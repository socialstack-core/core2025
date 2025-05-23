/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getList, getJson, getText } from 'UI/Functions/WebRequest';

import { ApiIncludes } from './Includes';
// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ProductCategoryIncludes } from 'Api/Includes';

import { Product } from 'Api/Product';

import { RouterNodeMetadata } from 'Api/ProductAttribute';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {ProductCategory} (Api.Payments.ProductCategory)
**/
export type ProductCategory = VersionedContent<uint> & {
    name?: string;
    slug?: string;
    description?: string;
    featureRef?: string;
    iconRef?: string;
    parentId?: uint;
    // HasVirtualField() fields (2 in total)
    productCategory?: ProductCategory;
    creatorUser?: User;
}

/**
* This type was generated to reflect {ProductNode} (Api.Payments.ProductNode)
**/
export type ProductNode = {
    product?: Product;
    slug?: string;
    isPrimary?: boolean;
}

/**
* This type was generated to reflect {ProductCategoryNode} (Api.Payments.ProductCategoryNode)
**/
export type ProductCategoryNode = {
    children?: ProductCategoryNode[][];
    parent?: ProductCategoryNode;
    products: ProductNode[][];
    fullPathSlug?: string;
    category?: ProductCategory;
}

/**
* This type was generated to reflect {TreeNodeDetail} (Api.Pages.PageController+TreeNodeDetail)
**/
export type TreeNodeDetail = {
    children: RouterNodeMetadata[];
    self: RouterNodeMetadata;
}

/**
* This type was generated to reflect {CategoryTreeLocation} (Api.Payments.CategoryTreeLocation)
**/
export type CategoryTreeLocation = {
    path?: string;
}
// ENTITY CONTROLLER

export class ProductCategoryApi extends AutoController<ProductCategory,uint>{

    constructor(){
        super('/v1/productcategory');
        this.includes = new ProductCategoryIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{Structure}
     * @url 'v1/productCategory/structure'
     */
    structure = (includeProducts?: boolean, includes?: ApiIncludes[]): Promise<ProductCategoryNode[]> => {
        return getJson<ProductCategoryNode[]>(this.apiUrl + '/structure?includeProducts=' + includeProducts + '' + (Array.isArray(includes) ? 'includes=' + includes.join(',') : '') + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{GetTreeNode}
     * @url 'v1/productCategory/tree'
     */
    getTreeNode = (location: CategoryTreeLocation): Promise<TreeNodeDetail> => {
        return getJson<TreeNodeDetail>(this.apiUrl + '/tree', location);
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{GetTreeNodePath}
     * @url 'v1/productCategory/tree'
     */
    getTreeNodePath = (path: string, includes?: ApiIncludes[]): Promise<TreeNodeDetail> => {
        return getJson<TreeNodeDetail>(this.apiUrl + '/tree?path=' + path + '' + (Array.isArray(includes) ? 'includes=' + includes.join(',') : '') + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{GetProducts}
     * @url 'v1/productCategory/{id}/products'
     */
    getProducts = (id: uint): Promise<Product[]> => {
        return getJson<Product[]>(this.apiUrl + '/' + id +'/products');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{GetProductCategories}
     * @url 'v1/productCategory/product/{id}'
     */
    getProductCategories = (id: uint, includes?: ApiIncludes[]): Promise<ApiList<ProductCategory>> => {
        return getList<ProductCategory>(this.apiUrl + '/product/' + id +'' + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{GetChildren}
     * @url 'v1/productCategory/{id}/children'
     */
    getChildren = (id: uint): Promise<ProductCategoryNode[]> => {
        return getJson<ProductCategoryNode[]>(this.apiUrl + '/' + id +'/children');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{GetParents}
     * @url 'v1/productCategory/{id}/parents'
     */
    getParents = (id: uint): Promise<ProductCategoryNode[]> => {
        return getJson<ProductCategoryNode[]>(this.apiUrl + '/' + id +'/parents');
    }

}

export default new ProductCategoryApi();

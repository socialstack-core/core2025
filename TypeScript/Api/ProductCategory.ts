/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getList, getJson } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ProductCategoryIncludes } from 'Api/Includes';

import { User } from 'Api/User';

import { Product } from 'Api/Product';

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
// ENTITY CONTROLLER

export class ProductCategoryApi extends AutoController<ProductCategory,uint>{

    constructor(){
        super('/v1/productCategory');
        this.includes = new ProductCategoryIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{Structure}
     * @url 'v1/productCategory/structure?includeProducts=' + includeProducts + ''
     */
    structure = (includeProducts?: boolean): Promise<ProductCategoryNode[]> => {
        return getJson<ProductCategoryNode[]>(this.apiUrl + '/structure?includeProducts=' + includeProducts + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{GetProducts}
     * @url 'v1/productCategory/' + id + '/products'
     */
    getProducts = (id: uint): Promise<Product[]> => {
        return getJson<Product[]>(this.apiUrl + '/' + id + '/products');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{GetProductCategories}
     * @url 'v1/productCategory/product/' + id + ''
     */
    getProductCategories = (id: uint): Promise<ApiList<ProductCategory>> => {
        return getList<ProductCategory>(this.apiUrl + '/product/' + id + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{GetChildren}
     * @url 'v1/productCategory/' + id + '/children'
     */
    getChildren = (id: uint): Promise<ProductCategoryNode[]> => {
        return getJson<ProductCategoryNode[]>(this.apiUrl + '/' + id + '/children');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ProductCategoryController}::{GetParents}
     * @url 'v1/productCategory/' + id + '/parents'
     */
    getParents = (id: uint): Promise<ProductCategoryNode[]> => {
        return getJson<ProductCategoryNode[]>(this.apiUrl + '/' + id + '/parents');
    }

}

export default new ProductCategoryApi();

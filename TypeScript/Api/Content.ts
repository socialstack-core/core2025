/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getList, getOne, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

import { ApiIncludes } from 'Api/Includes';

import { Role } from 'Api/Role';

import { Tag } from 'Api/Tag';

import { ProductAttributeValue } from 'Api/ProductAttributeValue';

import { ProductCategory } from 'Api/ProductCategory';

import { ProductQuantity } from 'Api/ProductQuantity';

import { Product } from 'Api/Product';

import { Subscription } from 'Api/Subscription';

import { User } from 'Api/User';

import { CustomContentTypeField } from 'Api/CustomContentType';

import { Category } from 'Api/Category';

import { Upload } from 'Api/Upload';

// OPEN GENERICS

export type Content<ID> = {
    type?: string;
    id: ID;
    // adding (21) global virtual fields.
    primaryUrl?: string;
    emailAddress?: string;
    signedRef128?: string;
    signedRef256?: string;
    signedRefOriginal?: string;
    rolePermits?: Role[];
    composition?: Role[];
    tags?: Tag[];
    attributes?: ProductAttributeValue[];
    productCategories?: ProductCategory[];
    productQuantities?: ProductQuantity[];
    tiers?: Product[];
    optionalExtras?: Product[];
    accessories?: Product[];
    suggestions?: Product[];
    subscriptions?: Subscription[];
    userPermits?: User[];
    customContentTypeFields?: CustomContentTypeField[];
    categories?: Category[];
    productImages?: Upload[];
    uploads?: Upload[];
}


export type UserCreatedContent<T> = Content<T> & {
    userId?: uint;
    createdUtc?: Date | string | number;
    editedUtc?: Date | string | number;
}


export type VersionedContent<T> = UserCreatedContent<T> & {
    revisionId: T;
    isDraft?: boolean;
    publishDraftDate?: Date | string | number;
    revision?: int;
}


// TYPES

/**
* This type was generated to reflect {FilterSortConfig} (Api.Startup.FilterSortConfig)
**/
export type FilterSortConfig = {
    field?: string;
    direction?: string;
}

/**
* This type was generated to reflect {ListFilter} (Api.Startup.ListFilter)
**/
export type ListFilter = {
    pageSize?: int;
    pageIndex?: int;
    query?: string;
    sort: FilterSortConfig;
    includeTotal?: boolean;
    args?: Object[];
}
// AUTO CONTROLLERS

export class AutoController<T extends Content<uint>, ID> {

    protected apiUrl: string;

    public includes: ApiIncludes;
    constructor(baseUrl: string = '') {
        this.apiUrl = baseUrl?.toLowerCase();
        this.includes = new ApiIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {T}::{LoadRevision}
     * @url 'revision/{id}'
     */
    loadRevision = (id: ID, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/revision/' + id  + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '')
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{DeleteRevision}
     * @url 'revision/{id}'
     */
    deleteRevision = (id: ID, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/revision/' + id  + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '', {}, { method: 'DELETE' } )
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{RevisionList}
     * @url 'revision/list'
     */
    revisionList = (filters?: ListFilter, includes?: ApiIncludes[]): Promise<ApiList<T> | undefined> => {
        return getJson<ApiList<T> | undefined>(this.apiUrl + '/revision/list' + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '', filters)
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{UpdateRevision}
     * @url 'revision/{id}'
     */
    updateRevision = (id: ID, body?: Partial<T>, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/revision/' + id  + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '', body)
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{PublishRevision}
     * @url 'publish/{id}'
     */
    publishRevision = (id: ID, body?: Partial<T>, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/publish/' + id  + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '', body)
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{CreateDraft}
     * @url 'draft'
     */
    createDraft = (body?: Partial<T>, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/draft' + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '', body)
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{Load}
     * @url '{id}'
     */
    load = (id: ID, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/' + id  + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '')
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{Delete}
     * @url '{id}'
     */
    delete = (id: ID, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/' + id  + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '', {}, { method: 'DELETE' } )
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{InvalidateCachedItem}
     * @url 'cache/invalidate/{id}'
     */
    invalidateCachedItem = (id: ID, includes?: ApiIncludes[]): Promise<string> => {
        return getText(this.apiUrl + '/cache/invalidate/' + id  + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '')
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{InvalidateCache}
     * @url 'cache/invalidate'
     */
    invalidateCache = (includes?: ApiIncludes[]): Promise<string> => {
        return getText(this.apiUrl + '/cache/invalidate' + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '')
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{ListAll}
     * @url 'list'
     */
    listAll = (includes?: ApiIncludes[]): Promise<ApiList<T> | undefined> => {
        return getJson<ApiList<T> | undefined>(this.apiUrl + '/list' + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '')
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{List}
     * @url 'list'
     */
    list = (filters?: ListFilter, includes?: ApiIncludes[]): Promise<ApiList<T> | undefined> => {
        return getJson<ApiList<T> | undefined>(this.apiUrl + '/list' + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '', filters)
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{Create}
     * @url ''
     */
    create = (body?: Partial<T>, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/' + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '', body)
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{Update}
     * @url '{id}'
     */
    update = (id: ID, body?: Partial<T>, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/' + id  + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '', body)
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{ListPOTUpdate}
     * @url 'list.pot'
     */
    listPOTUpdate = (includes?: ApiIncludes[]): Promise<string> => {
        return getText(this.apiUrl + '/list.pot' + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '')
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{ListPOT}
     * @url 'list.pot'
     */
    listPOT = (filters?: Partial<T>, ignoreFields?: String, includes?: ApiIncludes[]): Promise<string> => {
        return getText(this.apiUrl + '/list.pot?ignoreFields=' + ignoreFields + '' + (Array.isArray(includes) ? '&includes=' + includes.join(',') : '') + '', filters)
    };

}


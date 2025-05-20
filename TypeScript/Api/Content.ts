/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getList, getOne, getJson } from 'UI/Functions/WebRequest';

// IMPORTS

import { ApiIncludes } from 'Api/Includes';

// OPEN GENERICS

export type Content<ID> = {
    type?: string;
    id: ID;
    // adding (22) global virtual fields.
    primaryUrl?: unknown;
    emailAddress?: unknown;
    signedRef128?: unknown;
    signedRef256?: unknown;
    signedRefOriginal?: unknown;
    rolePermits?: unknown;
    composition?: unknown;
    tags?: unknown;
    attributeGroups?: unknown;
    childGroups?: unknown;
    productCategories?: unknown;
    productQuantities?: unknown;
    tiers?: unknown;
    optionalExtras?: unknown;
    accessories?: unknown;
    suggestions?: unknown;
    subscriptions?: unknown;
    userPermits?: unknown;
    customContentTypeFields?: unknown;
    categories?: unknown;
    productImages?: unknown;
    uploads?: unknown;
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
        this.apiUrl = baseUrl;
        this.includes = new ApiIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {T}::{LoadRevision}
     * @url 'revision/' + id + ''
     */
    loadRevision = (id: ID, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/revision/' + id + '' + (Array.isArray(includes) ? '&includes=' + includes.map(t => t.getText()).join(',') : ''));
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{DeleteRevision}
     * @url 'revision/' + id + ''
     */
    deleteRevision = (id: ID, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/revision/' + id + '' + (Array.isArray(includes) ? '&includes=' + includes.map(t => t.getText()).join(',') : ''));
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{RevisionList}
     * @url 'revision/list'
     */
    revisionList = (filters?: ListFilter): Promise<ApiList<T>> => {
        return getJson<ApiList<T>>(this.apiUrl + '/revision/list', filters);
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{UpdateRevision}
     * @url 'revision/' + id + ''
     */
    updateRevision = (id: ID, body?: T, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/revision/' + id + '' + (Array.isArray(includes) ? '&includes=' + includes.map(t => t.getText()).join(',') : ''), body);
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{PublishRevision}
     * @url 'publish/' + id + ''
     */
    publishRevision = (id: ID, body?: T, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/publish/' + id + '' + (Array.isArray(includes) ? '&includes=' + includes.map(t => t.getText()).join(',') : ''), body);
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{CreateDraft}
     * @url 'draft'
     */
    createDraft = (body?: T, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/draft' + (Array.isArray(includes) ? '&includes=' + includes.map(t => t.getText()).join(',') : ''), body);
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{Load}
     * @url '' + id + ''
     */
    load = (id: ID, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/' + id + '' + (Array.isArray(includes) ? '&includes=' + includes.map(t => t.getText()).join(',') : ''));
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{Delete}
     * @url '' + id + ''
     */
    delete = (id: ID, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/' + id + '' + (Array.isArray(includes) ? '&includes=' + includes.map(t => t.getText()).join(',') : ''));
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{InvalidateCachedItem}
     * @url 'cache/invalidate/' + id + ''
     */
    invalidateCachedItem = (id: ID): Promise<void> => {
        return new Promise<void>((resolve, reject) => getJson(this.apiUrl + '/cache/invalidate/' + id + '').then(() => resolve()).catch(reject));
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{InvalidateCache}
     * @url 'cache/invalidate'
     */
    invalidateCache = (): Promise<void> => {
        return new Promise<void>((resolve, reject) => getJson(this.apiUrl + '/cache/invalidate').then(() => resolve()).catch(reject));
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{ListAll}
     * @url 'list'
     */
    listAll = (): Promise<ApiList<T>> => {
        return getJson<ApiList<T>>(this.apiUrl + '/list');
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{List}
     * @url 'list'
     */
    list = (filters?: ListFilter): Promise<ApiList<T>> => {
        return getJson<ApiList<T>>(this.apiUrl + '/list', filters);
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{Create}
     * @url ''
     */
    create = (body?: T, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/' + (Array.isArray(includes) ? '&includes=' + includes.map(t => t.getText()).join(',') : ''), body);
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{Update}
     * @url '' + id + ''
     */
    update = (id: ID, body?: T, includes?: ApiIncludes[]): Promise<T> => {
        return getOne<T>(this.apiUrl + '/' + id + '' + (Array.isArray(includes) ? '&includes=' + includes.map(t => t.getText()).join(',') : ''), body);
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{ListPOTUpdate}
     * @url 'list.pot'
     */
    listPOTUpdate = (): Promise<Object> => {
        return getJson<Object>(this.apiUrl + '/list.pot');
    };


    /**
     * Generated from a .NET type.
     * @see {T}::{ListPOT}
     * @url 'list.pot?includes=' + includes + '&ignoreFields=' + ignoreFields + ''
     */
    listPOT = (filters?: T, includes?: String, ignoreFields?: String): Promise<void> => {
        return new Promise<void>((resolve, reject) => getJson(this.apiUrl + '/list.pot?includes=' + includes + '&ignoreFields=' + ignoreFields + '', filters).then(() => resolve()).catch(reject));
    };

}


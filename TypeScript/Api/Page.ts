/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { PageIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Page} (Api.Pages.Page)
**/
export type Page = VersionedContent<uint> & {
    url?: string;
    title?: string;
    key?: string;
    bodyJson?: string;
    description?: string;
    canIndex?: boolean;
    noFollow?: boolean;
    preferIfLoggedIn?: boolean;
    // HasVirtualField() fields (1 in total)
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
* This type was generated to reflect {RouterTreeNodeDetail} (Api.Pages.PageController+RouterTreeNodeDetail)
**/
export type RouterTreeNodeDetail = {
    children: RouterNodeMetadata[];
    self: RouterNodeMetadata;
}

/**
* This type was generated to reflect {RouterTreeLocation} (Api.Pages.PageController+RouterTreeLocation)
**/
export type RouterTreeLocation = {
    url?: string;
}

/**
* This type was generated to reflect {PageStateResult} (Api.Pages.PageStateResult)
**/
export type PageStateResult = {
    oldVersion?: boolean;
    redirect?: string;
    config?: string[];
    page?: Page;
    description?: string;
    title?: string;
    po?: Object;
    tokenNames?: string[];
    tokens?: string[];
}

/**
* This type was generated to reflect {PageDetails} (Api.Pages.PageController+PageDetails)
**/
export type PageDetails = {
    url?: string;
    version?: long;
}
// ENTITY CONTROLLER

export class PageApi extends AutoController<Page,uint>{

    constructor(){
        super('/v1/page');
        this.includes = new PageIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Pages.PageController}::{GetRouterTreeNode}
     * @url 'v1/page/tree'
     */
    getRouterTreeNode = (location: RouterTreeLocation): Promise<RouterTreeNodeDetail> => {
        return getJson<RouterTreeNodeDetail>(this.apiUrl + '/tree', location);
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Pages.PageController}::{GetRouterTreeNodePath}
     * @url 'v1/page/tree?url=' + url + ''
     */
    getRouterTreeNodePath = (url: string): Promise<RouterTreeNodeDetail> => {
        return getJson<RouterTreeNodeDetail>(this.apiUrl + '/tree?url=' + url + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Pages.PageController}::{PageState}
     * @url 'v1/page/state'
     */
    pageState = (pageDetails: PageDetails): Promise<PageStateResult> => {
        return getJson<PageStateResult>(this.apiUrl + '/state', pageDetails);
    }

}

export default new PageApi();

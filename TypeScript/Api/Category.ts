/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiIncludes } from './Includes';
// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { CategoryIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Category} (Api.Categories.Category)
**/
export type Category = VersionedContent<uint> & {
    name?: string;
    description?: string;
    featureRef?: string;
    iconRef?: string;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class CategoryApi extends AutoController<Category,uint>{

    constructor(){
        super('/v1/category');
        this.includes = new CategoryIncludes();
    }

}

export default new CategoryApi();

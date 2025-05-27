/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { PermalinkIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Permalink} (Api.Pages.Permalink)
**/
export type Permalink = VersionedContent<uint> & {
    url?: string;
    target?: string;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class PermalinkApi extends AutoController<Permalink,uint>{

    constructor(){
        super('/v1/permalink');
        this.includes = new PermalinkIncludes();
    }

}

export default new PermalinkApi();

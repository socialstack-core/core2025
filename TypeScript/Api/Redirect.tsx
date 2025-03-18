/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {RedirectIncludes, UserIncludes} from './Includes'
import {User} from 'Api/User'
import {getJson, ApiList} from 'UI/Functions/WebRequest'

// Module
/*
  A Redirect
*/
export type Redirect = VersionedContent & {
    from?: string,
    to?: string,
    permanentRedirect?: boolean,
    revision?: number,
    userId?: number,
    createdUtc?: Date,
    editedUtc?: Date,
    id?: number,
    revisionId?: number,
    isDraft?: boolean,
    type?: string,
    creatorUser?: User,
}

/**
    Auto generated API for Redirect
    Handles redirect endpoints.
*/
export class RedirectApi extends AutoApi<Redirect, RedirectIncludes>{
    /**
      This extends the AutoApi class, which provides CRUD functionality, any methods seen in are from custom endpoints in the controller

    */
    public constructor(){
        super('v1/redirect')
        this.includes = new RedirectIncludes();
    }

}

export default new RedirectApi();

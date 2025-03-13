/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes, VersionedContent} from 'Api/ApiEndpoints'
import {User, UserIncludes} from './User'

// Module
export class RedirectIncludes extends ApiIncludes{
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'CreatorUser')
    }

}

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

export class RedirectApi extends AutoApi<Redirect, RedirectIncludes>{
    public constructor(){
        super('v1/redirect')
    }

}

export default new RedirectApi();

/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, VersionedContent} from 'TypeScript/Api/ApiEndpoints'

// Module
export type Redirect = VersionedContent<number> & {
    from?: string,
    to?: string,
    permanentRedirect?: boolean,
}

export class RedirectApi extends AutoApi<Redirect>{
    public constructor(){
        super('v1/redirect')
    }

}



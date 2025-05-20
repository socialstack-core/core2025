/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson } from 'UI/Functions/WebRequest';

// TYPES

/**
* This type was generated to reflect {FileContent} (Api.Startup.Routing.FileContent)
**/
export type FileContent = {
    mimeType?: string;
    rawBytes?: byte[][];
    lastModifiedUtc?: string;
    etag?: string;
    isCompressed?: boolean;
}
// NON-ENTITY CONTROLLERS

export class DomainCertificateChallengeController {

   private apiUrl: string = '/.well-known/acme-challenge';

    /**
     * Generated from a .NET type.
     * @see {Api.CloudHosts.DomainCertificateChallengeController}::{CatchAll}
     * @url '.well-known/acme-challenge/' + token + ''
     */
    catchAll = (token: string): Promise<FileContent> => {
        return getJson<FileContent>(this.apiUrl + '/' + token + '');
    }

}

export default new DomainCertificateChallengeController();

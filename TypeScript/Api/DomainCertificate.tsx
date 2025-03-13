/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes, VersionedContent} from 'Api/ApiEndpoints'
import {User, UserIncludes} from './User'

// Module
export class DomainCertificateIncludes extends ApiIncludes{
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'CreatorUser')
    }

}

export type DomainCertificate = VersionedContent & {
    domain?: string,
    ready?: boolean,
    fileKey?: string,
    expiryUtc?: Date,
    serverId?: number,
    orderUrl?: string,
    status?: number,
    lastPingUtc?: Date,
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

export class DomainCertificateApi extends AutoApi<DomainCertificate, DomainCertificateIncludes>{
    public constructor(){
        super('v1/domainCertificate')
    }

}

export default new DomainCertificateApi();

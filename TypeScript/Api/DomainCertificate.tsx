/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {User, UserIncludes} from './User'
import {getJson, ApiList} from 'UI/Functions/WebRequest'

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

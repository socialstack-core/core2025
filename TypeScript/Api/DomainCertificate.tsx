/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {User, UserIncludes} from './User'
import {getJson, ApiList} from 'UI/Functions/WebRequest'

// Module
/**
    Allows custom chained includes inside the list & load methods.
*/
export class DomainCertificateIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'CreatorUser')
    }

}

/*
  A DomainCertificate
*/
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

/**
    Auto generated API for DomainCertificate
    Handles domainCertificate endpoints.
*/
export class DomainCertificateApi extends AutoApi<DomainCertificate, DomainCertificateIncludes>{
    /**
      This extends the AutoApi class, which provides CRUD functionality, any methods seen in are from custom endpoints in the controller

    */
    public constructor(){
        super('v1/domainCertificate')
    }

}

export default new DomainCertificateApi();

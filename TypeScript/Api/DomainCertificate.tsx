/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {DomainCertificateIncludes, UserIncludes} from './Includes'
import {User} from 'Api/User'
import {getJson, ApiList} from 'UI/Functions/WebRequest'

// Module
/*
  A DomainCertificate
*/
export type DomainCertificate = VersionedContent & {
    domain?: string,
    ready: boolean,
    fileKey?: string,
    expiryUtc?: Date,
    serverId: uint,
    orderUrl?: string,
    status: uint,
    lastPingUtc: Date,
    creatorUser: User,
}

/**
    Auto generated API for DomainCertificate
    Handles domainCertificate endpoints.
*/
export class DomainCertificateApi extends AutoApi<DomainCertificate, DomainCertificateIncludes>{
    /**
      This extends the AutoApi class, which provides CRUD functionality, any methods seen in are from custom endpoints in the controller

    */
    public constructor (){
        super('domainCertificate')
        this.includes = new DomainCertificateIncludes();
    }

}

export default new DomainCertificateApi();

/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, VersionedContent} from 'TypeScript/Api/ApiEndpoints'

// Module
export type DomainCertificate = VersionedContent<number> & {
    domain?: string,
    ready?: boolean,
    fileKey?: string,
    serverId?: number,
    orderUrl?: string,
    status?: number,
    lastPingUtc?: Date,
}

export class DomainCertificateApi extends AutoApi<DomainCertificate>{
    public constructor(){
        super('v1/domainCertificate')
    }

}



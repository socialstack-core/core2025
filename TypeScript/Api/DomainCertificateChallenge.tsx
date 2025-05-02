/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {FileContent} from './FileContent'
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/**
*/
export class DomainCertificateChallengeApi{
    public apiUrl: string = '.well-known/acme-challenge';

    /**
      Handles all token requests.

    */
    public catchAll = (token: string): Promise<FileContent>  => {
        return getJson<FileContent>(this.apiUrl + '/' + token + '')
    }

}

export default new DomainCertificateChallengeApi();

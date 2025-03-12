/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import { AutoApi, VersionedContent } from 'TypeScript/Api/ApiEndpoints';
import webRequest, { ApiSuccess, ApiFailure } from 'UI/Functions/WebRequest';

// Module
export type User = VersionedContent<number> & {
    email?: string,
    emailOptOutFlags?: number,
    emailVerifyToken?: string,
    passwordHash?: string,
    loginAttempts?: number,
    loginRevokeCount?: number,
    role?: number,
    privateVerify?: number,
    featureRef?: string,
    avatarRef?: string,
    username?: string,
}

class UserApi extends AutoApi<User>{
    public constructor(){
        super('v1/user')
    }

    public self(): Promise<ApiSuccess<User> | ApiFailure> {
        return webRequest(this.apiUrl + '/self')
    }
    
    public unpersonate(): Promise<ApiSuccess<User> | ApiFailure> {
        return webRequest(this.apiUrl + '/unpersonate')
    }

}

const api = new UserApi();
export default api;


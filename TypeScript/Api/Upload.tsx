/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {User, UserIncludes} from './User'
import {getJson, ApiList} from 'UI/Functions/WebRequest'

// Module
export class UploadIncludes extends ApiIncludes{
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'CreatorUser')
    }

}

export type Upload = VersionedContent & {
    originalName?: string,
    fileType?: string,
    variants?: string,
    blurhash?: string,
    width?: number,
    height?: number,
    focalX?: number,
    focalY?: number,
    alt?: string,
    author?: string,
    usageCount?: number,
    isImage?: boolean,
    isPrivate?: boolean,
    isVideo?: boolean,
    isAudio?: boolean,
    transcodeState?: number,
    revision?: number,
    userId?: number,
    createdUtc?: Date,
    editedUtc?: Date,
    id?: number,
    ref?: string,
    revisionId?: number,
    isDraft?: boolean,
    type?: string,
    creatorUser?: User,
}

export type List`1 = {
    items?: MediaRef[],
    size?: number,
    version?: number,
}

export type List`1 = {
    items?: MediaRef[],
    size?: number,
    version?: number,
}

export type List`1 = {
    items?: MediaRef[],
    size?: number,
    version?: number,
}

export class UploadApi extends AutoApi<Upload, UploadIncludes>{
    public constructor(){
        super('v1/upload')
    }

    public upload(): Promise<void> {
        return getJson(this.apiUrl + '/create', { })
    }

    public upload(): Promise<void> {
        return getJson(this.apiUrl + '/create', { })
    }

    public transcodedTar(): Promise<void> {
        return getJson(this.apiUrl + '/transcoded/{id}', { })
    }

    public active(): Promise<void> {
        return getJson(this.apiUrl + '/active', { })
    }

    public activePost(): Promise<void> {
        return getJson(this.apiUrl + '/active', { })
    }

    public fileConsistency(): Promise<void> {
        return getJson(this.apiUrl + '/file-consistency', { })
    }

    public replace(): Promise<List`1> {
        return getJson(this.apiUrl + '/replace', { })
    }

    public updateAlts(): Promise<void> {
        return getJson(this.apiUrl + '/update-alts', { })
    }

    public updateRefs(): Promise<List`1> {
        return getJson(this.apiUrl + '/update-refs', { })
    }

    public preview(): Promise<List`1> {
        return getJson(this.apiUrl + '/replace/preview', { })
    }

}

export default new UploadApi();

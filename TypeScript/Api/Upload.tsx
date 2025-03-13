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

export type FileUploadBody = {
}

export type MediaRef = {
}

export class UploadApi extends AutoApi<Upload, UploadIncludes>{
    public constructor(){
        super('v1/upload')
    }

    public upload(body: FileUploadBody): Promise<void> {
        return getJson(`${this.apiUrl}/create`, { body })
    }

    public transcodedTar(id: number, token: string): Promise<void> {
        return getJson(`${this.apiUrl}/transcoded/${id}?token=${token}`)
    }

    public active(includes: string): Promise<void> {
        return getJson(`${this.apiUrl}/active?includes=${includes}`)
    }

    public activePost(includes: string): Promise<void> {
        return getJson(`${this.apiUrl}/active?includes=${includes}`)
    }

    public fileConsistency(regenBefore: string, idRange: string): Promise<void> {
        return getJson(`${this.apiUrl}/file-consistency?regenBefore=${regenBefore}&idRange=${idRange}`)
    }

    public replace(sourceRef: string, targetRef: string): Promise<ApiList<MediaRef>> {
        return getJson(`${this.apiUrl}/replace?sourceRef=${sourceRef}&targetRef=${targetRef}`)
    }

    public updateAlts(): Promise<void> {
        return getJson(`${this.apiUrl}/update-alts`)
    }

    public updateRefs(update: boolean): Promise<ApiList<MediaRef>> {
        return getJson(`${this.apiUrl}/update-refs?update=${update}`)
    }

    public preview(uploadRef: string): Promise<ApiList<MediaRef>> {
        return getJson(`${this.apiUrl}/replace/preview?uploadRef=${uploadRef}`)
    }

}

export default new UploadApi();

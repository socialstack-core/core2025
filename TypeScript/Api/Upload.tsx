/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes, VersionedContent} from 'Api/ApiEndpoints'
import {User, UserIncludes} from './User'

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

export class UploadApi extends AutoApi<Upload, UploadIncludes>{
    public constructor(){
        super('v1/upload')
    }

}

export default new UploadApi();

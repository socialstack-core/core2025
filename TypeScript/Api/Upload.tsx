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
export class UploadIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'CreatorUser')
    }

}

/*
  Meta for uploaded files.
*/
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

/*
  The post body when uploading a file.
*/
export type FileUploadBody = {
    isPrivate?: boolean,
}

/*
  List of changes when replacing media refs
*/
export type MediaRef = {
    type?: string,
    id?: number,
    name?: string,
    description?: string,
    field?: string,
    url?: string,
    existingRef?: string,
    updatedRef?: string,
    status?: string,
    localeId?: number,
}

/**
    Auto generated API for Upload
    Handles file upload endpoints.
*/
export class UploadApi extends AutoApi<Upload, UploadIncludes>{
    /**
      This extends the AutoApi class, which provides CRUD functionality, any methods seen in are from custom endpoints in the controller

    */
    public constructor(){
        super('v1/upload')
    }

    /**
      Upload a file with efficient support for huge ones.

    */
    public upload(body: FileUploadBody): Promise<void> {
        return getJson(`${this.apiUrl}/create`, { body })
    }

    /**
      Uploads a transcoded file. The body of the client request is expected to be a tar of the files, using a directory called "output" at its root.

    */
    public transcodedTar(id: number, token: string): Promise<void> {
        return getJson(`${this.apiUrl}/transcoded/${id}?token=${token}`)
    }

    /**
      List any active media items

    */
    public active(includes: string): Promise<void> {
        return getJson(`${this.apiUrl}/active?includes=${includes}`)
    }

    /**
      List any active media refs

    */
    public activePost(includes: string): Promise<void> {
        return getJson(`${this.apiUrl}/active?includes=${includes}`)
    }

    /**
      Performs a file consistency check, where it will make sure each identified ref file matches the current upload policy.
            In the future this will also add any missing database entries.
      @param {regenBefore} - Api.AvailableEndpoints.XmlDocMember
      @param {idRange} - Api.AvailableEndpoints.XmlDocMember

    */
    public fileConsistency(regenBefore: string, idRange: string): Promise<void> {
        return getJson(`${this.apiUrl}/file-consistency?regenBefore=${regenBefore}&idRange=${idRange}`)
    }

    /**
      Replace any existing refs with new ones

    */
    public replace(sourceRef: string, targetRef: string): Promise<ApiList<MediaRef>> {
        return getJson(`${this.apiUrl}/replace?sourceRef=${sourceRef}&targetRef=${targetRef}`)
    }

    /**
      Update alt names based on image data

    */
    public updateAlts(): Promise<void> {
        return getJson(`${this.apiUrl}/update-alts`)
    }

    /**
      Upgrade refs such that any ref fields hold the latest version of a specified ref.

    */
    public updateRefs(update: boolean): Promise<ApiList<MediaRef>> {
        return getJson(`${this.apiUrl}/update-refs?update=${update}`)
    }

    /**
      Preview any media refs changes

    */
    public preview(uploadRef: string): Promise<ApiList<MediaRef>> {
        return getJson(`${this.apiUrl}/replace/preview?uploadRef=${uploadRef}`)
    }

}

export default new UploadApi();

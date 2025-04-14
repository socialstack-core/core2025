/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {UploadIncludes, UserIncludes} from './Includes'
import {User} from 'Api/User'
import {getJson, ApiList} from 'UI/Functions/WebRequest'

// Module
/*
  Meta for uploaded files.
*/
export type Upload = VersionedContent & {
    originalName?: string,
    fileType?: string,
    variants?: string,
    blurhash?: string,
    width?: int,
    height?: int,
    focalX?: int,
    focalY?: int,
    alt?: string,
    author?: string,
    usageCount?: int,
    isImage: boolean,
    isPrivate: boolean,
    isVideo: boolean,
    isAudio: boolean,
    transcodeState: int,
    subdirectory?: string,
    temporaryPath?: string,
    ref?: FileRef,
    creatorUser: User,
}

/*
  A context constructed primarily from a cookie value. 
            Uses other locale hints such as Accept-Lang when the user doesn't specifically have one set in the cookie.
*/
export type Context = {
    roleId: uint,
    role?: Role,
    siteDomainId: uint,
    localeId: uint,
    userId: uint,
    user?: User,
    ignorePermissions: boolean,
    permitEditedUtcChange: boolean,
}

/*
  The post body when uploading a file.
*/
export type FileUploadBody = {
    isPrivate: boolean,
}

/*
  List of changes when replacing media refs
*/
export type MediaRef = {
    type?: string,
    id: uint,
    name?: string,
    description?: string,
    field?: string,
    url?: string,
    existingRef?: string,
    updatedRef?: string,
    status?: string,
    localeId: uint,
}

/**
    Auto generated API for Upload
    Handles file upload endpoints.
*/
export class UploadApi extends AutoApi<Upload, UploadIncludes>{
    /**
      This extends the AutoApi class, which provides CRUD functionality, any methods seen in are from custom endpoints in the controller

    */
    public constructor (){
        super('upload')
        this.includes = new UploadIncludes();
    }

    /**
      Upload a file with efficient support for huge ones.

    */
    public upload = (context: SessionResponse, body: FileUploadBody): Promise<Upload>  => {
        return getJson(this.apiUrl + '/create', body )
    }

    /**
      Uploads a transcoded file. The body of the client request is expected to be a tar of the files, using a directory called "output" at its root.

    */
    public transcodedTar = (httpContext: HttpContext, context: SessionResponse, id: uint, token: string): Promise<void>  => {
        return getJson(this.apiUrl + '/transcoded/' + id + '?token=' + token + '')
    }

    /**
      List any active media items

    */
    public active = (context: SessionResponse): Promise<ApiList<Upload>>  => {
        return getJson(this.apiUrl + '/active')
    }

    /**
      List any active media refs

    */
    public activePost = (context: SessionResponse): Promise<void>  => {
        return getJson(this.apiUrl + '/active')
    }

    /**
      Performs a file consistency check, where it will make sure each identified ref file matches the current upload policy.
            In the future this will also add any missing database entries.
      @param {regenBefore} - Api.AvailableEndpoints.XmlDocMember
      @param {idRange} - Api.AvailableEndpoints.XmlDocMember

    */
    public fileConsistency = (context: SessionResponse, regenBefore: string, idRange: string): Promise<void>  => {
        return getJson(this.apiUrl + '/file-consistency?regenBefore=' + regenBefore + '&idRange=' + idRange + '')
    }

    /**
      Replace any existing refs with new ones

    */
    public replace = (context: SessionResponse, sourceRef: string, targetRef: string): Promise<ApiList<MediaRef>>  => {
        return getJson(this.apiUrl + '/replace?sourceRef=' + sourceRef + '&targetRef=' + targetRef + '')
    }

    /**
      Update alt names based on image data

    */
    public updateAlts = (context: SessionResponse): Promise<void>  => {
        return getJson(this.apiUrl + '/update-alts')
    }

    /**
      Upgrade refs such that any ref fields hold the latest version of a specified ref.

    */
    public updateRefs = (context: SessionResponse, update: boolean): Promise<ApiList<MediaRef>>  => {
        return getJson(this.apiUrl + '/update-refs?update=' + update + '')
    }

    /**
      Preview any media refs changes

    */
    public preview = (context: SessionResponse, uploadRef: string): Promise<ApiList<MediaRef>>  => {
        return getJson(this.apiUrl + '/replace/preview?uploadRef=' + uploadRef + '')
    }

}

export default new UploadApi();

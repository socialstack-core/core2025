/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getList, getOne, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { UploadIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Upload} (Api.Uploader.Upload)
**/
export type Upload = VersionedContent<uint> & {
    temporaryPath?: string;
    ref?: string;
    originalName?: string;
    fileType?: string;
    variants?: string;
    blurhash?: string;
    width?: int;
    height?: int;
    focalX?: int;
    focalY?: int;
    alt?: string;
    author?: string;
    usageCount?: int;
    isImage?: boolean;
    isPrivate?: boolean;
    isVideo?: boolean;
    isAudio?: boolean;
    transcodeState?: int;
    subdirectory?: string;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}

/**
* This type was generated to reflect {MediaRef} (Api.Uploader.MediaRef)
**/
export type MediaRef = {
    type?: string;
    id?: uint;
    name?: string;
    description?: string;
    field?: string;
    url?: string;
    existingRef?: string;
    updatedRef?: string;
    status?: string;
    localeId?: uint;
}
// ENTITY CONTROLLER

export class UploadApi extends AutoController<Upload,uint>{

    constructor(){
        super('/v1/upload');
        this.includes = new UploadIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Uploader.UploadController}::{Upload}
     * @url 'v1/upload/create'
     */
    upload = (): Promise<Upload> => {
        return getOne<Upload>(this.apiUrl + '/create');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Uploader.UploadController}::{TranscodedTar}
     * @url 'v1/upload/transcoded/' + id + '?token=' + token + ''
     */
    transcodedTar = (id: uint, token: string): Promise<void> => {
        return getJson<void>(this.apiUrl + '/transcoded/' + id + '?token=' + token + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Uploader.UploadController}::{Active}
     * @url 'v1/upload/active'
     */
    active = (): Promise<ApiList<Upload>> => {
        return getList<Upload>(this.apiUrl + '/active');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Uploader.UploadController}::{ActivePost}
     * @url 'v1/upload/active'
     */
    activePost = (): Promise<void> => {
        return getJson<void>(this.apiUrl + '/active');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Uploader.UploadController}::{FileConsistency}
     * @url 'v1/upload/file-consistency?regenBefore=' + regenBefore + '&idRange=' + idRange + ''
     */
    fileConsistency = (regenBefore?: string, idRange?: string): Promise<void> => {
        return getJson<void>(this.apiUrl + '/file-consistency?regenBefore=' + regenBefore + '&idRange=' + idRange + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Uploader.UploadController}::{Replace}
     * @url 'v1/upload/replace?sourceRef=' + sourceRef + '&targetRef=' + targetRef + ''
     */
    replace = (sourceRef: string, targetRef: string): Promise<MediaRef[]> => {
        return getJson<MediaRef[]>(this.apiUrl + '/replace?sourceRef=' + sourceRef + '&targetRef=' + targetRef + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Uploader.UploadController}::{UpdateAlts}
     * @url 'v1/upload/update-alts'
     */
    updateAlts = (): Promise<void> => {
        return getJson<void>(this.apiUrl + '/update-alts');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Uploader.UploadController}::{UpdateRefs}
     * @url 'v1/upload/update-refs?update=' + update + ''
     */
    updateRefs = (update: boolean): Promise<MediaRef[]> => {
        return getJson<MediaRef[]>(this.apiUrl + '/update-refs?update=' + update + '');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Uploader.UploadController}::{Preview}
     * @url 'v1/upload/replace/preview?uploadRef=' + uploadRef + ''
     */
    preview = (uploadRef: string): Promise<MediaRef[]> => {
        return getJson<MediaRef[]>(this.apiUrl + '/replace/preview?uploadRef=' + uploadRef + '');
    }

}

export default new UploadApi();

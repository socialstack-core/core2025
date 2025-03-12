/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, VersionedContent} from 'TypeScript/Api/ApiEndpoints'

// Module
export type Upload = VersionedContent<number> & {
    originalName?: string,
    fileType?: string,
    variants?: string,
    blurhash?: string,
    alt?: string,
    author?: string,
    isImage?: boolean,
    isPrivate?: boolean,
    isVideo?: boolean,
    isAudio?: boolean,
    transcodeState?: number,
    subdirectory?: string,
}

export class UploadApi extends AutoApi<Upload>{
    public constructor(){
        super('v1/upload')
    }

}



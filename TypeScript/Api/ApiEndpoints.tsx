/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import webRequest, {ApiSuccess, ApiFailure} from 'UI/Functions/WebRequest'

// Module
export type Content<ID> = {
    id?: ID,
}

export type VersionedContent<T> = UserCreatedContent<T> & {
    revision?: number,
    isDraft?: boolean,
    revisionId?: T,
}

export type UserCreatedContent<T> = Content<T> & {
    userId?: number,
    createdUtc?: Date,
    editedUtc?: Date,
}

export class AutoApi<EntityType extends VersionedContent<number>>{
    protected apiUrl: string;

    public constructor(apiUrl: string){
        this.apiUrl = apiUrl;
    }

    public list(where: Partial<Record<keyof(EntityType), string | number | boolean>> = {}, includes: string[] = []): Promise<ApiSuccess<EntityType[]> | ApiFailure> {
        return webRequest(this.apiUrl + '/list', { where }, { method: 'POST', includes })
    }

    public load(id: number): Promise<ApiSuccess<EntityType> | ApiFailure> {
        return webRequest(this.apiUrl + '/' + id)
    }

    public create(entity: EntityType): Promise<ApiSuccess<EntityType> | ApiFailure> {
        return webRequest(this.apiUrl, entity)
    }

    public update(entity: EntityType): Promise<ApiSuccess<EntityType> | ApiFailure> {
        return webRequest(this.apiUrl + '/' + entity.id, entity)
    }

    public delete(entityId: number): Promise<ApiSuccess<EntityType> | ApiFailure> {
        return webRequest(this.apiUrl + '/' + entityId, {} , { method: 'DELETE', includes: [] })
    }

}



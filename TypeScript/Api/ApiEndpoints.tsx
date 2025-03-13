/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {getOne, getList, ApiList} from 'UI/Functions/WebRequest'
import {Content, VersionedContent, UserCreatedContent} from 'Api/Content'
import {getOne, getList, ApiList} from 'UI/Functions/WebRequest'
import {Content, VersionedContent, UserCreatedContent} from 'Api/Content'

// Module
/**
*/
export class ApiIncludes{
    /**

    */
    public constructor(prev?: string, extra?: string){
        this.text = (prev ? prev + '.' : '') + (extra || '');
    }

    protected text: string;

    /**

    */
    public toString(): string {
        return this.text;
    }

}

/**
*/
export class AutoApi<EntityType extends VersionedContent, IncludeSet extends ApiIncludes>{
    protected apiUrl: string;

    /**

    */
    public constructor(apiUrl: string){
        this.apiUrl = apiUrl;
    }

    /**

    */
    public list(where: Partial<Record<keyof(EntityType), string | number | boolean>> = {}, includes: IncludeSet[] = []): Promise<ApiList<EntityType>> {
        return getList(this.apiUrl + '/list', { where }, { method: 'POST', includes: includes.map(include => include.toString()) })
    }

    /**

    */
    public load(id: number, includes: IncludeSet[] = []): Promise<EntityType> {
        return getOne(this.apiUrl + '/' + id, { includes: includes.map(include => include.toString()) })
    }

    /**

    */
    public create(entity: EntityType): Promise<EntityType> {
        return getOne(this.apiUrl, entity)
    }

    /**

    */
    public update(entity: EntityType): Promise<EntityType> {
        return getOne(this.apiUrl + '/' + entity.id, entity)
    }

    /**

    */
    public delete(entityId: number, includes: IncludeSet[] = []): Promise<EntityType> {
        return getOne(this.apiUrl + '/' + entityId, {} , { method: 'DELETE', includes: includes.map(include => include.toString()) })
    }

}

export class ApiIncludes{
    public constructor(prev?: string, extra?: string){
        this.text = (prev ? prev + '.' : '') + (extra || '');
    }

    protected text: string;

    public toString(): string {
        return this.text;
    }

}

export class AutoApi<EntityType extends VersionedContent, IncludeSet extends ApiIncludes>{
    protected apiUrl: string;

    public constructor(apiUrl: string){
        this.apiUrl = apiUrl;
    }

    public list(where: Partial<Record<keyof(EntityType), string | number | boolean>> = {}, includes: IncludeSet[] = []): Promise<ApiList<EntityType>> {
        return getList(this.apiUrl + '/list', { where }, { method: 'POST', includes: includes.map(include => include.toString()) })
    }

    public load(id: number, includes: IncludeSet[] = []): Promise<EntityType> {
        return getOne(this.apiUrl + '/' + id, { includes: includes.map(include => include.toString()) })
    }

    public create(entity: EntityType): Promise<EntityType> {
        return getOne(this.apiUrl, entity)
    }

    public update(entity: EntityType): Promise<EntityType> {
        return getOne(this.apiUrl + '/' + entity.id, entity)
    }

    public delete(entityId: number, includes: IncludeSet[] = []): Promise<EntityType> {
        return getOne(this.apiUrl + '/' + entityId, {} , { method: 'DELETE', includes: includes.map(include => include.toString()) })
    }

}



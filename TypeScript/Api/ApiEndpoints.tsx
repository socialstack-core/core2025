/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {ApiIncludes} from './Includes'
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText, ApiList} from 'UI/Functions/WebRequest'

// Module
/*
*/
export type ListFilter = {
    pageSize?: int,
    pageIndex?: int,
    query?: string,
    sort?: FilterSortConfig,
    includeTotal?: boolean,
    args?: Object[],
}

/**
*/
export class AutoController<T, ID>{
    public apiUrl: string;

    public includes?: ApiIncludes;

    /**

    */
    public constructor (apiUrl: string){
        this.apiUrl = apiUrl
    }

    /**
      GET /v1/entityTypeName/revision/2/
            Returns the data for 1 entity revision.

    */
    public loadRevision = (id: ID, includes: ApiIncludes[] = []): Promise<T>  => {
        return getOne<T>(this.apiUrl + '/revision/' + id + ''+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''))
    }

    /**
      DELETE /v1/entityTypeName/revision/2/
            Deletes an entity

    */
    public deleteRevision = (id: ID, includes: ApiIncludes[] = []): Promise<T>  => {
        return getOne<T>(this.apiUrl + '/revision/' + id + ''+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''))
    }

    /**
      POST /v1/entityTypeName/revision/list
            Lists filtered entity revisions available to this user.
            See the filter documentation for more details on what you can request here.

    */
    public revisionList = (filters: ListFilter, includes: ApiIncludes[] = []): Promise<ApiList<T>>  => {
        return getList<T>(this.apiUrl + '/revision/list'+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''), filters)
    }

    /**
      POST /v1/entityTypeName/revision/1
            Updates an entity revision with the given RevisionId.

    */
    public updateRevision = (id: ID, body: Partial<T>, includes: ApiIncludes[] = []): Promise<T>  => {
        return getOne<T>(this.apiUrl + '/revision/' + id + ''+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''), body)
    }

    /**
      POST /v1/entityTypeName/publish/1
            Publishes the given posted object as an extension to the given revision (if body is not null).

    */
    public publishRevision = (id: ID, body: Partial<T>, includes: ApiIncludes[] = []): Promise<T>  => {
        return getOne<T>(this.apiUrl + '/publish/' + id + ''+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''), body)
    }

    /**
      POST /v1/entityTypeName/draft/
            Creates a draft.

    */
    public createDraft = (body: Partial<T>, includes: ApiIncludes[] = []): Promise<T>  => {
        return getOne<T>(this.apiUrl + '/draft'+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''), body)
    }

    /**
      GET /v1/entityTypeName/2/
            Returns the data for 1 entity.

    */
    public load = (id: ID, includes: ApiIncludes[] = []): Promise<T>  => {
        return getOne<T>(this.apiUrl + '/' + id + ''+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''))
    }

    /**
      DELETE /v1/entityTypeName/2/
            Deletes an entity

    */
    public delete = (id: ID, includes: ApiIncludes[] = []): Promise<T>  => {
        return getOne<T>(this.apiUrl + '/' + id + ''+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''))
    }

    /**
      GET /v1/entityTypeName/cache/invalidate/{id}
            Repopulates the referenced item for this service (if it is cached, and if you are an admin).

    */
    public invalidateCachedItem = (id: ID): Promise<string>  => {
        return getText(this.apiUrl + '/cache/invalidate/' + id + '')
    }

    /**
      GET /v1/entityTypeName/cache/invalidate
            Repopulates the cache for this service (if it is cached, and if you are an admin).

    */
    public invalidateCache = (): Promise<string>  => {
        return getText(this.apiUrl + '/cache/invalidate')
    }

    /**
      GET /v1/entityTypeName/list
            Lists all entities of this type available to this user.

    */
    public listAll = (includes: ApiIncludes[] = []): Promise<ApiList<T>>  => {
        return getList<T>(this.apiUrl + '/list'+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''))
    }

    /**
      POST /v1/entityTypeName/list
            Lists filtered entities available to this user.
            See the filter documentation for more details on what you can request here.

    */
    public list = (filters: ListFilter, includes: ApiIncludes[] = []): Promise<ApiList<T>>  => {
        return getList<T>(this.apiUrl + '/list'+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''), filters)
    }

    /**
      POST /v1/entityTypeName/
            Creates a new entity. Returns the ID. Includes everything by default.

    */
    public create = (body: Partial<T>, includes: ApiIncludes[] = []): Promise<T>  => {
        return getOne<T>(this.apiUrl+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''), body)
    }

    /**
      POST /v1/entityTypeName/1/
            Updates an entity with the given ID. Includes everything by default.

    */
    public update = (id: ID, body: Partial<T>, includes: ApiIncludes[] = []): Promise<T>  => {
        return getOne<T>(this.apiUrl + '/' + id + ''+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : ''), body)
    }

    /**
      /// PUT /v1/entityTypeName/list.pot
            Update translations for this content type.

    */
    public listPOTUpdate = (): Promise<any>  => {
        return getText(this.apiUrl + '/list.pot')
    }

    /**
      POST /v1/entityTypeName/list.pot
            Lists filtered entities available to this user.
            See the filter documentation for more details on what you can request here.
      @param {httpContext} - Api.AvailableEndpoints.XmlDocMember
      @param {context} - Api.AvailableEndpoints.XmlDocMember
      @param {filters} - Api.AvailableEndpoints.XmlDocMember
      @param {includes} - Api.AvailableEndpoints.XmlDocMember
      @param {ignoreFields} - Api.AvailableEndpoints.XmlDocMember

    */
    public listPOT = (filters: Record<string, string | number | boolean>, ignoreFields: string, includes: string): Promise<string>  => {
        return getText(this.apiUrl + '/list.pot?includes=' + includes + '&ignoreFields=' + ignoreFields + '', filters)
    }

}

/*
*/
export type FilterSortConfig = {
    field?: string,
    direction?: string,
}



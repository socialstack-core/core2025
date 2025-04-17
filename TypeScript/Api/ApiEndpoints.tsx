/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {ApiIncludes} from './Includes'
import {getOne, getList, getJson, ApiList} from 'UI/Functions/WebRequest'

// Module
/*
*/
export type ListFilter = {
    pageSize: int,
    pageIndex: int,
    query?: string,
    sort?: FilterSortConfig,
    includeTotal?: boolean,
    args?: Record<string, string | number | boolean>[],
}

/**
*/
export class AutoController<T, ID, ApiIncludes>{
    public apiUrl: string;

    public includes?: ApiIncludes = undefined;

    /**

    */
    public constructor (apiUrl: string){
        this.apiUrl = apiUrl
    }

    /**
      GET /v1/entityTypeName/revision/2/
            Returns the data for 1 entity revision.

    */
    public loadRevision = (id: ID): Promise<T>  => {
        return getOne(this.apiUrl + '/revision/' + id + '', {}, { method: 'GET' })
    }

    /**
      DELETE /v1/entityTypeName/revision/2/
            Deletes an entity

    */
    public deleteRevision = (id: ID): Promise<T>  => {
        return getOne(this.apiUrl + '/revision/' + id + '', {}, { method: 'DELETE' })
    }

    /**
      POST /v1/entityTypeName/revision/list
            Lists filtered entity revisions available to this user.
            See the filter documentation for more details on what you can request here.

    */
    public revisionList = (filters: ListFilter): Promise<ApiList<T>>  => {
        return getList(this.apiUrl + '/revision/list', filters, { method: 'POST' })
    }

    /**
      POST /v1/entityTypeName/revision/1
            Updates an entity revision with the given RevisionId.

    */
    public updateRevision = (id: ID, body: Record<string, string | number | boolean>): Promise<T>  => {
        return getOne(this.apiUrl + '/revision/' + id + '', body, { method: 'POST' })
    }

    /**
      POST /v1/entityTypeName/publish/1
            Publishes the given posted object as an extension to the given revision (if body is not null).

    */
    public publishRevision = (id: ID, body: Record<string, string | number | boolean>): Promise<T>  => {
        return getOne(this.apiUrl + '/publish/' + id + '', body, { method: 'POST' })
    }

    /**
      POST /v1/entityTypeName/draft/
            Creates a draft.

    */
    public createDraft = (body: Record<string, string | number | boolean>): Promise<T>  => {
        return getOne(this.apiUrl + '/draft', body, { method: 'POST' })
    }

    /**
      GET /v1/entityTypeName/2/
            Returns the data for 1 entity.

    */
    public load = (id: ID): Promise<T>  => {
        return getOne(this.apiUrl + '/' + id + '', {}, { method: 'GET' })
    }

    /**
      DELETE /v1/entityTypeName/2/
            Deletes an entity

    */
    public delete = (id: ID): Promise<T>  => {
        return getOne(this.apiUrl + '/' + id + '', {}, { method: 'DELETE' })
    }

    /**
      GET /v1/entityTypeName/list
            Lists all entities of this type available to this user.

    */
    public listAll = (): Promise<ApiList<T>>  => {
        return getList(this.apiUrl + '/list', {}, { method: 'GET' })
    }

    /**
      POST /v1/entityTypeName/list
            Lists filtered entities available to this user.
            See the filter documentation for more details on what you can request here.

    */
    public list = (filters: ListFilter): Promise<ApiList<T>>  => {
        return getList(this.apiUrl + '/list', filters, { method: 'POST' })
    }

    /**
      POST /v1/entityTypeName/
            Creates a new entity. Returns the ID. Includes everything by default.

    */
    public create = (body: Record<string, string | number | boolean>): Promise<T>  => {
        return getOne(this.apiUrl, body, { method: 'POST' })
    }

    /**
      POST /v1/entityTypeName/1/
            Updates an entity with the given ID. Includes everything by default.

    */
    public update = (id: ID, body: Record<string, string | number | boolean>): Promise<T>  => {
        return getOne(this.apiUrl + '/' + id + '', body, { method: 'POST' })
    }

    /**
      /// PUT /v1/entityTypeName/list.pot
            Update translations for this content type.

    */
    public listPOTUpdate = (): Promise<Object>  => {
        return getOne(this.apiUrl + '/list.pot', {}, { method: 'PUT' })
    }

}

/*
*/
export type FilterSortConfig = {
    hasValue: boolean,
    value: FilterSortConfig,
}



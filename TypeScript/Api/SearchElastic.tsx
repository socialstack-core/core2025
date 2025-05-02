/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {DocumentsResult} from './DocumentsResult'
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/*
*/
export type AggregationStructure = {
    aggregationOverrides?: AggregationOverride[],
}

/*
*/
export type ClusterHealthResponse = {
    activePrimaryShards: int,
    activeShards: int,
    activeShardsPercentAsNumber: double,
    clusterName?: string,
    delayedUnassignedShards: int,
    indices?: Record<IndexName, IndexHealthStats>,
    initializingShards: int,
    numberOfDataNodes: int,
    numberOfInFlightFetch: int,
    numberOfNodes: int,
    numberOfPendingTasks: int,
    relocatingShards: int,
    status: Health,
    taskMaxWaitTimeInQueueInMilliseconds: long,
    timedOut: boolean,
    unassignedShards: int,
}

/*
*/
export type CatShardsRecord = {
    completionSize?: string,
    docs?: string,
    fielddataEvictions?: string,
    fielddataMemorySize?: string,
    filterCacheMemorySize?: string,
    flushTotal?: string,
    flushTotalTime?: string,
    getCurrent?: string,
    getExistsTime?: string,
    getExistsTotal?: string,
    getMissingTime?: string,
    getMissingTotal?: string,
    getTime?: string,
    getTotal?: string,
    id?: string,
    idCacheMemorySize?: string,
    index?: string,
    indexingDeleteCurrent?: string,
    indexingDeleteTime?: string,
    indexingDeleteTotal?: string,
    indexingIndexCurrent?: string,
    indexingIndexTime?: string,
    indexingIndexTotal?: string,
    ip?: string,
    mergesCurrent?: string,
    mergesCurrentDocs?: string,
    mergesCurrentSize?: string,
    mergesTotalDocs?: string,
    mergesTotalSize?: string,
    mergesTotalTime?: string,
    node?: string,
    percolateCurrent?: string,
    percolateMemorySize?: string,
    percolateQueries?: string,
    percolateTime?: string,
    percolateTotal?: string,
    primaryOrReplica?: string,
    refreshTime?: string,
    refreshTotal?: string,
    searchFetchCurrent?: string,
    searchFetchTime?: string,
    searchFetchTotal?: string,
    searchOpenContexts?: string,
    searchQueryCurrent?: string,
    searchQueryTime?: string,
    searchQueryTotal?: string,
    segmentsCount?: string,
    segmentsFixedBitsetMemory?: string,
    segmentsIndexWriterMaxMemory?: string,
    segmentsIndexWriterMemory?: string,
    segmentsMemory?: string,
    segmentsVersionMapMemory?: string,
    shard?: string,
    state?: string,
    store?: string,
    warmerCurrent?: string,
    warmerTotal?: string,
    warmerTotalTime?: string,
}

/**
*/
export class SearchElasticApi{
    public apiUrl: string = 'sitesearch';

    /**
      Exposes the site search

    */
    public query = (filters: Record<string, string | number | boolean>): Promise<DocumentsResult>  => {
        return getJson<DocumentsResult>(this.apiUrl + '/query', filters)
    }

    /**
      Exposes the taxonomy values based on categories

    */
    public taxonomy = (filters: Record<string, string | number | boolean>): Promise<AggregationStructure>  => {
        return getJson<AggregationStructure>(this.apiUrl + '/taxonomy', filters)
    }

    /**
      Reset the indexer

    */
    public reset = (): Promise<Boolean>  => {
        return getJson<boolean>(this.apiUrl + '/reset')
    }

    /**
      Reset a single index (delete all documents)

    */
    public resetIndex = (indexName: string): Promise<Boolean>  => {
        return getJson<boolean>(this.apiUrl + '/reset/index/' + indexName + '')
    }

    /**
      Reset a single index (delete all documents)

    */
    public deleteIndex = (indexName: string): Promise<Boolean>  => {
        return getJson<boolean>(this.apiUrl + '/delete/index/' + indexName + '')
    }

    /**
      Exposes the current status of the elastic store

    */
    public health = (): Promise<ClusterHealthResponse>  => {
        return getJson<ClusterHealthResponse>(this.apiUrl + '/health')
    }

    /**
      Exposes details of the current shards in the elastic store

    */
    public shards = (): Promise<CatShardsRecord[]>  => {
        return getJson<CatShardsRecord>(this.apiUrl + '/shards')
    }

}

/*
*/
export type AggregationOverride = {
    name?: string,
    label?: string,
    groups?: Group[],
    buckets?: Bucket[],
}

/*
*/
export type Health = {
    value__?: int,
}

export default new SearchElasticApi();

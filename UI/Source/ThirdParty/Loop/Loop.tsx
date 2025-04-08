import { ApiList } from 'UI/Functions/WebRequest';
import Failure from 'UI/Failed';
import Paginator from 'UI/Paginator';
import { AutoApi, ApiIncludes } from 'Api/ApiEndpoints';
import { Content } from 'Api/Content';
import useApi from 'UI/Functions/UseApi';
import { useEffect, useState } from 'react';
const DEFAULT_PAGE_SIZE = 50;

export interface LoopPageConfig {
	top?: boolean,

	bottom?:boolean,

	showInput?:boolean,
	noScroll?:boolean,

	maxLinks?:number,

	pageSize?: number,
}

export interface Filter<T> {
	where: Partial<Record<keyof (T), string | number | boolean>>
}

/**
 * Props for the Loop component.
 */
export interface LoopProps<T extends Content, I extends ApiIncludes> {

	/**
	 * Loop will pull data from the specified api. Alternatively if you need something other than an 
	 * api source, use source instead.
	 */
	over?: AutoApi<T, I>;

	/**
	 * An alternative to 'over', where you can specify a custom data source function.
	 * @param filter
	 * @param includes
	 * @returns
	 */
	source?: (filter?: Filter<T>, includes?: I[]) => Promise<ApiList<T>>;

	includes?: I[];

	filter?: any;

	paged?: LoopPageConfig | boolean;

	/**
	 * Custom failure handler.
	 * @param e
	 * @returns
	 */
	onFailure?: (e: PublicError) => React.ReactNode;

	/**
	 * Custom loader.
	 * @returns
	 */
	loader?: () => React.ReactNode;
	
	/**
	 * Custom message when no results are found.
	 * @returns
	 */
	orNone?: () => React.ReactNode;

	/**
	 * Child render function.
	 * @param item The item itself.
	 * @param index Current iteration index.
	 * @param fragmentCount The number of results in the current array. Not the same as the total number of results.
	 * @returns
	 */
	children: (item: T, index: number, fragmentCount: number) => React.ReactNode;

	/**
	 * Optional custom class name.
	 */
	className?: string

	/**
	 * Starting page index. The first page is assumed if not specified.
	 */
	defaultPage?: number,

	/**
	 * An optional function which can manipulate the results set before it is iterated.
	 * @param results
	 * @param listObj
	 * @returns
	 */
	onResults?: (results: T[], listObj: ApiList<T>) => T[]

	/**
	 * True to reverse the result set before iterating over it. 
	 * Note that this happens after onResults is invoked.
	 */
	reverse?: boolean,

}

/**
 * This component repeatedly renders its child using either an explicit array of data or an endpoint.
 */
const Loop = <T extends Content, I extends ApiIncludes>(props: LoopProps<T, I>) => {

	const [pageIndex, setPageIndex] = useState(props.defaultPage || 1);
	const [totalResults, setTotalResults] = useState(0);
	const [errored, setErrored] = useState<PublicError | null>(null);
	const [results, setResults] = useApi<T[] | null>(() => load(), [props.filter, props.paged]);
	
	const getPagedFilter = (filter: any, pageIndex: number, paged?: LoopPageConfig | boolean) => {
		if (!paged) {
			return filter;
		}

		var pgCfg = getPageConfig(paged);

		if (!filter) {
			filter = {};
		}

		filter = { ...filter };
		filter.pageIndex = pageIndex - 1;
		filter.includeTotal = true;
		var pageSize = pgCfg.pageSize || DEFAULT_PAGE_SIZE;

		if (!filter.pageSize) {
			filter.pageSize = pageSize;
		}

		return filter;
	};
	
	const load = (newPageIndex? : number) => {
		setErrored(null);

		if(newPageIndex){
			setPageIndex(newPageIndex);
		}

		var pgIndex = newPageIndex || pageIndex;
		var filter = getPagedFilter(props.filter, pgIndex, props.paged);

		var source : Promise<ApiList<T>> | null = null;

		if (props.over) {
			source = props.over.list(filter, props.includes);
		} else if (props.source) {
			source = props.source(filter, props.includes);
		}

		if (!source) {
			return Promise.reject({
				type: 'loop/error',
				message: `No source`
			} as PublicError);
		}

		return source
		.then(list => {
			var results = list.results;

			if (props.onResults) {
				results = props.onResults(results, list);
			}

			if (props.reverse) {
				results = results.reverse();
			}

			setTotalResults(list.totalResults);
			return results;
		})
		.catch(e => {
			console.log('Loop caught an error:');
			console.error(e);

			if (e?.message) {
				setErrored(e as PublicError);
			} else {
				setErrored({
					type: 'loop/error',
					message: `An error occurred`,
					detail: e
				} as PublicError);
			}

			return null;
		});
	}
	
	if (errored) {
		// is a specific failure set?
		if (props.onFailure) {
			return props.onFailure(errored);
		}

		return <Failure />;
	}

	if (!results) {
		// Loading
		if (props.loader) {
			return props.loader();
		}

		return null;
	}

	var renderFunc = props.children;

	if (!results.length) {
		return props.orNone ? props.orNone() : null;
	}

	var className = 'loop ';
	if (props.className) {
		className += props.className;
	}

	var content = results.map((item, i) => renderFunc(item, i, results.length));
	
	if(!props.paged){
		return content;
	}

	var pageCfg = getPageConfig(props.paged);
	var pageSize = pageCfg.pageSize || DEFAULT_PAGE_SIZE;
	var showInput = pageCfg.showInput !== undefined ? pageCfg.showInput : undefined;
	var maxLinks = pageCfg.maxLinks || undefined;
	var noScroll = pageCfg.noScroll || false;

	if(typeof pageCfg == "number"){
		pageSize = pageCfg;
	}
	
	// if filter contains pagesize use that
	if (props.filter?.pageSize) {
		pageSize = props.filter.pageSize;
	}

	var paginator = <Paginator
		pageSize={pageSize}
		showInput={showInput}
		maxLinks={maxLinks}
		pageIndex={pageIndex}
		totalResults={totalResults}
		onChange={pageIndex => {
			load(pageIndex);
			if (!noScroll) {
				window.scrollTo(0, 0);
			}
		}}
	/>;

	var result = [];
		
	if(pageCfg.top){
		result.push(paginator);
	}
	
	result.push(content);
	
	if(pageCfg.bottom !== false){
		// Bottom is true unless it's explicitly false
		result.push(paginator);
	}
	
	return result;
}

function getPageConfig(pgCfg: LoopPageConfig | true) {
	if (pgCfg === true) {
		return {} as LoopPageConfig;
	}

	return pgCfg as LoopPageConfig;
}

export default Loop;
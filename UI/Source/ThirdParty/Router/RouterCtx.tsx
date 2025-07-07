import { PageStateResult } from 'Api/Page';
import { createContext, useContext } from 'react';

export interface PageState extends PageStateResult {
	url: string;
	query: URLSearchParams;
}

export interface RouterContext {
	setPage: (url: string) => void;
	changeQuery: (query: URLSearchParams) => void;
	updateQuery: (query: URLSearchParams) => void;
	pageState: PageState;
	canGoBack: () => boolean;
}

const routerCtx = createContext<RouterContext>({
	setPage: (url: string) => { },
	canGoBack: () => false,
	pageState: {},
	changeQuery: (query: URLSearchParams) => {},
	updateQuery: (query: URLSearchParams) => {},
} as RouterContext);

export { routerCtx };

export function useRouter() {
	// returns {page, setPage}
	return useContext(routerCtx);
};

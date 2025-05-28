import { PageStateResult } from 'Api/Page';
import { createContext, useContext } from 'react';

export interface PageState extends PageStateResult {
	url: string;
	query: URLSearchParams;
}

export interface RouterContext {
	setPage: (url: string) => void;
	pageState: PageState;
	canGoBack: () => boolean;
}

const routerCtx = createContext<RouterContext>({
	setPage: (url: string) => { },
	canGoBack: () => false,
	pageState: {}
} as RouterContext);

export { routerCtx };

export function useRouter() {
	// returns {page, setPage}
	return useContext(routerCtx);
};

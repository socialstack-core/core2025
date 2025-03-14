import userApi from 'Api/User';

/**
 * Logs out and then navigates to the specified URL, or the homepage if one is not specified.
 * Provide setSession from UI/Session and setPage from UI/Router.
 */
export default (url: string, setSession: (s: SessionResponse) => Session, setPage : (url: string) => void) => {
	if(!setSession || !setPage){
		throw new Error('Logout requires ctx');
	}
	
	return userApi.logout()
		.then(response => clearAndNav(url || '/', setSession, setPage, response))
		.catch(e => clearAndNav(url || '/', setSession, setPage));
};

/**
 * Clears the local session and navigates to the stated page.
 * @param url
 * @param setSession
 * @param setPage
 * @param ctx
 */
function clearAndNav(url: string, setSession: (s: SessionResponse) => Session, setPage: (url: string) => void, ctx?: SessionResponse) {
	if (ctx) {
		setSession(ctx);
	} else {
		setSession({});
	}
	setPage(url);
}

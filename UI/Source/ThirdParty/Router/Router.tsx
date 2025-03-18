import webRequest, { expandIncludes } from 'UI/Functions/WebRequest';
import Canvas from 'UI/Canvas';
import getBuildDate from 'UI/Functions/GetBuildDate';
import AdminTrigger from 'UI/AdminTrigger';
import { createContext, useContext, useRef, useState, useEffect } from 'react';

interface RouterContext {
	setPage: (url: string) => void;
}

const routerCtx = createContext < RouterContext | null > (null);

export { routerCtx };

export function useRouter() {
	// returns {page, setPage}
	return useContext(routerCtx);
};

const { config, location } = window;
const routerCfg = config && config.pageRouter || {};
const { hash, localRouter } = routerCfg;

function currentUrl(){
	return hash ? location.hash?.substring(1) ?? "/" : `${location.pathname}${location.search}`;
}

const initialUrl = currentUrl();

// Initial event:

var pgStateHold = document.getElementById('pgState');
const initState = localRouter ? localRouter(initialUrl, webRequest) : (pgStateHold ? JSON.parse(pgStateHold.innerHTML) : {});

if(!initState.loading){
	triggerEvent(initState.page);
}

function triggerEvent(pgInfo) {
	if(pgInfo){
		var e;
		if(typeof(Event) === 'function') {
			e = new Event('xpagechange');
		}else{
			e = document.createEvent('Event');
			e.initEvent('xpagechange', true, true);
		}
		e.pageInfo = pgInfo;
		window.dispatchEvent(e);
	}
}

function historyLength(){
	return window && window.history ? window.history.length : 0;
}

var initLength = historyLength();

function canGoBack(){
	return historyLength() > initLength;
}

export default (props) => {
	var [pageState, setPage] = useState({url: initialUrl, ...initState});
	var [scrollTarget, setScrollTarget] = useState();
	const scrollTimer = useRef(null);
	
	if(pageState.loading && !pageState.handled){
		pageState.loading.then(pgState => {
			triggerEvent(pgState.page);
			setPage(pgState);
		});
		pageState.handled = true;
	}
	
	function go(url) {
		if(window.beforePageLoad){
			window.beforePageLoad(url).then(() => {
				window.beforePageLoad = null;
				goNow(url);
			}).catch(e => console.log(e));
		}else{
			goNow(url);
		}
	}
	
	function goNow(url) {
		if(useDefaultNav(hash ? '' : document.location.pathname, url)){
			document.location = url;
			return;
		}
		
		// Store the scroll position:
		var html = document.body.parentNode as HTMLHtmlElement;
		window.history.replaceState({
			scrollTop: html.scrollTop,
			scrollLeft: html.scrollLeft
		}, '');
		
		// Push nav event:
		window.history.pushState({
			scrollTop: 0
		}, '', hash ? '#' + url : url);
		
		return setPageState(url).then(() => {
			html.scrollTo({top: 0, left: 0, behavior: 'instant'});
		});
	}
	
	function useDefaultNav(a,b){
		if(b.indexOf(':') != -1 || b[0]=='#' || (b[0] == '/' && (b.length>1 && b[1] == '/'))){
			return true;
		}
		
		var isOnExternPage = a.indexOf('/en-admin') == 0 || a.indexOf('/v1') == 0;
		var targetIsExternPage = b[0] == '/' ? (b.indexOf('/en-admin') == 0 || b.indexOf('/v1') == 0) : isOnExternPage;
		
		return isOnExternPage != targetIsExternPage;
	}
	 
	function setPageState(url : string) {
		if(localRouter){
			var pgState = localRouter(url, webRequest);
			pgState.url = url;
			
			if(pgState.loading){
				pgState.loading.then(pgState => {
					setPage(pgState);
					triggerEvent(pgState);
				});
			}else{
				setPage(pgState);
				triggerEvent(pgState);
			}
			
			return Promise.resolve(true);
		}else{
			return webRequest("page/state", {
				url,
				version: getBuildDate().timestamp
			}).then(res => {
				if (res.json.oldVersion) {
					console.log("UI updated - forced reload");
					document.location = url;
					return;
				} else if (res.json.redirect) {
					// Bounce:
					console.log("Redirect");
					document.location = res.json.redirect;
					return;
				}
				
				var {config} = res.json;
				
				if(config){
					delete res.json.config;
					window.__cfg = config;
				}
				
				var pgState = {url, ...res.json};
				setPage(pgState);
				triggerEvent(res.json);
			});
		}
	}
	
	const onPopState = (e) => {
		var newScrollTarget = null;
		
		if(e && e.state && e.state.scrollTop !== undefined){
			newScrollTarget = {
				x: e.state.scrollLeft,
				y: e.state.scrollTop
			};
		}
		
		setPageState(currentUrl()).then(() => {
			if(newScrollTarget){
				setScrollTarget(newScrollTarget);
			} else {
				setScrollTarget(null);
			}
		});
	}
	
    const onLinkClick = (e) => {
        if (e.button != 0 || e.defaultPrevented) {
            // Browser default action for right/ middle clicks
            return;
        }
        var cur = e.target;
        while (cur && cur !== document) {
            if (cur.nodeName === 'A') {
                var href = cur.getAttribute('href'); // cur.href is absolute
                if (cur.getAttribute('target') || cur.getAttribute('download')) {
                    return;
                }

                if (href && href.length) {
                    if (href.includes('#')) {
                        const [pathname, hash] = href.split('#');
                        if (pathname === '' || pathname === window.location.pathname) {
                            e.preventDefault();
                            const targetElement = document.getElementById(hash);
                            if (targetElement) {
                                targetElement.scrollIntoView({ behavior: 'smooth' });
                                history.pushState(null, '', `${window.location.pathname}#${hash}`);
                            }
                            return;
                        }
                    } else {
                        if (useDefaultNav(document.location.pathname, href)) {
                            return;
                        }
                        e.preventDefault();
                        go(href);
                        return;
                    }
                }
            }
            cur = cur.parentNode;
        }
    };

	const scroll = () => {
		if (scrollTarget) {
			var html = document.body.parentNode as HTMLHtmlElement;
			html.scrollTo({ top: scrollTarget.y, left: scrollTarget.x, behavior: 'instant' });
		}
	}

	useEffect(() => {
		if (scrollTimer.current) {
			clearInterval(scrollTimer.current);
			scrollTimer.current = null;
		}

		if(scrollTarget){
			scrollTimer.current = setTimeout(scroll, 100);
		}
	}, [scrollTarget]);
	
	useEffect(() => {
		
		const onContentChange = (e : CustomEvent) => {
			var {po} = pageState;
			var detail = e.detail as ContentChangeDetail;
			if(po && po.type == detail.type && po.id == detail.entity.id){
				var pgState = {...pageState, po: detail.entity};
				setPage(pgState);
			}
		};
		
		const onWsMessage = (e) => {
			var {po} = pageState;
			if(po && po.type == e.message.type && po.id == e.message.entity.id){
				var pgState = {...pageState, po: e.message.entity};
				setPage(pgState);
			}
		};
		
		window.addEventListener("popstate", onPopState);
		document.addEventListener("click", onLinkClick);
		document.addEventListener("contentchange", onContentChange);
		document.addEventListener("websocketmessage", onWsMessage);
		
		return () => {
			if (scrollTimer.current) {
				clearInterval(scrollTimer.current);
			}

			window.removeEventListener("popstate", onPopState);
			document.removeEventListener("click", onLinkClick);
			document.removeEventListener("contentchange", onContentChange);
			document.removeEventListener("websocketmessage", onWsMessage);
		};
	}, []);
	
	var { page } = pageState;
	
	useEffect(() => {
		if (pageState && pageState.title) {
			// The page state title includes resolved tokens
			document.title = pageState.title;
		}

		if (pageState && pageState.description) {
			// The page state description includes resolved tokens
			document.querySelector('meta[name="description"]')?.setAttribute("content", pageState.description);
		}
	});
	
	return <routerCtx.Provider
			value={{
				canGoBack,
				pageState,
				setPage: go
			}}
		>
		{
			page ? (typeof page.bodyJson == 'string' ? <Canvas>{page.bodyJson}</Canvas> : <Canvas bodyJson={ page.bodyJson } />) : null
		}
		<AdminTrigger page={page}/>
	</routerCtx.Provider>;
}

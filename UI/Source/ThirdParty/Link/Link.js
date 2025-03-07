import { useSession } from 'UI/Session';
import { useState, useEffect } from 'react';
import { useTokens } from 'UI/Token';

export default function Link({ children, text, href, _rte, doNotLower, hreflang, ...attribs}) {
	const [initialRender, setInitialRender] = useState(true);

	useEffect(() => {
		setInitialRender(false);
	}, []);

	const { session, setSession } = useSession();
	
	attribs.alt = attribs.alt || attribs.title;
	attribs.ref = attribs.rootRef;
	
	var children = children || text;
	var url = useTokens(href);

	function isExternalUrl(string) {

		// SSR or initial client render detected - treat as an external link (don't change case)
		if (initialRender || window.SERVER) {
			return true;
		}

		var r = new RegExp('^(?:[a-z+]+:)?//', 'i');
		var isExternal = r.test(string);

		if (isExternal) {
			// check domain
			var url = new URL(string);
			isExternal = url.origin != window.origin;
		}
		
		return isExternal;
	}

	if (url) {
		// if url contains :// it must be as-is (which happens anyway).
		if (url[0] == '/') {
			if (url.length > 1 && url[1] == '/') {
				// as-is
			} else {
				var prefix = window.urlPrefix || '';

				if (prefix) {
					if (prefix[0] != '/') {
						// must return an absolute url
						prefix = '/' + prefix;
					}

					if (prefix[prefix.length - 1] == "/") {
						url = url.substring(1);
					}

					let disablePrefix = "/" + session.locale.code.toLowerCase() == prefix && session.locale.isRedirected && session.locale.permanentRedirect;

					if (!disablePrefix) {
						url = prefix + url;
					}
				}
			}
		}

		// ensure internal links are lowercase unless doNotLower is passed to this component
		if (!doNotLower && !isExternalUrl(url) && !url.includes("/content/")) {
			url = url.toLowerCase();
		}

		if (url != "/") {

			// strip any trailing slashes
			while (url.endsWith("/")) {
				url = url.slice(0, -1);
			}

		}

	}

	return <a href={url} {...attribs}>
		{children}
	</a>;
}

/*
Link.editable = true;

Link.propTypes = {
	title: 'string',
	href: 'string',
	children: 'jsx'
};
*/
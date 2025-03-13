import { useSession } from 'UI/Session';
import { useTokens } from 'UI/Token';


/**
 * Props for the link component.
 */
interface LinkProps extends React.HTMLAttributes<HTMLAnchorElement> {
	/**
	 * The href for the link itself.
	 */
	href: string
}

const Link: React.FC<React.PropsWithChildren<LinkProps>> = ({
	children,
	href,
	...attribs
}) => {
	const { session } = useSession();
	const { locale } = session;

	var children = children;
	var url = useTokens(href);

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

					let disablePrefix = "/" + locale?.code?.toLowerCase() == prefix && locale?.isRedirected && locale?.permanentRedirect;

					if (!disablePrefix) {
						url = prefix + url;
					}
				}
			}
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

export default Link;

/*
Link.editable = true;

Link.propTypes = {
	title: 'string',
	href: 'string',
	children: 'jsx'
};
*/
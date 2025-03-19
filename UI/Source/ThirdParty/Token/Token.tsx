import { useSession } from 'UI/Session';
import { useRouter, PageState } from 'UI/Router';

var modes : Record<string, boolean> = {
	'content': true,
	'context': true,
	'session': true,
	'url': true,
	'customdata': true,
	'primary': true,
	'theme': true
};

interface TokenOptions {
	/**
	 * Optional content object (can be anything).
	 */
	content?: any;
}

/**
 * Replaces any ${tokens} in the given token string, returning the resolved string.
 * @param {any} str
 * @param {any} opts
 * @returns
 */
export function useTokens(str : string, opts? : TokenOptions) {
	var { session } = useSession();
	var { pageState } = useRouter();
	return handleString(str, session, opts?.content, pageState, opts);
}

export function handleString(str: string, session : Session, localContent? : any, pageState? : PageState, opts? : TokenOptions) {
	return (str || '').toString().replace(/\$\{(\w|\.)+\}/g, function (textToken) {
		var fields = textToken.substring(2, textToken.length - 1).split('.');

		var mode = '';
		var first = fields[0].toLowerCase();
		if (modes[first]) {
			fields.shift();
			mode = first;
		}

		return resolveValue(mode, fields, session, localContent, pageState, opts);
	});
}

export function resolveValue(
	mode: string,
	fields: string[],
	session: Session,
	localContent?: any,
	pageState?: PageState,
	opts?: TokenOptions
): string {
	var token;

	if (mode) {
		mode = mode.toLowerCase();
	}

	if (mode == "content" || mode == "context") {
		token = localContent;
	} else if (mode == "url") {
		if (!pageState || !pageState.tokenNames) {
			return '';
		}
		var index = pageState.tokenNames.indexOf(fields.join('.'));
		return (index == null || index == -1) ? '' : (pageState.tokens ? pageState.tokens[index] : '');
	} else if (mode == "theme") {
		return 'var(--' + fields.join('-') + ')';
	} else if (mode == "customdata" || mode == "primary") {
		// Used by emails mostly. Passes through via primary object.
		if (!pageState || !pageState.po) {
			return '';
		}
		token = pageState.po;
	} else {
		token = session;
	}

	if (!token) {
		return '';
	}

	var fields = fields;

	if (Array.isArray(fields) && fields.length) {
		try {
			for (var i = 0; i < fields.length; i++) {
				token = token[fields[i]];
				if (token === undefined || token === null) {
					return '';
				}
			}
		} catch (e) {
			console.log(e);
			token = null;
		}

		return token;
	} else if (typeof fields == 'string') {
		return token[fields];
	}

	return '';
}

export interface TokenProps extends TokenOptions {
	mode?: string;
	s?: string;
	fields: string[];
}

/**
* Contextual token. 
* Available values either come from the primary type on the page, or the global state.
*/
const Token : React.FC<React.PropsWithChildren<TokenProps>> = (props) => {
	// If editor, display the thing and its children:
	var { session } = useSession();
	var { pageState } = useRouter();

	if (!props.mode) {
		// Resolve from child string if there is one.
		var str = props.s || (props.children as string);
	
		if(Array.isArray(str)){
			str = str.length ? str[0] : null;
		}
		
		if (typeof str == 'string') {
			return handleString(str.indexOf('$') == -1 ? '${' + str + '}' : str, session, null, pageState, props);
		}

		return '{Incorrect token, see wiki}';
	}

	// Resolved value. No wrapper - just plain value.
	return resolveValue(props.mode, props.fields, session, null, pageState, props);
}

export default Token;
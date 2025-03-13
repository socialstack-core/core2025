declare global {
	/**
	 * Represents a socialstack file ref as a string.
	 * Use this type whenever you need to indicate that a string is specifically a ref.
	 */
	type FileRef = string;

	/**
	 * An integer
	 */
	type int = number & { __type: "int" };

	/**
	 * A floating point number (double)
	 */
	type float = number;

	/**
	 * Convertible to a date with isoConvert from DateTools.
	 */
	type Dateish = number | Date | string;

	interface PublicError {
		/**
		 * Textual message to display.
		 */
		message: string,

		/**
		 * Error type.
		 */
		type: string
	}
	
	/**
	* Public configuration objects from the API.
	*/
	interface Config {
		
	}

	/**
	 * Internal site configuration.
	 */
	var __cfg: Record<string, Config>;

	/**
	 * URL where static content originates from.
	 */
	var staticContentSource: string;

	/**
	 * URL where upload content originates from.
	 */
	var contentSource: string;

	/**
	 * Optionally prefix all links with this.
	 */
	var urlPrefix?: string;

	/**
	 * Global Session Init data. Originates from the server serialising the user's context.
	 */
	var gsInit: SessionResponse | undefined;
	
	/**
	 * True if this is executing on the serverside. Only use if absolutely necessary 
	 * as it will cause odd behaviour (mainly React not hydrating correctly) with the SSR if this is used too much.
	 * Most of the time you should do client specific things in useEffect instead 
	 * as a useEffect does not run on the server and is designed by react to be able to manipulate the DOM after rendering.
	 */
	var SERVER: boolean;

	/**
	 * A base URL for the API.
	 */
	var apiHost: string;

	/**
	 * Optional token which will be used as a Token header by webRequest.
	 */
	var storedToken: string | null;

	/**
	 * Require modules by their alias, available at runtime.
	 * Returns the raw UMD module meaning e.g. a default export is available as .default
	 */
	var require: (moduleName: string) => any;
}

export {};
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
}

export {};
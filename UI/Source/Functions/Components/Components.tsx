/**
 * Generates an array of class names based on the props provided.
 *
 * @param props - object containing boolean values for responsive size keys (xs, sm, md, lg, xl)
 * @param prefix - string prefix to be used in the generated class names (e.g. "form__field")
 * @returns array of class names for each enabled size (e.g. "form__field--sm")
 */
const getSizeClasses = (prefix: string, props: Record<string, boolean>): string[] => {
	const sizes: string[] = ['xs', 'sm', 'md', 'lg', 'xl'];
	return sizes.filter(size => props[size]).map(size => `${prefix}--${size}`);
};

/**
 * 
 * NB: UNTESTED replacement for the likes of:
 * 
  		const colorProps: React.InputHTMLAttributes<HTMLInputElement> =
			Object.fromEntries(
				Object.entries(rest).filter(([key]) =>
					key in document.createElement("input")
				)
			) as React.InputHTMLAttributes<HTMLInputElement>;
 * 
 * Filters given list of props to only those valid for use with the given HTML element type
 *
 * @param elementType - HTML element type to check against
 * @param props - list of props to be filtered
 * @returns filtered list of valid props suitable for use with the given element
 */
const getValidProps = <T extends HTMLElement>(elementType: keyof JSX.IntrinsicElements, rest: Record<string, unknown>) => {
	const element = document.createElement(elementType) as T;
	return Object.fromEntries(
		Object.entries(rest).filter(([key]) => key in element)
	) as React.HTMLProps<T>;
};

export {
	getSizeClasses,
	getValidProps
};

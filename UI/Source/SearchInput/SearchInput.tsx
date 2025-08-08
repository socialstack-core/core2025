import { useEffect, useRef, useState } from "react";
import { useRouter } from "UI/Router";
import Debounce from "UI/Functions/Debounce";

/**
 * Props for the SearchInput component.
 */
interface SearchInputProps {
	/** 
	 * optional placeholder text for search field
	 */
	searchPlaceholder?: string,

	/**
	 * Name of the URL query field to get/ set the input to. 
	 * Read from the query state to get the value of this bar.
	 */
	queryField?: string
}

/**
 * Used for a searchbar which optionally updates the query string. 
 * Unlike UI/Search this is just the input itself - it does not perform any requests.
 * @param props React props.
 */
const SearchInput: React.FC<SearchInputProps> = (props) => {
	const { searchPlaceholder } = props;
	const { pageState, updateQuery } = useRouter();
	const queryField = props.queryField || 'q';

	const updateQueryRef = useRef(updateQuery);
	const debounce = useRef(
		new Debounce(
			(query: string) => {
				updateQueryRef.current({ [queryField]: query });
			}
		)
	);

	updateQueryRef.current = updateQuery;

	let searchClasses = ["form-control ui-form-control"];

	if (props.className) {
		searchClasses.push(props.className);
	}

	return (
		<input type="search" className={searchClasses.join(' ')} placeholder={searchPlaceholder} defaultValue={
			pageState.query.has(queryField) ? pageState.query.get(queryField)! : ''
		} onInput={(ev) => {
			var qs = (ev.target as HTMLInputElement).value;
			debounce.current.handle(qs);
		}} />
	);
}

export default SearchInput;
import {useEffect, useRef, useState} from "react";
import Loop from "UI/Loop";
import productCategoryApi from 'Api/ProductCategory';
import Link from "UI/Link";
import RecentSearches from "UI/Header/RecentSearches";
import {useRouter} from "UI/Router";
import Debounce from "UI/Functions/Debounce";

/**
 * Props for the Search component.
 */
interface SearchProps {
	/** 
	 * optional placeholder text for search field
	 */
	searchPlaceholder?: string,
}

const highlightMatch = (text: string, query: string) => {
	const regex = new RegExp(`(${query})`, 'gi');
	const parts = text.split(regex);
	return parts.map((part, index) =>
		part.toLowerCase() === query.toLowerCase() ? (
			<b key={index}>{part}</b>
		) : (
			<span key={index}>{part}</span>
		)
	);
};

/**
 * The website Search React component.
 * @param props React props.
 */
const Search: React.FC<SearchProps> = ({ searchPlaceholder, ...props }) => {

	const { setPage, pageState, updateQuery } = useRouter();
	const [query, setQuery] = useState(() => pageState.query?.get('q'));
	
	if (!searchPlaceholder || !searchPlaceholder.length) {
		searchPlaceholder = `Search by name, category or code`
	}

	const updateQueryRef = useRef(updateQuery);
	const debounce = useRef(
		new Debounce(
			(query: string) => {
				updateQueryRef.current({q: query});
			}
		)
	);

	useEffect(() => {
		updateQueryRef.current = updateQuery;
	}, [updateQuery]);

	return (
		<div className="site-nav__search">
			<button type="button" className="btn site-nav__search-trigger" popoverTarget="search_popover">
				<svg viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
					<path d="M14.3 12.58h-.91l-.32-.31a7.43 7.43 0 10-.8.8l.3.32v.9L18.3 20l1.7-1.7-5.7-5.72zm-6.87 0A5.14 5.14 0 117.42 2.3a5.14 5.14 0 01.01 10.28z" fill="currentColor" />
				</svg>
				{/*
				<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 20 19">
					<path d="M15.4 14.27a8.71 8.71 0 10-1.12 1.12l3.37 3.38a.81.81 0 001.12 0c.32-.3.32-.8 0-1.12l-3.36-3.37zm-6.68 1.56a7.14 7.14 0 010-14.25 7.14 7.14 0 010 14.25z" fill="#B50E7C" />
				</svg>
				*/}
			</button>
			<div className="site-nav__search-wrapper" popover="auto" id="search_popover">
				<input type="search" placeholder={searchPlaceholder} defaultValue={pageState.query.has('q') ? pageState.query.get('q')! : ''} onInput={(ev) => {
					var qs = (ev.target as HTMLInputElement).value;
					debounce.current.handle(qs);
					setQuery(qs);
				}} />
			</div>
			<div className="site-nav__search-dropdown">
				{(!query || query.length == 0) && (
					<RecentSearches />
				)}
				{query && query.length != 0 && (
					// this may need updating further
					// but poses as a base result set
					// when searching for a category
					<div className={'search-listing'}>
						<Loop
							over={productCategoryApi}
							filter={{
								query: "name contains ?",
								args: [query],
								pageIndex: 1 as uint,
								pageSize: 10 as uint
							}}
						>
							{(category) => {
								return (
									<Link
										href={"/category/" + category.slug}
									>
										<li className={'search-listing-category'}>
											<i className={'fas fa-tag'}/>
											<span>{highlightMatch(category.name, query)}</span>
										</li>
									</Link>
								)
							}}
						</Loop>
					</div>
				)}
			</div>
		</div>
	);
}

export default Search;

declare global {
    interface WindowEventMap {
        'search': CustomEvent<{ query: string }>;
    }
}
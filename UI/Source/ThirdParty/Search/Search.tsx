import { useState, useEffect, useRef } from 'react';
import { ApiIncludes } from 'Api/Includes';
import { ListFilter, Content } from 'Api/Content'
import { ApiList } from 'UI/Functions/WebRequest';

export type SearchProps<T extends Content<uint>> = {
    startHidden?: boolean;
    value?: string;
    minLength?: number;
    exclude?: uint[];
    includes?: ApiIncludes;
    field?: string;
    limit?: number;
    onResults?: (results: T[]) => void;
    onQuery?: (filter: ListFilter, query: string) => void;
    onFind?: (result: T) => void;
    placeholder?: string;
    searchText?: string;
    name?: string;
    className?: string;
    'data-theme'?: string;
    endpoint?: (filter?: ListFilter, includes?: ApiIncludes) => Promise<ApiList<T>>;
};

type NoFieldWhereQuery = {
    name: string
}

/**
 * Used to search for things.
 */
const Search = <T extends Content<uint>,>(props: SearchProps<T>) => {

    const { onFind, exclude } = props;

    const [loading, setLoading] = useState<boolean>(false);
    const [hidden, setHidden] = useState<boolean>(Boolean(props.startHidden));
    const [results, setResults] = useState<T[] | null>(null); // Typed results as T[]
    const [selected, setSelected] = useState<T | null>(null);

    const inputRef = useRef<HTMLInputElement | null>(null);

    // Function to fetch search results from endpoint
    const fetchResults = (query: string) => {
        if (props.minLength && query.length < props.minLength) {
            setResults(null); // Clear results if query is too short
            return;
        }

        setLoading(true); // Set loading to true while fetching
        
        const { field } = props;

        var filter : ListFilter = {
            query: field + " contains ?",
            args: [query]
        };

        const { includes } = props;

        props.onQuery && props.onQuery(filter, query)

        props.endpoint && props.endpoint(filter, includes)
            .then((fetchedResults) => {

                var res = fetchedResults.results;

                if (exclude) {
                    res = res.filter(r => !exclude.find(id => id == r.id));
                }

                setResults(res);
                setLoading(false); // Set loading to false after fetching
            })
            .catch((error) => {
                console.error('Error fetching results:', error);
                setLoading(false); // Set loading to false on error
            });
    };

    useEffect(() => {
        if (results) {
            props.onResults && props.onResults(results);
        }
    }, [results, props])

    const selectValue = (value: T) => {
        onFind && onFind(value);
        setSelected(value);
    };

    return (
        <div className={`search ${props.className}`} data-theme={props['data-theme'] || 'search-theme'}>
            <input
                ref={inputRef}
                onBlur={() => setResults(null)} // Clear results on blur
                autoComplete="false"
                className="form-control"
                defaultValue={props.searchText}
                value={props.searchText}
                placeholder={props.placeholder || 'Search...'}
                type="text"
                onKeyUp={(e) => {
                    fetchResults((e.target as HTMLInputElement).value); 
                }}
                onFocus={(e) => {
                    if (e.target.value.length > 0) {
                        fetchResults((e.target as HTMLInputElement).value); // Trigger fetch on focus if text is present
                    }
                }}
                onKeyDown={(e) => {
                    if (e.keyCode === 13 && results && results.length === 1) {
                        selectValue(results[0]); // Select the first result if it's a match
                        e.preventDefault();
                    }
                    if (e.keyCode === 27) {
                        setResults(null); // Clear results on escape
                    }
                }}
            />
            {results && !props.onResults && (
                <div className="suggestions">
                    {results.length ? (
                        results.map((result, i) => (
                            <button
                                type="button"
                                key={i}
                                onMouseDown={() => selectValue(result)}
                                className="btn suggestion"
                            >
                                {/* Customize the display of the result here */}
                                {result && (result as any).name}
                            </button>
                        ))
                    ) : (
                        <div className="no-results">No results found</div>
                    )}
                </div>
            )}
            {props.name && <input type="hidden" value={(selected ? selected.id : '') || props.value} name={props.name} />}
        </div>
    );
};

export default Search;

import { Product } from "Api/Product";
import { useEffect, useState } from "react";
import RecentSearchProductItem from "./ProductItem";
import RecentSearchApi from "Api/RecentSearch";
import {RecentSearchIncludes} from "Api/Includes";
import {useSession} from "UI/Session";


const getRecentSearches = async (session: Session): Promise<Product[]> => {
    if (!session.user) {
        if (localStorage.getItem('recent_searches')) {
            return (JSON.parse(localStorage.getItem('recent_searches') ?? '[]') ?? []) as Product[];
        }
        else {
            localStorage.setItem('recent_searches', JSON.stringify([]));
            return [];
        }
    } else {
        const results = await RecentSearchApi.list({
            query: 'userId = ?',
            args: [
                session.user.id!
            ],
            pageIndex: 1 as int,
            pageSize: 5 as int,
        }, [
            new RecentSearchIncludes().product
        ]);
        
        return results.results
                      .filter((search) => Boolean(search))
                      .map(search => search.product!);
    }
}

/**
 * React component that displays a list of recently searched products in a horizontal scrollable list.
 *
 * The recent searches are intended to provide quick access to previously viewed or queried products.
 * Currently, the data is statically generated using `generateDummyData()` and rendered using the
 * `RecentSearchProductItem` component.
 *
 * ### Features:
 * - Renders a scrollable horizontal list of product items.
 * - Displays product name and image using the child component.
 * - Stubbed data generation to be replaced by actual search history logic.
 *
 * ### Future Enhancements:
 * - Replace `generateDummyData()` with actual user search history from a backend or localStorage.
 * - Add click tracking or navigation logic when items are interacted with.
 * - Limit number of stored searches and add deduplication logic.
 *
 * @component
 * @example
 * ```tsx
 * <RecentSearches />
 * ```
 *
 * @returns {React.ReactElement} The rendered list of recent product searches.
 */
const RecentSearches: React.FC<{}> = (props): React.ReactElement | null => {
    /**
     * State variable holding the list of recently searched products.
     * This will be populated with real user history in a later iteration.
     */
    const [recentSearches, setRecentSearches] = useState<Product[]>();
    
    const { session } = useSession();

    useEffect(() => {
        if (!recentSearches) {
            getRecentSearches(session).then((products: Product[]) => {
                setRecentSearches(products);
            })
        }
    }, [recentSearches]);
    
    if (!recentSearches || recentSearches?.length == 0) {
        return null;
    }
    return (
        <div className={'recent-searches'}>
            <div className={'panel-header'}>
                {`Recent searches`}
            </div>
            <div className={'panel-body'}>
                <ul className={'recent-searches-product-list'}>
                    {recentSearches.map(product => (
                        <RecentSearchProductItem product={product} key={product.id} />
                    ))}
                </ul>
            </div>
        </div>
    );
};

export default RecentSearches;

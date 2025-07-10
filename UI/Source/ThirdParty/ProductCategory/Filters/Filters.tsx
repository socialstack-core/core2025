import { ProductCategory } from "Api/ProductCategory";
import Button from "UI/Button";
import Input from 'UI/Input';
import Link from "UI/Link";
import { ProductCategoryFacet } from "UI/Product/Search/Facets";
import {useSession} from "UI/Session";
import {useEffect, useMemo, useState} from "react";

/**
 * Props for the List component.
 */
interface FiltersProps {
	/**
	 * The categories to list. Must have included 'primaryUrl'.
	 */
	content?: ProductCategoryFacet[]
}

/**
 * The Filters React component.
 * @param props React props.
 */
const Filters: React.FC<FiltersProps> = (props) => {
	
	// currently unused [v]
	const { session } = useSession();
	
	const [filter, setFilter] = useState<string>('');
	const [maxCategoriesVisible, setMaxCategoriesVisible] = useState<number>(3);

	if (!props.content) {
		return null;
	}

	const content = uniqueFacetCategories(props.content);

	const filteredContent = useMemo(() => {
		return content
			.filter(
				facet => (
					!filter || facet.category.name.toLowerCase().includes(filter.toLowerCase())
				) && facet.category.primaryUrl
			) ;
	}, [filter, content]);

	// currently unused [v]
	// const { locale } = session;
	
	// TODO: need an individual filter flag for each category;
	//       currently 'include' just triggers the checkbox rendering, but knows nothing about state
	return (
		<div className="ui-productcategory-filters">
			<Input 
				type="search" 
				placeholder={`Search for a category`} 
				noWrapper 
				onInput={(ev) => setFilter((ev.target as HTMLInputElement).value)}
			/>
			{filteredContent.map((facet, idx) => {
				if (idx > maxCategoriesVisible - 1) {
					return;
				}

				const { category } = facet;

				return (
					<Link key={'link-' + category.id} href={'/category/' + category.slug}>
						{category.name} ({ facet.count ?? 0 })
					</Link>
				)
			})}
			{filteredContent.length > maxCategoriesVisible && (
				<Button
					type="button"
					onClick={() =>
						setMaxCategoriesVisible(prev =>
							Math.min(prev + 5, filteredContent.length)
						)
					}
				>
					<span>{`Show 5 more (${filteredContent.length - maxCategoriesVisible})`}</span>
				</Button>
			)}
		</div>
	);
}

export default Filters;


/**
 * Filters and returns a list of unique categories.
 *
 * @param {ProductCategoryFacet[]} categories - List of attributes
 * @returns {ProductCategoryFacet[]} Unique attributes
 */
const uniqueFacetCategories = (facets: ProductCategoryFacet[]): ProductCategoryFacet[] => {
	const unique: ProductCategoryFacet[] = [];
	
	facets.forEach((facet) => {
		if (unique.find(unq => unq.category.name === facet.category.name)) {
			return;
		}
		unique.push(facet);
	})
	return unique.sort((a, b) => a.category.name!.localeCompare(b.category.name!, undefined, { numeric: true }));;
};
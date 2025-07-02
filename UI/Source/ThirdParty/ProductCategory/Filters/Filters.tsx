import { ProductCategory } from "Api/ProductCategory";
import Button from "UI/Button";
import Input from 'UI/Input';
import Link from "UI/Link";
import { ProductCategoryFacet } from "UI/Product/Search/Facets";
import {useSession} from "UI/Session";
import {useEffect, useState} from "react";

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
	
	const { session } = useSession();
	
	const [filter, setFilter] = useState<string>('');
	const [showMore, setShowMore] = useState(false);

	if (!props.content) {
		return null;
	}

	const content = uniqueFacetCategories(props.content);

	const { locale } = session;
	
	// TODO: need an individual filter flag for each category;
	//       currently 'include' just triggers the checkbox rendering, but knows nothing about state
	return (
		<div className="ui-productcategory-filters">
			<Input type="search" placeholder={`Search for a category`} noWrapper onInput={(ev) => setFilter((ev.target as HTMLInputElement).value)}/>
			{content.map((facet, idx) => {
				if (!filter && idx > 2 && !showMore) {
					return;
				}

				const { category } = facet;

				if (!category.primaryUrl) {
					return;
				}

				if (filter && filter.length != 0) {
					if (!category.name.toLowerCase().includes(filter.toLowerCase())) {
						return;
					}
				}

				// const facet = facets.find(f => f.category.id === category.id);

				return (
					<Link href={'/category/' + category.slug}>
						{category.name} ({ facet.count ?? 0 })
					</Link>
				)
			})}
			{content.length > 2 && !filter && <Button
				type={'button'}
				onClick={() => setShowMore(!showMore)}
			>
				<span>{showMore ? `Show less` : `Show more`}</span>
			</Button>}
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
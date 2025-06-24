import Input from 'UI/Input';
import Button from 'UI/Button';
import { useState } from "react";
import { ProductCategoryFacet, AttributeValueFacet, AttributeFacetGroup } from 'UI/Product/Search/Facets';

/**
 * Props for the FilterList component.
 */
interface SearchProps {
	/**
	 * filter facets array
	 */
	facets: (ProductCategoryFacet | AttributeValueFacet)[],

	/**
	 * max number of filters initially visible
	 */
	maxVisible?: int,

	/**
	 * optional search filter
	 */
	searchFilter?: string,

	/**
	 * set true to remove border and padding
	 */
	noBorder?: boolean
}

const DEFAULT_MAX_VISIBLE_FILTERS = 3;

/**
 * The FilterList React component.
 * @param props React props.
 */
const FilterList: React.FC<SearchProps> = ({ facets, searchFilter, noBorder, ...props }) => {
	const [expanded, setExpanded] = useState(false);
	const maxVisible = props.maxVisible || DEFAULT_MAX_VISIBLE_FILTERS;
	const hasFilter = searchFilter?.length > 0;

	if (!facets || !facets.length) {
		return;
	}

	// check - category or attribute facets?
	const field = facets[0].type === "ProductCategoryFacet" ? 'category' : 'value';
	const subField = facets[0].type === "ProductCategoryFacet" ? 'name' : 'value';

	// filter visible items to searchFilter, if supplied
	const filteredFacets = hasFilter ? facets.filter(facet => {
		return (
			facet[field] && facet[field][subField] &&
			facet[field][subField].toLowerCase().includes(searchFilter.toLowerCase())
		);
	}) : facets;

	const toggleText = expanded ? `Show less` : `Show more (${filteredFacets.length - maxVisible})`;

	let filterListClasses = ["ui-filter-list"];

	if (noBorder) {
		filterListClasses.push("ui-filter-list--no-border");
	}

	return (
		<div className={filterListClasses.join(' ')}>
			{filteredFacets.map((facet, index) => {

				if (!facet[field] || !facet[field][subField]) {
					return null;
				}

				return <Input style={{ display: expanded || index < maxVisible ? "flex" : "none" }} type="checkbox" sm
					label={`${facet[field][subField]} (${facet.count})`} noWrapper />;
			})}
			{filteredFacets.length > maxVisible && <>
				<Button variant="link" onClick={() => setExpanded(!expanded)} className={`ui-filter-list__toggle ${expanded ? 'ui-filter-list__toggle--expanded' : ''}`}>
					<span>
						{toggleText}
					</span>
					<i className="fr fr-chevron-down"></i>
				</Button>
			</>}
		</div>
	);

}

export default FilterList;
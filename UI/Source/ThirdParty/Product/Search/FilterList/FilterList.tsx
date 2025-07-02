import Input from 'UI/Input';
import Button from 'UI/Button';
import { useEffect, useState, useRef } from "react";
import { ProductCategoryFacet, AttributeValueFacet } from 'UI/Product/Search/Facets';

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
	noBorder?: boolean,

	/**
	 * Any particular units to display with the filter.
	 */
	units?: string,

	/**
	 * Do something when the filter changes.
	 */
	onChange: (values: ulong[]) => void
}

const DEFAULT_MAX_VISIBLE_FILTERS = 3;

const DEFAULT_SELECTED: ulong[] = [];

/**
 * The FilterList React component.
 * @param props React props.
 */
const FilterList: React.FC<SearchProps> = ({ facets, searchFilter, noBorder, ...props }) => {
	const [expanded, setExpanded] = useState(false);
	const maxVisible = props.maxVisible || DEFAULT_MAX_VISIBLE_FILTERS;
	const hasFilter = searchFilter?.length > 0;

	const [selectedValues, setSelectedValues] = useState<ulong[]>(DEFAULT_SELECTED);

	const { onChange } = props;

	if (!facets || !facets.length) {
		return null;
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

				return (
					<Input
						key={facet[field].value.id}
						style={{ display: expanded || index < maxVisible ? "flex" : "none" }}
						type="checkbox"
						sm
						label={`${facet[field][subField]}${props.units ?? ''} (${facet.count})`}
						noWrapper
						onChange={(ev) => {
							const target = ev.target as HTMLInputElement;
						

							const newArr = target.checked ?
								[...selectedValues, facet[field].id] :
								selectedValues.filter(id => id !== facet[field].id);

							setSelectedValues(newArr);

							onChange(newArr)
						}}
					/>
				);
			})}
			{filteredFacets.length > maxVisible && (
				<Button
					variant="link"
					onClick={() => setExpanded(!expanded)}
					className={`ui-filter-list__toggle ${expanded ? 'ui-filter-list__toggle--expanded' : ''}`}
				>
					<span>{toggleText}</span>
					<i className="fr fr-chevron-down"></i>
				</Button>
			)}
		</div>
	);
};

export default FilterList;
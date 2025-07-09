import Input from 'UI/Input';
import Button from 'UI/Button';
import { useEffect, useState, useRef } from "react";
import { ProductCategoryFacet, AttributeValueFacet } from 'UI/Product/Search/Facets';
import { ProductAttributeValue } from 'Api/ProductAttributeValue';

/**
 * Props for the FilterList component.
 */
interface SearchProps {
	/**
	 * All current attribute facets (attributes + values + counts of each one)
	 */
	facets: AttributeValueFacet[],

	/**
	 * An array of selected attribute values (red, green, whatever)
	 */
	selectedAttributeValues: ProductAttributeValue[],

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
	setSelectedAttributeValues: (values: ProductAttributeValue[]) => void
}

const DEFAULT_MAX_VISIBLE_FILTERS = 3;

const DEFAULT_SELECTED: ulong[] = [];

/**
 * The FilterList React component.
 * @param props React props.
 */
const FilterList: React.FC<SearchProps> = ({ facets, noBorder, selectedAttributeValues, setSelectedAttributeValues, ...props }) => {
	const [expanded, setExpanded] = useState(false);
	const maxVisible = props.maxVisible || DEFAULT_MAX_VISIBLE_FILTERS;
	const { onChange } = props;

	if (!facets || !facets.length) {
		return null;
	}

	const toggleText = expanded ? `Show less` : `Show more (${facets.length - maxVisible})`;

	let filterListClasses = ["ui-filter-list"];

	if (noBorder) {
		filterListClasses.push("ui-filter-list--no-border");
	}

	return (
		<div className={filterListClasses.join(' ')}>
			{facets.map((attributeFacet, index) => {
				const attributeValue = attributeFacet.value;
				const attributeValueText = attributeValue.value;

				if (!attributeValue || !attributeValueText) {
					return null;
				}

				return (
					<Input
						key={attributeValue.id}
						style={{ display: expanded || index < maxVisible ? "flex" : "none" }}
						type="checkbox"
						sm
						label={`${attributeValueText}${props.units ?? ''} (${attributeFacet.count})`}
						noWrapper
						checked={!!selectedAttributeValues.find(av => av.id == attributeValue.id)}
						onChange={(ev) => {
							const target = ev.target as HTMLInputElement;

							// This checkbox represents a singular *attribute value*.
							// Thus when it's ticked, it's in the array and when it's not ticked we ensure it's excluded.

							if (target.checked) {
								if (selectedAttributeValues.find(av => av.id == attributeValue.id)) {
									// Already in there - ignore
									return;
								}

								// Add
								setSelectedAttributeValues([...selectedAttributeValues, attributeValue]);
							} else {
								// Remove
								setSelectedAttributeValues(selectedAttributeValues.filter(av => av.id != attributeValue.id));
							}

						}}
					/>
				);
			})}
			{facets.length > maxVisible && (
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
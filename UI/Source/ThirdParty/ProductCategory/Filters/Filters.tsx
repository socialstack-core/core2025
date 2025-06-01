import { ProductCategory } from "Api/ProductCategory";
import Input from 'UI/Input';

/**
 * Props for the List component.
 */
interface FiltersProps {
	/**
	 * The categories to list. Must have included 'primaryUrl'.
	 */
	content?: ProductCategory[],
}

/**
 * The Filters React component.
 * @param props React props.
 */
const Filters: React.FC<FiltersProps> = (props) => {
	const { content } = props;

	if (!content) {
		return null;
	}

	// TODO: need an individual filter flag for each category;
	//       currently 'include' just triggers the checkbox rendering, but knows nothing about state
	return (
		<div className="ui-productcategory-filters">
			<Input type="search" placeholder={`Search for a category`} noWrapper />
			{content.map(category => {
				// TODO: filter state required for each category
				let include = true;
				return <>
					<Input type="checkbox" label={category.name} value={include} noWrapper />
				</>;
			})}
		</div>
	);
}

export default Filters;
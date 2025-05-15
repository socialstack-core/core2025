import { Product } from "Api/Product";
import Signpost from 'UI/ProductCategory/Signpost';

/**
 * Props for the List component.
 */
interface ListProps {
	/**
	 * The products to list. Must have included 'primaryUrl'.
	 */
	content?: Product[]
}

/**
 * The List React component.
 * @param props React props.
 */
const List: React.FC<ListProps> = (props) => {

	const { content } = props;

	if (!content) {
		return null;
	}

	return (
		<div className="ui-productcategory-list">
			{
				content.map(category => <Signpost content={category} />)
			}
		</div>
	);
}

export default List;
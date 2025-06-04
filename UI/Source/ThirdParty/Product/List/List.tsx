import { Product } from "Api/Product";
import Signpost from 'UI/Product/Signpost';

/**
 * Props for the List component.
 */
interface ListProps {
	/**
	 * The products to list. Must have included 'primaryUrl'.
	 */
	content?: Product[],

	/** 
	 * determines if products should be shown in list or grid format
	 */
	viewStyle?: string
}

/**
 * The List React component.
 * @param props React props.
 */
const List: React.FC<ListProps> = (props: ListProps) => {
	const { content, viewStyle } = props;

	if (!content) {
		return null;
	}

	let productListClasses = ["ui-product-list"];

	if (viewStyle == 'grid') {
		productListClasses.push("ui-product-list--grid");
	}

	return (
		<ul className={productListClasses.join(' ')}>
			{content.map(product => {
				return <li className="ui-product-list__product">
					<Signpost content={product} />
				</li>;
			})}
		</ul>
	);
}

export default List;
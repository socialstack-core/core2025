import { Product } from 'Api/Product';
import Canvas from 'UI/Canvas';

/**
 * Props for the View component.
 */
interface ViewProps {
	/**
	 * Must have included at least productCategories, firstCategory, firstCategory.categoryBreadcrumb
	 * Usually provided by a graph.
	 */
	product: Product
}

/**
 * The View React component.
 * @param props React props.
 */
const View: React.FC<ViewProps> = (props) => {

	const { product } = props;

	return (
		<div className="ui-product-view">
			<h1>{
				product.name
			}</h1>
			<Canvas>
				{product.descriptionJson}
			</Canvas>
		</div>
	);
}

export default View;
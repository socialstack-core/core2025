import { Product } from 'Api/Product';
import Canvas from 'UI/Canvas';
import ProductQuantity from 'UI/Payments/ProductQuantity';

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
			<ProductQuantity product={product} quantity={1} addText={`Add to basket`} allowMultiple={true} goStraightToCart={true} />
		</div>
	);
}

export default View;
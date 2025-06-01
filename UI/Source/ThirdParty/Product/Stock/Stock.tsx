import { Product } from 'Api/Product';

/**
 * Props for the Stock component.
 */
interface StockProps {
	/**
	 * The content to display
	 */
	product: Product,
}

/**
 * The Stock React component.
 * @param props React props.
 */
const Stock: React.FC<StockProps> = (props) => {
	const { product } = props;

	// TODO: determine when product has options
	let hasOptions = false;
	let options = 12;
	let approved = 4;

	return <>
		<div className="ui-product-view__stock-wrapper">
			{!hasOptions && <>
				<span className="ui-product-view__stock">
					<i className="fr fr-barcode"></i>
					<span>
						{product.sku}
					</span>
					<span>
						&mdash;
					</span>
					{product.stock > 0 && `${product.stock} in stock`}
					{!product.stock && `OUT OF STOCK`}
				</span>
			</>}

			{hasOptions && <>
				<span className="ui-product-view__options">
					{`${options} options - ${approved} approved`}
				</span>
			</>}
		</div>
	</>;
}

export default Stock;
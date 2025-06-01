import { Product } from 'Api/Product';

/**
 * Props for the Price component.
 */
interface PriceProps {
	/**
	 * The content to display
	 */
	product: Product,
}

/**
 * The Price React component.
 * @param props React props.
 */
const Price: React.FC<PriceProps> = (props) => {
	const { product } = props;

	// TODO: retrieve associated price (see priceId)
	// NB: also needs to account for:
	// - global incl / excl VAT setting (see header)
	// - multiple options (set to lowest price so this can be displayed as "From £X.XX")
	let price = 85.02;

	// TODO: optional previous price
	let oldPrice = 96.99;

	// TODO: determine when product has options
	let hasOptions = false;
	let options = 12;
	let approved = 4;

	let GBPound = new Intl.NumberFormat('en-GB', {
		style: 'currency',
		currency: 'GBP',
	});

	return (
		<span className="ui-product-view__price">
			<span className="ui-product-view__price-internal">
				{hasOptions && <span>{`From`}</span>}
				{GBPound.format(price)}
				{oldPrice && <span>{`Was ${GBPound.format(oldPrice)}`}</span>}
			</span>
		</span>
	);
}

export default Price;
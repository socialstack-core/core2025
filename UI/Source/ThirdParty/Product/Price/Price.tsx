import { Product } from 'Api/Product';
import { useSession } from 'UI/Session';
import { formatCurrency, formatPOA } from "UI/Functions/CurrencyTools";
import { useCart } from 'UI/Payments/CartSession';

/**
 * Props for the Price component.
 */
interface PriceProps {
	/**
	 * The content to display
	 */
	product: Product,

	/**
	 * Overriding price to display
	 */
	override?: CurrencyAmount,

	/**
	 * true if this a "from" price (i.e. the product has variants)
	 */
	isFrom?: boolean
}

export interface CurrencyAmount {
	currencyCode: string,
	amount: ulong
}

/**
 * The Price React component.
 * @param props React props.
 */
const Price: React.FC<PriceProps> = (props) => {
	const { product, override, isFrom } = props;
	const { session } = useSession();
	const { lessTax } = useCart();
	const { locale } = session;

	// TODO: retrieve associated price (see priceId)
	// NB: also needs to account for:
	// - global incl / excl VAT setting (see header)
	// - multiple options (set to lowest price so this can be displayed as "From �X.XX")
	let currencyCode: string | undefined;
	let amount: ulong | undefined;
	let hasOptions = false;
	let oldPrice = 0;

	if (override) {
		currencyCode = override.currencyCode;
		amount = override.amount;
		hasOptions = !!isFrom;
	} else {
		// NB: This will be replaced again when per-user pricing and the tax resolver is added
		var tiers = null;
		if(product?.calculatedPrice && locale){
			var calculatedPrice = product.calculatedPrice;
			
			if(calculatedPrice.discountedPrice && calculatedPrice.discountedPrice.length > 0){
				tiers = calculatedPrice.discountedPrice;
				oldPrice = calculatedPrice.listPrice;
			}else if(calculatedPrice.listPrice && calculatedPrice.listPrice.length > 0){
				tiers = calculatedPrice.listPrice;
			}
		}

		if(!tiers){
			currencyCode = locale.currencyCode;

			return (
				<span className="ui-product-price">
					{formatPOA({ currencyCode })}
				</span>
			);
		}

		hasOptions = tiers.length > 1;

		// Get the lowest one:
		var tier = tiers[0];

		if (hasOptions) {
			for (var i = 1; i < tiers.length; i++) {
				var current = tiers[i];

				if (current.amount < tier.amount) {
					tier = current;
				}
			}
		}

		currencyCode = locale.currencyCode;
		amount = lessTax ? tier.amountLessTax : tier.amount;
	}
	
	return (
		<span className="ui-product-price">
			{hasOptions && <span className="ui-product-price--from">{`From`}</span>}
			{formatCurrency(amount, { currencyCode })}
			<span>{lessTax ? `ex VAT` : `inc VAT`}</span>
			{oldPrice ? <span className="ui-product-price--was">{`Was ${formatCurrency(oldPrice, { currencyCode })}`}</span> : null}
		</span>
	);
}

export default Price;
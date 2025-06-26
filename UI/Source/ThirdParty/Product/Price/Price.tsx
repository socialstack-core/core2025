import { Product } from 'Api/Product';
import { useSession } from 'UI/Session';
import { formatCurrency } from "UI/Functions/CurrencyTools";

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
	override?: CurrencyAmount
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
	const { product, override } = props;
	const { session } = useSession();
	const { locale } = session;

	// TODO: retrieve associated price (see priceId)
	// NB: also needs to account for:
	// - global incl / excl VAT setting (see header)
	// - multiple options (set to lowest price so this can be displayed as "From £X.XX")
	let currencyCode: string | undefined;
	let amount: ulong | undefined;
	let hasOptions = false;

	if (override) {
		currencyCode = override.currencyCode;
		amount = override.amount;
	} else if (product?.priceTiers && product?.priceTiers.length && locale) {
		// NB: This will be replaced again when per-user pricing and the tax resolver is added
		var tiers = product.priceTiers;
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
		amount = tier.amount;
	}
	else
	{
		return null;
	}

	// TODO: optional previous price
	let oldPrice = 0;

	return (
		<span className="ui-product-view__price">
			<span className="ui-product-view__price-internal">
				{hasOptions && <span>{`From`}</span>}
				{formatCurrency(amount, { currencyCode })}
				{oldPrice ? <span>{`Was ${formatCurrency(oldPrice, {currencyCode})}`}</span> : null}
			</span>
		</span>
	);
}

export default Price;
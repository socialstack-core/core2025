import { formatCurrency } from "UI/Functions/CurrencyTools";
import Loop from 'UI/Loop';
import Alert from 'UI/Alert';
import Icon from 'UI/Icon';
import productApi from 'Api/Product';
import { recurrenceText } from 'UI/Functions/Payments';
import { useSession } from 'UI/Session';
import BasketItem from 'UI/Product/Signpost';
import Quantity from 'UI/Product/Quantity';
import Button from 'UI/Button';

const STRATEGY_STD = 0;
const STRATEGY_STEP1 = 1;
const STRATEGY_STEPALWAYS = 2;

/**
 * Props for the ProductTable component.
 */
interface ProductTable {
}

/**
 * The ProductTable React component.
 * @param props React props.
 */
const ProductTable: React.FC<ProductTableProps> = (props) => {
	var { shoppingCart, addToCart, readonly, lessTax } = props;
	const { session } = useSession();
	var pricedCart = shoppingCart?.cartContents;
	
	if (!pricedCart || !pricedCart.contents.length) {
		return <Alert type="info">
			{readonly ? <>
				{`This purchase is empty`}
			</> : <>
				{`Your shopping cart is empty.`}
			</>}

		</Alert>;
	}
	
	var itemSet = pricedCart.contents;
	var currencyCode = pricedCart.currencyCode;
	var hasAtLeastOneSubscription = pricedCart.hasSubscriptionProducts;
	
	return <>
		<ul className="shopping-cart__table">

			{itemSet.map(lineInfo => {
				var product = lineInfo.product;
				var qty = lineInfo.quantity;

				var formattedCost = formatCurrency(lessTax ? lineInfo.totalLessTax : lineInfo.total, { currencyCode });

				if (product.billingFrequency) {
					formattedCost += ' ' + recurrenceText(product.billingFrequency);
				}

				// subscription
				if (product.billingFrequency) {

					// TODO
					return;

					{/*
					return <li>
						<td>
							{product.name} <span className="footnote-asterisk" title={`Subscription`}></span>
						</td>
						<td className="qty-column">
							{qty}
						</td>
						<td className="currency-column">
							{formattedCost}
						</td>
						{!readonly && <td className="actions-column">
							<button type="button" className="btn btn-small btn-outline-danger" title={`Remove`}
								onClick={() => {
									addToCart(product.id, 0)
								}}>
								<Icon type='fa-trash' />
							</button>
						</td>}
					</li>;
					*/}
				}

				// standard quantity of product
				return <li>
					<BasketItem content={product} quantity={lineInfo.quantity} disableLink hideOrder showRemove={!readonly} />
				</li>;
			})}
		</ul>
	</>;
}

export default ProductTable;
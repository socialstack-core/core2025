import { formatCurrency } from "UI/Functions/CurrencyTools";
import Loop from 'UI/Loop';
import Alert from 'UI/Alert';
import Icon from 'UI/Icon';
import productApi from 'Api/Product';
import { recurrenceText } from 'UI/Functions/Payments';
import { useSession } from 'UI/Session';
import BasketItem from 'UI/Product/Signpost';
import Quantity from 'UI/Product/Quantity';

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
	
	return <table className="table shopping-cart__table">
		<thead>
			<tr>
				<th>
					{`Product`}
				</th>
				<th className="qty-column">
					{`Quantity`}
				</th>
				<th className="currency-column">
					{`Cost`}
				</th>
				{!readonly && <th className="actions-column">
					&nbsp;
				</th>}
			</tr>
		</thead>
		<tbody>
			{itemSet.map(lineInfo => {
				var product = lineInfo.product;
				var qty = lineInfo.quantity;

				var formattedCost = formatCurrency(lessTax ? lineInfo.totalLessTax : lineInfo.total, { currencyCode });
				
				if (product.billingFrequency) {
					formattedCost += ' ' + recurrenceText(product.billingFrequency);
				}
				
				// subscription
				if (product.billingFrequency) {

					return <tr>
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
					</tr>;
				}

				// standard quantity of product
				return <tr>
					<td>
						<BasketItem content={product} quantity={lineInfo.quantity} disableLink hideQuantity hideOrder />
					</td>
					<td className="qty-column">
						{readonly ? lineInfo.quantity : <Quantity product={product}  inBasket={lineInfo.quantity} />}
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
				</tr>;
			})}
			<tr>
				<td>
					<strong>{`Total`}</strong>
				</td>
				<td className="qty-column">
				</td>
				<td className="currency-column" style={{ fontWeight: 'bold' }}>
					{formatCurrency(lessTax ? pricedCart.totalLessTax : pricedCart.total, {currencyCode})}
				</td>
				<td>
					&nbsp;
				</td>
			</tr>
			<tr>
				<td colspan='3'>
					{hasAtLeastOneSubscription && <small>
						<span className="footnote-asterisk"></span> {`Your payment information will be securely stored in order to process future subscription payments. The total stated will also be charged today.`}
					</small>}
				</td>
			</tr>
		</tbody>
	</table>;
}

export default ProductTable;
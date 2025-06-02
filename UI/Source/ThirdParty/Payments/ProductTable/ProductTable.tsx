import { formatCurrency } from "UI/Functions/CurrencyTools";
import Loop from 'UI/Loop';
import Alert from 'UI/Alert';
import Icon from 'UI/Icon';
import productApi from 'Api/Product';
import { calculatePrice, recurrenceText } from 'UI/Functions/Payments';
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
	var { shoppingCart, addToCart, readonly } = props;
	const { session } = useSession();

	function renderTotals(cartTotals, options) {
		var totals = [];
		var coupon = options && options.coupon;

		for (var i = 0; i < 5; i++) {
			if (cartTotals[i]) {
				var total = cartTotals[i];
				var totalCost = total;

				if (coupon != null) {
					if (coupon.minSpendPrice) {
						// Are we above it?
						if (totalCost < coupon.minSpendPrice.amount) {
							// No!
							coupon = null;
						}
					}

					if (coupon && coupon.discountPercent != 0) {
						var discountedTotal = totalCost * (1 - (coupon.discountPercent / 100));

						if (discountedTotal <= 0) {
							// Becoming free!
							totalCost = 0;
						} else {
							// Round to nearest pence/ cent
							totalCost = Math.ceil(discountedTotal);
						}
					}

					if (coupon && coupon.discountAmount) {
						if (totalCost < coupon.discountAmount.amount) {
							// Becoming free!
							totalCost = 0;
						} else {
							// Discount a fixed number of units:
							totalCost -= coupon.discountAmount.amount;
						}
					}
				}

				var recurTitle = recurrenceText(i);

				if (totalCost != total) {
					if (i) {
						// i is 0 for one off payments.
						// This is any recurring things with a discount, where the discount is applied on the first payment only.
						totals.push(<div>{formatCurrency(totalCost, options)} today, then {
							formatCurrency(total, options)
						} {recurTitle}</div>);
					} else {
						totals.push(<div><small><s>{
							formatCurrency(total, options)
						}</s></small> {formatCurrency(totalCost, options)}</div>);
					}
				} else {
					totals.push(<div>{
						formatCurrency(totalCost, options)
					} {recurTitle}</div>);
				}
			}
		}

		return totals;
	}

	var items = shoppingCart?.productQuantities || [];

	if (!items.length) {
		return <Alert type="info">
			{readonly ? <>
				{`This purchase is empty`}
			</> : <>
				{`Your shopping cart is empty.`}
			</>}

		</Alert>;
	}

	// 'price', 'tiers', 'tiers.price'

	var cartTotalByFrequency = [0, 0, 0, 0, 0];

	var itemSet = [];
	var currencyCode = null;
	var hasAtLeastOneSubscription = false;

	items.forEach(cartInfo => {
		var product = cartInfo.product;
		if (!product) {
			return;
		}

		if (product.billingFrequency) {
			hasAtLeastOneSubscription = true;
		}

		var qty = cartInfo.quantity;

		if (qty < product.minQuantity) {
			qty = product.minQuantity;
		}

		var cost = calculatePrice(product, qty);

		if (cost) {
			cartTotalByFrequency[product.billingFrequency] += cost.amount;

			itemSet.push({
				...cartInfo,
				cost
			});

			if (!currencyCode) {
				currencyCode = cost.currencyCode;
			}
		}
	});

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
			{itemSet.map(cartInfo => {
				var product = cartInfo.product;
				var qty = cartInfo.quantity;
				var cost = cartInfo.cost;

				if (qty < product.minQuantity) {
					qty = product.minQuantity;
				}

				var formattedCost = formatCurrency(cost.amount, { currencyCode });

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
						<BasketItem content={product} quantity={cartInfo.quantity} disableLink hideQuantity hideOrder />
					</td>
					<td className="qty-column">
						<Quantity inBasket={cartInfo.quantity} />
					</td>
					<td className="currency-column">
						{formatCurrency(product.price.amount, { currencyCode })}
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
					{currencyCode ? renderTotals(cartTotalByFrequency, { currencyCode, coupon: props.coupon }) : '-'}
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
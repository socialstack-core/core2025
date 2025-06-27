import Modal from 'UI/Modal';
import Loading from 'UI/Loading';
import { useRouter } from 'UI/Router';
import { useCart } from 'UI/Payments/CartSession';
import ProductTable from 'UI/Payments/ProductTable';
import { useState } from 'react';
import { Coupon } from 'Api/Coupon';
import Input from 'UI/Input';
import Button from 'UI/Button';
import Form from 'UI/Form';

/**
 * Props for the Cart component.
 */
interface CartProps {
	/**
	 * associated title
	 */
	title?: string,
}

/**
 * The Cart React component.
 * @param props React props.
 */
const Cart: React.FC<CartProps> = (props) => {
	const title = props.title?.length ? props.title : `Shopping Cart`;
	const { setPage } = useRouter();
	var { addToCart, emptyCart, shoppingCart, cartIsEmpty, loading, lessTax, setCoupon } = useCart();
	var [showEmptyCartPrompt, setShowEmptyCartPrompt] = useState(null);

	const renderActiveCoupon = (coupon: Coupon) => {
		const couponTitle = coupon.description;

		return <>
			{`Active coupon: ${couponTitle}`} <Button onClick={() => setCoupon(null)}>{ `Remove` }</Button>
		</>
	};

	return <>
		<h1 className="shopping-cart__title">
			{title}
		</h1>
		{loading ? <Loading /> : <ProductTable shoppingCart={shoppingCart} addToCart={addToCart} lessTax={lessTax} />}

		{!cartIsEmpty() && <>
			<div className="shopping-cart__footer">
				<button type="button" className="btn btn-outline-danger" onClick={() => setShowEmptyCartPrompt(true)}>
					<i className="fal fa-fw fa-trash" />
					{`Empty Cart`}
				</button>

				<button type="button" className="btn btn-primary" onClick={() => setPage('/cart/checkout')}>
					<i className="fal fa-fw fa-credit-card" />
					{`Checkout`}
				</button>
			</div>
		</>}
		{
			showEmptyCartPrompt && <>
				<Modal visible isSmall className="empty-cart-modal" title={`Empty Cart`} onClose={() => setShowEmptyCartPrompt(false)}>
					<p>{`This will remove all selected products from your shopping cart.`}</p>
					<p>{`Are you sure you wish to do this?`}</p>
					<footer>
						<button type="button" className="btn btn-outline-danger" onClick={() => setShowEmptyCartPrompt(false)}>
							{`Cancel`}
						</button>
						<button type="button" className="btn btn-danger" onClick={() => {
							emptyCart();
							setShowEmptyCartPrompt(false);
						}}>
							{`Empty`}
						</button>
					</footer>
				</Modal>
			</>
		}
		<div>
			<h1>{`Promo code`}</h1>
			{
				shoppingCart?.coupon && renderActiveCoupon(shoppingCart?.coupon)
			}
			<div>
				<Form action={(fields) => {
					return setCoupon(fields.coupon);
				}} submitLabel={`Apply`} successMessage={`Coupon applied`} failedMessage={`Unable to apply coupon`}>
					<Input type='text' name='coupon' placeholder={`Enter a promo code`} />
				</Form>
			</div>
		</div>
	</>;
}

export default Cart;
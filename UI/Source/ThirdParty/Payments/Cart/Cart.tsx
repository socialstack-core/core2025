import Loading from 'UI/Loading';
import { useCart } from 'UI/Payments/CartSession';
import ProductTable from 'UI/Payments/ProductTable';
import CartTotal from 'UI/Payments/CartTotal';
import { useState } from 'react';
import { Coupon } from 'Api/Coupon';
import Input from 'UI/Input';
import Button from 'UI/Button';
import Form from 'UI/Form';
import Alert from 'UI/Alert';

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
	var { addToCart, emptyCart, shoppingCart, cartIsEmpty, loading, lessTax, setCoupon } = useCart();
	var [applyCodeEnabled, setApplyCodeEnabled] = useState(false);

	const renderActiveCoupon = (coupon: Coupon) => {
		const couponTitle = coupon.description;

		return <Alert type="info">
			{`Active coupon:`}
			<pre>
				{couponTitle}
			</pre>
			<Button outlined variant="danger" onClick={() => setCoupon(null)}>{`Remove`}</Button>
		</Alert>
	};

	const updateApplyCodeEnabled = (e) => {
		setApplyCodeEnabled(e.target.value.trim().length);
	}

	return <>
		<div className="shopping-cart">
		<h1 className="shopping-cart__title">
			{title}
		</h1>

			{loading && <Loading />}

			{!loading && <>
				<ProductTable shoppingCart={shoppingCart} addToCart={addToCart} lessTax={lessTax} />

		{!cartIsEmpty() && <>
					<fieldset className="shopping-cart__promo fieldset--bordered">
						<legend>
							{`Have a promotional code? Enter it here`}
						</legend>
			{
				shoppingCart?.coupon && renderActiveCoupon(shoppingCart?.coupon)
			}
						<Form className="shopping-cart__promo-form" action={(fields) => {
					return setCoupon(fields.coupon);
						}} successMessage={`Coupon applied`} failedMessage={`Unable to apply coupon`}>
							<Input type='text' name='coupon' placeholder={`Enter promotional code`} noWrapper onChange={(e) => updateApplyCodeEnabled(e)} />
							<Input type="submit" label={`Apply code`} noWrapper disabled={applyCodeEnabled ? undefined : true} />
				</Form>
					</fieldset>

					<CartTotal shoppingCart={shoppingCart} emptyCart={emptyCart} lessTax={lessTax} />
				</>}

			</>}
		</div>
	</>;
}

export default Cart;
import ProductTable from 'UI/Payments/ProductTable';
import Input from 'UI/Input';
import Form from 'UI/Form';
import Alert from 'UI/Alert';
import Loading from 'UI/Loading';
import { useState, useEffect } from 'react';
import { useSession } from 'UI/Session';
import { useRouter } from 'UI/Router';
import useApi from 'UI/Functions/UseApi';
import { useCart } from 'UI/Payments/CartSession';
import shoppingCartApi from 'Api/ShoppingCart';
import deliveryOptionApi, { DeliveryOption } from 'Api/DeliveryOption';
import addressApi, { Address } from 'Api/Address';
import { ApiList } from 'UI/Functions/WebRequest';
import AddressSelection from './AddressSelection';
import CheckoutSection from './CheckoutSection';
import DeliveryEstimates from './DeliveryEstimates';

/**
 * Props for the Checkout component.
 */
interface CheckoutProps {
	canPayLater: (session:Session) => boolean;
}

/**
 * The Cart React component.
 * @param props React props.
 */
const Checkout: React.FC<CheckoutProps> = (props) => {
	const { canPayLater } = props;
	const { session } = useSession();
	const { setPage } = useRouter();
	var { shoppingCart, cartIsEmpty, emptyCart, addToCart, lessTax, getCartId } = useCart();
	
	const deferPayment = canPayLater ? canPayLater(session) : false;
	const [deliveryAddress, setDeliveryAddress] = useState<Address | undefined>();
	const [sameAsDelivery, setSameAsDelivery] = useState<boolean>(true);
	const [billingAddress, setBillingAddress] = useState<Address | undefined>();
	const [deliveryDate, setDeliveryDate] = useState<DeliveryOption | undefined>();
	const [estimates, setEstimates] = useState < ApiList<DeliveryOption> | undefined>();
	
	// Gets the billing and delivery addresses
	const [savedAddresses] = useApi(() => addressApi
		.getCartAddresses()
		.then(addrSet => {
			var deliveryAddr = addrSet.results.find(addr => addr.isDefaultDeliveryAddress);
			var billingAddr = addrSet.results.find(addr => addr.isDefaultBillingAddress);
			
			setDeliveryAddress(deliveryAddr);
			setBillingAddress(billingAddr);

			if (billingAddr) {
				setSameAsDelivery((billingAddr.id == deliveryAddr?.id));
			} else {
				setSameAsDelivery(true);
			}

			return addrSet;
		}
	), []);
	
	// Load delivery options for this cart
	useEffect(() => {
		
		if(!deliveryAddress){
			return;
		}
		
		deliveryOptionApi.estimate(getCartId(), {
			deliveryAddressId: deliveryAddress.id
		}).then(estimates => {

			if (estimates.results.length > 0) {
				// Pick the first one by default always:
				setDeliveryDate(estimates.results[0]);
			}

			setEstimates(estimates);
		});
		
	}, [deliveryAddress]);
	
	if (!savedAddresses) {
		// Addresses or delivery options currently loading
		return <div className="payment-checkout">
			<Loading />
		</div>;
	}

	if (cartIsEmpty()) {
		return <div className="payment-checkout">
			<Alert type='info'>
				{`Your cart is currently empty`}
			</Alert>
		</div>;
	}

	return <div className="payment-checkout">
		<h2 className="payment-checkout__title">
			{`Checkout`}
		</h2>
			<div className="mb-3">
			<AddressSelection title={`Delivery Address`} name='delivery' savedAddresses={savedAddresses} value={deliveryAddress} setValue={setDeliveryAddress} addressType='delivery' />
			<AddressSelection title={`Billing Address`} name='billing' savedAddresses={savedAddresses} value={billingAddress} setValue={setBillingAddress} same={true} isSame={sameAsDelivery} setSameAs={setSameAsDelivery} addressType='billing' />
			<CheckoutSection title={`Delivery date`} enabled={!!deliveryAddress} >
				{estimates ? <DeliveryEstimates estimates={estimates} value={deliveryDate} setValue={setDeliveryDate} /> : <Loading />}
				</CheckoutSection>
				<CheckoutSection title={`Payment method`} enabled={!!deliveryAddress} >
					{deferPayment ? <>
						{`Buy now pay later: This order will be billed to your account.`}
					</> : 
						<Input type='payment' name='paymentMethod' label='Payment method' validate={['Required']} />
					}
				</CheckoutSection>
				<ProductTable shoppingCart={shoppingCart} addToCart={addToCart} readonly lessTax={lessTax} />
				<div className="form-check">
					<label className="form-check-label" htmlFor="termsCheckbox">
						{`By placing your order you agree to both the `}
						<a href="/terms-and-conditions" target="_blank" rel="noopener noreferrer">
							{`terms and conditions`}
						</a>
						{` and `}
						<a href="/privacy-policy" target="_blank" rel="noopener noreferrer">
							{`privacy policy`}
						</a>
					</label>
				</div>
			</div>
		
		<Form 
			action={shoppingCartApi.checkout}
			failedMessage={`Unable to purchase`}
			loadingMessage={`Purchasing..`}
			onSuccess={info => {
				
				// Clear cart:
				emptyCart();
				
				if (info?.action){
					// Go to it now:
					window.location.href = info.action;
				} else {
					var status = info?.purchase?.status || 0;

					if (status >= 200 && status < 300){
						setPage('/cart/complete?status=success');
					}else if(status < 300){
						setPage('/cart/complete?status=pending');
					}else{
						setPage('/cart/complete?status=failed');
					}
				}
			}}
		>
			<div className="payment-checkout__footer">
				<button type="submit" className="btn btn-primary" disabled={!deliveryDate}>
					<i className="fal fa-fw fa-credit-card" />
					{`Confirm Purchase`}
				</button>
			</div>
		</Form>
	</div>;
}

export default Checkout;
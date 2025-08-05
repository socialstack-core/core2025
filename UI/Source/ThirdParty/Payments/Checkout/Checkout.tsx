import ProductTable from 'UI/Payments/ProductTable';
import Input from 'UI/Input';
import Button from 'UI/Button';
import Link from 'UI/Link';
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
	const { setPage, pageState } = useRouter();
	const { query } = pageState;
	var { shoppingCart, cartIsEmpty, emptyCart, addToCart, lessTax, getCartId } = useCart();
	
	const deferPayment = query?.get("card") ? false : true; // canPayLater ? canPayLater(session) : false;
	const [deliveryAddress, setDeliveryAddress] = useState<Address | undefined>();
	const [sameAsDelivery, setSameAsDelivery] = useState<boolean>(true);
	const [billingAddress, setBillingAddress] = useState<Address | undefined>();
	const [deliveryDate, setDeliveryDate] = useState<DeliveryOption | undefined>();
	const [estimates, setEstimates] = useState<ApiList<DeliveryOption> | undefined>();

	// TODO: currently this infers acceptance as soon as the order is confirmed;
	//       allow option to display this as a checkbox which needs to be actively selected
	const [acceptedTerms, setAcceptedTerms] = useState<boolean>(true);

	enum CheckoutStep {
		OrderContents,
		DeliveryAddress,
		BillingAddress,
		DeliveryDate,
		PaymentMethod,
		TermsConditions
	}

	//const [currentStep, setCurrentStep] = useState(CheckoutStep.DeliveryAddress);
	//useEffect(() => {
	//}, [deliveryAddress]);

	let currentStep: CheckoutStep = CheckoutStep.DeliveryAddress;

	if (!!deliveryAddress) {
		currentStep = CheckoutStep.BillingAddress;

		if (sameAsDelivery || !!billingAddress) {
			currentStep = CheckoutStep.DeliveryDate;

			if (!!deliveryDate) {
				currentStep = CheckoutStep.PaymentMethod;

				// TODO
				if (true) {
					currentStep = CheckoutStep.TermsConditions;
				}

			}
		}
	}

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
		var cartRef = getCartId();
		
		if(!deliveryAddress){
			return;
		}

		deliveryOptionApi.estimate(cartRef.id, cartRef.anonKey, {
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

	return <>
		<div className="payment-checkout">
			<h1 className="payment-checkout__title">
				{`Checkout`}
			</h1>
			<ol className="payment-checkout__steps">
				<li>
					{/* order contents */}
					<CheckoutSection title={`Order contents`} enabled={true}>
						<ProductTable shoppingCart={shoppingCart} readOnly lessTax={lessTax} />
					</CheckoutSection>
				</li>

				<li>
					{/* delivery address */}
					<AddressSelection selectedTitle={`Delivering to`} unselectedTitle={`Select a delivery address`}
						name='delivery' savedAddresses={savedAddresses.results}
						value={deliveryAddress} setValue={setDeliveryAddress} addressType='delivery'
						enabled={true}
					/>
				</li>

				<li className={currentStep < CheckoutStep.BillingAddress ? "payment-checkout__step--disabled" : ""}>
					{/* billing currentStep */}
					<AddressSelection selectedTitle={`Billing Address`} unselectedTitle={`Select a billing address`}
						name='billing' savedAddresses={savedAddresses.results}
						value={billingAddress} setValue={setBillingAddress}
						hasSame={true} isSame={sameAsDelivery} setSameAs={setSameAsDelivery} addressType='billing'
						enabled={currentStep >= CheckoutStep.BillingAddress}
					/>
				</li>

				<li className={currentStep < CheckoutStep.DeliveryDate ? "payment-checkout__step--disabled" : ""}>
					{/* delivery date */}
					<CheckoutSection title={`Delivery date`} enabled={currentStep >= CheckoutStep.DeliveryDate}>
						{estimates ? <DeliveryEstimates estimates={estimates} value={deliveryDate} setValue={setDeliveryDate} /> : <Loading />}
					</CheckoutSection>
				</li>
			</ol>
			<Form
				action={shoppingCartApi.checkout}
				failedMessage={`Unable to purchase`}
				loadingMessage={`Purchasing..`}
				onValues={vals => {
					var cartRef = getCartId();
					vals.shoppingCartId = cartRef.id;
					vals.anonymousCartKey = cartRef.anonKey;
					vals.billingAddressId = ((sameAsDelivery ? deliveryAddress?.id : billingAddress?.id) || 0) as int;
					vals.deliveryAddressId = (deliveryAddress?.id || 0) as int;
					vals.deliveryOptionId = (deliveryDate?.id || 0) as int;

					return vals;
				}}
				onSuccess={info => {

					if (info?.action) {
						// Go to it now:
						window.location.href = info.action;
					} else {
						var status = info?.purchase?.status || 0;

						if (status >= 200 && status < 300) {
							// Clear cart:
							emptyCart();

							setPage('/cart/complete?status=success');
						} else if (status < 300) {
							setPage('/cart/complete?status=pending');
						} else {
							setPage('/cart/complete?status=failed');
						}
					}
				}}
			>
				<ol className="payment-checkout__steps payment-checkout__steps--continued">
					<li className={currentStep < CheckoutStep.PaymentMethod ? "payment-checkout__step--disabled" : ""}>
						{/* payment method */}
						<CheckoutSection title={`Payment method`} enabled={currentStep >= CheckoutStep.PaymentMethod} >
							{deferPayment ? <>
								{`Buy now pay later: This order will be billed to your account.`}
							</> :
								<Input type='payment' name='paymentMethod' label='Payment method' validate={['Required']} />
							}
						</CheckoutSection>
					</li>

					<li className={currentStep < CheckoutStep.TermsConditions ? "payment-checkout__step--disabled" : ""}>
						{/* terms and conditions / privacy policy */}
						<CheckoutSection title={`Review terms`} enabled={currentStep >= CheckoutStep.TermsConditions}>
							{/*
							<Input type="checkbox" className="payment-checkout__terms"
								checked={acceptedTerms ? true : undefined}
								onChange={e => setAcceptedTerms(e.target.checked)} label={<>
								{`By placing your order you agree to both the `}
								<Link href="/terms-and-conditions" external>
									{`terms and conditions`}
								</Link>
								{` and `}
								<Link href="/privacy-policy" external>
									{`privacy policy`}
								</Link>.
							</>} />
							*/}
							<p>
								{`Please note, by placing your order you agree to both the `}
								<Link href="/terms-and-conditions" external>
									{`terms and conditions`}
								</Link>
								{` and `}
								<Link href="/privacy-policy" external>
									{`privacy policy`}
								</Link>.
							</p>
						</CheckoutSection>
					</li>

				</ol>

				<div className="payment-checkout__footer">
					<Button type="submit" disabled={currentStep < CheckoutStep.TermsConditions || !acceptedTerms ? true : undefined}>
						<i className="fal fa-fw fa-credit-card" />
						<span>
							{`Confirm Purchase`}
						</span>
					</Button>
				</div>
			</Form>
		</div>
	</>;
}

export default Checkout;
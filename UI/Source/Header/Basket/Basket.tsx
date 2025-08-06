import Popover from 'UI/Popover';
import Loading from "UI/Loading";
import BasketItem from 'UI/Product/Signpost';
import Link from "UI/Link";
import Button from 'UI/Button';
import Alert from "UI/Alert";
import { useCart } from 'UI/Payments/CartSession';
import { formatCurrency } from 'UI/Functions/CurrencyTools';
import { useSession } from 'UI/Session';

/**
 * Props for the Basket component.
 */
interface BasketProps {
}

/**
 * The Basket React component.
 * @param props React props.
 */
const Basket: React.FC<BasketProps> = ({ ...props }) => {
	var { cartContents, cartIsEmpty, loading, lessTax } = useCart();

	const { session } = useSession();
	var { locale } = session;

	// calculate basket totals
	var basketCount = 0;
	var cartEmpty = loading || cartIsEmpty();
	var basketTotal = cartEmpty ? 0 : (lessTax ? cartContents?.totalLessTax : cartContents?.total);

	let basketItems = cartEmpty ? [] : cartContents?.contents;
	let currencyCode = cartContents?.currencyCode || locale.currencyCode || "GBP";

	basketItems.forEach(cartInfo => {
		var qty = cartInfo.quantity;
		basketCount += qty;
	});

	return <>
		{/* toggle basket */}
		<Button sm outlined className="site-nav__actions-basket" popoverTarget="basket_popover">
			{loading && <>
				<Loading small />
			</>}
			{!loading && <>
				{/*
				<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 54 53" fill="none">
					<path d="M1.96524 4.44736C5.36241 4.84443 6.6198 5.3518 8.34045 7.07242C9.99492 9.16806 15.0245 27.4994 15.4657 30.5877L14.6716 33.7201C14.1642 35.7055 14.6054 37.779 15.8848 39.3894C17.1422 40.9997 19.0393 41.9262 21.0909 41.9262H44.1211C45.3344 41.9262 46.327 40.9335 46.327 39.7203C46.327 38.507 45.3344 37.5143 44.1211 37.5143H21.0909C20.407 37.5143 19.7673 37.2055 19.3482 36.6761C18.929 36.1467 18.7746 35.4408 18.9511 34.779L19.3702 33.1025H36.4002C41.9372 33.1025 46.9447 29.595 48.8418 24.389L52.5699 14.1755C53.3861 11.9475 53.0552 9.47689 51.6875 7.53567C50.3198 5.59445 48.1139 4.44736 45.7314 4.44736H17.6496C16.4363 4.44736 15.4436 5.44003 15.4436 6.6533C15.4436 7.86656 16.4363 8.85923 17.6496 8.85923H45.7314C46.68 8.85923 47.5182 9.30042 48.0697 10.0725C48.6212 10.8446 48.7315 11.7931 48.4227 12.6755L44.6946 22.889C43.4372 26.3744 40.1062 28.6906 36.4002 28.6906H19.5467C18.3776 21.3448 12.7965 5.26356 11.495 3.96206C8.71546 1.18258 6.28891 0.454619 2.4726 0.0134315C1.25933 -0.118925 0.178408 0.74139 0.0239907 1.95465C-0.108367 3.16792 0.751959 4.24883 1.96524 4.40324V4.44736Z" fill="currentColor" />
					<path d="M18.7526 53C20.58 53 22.0615 51.5186 22.0615 49.6911C22.0615 47.8636 20.58 46.3822 18.7526 46.3822C16.9251 46.3822 15.4436 47.8636 15.4436 49.6911C15.4436 51.5186 16.9251 53 18.7526 53Z" fill="currentColor" />
					<path d="M43.018 53C44.8455 53 46.327 51.5186 46.327 49.6911C46.327 47.8636 44.8455 46.3822 43.018 46.3822C41.1906 46.3822 39.7091 47.8636 39.7091 49.6911C39.7091 51.5186 41.1906 53 43.018 53Z" fill="currentColor" />
				</svg>
				*/}
				{/*
				<i className="fr fr-shopping-cart"></i>
				*/}
				<i className="fr fr-shopping-basket"></i>
				<span className="site-nav__actions-basket-label">
					{cartIsEmpty() && <>
						<strong>0</strong> {`items`}
						{/*`Basket`*/}
					</>}
					{!cartIsEmpty() && <>
						{/*
						<strong>
							{basketCount}
						</strong> {basketCount == 1 ? `item` : `items`}
					*/}
						{formatCurrency(basketTotal, { currencyCode })} {`(${basketCount})`}
						{/*
						<NotificationBadge count={basketCount} />
						*/}
					</>}
				</span>
			</>}
		</Button>

		{/* basket popover */}
		<Popover method="auto" id="basket_popover" alignment="right" blurBackground={true} className="site-nav__basket-wrapper">
			{loading && <>
				<Loading />
			</>}
			{!loading && <>
			<header>
				<h2 className="site-nav__basket-title">
					<i className="fr fr-shopping-basket"></i>
					{`Shopping basket`}
				</h2>
				{!cartIsEmpty() && <>
					{/*
					<span>
						{formatCurrency(basketTotal, { currencyCode })}
					</span>
					<NotificationBadge count={basketCount} />
				*/}
					{formatCurrency(basketTotal, { currencyCode })} {`(${basketCount})`}
				</>}
			</header>
			</>}
			{cartIsEmpty() && <>
				<Alert variant="info">
					{`Basket is empty`}
				</Alert>
			</>}


			{basketItems.map(lineItem => {
				return <BasketItem
					content={lineItem.product}
					quantity={lineItem.quantity}
					disableLink
					priceOverride={{
						currencyCode: currencyCode,
						amount: lessTax ? lineItem.totalLessTax : lineItem.total
					}}
				/>
			})}
			{!cartIsEmpty() && <>
				<Link href="/cart" variant="primary" className="site-header__basket-checkout">
					{`View order`}
				</Link>
			</>}
		</Popover>
	</>;
}

export default Basket;
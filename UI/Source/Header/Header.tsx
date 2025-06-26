import Image from 'UI/Image';
import Html from 'UI/Html';
import NotificationBadge from 'UI/NotificationBadge';
import { useSession } from 'UI/Session';
import exampleLogoRef from './example-logo.png';
import { getContactLink } from 'UI/Functions/ContactTools';
import { useState, useEffect } from "react";
import { getUrl } from 'UI/FileRef';
import containerQueryPolyfillJs from './static/container-query-polyfill.js';
import popoverPolyfillJs from './static/popover.min.js';
import { lazyLoad } from 'UI/Functions/WebRequest';
import productCategoryApi, { ProductCategory } from 'Api/ProductCategory';
import useApi from "UI/Functions/UseApi";
import Loading from "UI/Loading";
import BasketItem from 'UI/Product/Signpost';
import { useCart } from 'UI/Payments/CartSession';
import { recurrenceText } from 'UI/Functions/Payments';
import { formatCurrency } from 'UI/Functions/CurrencyTools';
import RecentSearches from "UI/RecentSearches";
import Loop from "UI/Loop";
import Link from "UI/Link";
import Button from "UI/Button";
import Input from "UI/Input";

// TODO: swap to 0 once "care-home-nursing-home-supplies-equipment" is no longer a thing
const PARENT_CATEGORY_ID = 1;

/**
 * Props for the Header component.
 */
interface HeaderProps {

	/** 
	 * contact number (e.g. "0123 456 7890")
	 */
	contactNumber?: string,

	/**
	 * The website logo.
	 */
	logoRef?: FileRef,

	/** 
	 * optional message to be displayed next to contact number
	 */
	message?: string,

	/** 
	 * optional placeholder text for search field
	 */
	searchPlaceholder?: string,

	/**
	 * outstanding notifications count
	 */
	notificationCount: number,

	/**
	 * set to true to force demo information
	 */
	demo?: boolean
}

/**
 * Props for each header link.
 */
interface HeaderLinkProps {

	/** 
	 * label
	 */
	label: string,

	/** 
	 * target URL (render as link)
	 */
	url?: string,

	/** 
	 * target URL (render as button with popover target)
	 */
	popoverTarget?: string,

	/**
	 * function to call on click (render as button with click event)
	 */
	//onClick?: () => void
}

/**
 * The website header React component.
 * @param props React props.
 */
const Header: React.FC<HeaderProps> = ({ contactNumber, logoRef, message, searchPlaceholder, notificationCount, demo, ...props }) => {
	var { addToCart, emptyCart, cartContents, cartIsEmpty, loading, lessTax, setLessTax } = useCart();
	const [categoryId, setCategoryId] = useState(PARENT_CATEGORY_ID);
	//const [basketItems, setBasketItems] = useState(loading ? [] : shoppingCart?.productQuantities || []);
	const [productCategory, setProductCategory] = useState<ProductCategory>();
	const [productSubCategories, setProductSubCategories] = useState<ProductCategory[]>();
	const [query, setQuery] = useState('');

	useApi(() => {
		return productCategoryApi.load(categoryId as uint, [
			productCategoryApi.includes!.primaryurl
		]).then(results => {
			setProductCategory(results);
		});
	}, [categoryId]);

	useApi(() => {
		return productCategoryApi.list({
			query: 'ParentId=?',
			args: [categoryId as uint],
			pageSize: 10 as int,
			pageIndex: 1 as int,
			sort: {
				field: "id",
				direction: "desc"
			}
		}, [
			productCategoryApi.includes!.primaryurl
		]).then(results => {
			setProductSubCategories(results.results);
		});
	}, [categoryId]);

	// temp
	demo = true;

	if (demo) {

		if (!contactNumber) {
			contactNumber = "0808 189 2044";
		}

		if (!logoRef) {
			logoRef = exampleLogoRef;
		}

		if (!message || !message.length) {
			message = `Free UK Delivery Over &pound;50`;
		}

		if (!searchPlaceholder || !searchPlaceholder.length) {
			searchPlaceholder = `Search for a product name, category or code`
		}

	}

	const contactHref = contactNumber ? getContactLink(contactNumber) : "";

	const [primaryLinks, setPrimaryLinks] = useState<HeaderLinkProps[]>([]);
	const { session } = useSession();
	var { user } = session;

	const notificationBellSVG = <>
		<svg viewBox="0 0 12 16" fill="none" xmlns="http://www.w3.org/2000/svg">
			<path d="M4.67333 14.1116C4.67333 14.8791 5.26667 15.5 6 15.5C6.73333 15.5 7.32667 14.8791 7.32667 14.1116H4.67333ZM6 3.63953C7.84 3.63953 9.33333 5.20233 9.33333 7.12791V12.0116H2.66667V7.12791C2.66667 5.20233 4.16 3.63953 6 3.63953ZM6 0.5C5.44667 0.5 5 0.967442 5 1.54651V2.36279C2.90667 2.83721 1.33333 4.7907 1.33333 7.12791V11.314L0 12.7093V13.407H12V12.7093L10.6667 11.314V7.12791C10.6667 4.7907 9.09333 2.83721 7 2.36279V1.54651C7 0.967442 6.55333 0.5 6 0.5ZM5.33333 5.03488H6.66667V7.82558H5.33333V5.03488ZM5.33333 9.22093H6.66667V10.6163H5.33333V9.22093Z" fill="currentColor" />
		</svg>
	</>;

	/*
	const [productCategories] = useApi(() => {
		return productCategoryApi.list({
			query: 'ParentId=?',
			args: [PARENT_CATEGORY_ID]
		}, [
			productCategoryApi.includes!.primaryurl
		])
	}, []);
	*/

	useEffect(() => {
		// check: iOS versions prior to v17 don't support popover API
		// lazy-load polyfill if required
		if (!isPopoverApiSupported()) {
			lazyLoad(getUrl(popoverPolyfillJs)!);
		}

		// check: iOS versions prior to v16 don't support CSS container queries
		// lazy-load polyfill if required
		if (!areContainerQueriesSupported()) {
			lazyLoad(getUrl(containerQueryPolyfillJs)!);
		}

		// TODO: retrieve primary links from DB
		setPrimaryLinks([
			{
				label: `Shop for products`,
				//url: `/products`
				//onClick: () => {}
				popoverTarget: 'products_popover'
			},
			{
				label: `Contact Us`,
				url: `/contact-us`
			},
			{
				label: `Delivery Information`,
				url: `/delivery`
			},
			{
				label: `About Us`,
				url: `/about-us`
			},
		]);

	}, [])

	function isPopoverApiSupported() {
		const test = document.createElement('div');
		return 'popover' in test &&
			typeof HTMLElement.prototype.showPopover === 'function' &&
			typeof HTMLElement.prototype.hidePopover === 'function';
	}

	function areContainerQueriesSupported() {
		return CSS.supports('container-type', 'inline-size');
	}

	if (!notificationCount) {
		notificationCount = 0;
	}

	// calculate basket totals
	var basketCount = 0;
	var cartEmpty = loading || cartIsEmpty();
	var basketTotal = cartEmpty ? 0 : (lessTax ? cartContents?.totalLessTax : cartContents?.total);

	let basketItems = cartEmpty ? [] : cartContents?.contents;
	let currencyCode = cartContents?.currencyCode;

	// TEMP
	if (!currencyCode) {
		currencyCode = "GBP";
	}

	basketItems.forEach(cartInfo => {
		var qty = cartInfo.quantity;
		basketCount += qty;
	});

	const showContact = contactHref?.length || message?.length;

	const highlightMatch = (text: string, query: string) => {
		const regex = new RegExp(`(${query})`, 'gi');
		const parts = text.split(regex);
		return parts.map((part, index) =>
			part.toLowerCase() === query.toLowerCase() ? (
				<b key={index}>{part}</b>
			) : (
				<span key={index}>{part}</span>
			)
		);
	};

	return (
		<div className="site-header">
			<div className="site-header__internal">
				<a href="/" className="site-header__logo">
					{logoRef && <Image fileRef={logoRef} />}
					{/*
					<img src="https://assets.codepen.io/5477427/acticare_logo.png" alt="" />
					*/}
				</a>

				<div className="site-header__search">
					<button type="button" className="btn site-header__search-trigger" popoverTarget="search_popover">
						<svg viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
							<path d="M14.3 12.58h-.91l-.32-.31a7.43 7.43 0 10-.8.8l.3.32v.9L18.3 20l1.7-1.7-5.7-5.72zm-6.87 0A5.14 5.14 0 117.42 2.3a5.14 5.14 0 01.01 10.28z" fill="currentColor" />
						</svg>
					</button>
					<div className="site-header__search-wrapper" popover="auto" id="search_popover">
						<input type="search" onInput={(ev) => setQuery((ev.target as HTMLInputElement).value)} placeholder={searchPlaceholder} />
					</div>
					<div className="site-header__search-dropdown">
						{/*...*/}
						{(!query || query.length == 0) && (
							<RecentSearches />
						)}
						{query && query.length != 0 && (
							// this may need updating further
							// but poses as a base result set
							// when searching for a category
							<div className={'search-listing'}>
								<Loop
									over={productCategoryApi}
									filter={{
										query: "name contains ?",
										args: [query],
										pageIndex: 1 as uint,
										pageSize: 10 as uint
									}}
								>
									{(category) => {
										return (
											<Link
												href={"/category/" + category.slug}
											>
												<li className={'search-listing-category'}>
													<i className={'fas fa-tag'} />
													<span>{highlightMatch(category.name, query)}</span>
												</li>
											</Link>
										)
									}}
								</Loop>
							</div>
						)}
					</div>
				</div>

				<div className="site-header__actions">

					{/* sign in link */}
					{!user && <>
						<Link sm outlined href="/en-admin" className="site-nav__actions-login">
							<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 28 36" fill="none">
								<path d="M22.5106 9C22.5106 4.035 18.4756 0 13.5106 0C8.54556 0 4.51056 4.035 4.51056 9C4.51056 13.965 8.54556 18 13.5106 18C18.4756 18 22.5106 13.965 22.5106 9ZM7.51056 9C7.51056 5.685 10.1956 3 13.5106 3C16.8256 3 19.5106 5.685 19.5106 9C19.5106 12.315 16.8256 15 13.5106 15C10.1956 15 7.51056 12.315 7.51056 9Z" fill="currentColor" />
								<path d="M0.0105591 33V34.5C0.0105591 35.325 0.685559 36 1.51056 36C2.33556 36 3.01056 35.325 3.01056 34.5V33C3.01056 28.035 7.04556 24 12.0106 24H15.0106C19.9756 24 24.0106 28.035 24.0106 33V34.5C24.0106 35.325 24.6856 36 25.5106 36C26.3356 36 27.0106 35.325 27.0106 34.5V33C27.0106 26.385 21.6256 21 15.0106 21H12.0106C5.39556 21 0.0105591 26.385 0.0105591 33Z" fill="currentColor" />
							</svg>
							<span className="site-header__actions-login-label">
								{`Sign in`}
							</span>
						</Link>
					</>}

					{/* account */}
					{user && <>
						<Button sm outlined className="site-nav__actions-account">
							<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 28 36" fill="none">
								<path d="M22.5106 9C22.5106 4.035 18.4756 0 13.5106 0C8.54556 0 4.51056 4.035 4.51056 9C4.51056 13.965 8.54556 18 13.5106 18C18.4756 18 22.5106 13.965 22.5106 9ZM7.51056 9C7.51056 5.685 10.1956 3 13.5106 3C16.8256 3 19.5106 5.685 19.5106 9C19.5106 12.315 16.8256 15 13.5106 15C10.1956 15 7.51056 12.315 7.51056 9Z" fill="currentColor" />
								<path d="M0.0105591 33V34.5C0.0105591 35.325 0.685559 36 1.51056 36C2.33556 36 3.01056 35.325 3.01056 34.5V33C3.01056 28.035 7.04556 24 12.0106 24H15.0106C19.9756 24 24.0106 28.035 24.0106 33V34.5C24.0106 35.325 24.6856 36 25.5106 36C26.3356 36 27.0106 35.325 27.0106 34.5V33C27.0106 26.385 21.6256 21 15.0106 21H12.0106C5.39556 21 0.0105591 26.385 0.0105591 33Z" fill="currentColor" />
							</svg>
							<span className="site-header__actions-account-label">
								{`Account`}
							</span>
							{notificationCount > 0 && <>
								<NotificationBadge icon={notificationBellSVG} />
							</>}
						</Button>
					</>}

					{/* toggle basket */}
					<Button sm outlined className="site-nav__actions-basket" popoverTarget="basket_popover">
						<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 54 53" fill="none">
							<path d="M1.96524 4.44736C5.36241 4.84443 6.6198 5.3518 8.34045 7.07242C9.99492 9.16806 15.0245 27.4994 15.4657 30.5877L14.6716 33.7201C14.1642 35.7055 14.6054 37.779 15.8848 39.3894C17.1422 40.9997 19.0393 41.9262 21.0909 41.9262H44.1211C45.3344 41.9262 46.327 40.9335 46.327 39.7203C46.327 38.507 45.3344 37.5143 44.1211 37.5143H21.0909C20.407 37.5143 19.7673 37.2055 19.3482 36.6761C18.929 36.1467 18.7746 35.4408 18.9511 34.779L19.3702 33.1025H36.4002C41.9372 33.1025 46.9447 29.595 48.8418 24.389L52.5699 14.1755C53.3861 11.9475 53.0552 9.47689 51.6875 7.53567C50.3198 5.59445 48.1139 4.44736 45.7314 4.44736H17.6496C16.4363 4.44736 15.4436 5.44003 15.4436 6.6533C15.4436 7.86656 16.4363 8.85923 17.6496 8.85923H45.7314C46.68 8.85923 47.5182 9.30042 48.0697 10.0725C48.6212 10.8446 48.7315 11.7931 48.4227 12.6755L44.6946 22.889C43.4372 26.3744 40.1062 28.6906 36.4002 28.6906H19.5467C18.3776 21.3448 12.7965 5.26356 11.495 3.96206C8.71546 1.18258 6.28891 0.454619 2.4726 0.0134315C1.25933 -0.118925 0.178408 0.74139 0.0239907 1.95465C-0.108367 3.16792 0.751959 4.24883 1.96524 4.40324V4.44736Z" fill="currentColor" />
							<path d="M18.7526 53C20.58 53 22.0615 51.5186 22.0615 49.6911C22.0615 47.8636 20.58 46.3822 18.7526 46.3822C16.9251 46.3822 15.4436 47.8636 15.4436 49.6911C15.4436 51.5186 16.9251 53 18.7526 53Z" fill="currentColor" />
							<path d="M43.018 53C44.8455 53 46.327 51.5186 46.327 49.6911C46.327 47.8636 44.8455 46.3822 43.018 46.3822C41.1906 46.3822 39.7091 47.8636 39.7091 49.6911C39.7091 51.5186 41.1906 53 43.018 53Z" fill="currentColor" />
						</svg>
						{loading && <>
							<Loading small />
						</>}
						{!loading && <>
							{cartIsEmpty() && <>
								<span className="site-header__actions-basket-label">
									{`Basket`}
								</span>
							</>}
							{!cartIsEmpty() && <>
								<span className="site-header__actions-basket-total">
									{formatCurrency(basketTotal, { currencyCode })}
								</span>
								<NotificationBadge count={basketCount} />
							</>}
						</>}
					</Button>
					<div className="site-header__basket-wrapper" popover="auto" id="basket_popover">
						<header>
							{loading && <>
								<Loading small />
							</>}
							{!loading && <>
								{cartIsEmpty() && <>
									<span>
										{`Basket is empty`}
									</span>
								</>}
								{!cartIsEmpty() && <>
									<span>
										{formatCurrency(basketTotal, { currencyCode })}
									</span>
									<NotificationBadge count={basketCount} />
								</>}
							</>}
						</header>
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
							<a href="/cart" className="btn btn-primary site-header__basket-checkout">
								{`View order`}
							</a>
						</>}
					</div>


					{/* VAT switch */}
					<div className="site-header__actions-vat">
						<Input type="checkbox" xs isSwitch flipped label={`Ex VAT`} onChange={() => setLessTax(!lessTax)} value={lessTax} noWrapper />
					</div>

				</div>

				{/* primary nav */}
				{primaryLinks?.length > 0 && <>
					<div className="site-header__nav">
						<button type="button" className="btn site-header__nav-trigger" popoverTarget="menu_popover">
							<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 15 14">
								<path d="M.97 2.41h12.08c.46 0 .83-.36.83-.8a.82.82 0 00-.83-.82H.97a.82.82 0 00-.83.81c0 .45.37.81.83.81zM13.05 6.19H.97A.82.82 0 00.14 7c0 .45.37.81.83.81h12.08c.46 0 .83-.36.83-.81a.82.82 0 00-.83-.81zM7.77 11.59H.97a.82.82 0 00-.83.8c0 .46.37.82.83.82h6.8c.46 0 .83-.36.83-.81a.82.82 0 00-.83-.81z" fill="currentColor" />
							</svg>
						</button>
						<div className="site-header__nav-links-wrapper" popover="auto" id="menu_popover">
							<menu className="site-header__nav-links">
								{primaryLinks.map(link => {
									return <li>
										{link.url && link.url.length > 0 && <>
											<a href={link.url}>
												{link.label}
											</a>
										</>}
										{link.popoverTarget && link.popoverTarget.length > 0 && <>
											<button type="button" className="btn" popoverTarget={link.popoverTarget}>
												{link.label}
											</button>
										</>}
									</li>;
								})}
							</menu>
						</div>
					</div>
				</>}
				<div className="site-header__products-wrapper" popover="auto" id="products_popover">
					{!productSubCategories && <Loading />}
					{productSubCategories && <>
						<h2 className="site-header__products-title">
							{productCategory?.parentId == null && <>
								{`Products`}
							</>}
							{productCategory?.parentId != null && <>
								<button type="button" className="btn" onClick={() => {
									if (productCategory?.parentId) {
										setCategoryId(productCategory.parentId!)
									}
								}}>
									{productCategory.name}
								</button>
							</>}
						</h2>
						{productCategory && <>
							<a href={productCategory.primaryUrl} className="site-header__products-link">
								{productCategory.parentId == null && <>
									{`All products`}
								</>}
								{productCategory.parentId != null && <>
									{`All ${productCategory.name}`}
								</>}
							</a>
						</>}
						{/*
						<h3 className="site-header__products-subtitle">
							{`All products`}
						</h3>
						*/}
						<menu className="site-header__products">
							{productSubCategories.map(subcategory => {
								// NB: filter on isDraft?
								return <li>
									<button type="button" className="btn site-header__products-link" onClick={() => {
										setCategoryId(subcategory.id)
									}}>
										{subcategory.name}
									</button>
								</li>;
							})}
							{/*
							{productCategories?.results?.map(category => {
								// NB: filter on isDraft?
								return <li>
									<a className="site-header__products-link" href={category.primaryUrl}>
										{category.name}
									</a>
								</li>;
							})}
								*/}
						</menu>
					</>}
				</div>

				{showContact && <>
					<div className="site-header__contact">

						{contactHref && <>
							<a href={contactHref} className="site-header__tel">
								<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 45 44" fill="none">
									<path d="M41.2431 27.9068L35.3209 24.9558C32.6623 23.6178 29.4903 24.6259 28.0602 27.2287C26.19 26.4955 23.0547 24.3143 21.2395 22.5547C19.4793 20.7218 17.3158 17.6608 16.564 15.7912C17.7925 15.1496 18.7276 14.1049 19.2043 12.7852C19.7177 11.3738 19.626 9.8158 18.9476 8.47776L16.0506 2.66736C14.3821 -0.192015 9.79834 -0.301992 7.83648 0.284547C5.0312 1.10937 2.61097 3.07061 1.18083 5.6917C-0.726028 9.1926 0.044049 13.4267 1.05248 17.6424C2.95934 23.2879 6.333 28.4567 11.0268 32.9108C15.5556 37.6764 20.7078 41.049 26.52 43.0102C28.7386 43.5418 31.0304 44 33.2307 44C35.0275 44 36.751 43.6884 38.3278 42.8269C40.9498 41.3972 42.9116 38.9778 43.7367 36.1734C44.3051 34.1938 44.2134 29.6115 41.2431 27.9068ZM40.198 35.1469C39.648 37.0165 38.3278 38.6478 36.5493 39.601C34.1841 40.884 30.7004 40.2242 27.5101 39.4727C22.4313 37.768 17.7741 34.707 13.5937 30.308C9.26663 26.2022 6.20465 21.5466 4.53616 16.6343C3.74774 13.28 3.06935 9.79747 4.35281 7.43299C5.32457 5.67337 6.93806 4.33533 8.80825 3.78545C9.11995 3.6938 9.54165 3.65714 10 3.65714C11.1368 3.65714 12.5119 3.93208 12.7686 4.39032L15.6106 10.0907C15.8306 10.549 15.8673 11.0439 15.7023 11.5204C15.5372 11.997 15.1889 12.3636 14.7305 12.5652C14.1438 12.8218 13.7037 13.0051 13.6854 13.0051C12.9887 13.2984 12.5486 13.9582 12.5486 14.7097C12.567 18.2106 17.2791 23.8011 18.5809 25.1391C19.8827 26.4038 25.4566 31.0962 28.9769 31.1328C29.6736 31.1328 30.3154 30.7479 30.6271 30.1064C30.6271 30.1064 30.8654 29.6115 31.1955 28.9883C31.6722 28.1085 32.7356 27.7602 33.6157 28.2001L39.4463 31.0961C40.198 31.5361 40.4547 34.0472 40.143 35.1103L40.198 35.1469Z" fill="currentColor" />
								</svg>
								{contactNumber}
							</a>
						</>}

						{message && message?.length > 0 && <>
							<Html className="site-header__message">
								{message}
							</Html>
						</>}

					</div>
				</>}

			</div>
		</div>
	);
}

export default Header;
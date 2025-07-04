import Image from 'UI/Image';
import Html from 'UI/Html';
import NotificationBadge from 'UI/NotificationBadge';
import { useSession } from 'UI/Session';
import defaultLogoRef from './example-logo.png';
import { getContactLink } from 'UI/Functions/ContactTools';
import { useState, useEffect, useRef } from "react";
import { getUrl } from 'UI/FileRef';
import containerQueryPolyfillJs from './static/container-query-polyfill.js';
import popoverPolyfillJs from './static/popover.min.js';
import { lazyLoad } from 'UI/Functions/WebRequest';
import productCategoryApi, { ProductCategory } from 'Api/ProductCategory';
import useApi from "UI/Functions/UseApi";
import Loading from "UI/Loading";
//import BasketItem from 'UI/Product/Signpost';
import { useCart } from 'UI/Payments/CartSession';
//import { recurrenceText } from 'UI/Functions/Payments';
//import { formatCurrency } from 'UI/Functions/CurrencyTools';
//import RecentSearches from "UI/RecentSearches";
//import Loop from "UI/Loop";
import Link from "UI/Link";
import Button from "UI/Button";
import Input from "UI/Input";
import HeaderSearch from 'UI/Header/Search';
import HeaderBasket from 'UI/Header/Basket';

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
	const [parentCategory, setParentCategory] = useState<ProductCategory>();
	const [productCategory, setProductCategory] = useState<ProductCategory>();
	const [productSubCategories, setProductSubCategories] = useState<ProductCategory[]>();
	const [primaryMenuOpen, setPrimaryMenuOpen] = useState(false);
	const [primaryLinks, setPrimaryLinks] = useState<HeaderLinkProps[]>([]);
	const [secondaryLinks, setSecondaryLinks] = useState<HeaderLinkProps[]>([]);
	const primaryMenuRefs = useRef([]);

	const { session } = useSession();
	var { user } = session;

	// clear and assign refs each render
	primaryMenuRefs.current = [];

	const setPrimaryMenuRef = (el, index) => {
		if (el) {
			primaryMenuRefs.current[index] = el;
		}
	};

	function getScrollbarWidth() {
		const el = document.createElement('div');
		el.style.cssText = 'width:100px;height:100px;overflow:scroll;position:absolute;top:-9999px;';
		document.body.appendChild(el);
		const scrollBarWidth = el.offsetWidth - el.clientWidth;
		document.body.removeChild(el);
		return scrollBarWidth;
	}

	// save page vertical scroll position
	useEffect(() => {
		let ticking = false;
		let lastScrollY = 0;

		// save vertical scrollbar width for later
		document.documentElement.style.setProperty('--vertical-scroll-width', `${getScrollbarWidth()}px`);

		const updateScrollVar = () => {
			ticking = false;

			// as long as scrolling is still active ..
			if (document.documentElement.scrollHeight > document.documentElement.clientHeight) {
				document.documentElement.style.setProperty('--page-scroll-y', `${lastScrollY}px`);
			}
		};

		const onScroll = () => {
			lastScrollY = window.scrollY;

			if (!ticking) {
				ticking = true;
				requestAnimationFrame(updateScrollVar);
			}
		};

		onScroll();
		window.addEventListener('scroll', onScroll);

		return () => {
			window.removeEventListener('scroll', onScroll);
		};
	}, []);

	// set up handler to watch for primary menu popovers being closed
	useEffect(() => {
		requestAnimationFrame(() => {

			if (!primaryMenuRefs.current) {
				return;
			}

			const popoverEl = primaryMenuRefs.current[0];

			const handleToggle = (e) => {

				if (e.newState == "closed") {
					setPrimaryMenuOpen(false);
				}

			};

			if (popoverEl) {
				popoverEl.addEventListener('toggle', handleToggle);
			}

			return () => {
				if (popoverEl) {
					popoverEl.removeEventListener('toggle', handleToggle);
				}
			};

		});
	}, [primaryLinks.length]);

	function useScrollLock(lock: boolean) {
		useEffect(() => {
			if (!lock) return;

			const scrollY = window.scrollY || window.pageYOffset;
			const body = document.body;

			body.style.position = 'fixed';
			body.style.top = `-${scrollY}px`;
			body.style.left = '0';
			body.style.right = '0';
			body.style.overflow = 'hidden';
			body.dataset.scrollY = String(scrollY);

			return () => {
				const y = parseInt(body.dataset.scrollY || '0', 10);
				body.style.position = '';
				body.style.top = '';
				body.style.left = '';
				body.style.right = '';
				body.style.overflow = '';
				window.scrollTo(0, y);
				delete body.dataset.scrollY;
			};
		}, [lock]);
	}

	useScrollLock(primaryMenuOpen);

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
			pageSize: 50 as int,
			pageIndex: 0 as int,
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

	useApi(() => {

		/* unsupported - default to product id 1 instead
		if (!productCategory || !productCategory.parentId) {
			setParentCategory(null);
			return;
		}
		*/

		return productCategoryApi.load(productCategory?.parentId || 1 as uint, [
			productCategoryApi.includes!.primaryurl
		]).then(results => {
			setParentCategory(results);
		});
	}, [productCategory]);

	// temp
	demo = true;

	if (demo) {

		if (!contactNumber) {
			contactNumber = "01432 271 271";
		}

		if (!logoRef) {
			logoRef = defaultLogoRef;
		}

		if (!message || !message.length) {
			message = `Free UK Delivery Over &pound;50`;
		}

		if (!searchPlaceholder || !searchPlaceholder.length) {
			searchPlaceholder = `Search by name, category or code`
		}

	}

	const contactHref = contactNumber ? getContactLink(contactNumber) : "";

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

		// TODO: retrieve links from DB
		setPrimaryLinks([
			{
				labelPrefix: <>
					<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 15 14">
						<path d="M.97 2.41h12.08c.46 0 .83-.36.83-.8a.82.82 0 00-.83-.82H.97a.82.82 0 00-.83.81c0 .45.37.81.83.81zM13.05 6.19H.97A.82.82 0 00.14 7c0 .45.37.81.83.81h12.08c.46 0 .83-.36.83-.81a.82.82 0 00-.83-.81zM7.77 11.59H.97a.82.82 0 00-.83.8c0 .46.37.82.83.82h6.8c.46 0 .83-.36.83-.81a.82.82 0 00-.83-.81z" fill="currentColor" />
					</svg>
				</>,
				label: `View all categories`,
				popoverTarget: 'products_popover',
				productCategories: true
			},
		]);

		setSecondaryLinks([
			...(contactHref ? [{
				labelPrefix: <>
					<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 45 44" fill="none">
						<path d="M41.2431 27.9068L35.3209 24.9558C32.6623 23.6178 29.4903 24.6259 28.0602 27.2287C26.19 26.4955 23.0547 24.3143 21.2395 22.5547C19.4793 20.7218 17.3158 17.6608 16.564 15.7912C17.7925 15.1496 18.7276 14.1049 19.2043 12.7852C19.7177 11.3738 19.626 9.8158 18.9476 8.47776L16.0506 2.66736C14.3821 -0.192015 9.79834 -0.301992 7.83648 0.284547C5.0312 1.10937 2.61097 3.07061 1.18083 5.6917C-0.726028 9.1926 0.044049 13.4267 1.05248 17.6424C2.95934 23.2879 6.333 28.4567 11.0268 32.9108C15.5556 37.6764 20.7078 41.049 26.52 43.0102C28.7386 43.5418 31.0304 44 33.2307 44C35.0275 44 36.751 43.6884 38.3278 42.8269C40.9498 41.3972 42.9116 38.9778 43.7367 36.1734C44.3051 34.1938 44.2134 29.6115 41.2431 27.9068ZM40.198 35.1469C39.648 37.0165 38.3278 38.6478 36.5493 39.601C34.1841 40.884 30.7004 40.2242 27.5101 39.4727C22.4313 37.768 17.7741 34.707 13.5937 30.308C9.26663 26.2022 6.20465 21.5466 4.53616 16.6343C3.74774 13.28 3.06935 9.79747 4.35281 7.43299C5.32457 5.67337 6.93806 4.33533 8.80825 3.78545C9.11995 3.6938 9.54165 3.65714 10 3.65714C11.1368 3.65714 12.5119 3.93208 12.7686 4.39032L15.6106 10.0907C15.8306 10.549 15.8673 11.0439 15.7023 11.5204C15.5372 11.997 15.1889 12.3636 14.7305 12.5652C14.1438 12.8218 13.7037 13.0051 13.6854 13.0051C12.9887 13.2984 12.5486 13.9582 12.5486 14.7097C12.567 18.2106 17.2791 23.8011 18.5809 25.1391C19.8827 26.4038 25.4566 31.0962 28.9769 31.1328C29.6736 31.1328 30.3154 30.7479 30.6271 30.1064C30.6271 30.1064 30.8654 29.6115 31.1955 28.9883C31.6722 28.1085 32.7356 27.7602 33.6157 28.2001L39.4463 31.0961C40.198 31.5361 40.4547 34.0472 40.143 35.1103L40.198 35.1469Z" fill="currentColor" />
					</svg>
				</>,
				label: contactNumber,
				url: contactHref,
				hideLabelMobile: true
			}] : []),
			{
				label: `Free UK Delivery Over &pound;50`,
				url: `/delivery`,
				hideMobile: true
			},
			{
				label: `More`,
				labelSuffix: <i className="fr fr-chevron-down"></i>,
				popoverTarget: 'more_popover2',
				children: [
					{
						label: `Contact Us`,
						url: `/contact-us`
					},
					{
						label: `Delivery Information`,
						url: `/delivery`
					},
					{
						label: `About ActiCare`,
						url: `/about-us`
					},
				]
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

	return (
		<nav className="site-nav">
			<div className="site-nav__header">
				<div className="site-nav__header-internal">
					<span>
						<a href="/" className="site-nav__logo">
							<Image fileRef={logoRef} />
						</a>
						<HeaderSearch searchPlaceholder={searchPlaceholder} />
					</span>
					<span>
						<div className="site-nav__actions">

							{/* VAT switch */}
							<div className="site-nav__actions-vat">
								<Input type="checkbox" xs isSwitch flipped label={`Ex VAT`} onChange={() => setLessTax(!lessTax)} value={lessTax} noWrapper />
							</div>

							{/* toggle basket */}
							<HeaderBasket />

							{/* sign in link */}
							{!user && <>
								<Link sm outlined href="/en-admin" className="site-nav__actions-login">
									<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 28 36" fill="none">
										<path d="M22.5106 9C22.5106 4.035 18.4756 0 13.5106 0C8.54556 0 4.51056 4.035 4.51056 9C4.51056 13.965 8.54556 18 13.5106 18C18.4756 18 22.5106 13.965 22.5106 9ZM7.51056 9C7.51056 5.685 10.1956 3 13.5106 3C16.8256 3 19.5106 5.685 19.5106 9C19.5106 12.315 16.8256 15 13.5106 15C10.1956 15 7.51056 12.315 7.51056 9Z" fill="currentColor" />
										<path d="M0.0105591 33V34.5C0.0105591 35.325 0.685559 36 1.51056 36C2.33556 36 3.01056 35.325 3.01056 34.5V33C3.01056 28.035 7.04556 24 12.0106 24H15.0106C19.9756 24 24.0106 28.035 24.0106 33V34.5C24.0106 35.325 24.6856 36 25.5106 36C26.3356 36 27.0106 35.325 27.0106 34.5V33C27.0106 26.385 21.6256 21 15.0106 21H12.0106C5.39556 21 0.0105591 26.385 0.0105591 33Z" fill="currentColor" />
									</svg>
									<span className="sr-only">
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
									<span className="sr-only">
										{`Account`}
									</span>
									{notificationCount > 0 && <>
										<NotificationBadge icon={notificationBellSVG} />
									</>}
								</Button>
							</>}

						</div>
					</span>
				</div>
			</div>
			<div className="site-nav__subheader">
				<div className="site-nav__subheader-internal">
					<span>
						{primaryLinks.map((link, i) => {
							let linkClasses = "";

							if (link.hideLabelMobile) {
								linkClasses += "site-nav__primary-link--hide-label-mobile";
							}

							if (link.hideMobile) {
								linkClasses += " site-nav__primary-link--hide-mobile";
							}

							return link.url ? <>
								<Link href={link.url} className={`site-nav__primary-link ${linkClasses}`}>
									{link.labelPrefix}
									<Html tag="span">
										{link.label}
									</Html>
									{link.labelSuffix}
								</Link>
							</> : <>
									<div className="site-nav__primary">
										<Button sm variant="link" className="site-nav__primary-trigger" popoverTarget={link.popoverTarget}
											onClick={() => setPrimaryMenuOpen(!primaryMenuOpen)}>
										{link.labelPrefix}
										<Html tag="span">
											{link.label}
										</Html>
										{link.labelSuffix}
									</Button>
										<div key={`primary_${i}`} ref={(el) => setPrimaryMenuRef(el, i)} className="site-nav__primary-wrapper" popover="auto" id={link.popoverTarget}>
										{/* standard dropdown list of sublinks */}
										{link.children && <>
											<menu className="site-nav__primary-menu">
												{link.children.map(sublink => {
													return <li>
														{sublink.url && sublink.url.length > 0 && <>
															<Link href={sublink.url} className="site-nav__primary-sublink">
																{sublink.label}
															</Link>
														</>}
													</li>;
												})}
											</menu>
										</>}

										{/* special case - list product categories */}
										{link.productCategories && <>
											{!productSubCategories && <Loading />}
											{productSubCategories && <>

												{/* link back to parent category */}
												{productCategory?.parentId != null && <>
													<Button sm variant="link" className="site-nav__primary-back" onClick={() => {
														if (productCategory?.parentId) {
															setCategoryId(productCategory.parentId!)
														}
													}}>
														<i className="fr fr-arrow-90"></i>
														<span>
															{/* workaround for top level products currently labelled "Care Home & Nursing Home Supplies & Equipment" */}
															{parentCategory.id == 1 ? `All products` : parentCategory.name}
														</span>
													</Button>
													<h2 className="site-nav__primary-title">
														{productCategory.name}
													</h2>
												</>}

												{/* current category */}
												{productCategory && <>
													<Link href={productCategory.primaryUrl} className="site-nav__primary-sublink">
														<span>
															{productCategory.parentId == null && <>
																{`All products`}
															</>}
															{productCategory.parentId != null && <>
																{`All ${productCategory.name}`}
															</>}
														</span>
														<i className="fr fr-arrow-right"></i>
													</Link>
												</>}

												{/* subcategories */}
												<menu className="site-nav__primary-menu">
													{productSubCategories.map(subcategory => {
														// NB: filter on isDraft?
														return <li>
															<Button variant="link" className="site-nav__primary-sublink" externalLink={subcategory.primaryUrl} onClick={() => {
																setCategoryId(subcategory.id)
															}}>
																<span>
																	{subcategory.name}
																</span>
																<i className="fr fr-arrow-right"></i>
															</Button>
														</li>;
													})}
												</menu>
											</>}
										</>}
									</div>
								</div>
							</>;
						})}
					</span>
					<span>
						{secondaryLinks.map(link => {
							let linkClasses = "";

							if (link.hideLabelMobile) {
								linkClasses += "site-nav__secondary-link--hide-label-mobile";
							}

							if (link.hideMobile) {
								linkClasses += " site-nav__secondary-link--hide-mobile";
							}

							return link.url ? <>
								<Link href={link.url} className={`site-nav__secondary-link ${linkClasses}`}>
									{link.labelPrefix}
									<Html tag="span">
										{link.label}
									</Html>
									{link.labelSuffix}
								</Link>
							</> : <>
									<div className="site-nav__secondary">
										<Button sm variant="link" className="site-nav__secondary-trigger" popoverTarget={link.popoverTarget}>
											{link.labelPrefix}
											<Html tag="span">
												{link.label}
											</Html>
											{link.labelSuffix}
										</Button>
										<div className="site-nav__secondary-wrapper" popover="auto" id={link.popoverTarget}>
											<menu className="site-nav__secondary-menu">
												{link.children.map(sublink => {
													return <li>
														{sublink.url && sublink.url.length > 0 && <>
															<Link href={sublink.url} className="site-nav__secondary-sublink">
																{sublink.label}
															</Link>
														</>}
													</li>;
												})}
											</menu>
										</div>
									</div>
							</>;
						})}
					</span>
				</div>
			</div>
		</nav>
	);
}

export default Header;
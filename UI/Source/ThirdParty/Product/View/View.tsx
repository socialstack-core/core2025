import { Product } from 'Api/Product';
import ProductCarousel, {CarouselItem} from 'UI/Product/Carousel';
import ProductAbout from 'UI/Product/About';
import ProductAttributes from 'UI/Product/Attributes';
import ProductPrice from 'UI/Product/Price';
import ProductQuantity from 'UI/Product/Quantity';
import {useEffect, useState} from 'react';
import Breadcrumb, { Crumb } from 'UI/Breadcrumb';
import ProductHeader from 'UI/Product/Header';
import { useRouter } from 'UI/Router';
import Button from 'UI/Button';
import ProductVariants from 'UI/Product/Variants';


const ROOT_CATEGORY_ID: uint = 1 as uint;

/**
 * Props for the View component.
 */
interface ViewProps {
	/**
	 * Must have included at least productCategories, firstCategory, firstCategory.categoryBreadcrumb
	 * Usually provided by a graph.
	 */
	product: Product
}

/**
 * The View React component.
 * @param props React props.
 */
const View: React.FC<ViewProps> = (props) => {
	const { product } = props;
	const { pageState, setPage } = useRouter();
	const { query, url } = pageState;
	
	// Selected variant (if any) is..
	const variantSku = query?.get("sku");
	const currentVariant: Product | undefined = variantSku ? product.variants?.find(prod => prod.sku == variantSku) : undefined;

	// NB: if additional tabs are required, be sure to update $tab-count accordingly (see View.scss)
	enum ProductTab {
		About = `About this product`,
		Details = `Details & Specification`,
		// hide FAQs tab until we have content to show
		//FAQs = `FAQs`
	}
	const productTabs = Object.values(ProductTab);

	const [selectedTab, setSelectedTab] = useState(ProductTab.About);
	const [selectedThumbnail, setSelectedThumbnail] = useState<CarouselItem>();

	useEffect(() => {
		// here we reset the selected thumbnail and load the variant one in.
		setSelectedThumbnail((prev) => variantSku ? currentVariant : prev);
	}, [variantSku]);

	// TODO: approved?
	const isApproved = true;

	// variant checks
	const hasVariants = product.variants?.length > 0;

	var sortedPrices = hasVariants ? product.variants.map(product => {
		if (!product?.calculatedPrice?.length) {
			return null;
		}

		// The highest tier is always the cheapest per-unit price
		return product.calculatedPrice[product.calculatedPrice.length - 1];
	})
		.filter(price => !!price) // strip the nulls
		.sort((a, b) => a.amount - b.amount) : null;

	var cheapestPrice = sortedPrices?.length ? sortedPrices[0] : null;
	
	// Added the required home breadcrumb as well as  
	// overwrite of the root categories name to "All products"
	return <>
		<div className="ui-product-view">
			{/* breadcrumb links */}
			{product.breadcrumb && <Breadcrumb crumbs={[{ name: `Home`, href: '/' }, ...product.breadcrumb.map(crumb => {
				
				return {
					name: crumb.id === ROOT_CATEGORY_ID ? `All products` : crumb.name,
					href: crumb.primaryUrl
				} as Crumb;

			}), { name: product?.name, href: pageState.url }]} />}

			{/* product images */}
			<ProductCarousel 
				product={product} 
				currentVariant={currentVariant}
				// this holds the current selected thumbnail from
				// the useState above, this component tells
				// the product carousel what to render
				// a user can select a thumbnail
				// different to the one from the attribute matrix
				// thus we hold it as a separate state item. 
				// when the attribute matrix is mutated, it
				// should clear the selected thumbnail
				selectedThumbnail={selectedThumbnail}
				
				// little subscriber to listen to when the thumb
				// changes, this doesn't change the selected product
				// discovered by the attribute matrix, this is just
				// for looking at the different variants.
				onThumbSelected={(thumbInfo: CarouselItem) => {
					setSelectedThumbnail(thumbInfo);
				}}
			/>

			{/* featured / title / stock info */}
			<ProductHeader product={product} currentVariant={currentVariant} />

			{/*
			<TabSet className="ui-product-view__tabs">
				<Tab label={`About this product`}>
					<ProductAbout title={`About this product`} product={product} />
				</Tab>
				<Tab label={`Details & Specification`}>
					<ProductAttributes title={`Product details`} product={product} />
				</Tab>
				<Tab label={`FAQs`}>
					{`:: TODO ::`}
				</Tab>
			</TabSet>
			*/}

			{/* tab links */}
			<div className="ui-product-view__tab-links">
				{productTabs.map((tab, i) => {
					const linkId = `tab-link${i + 1}`;
					const panelId = `tab-panel${i + 1}`;
					const selected = selectedTab === tab ? true : undefined;

					return <>
						<div className="ui-product-view__tab-link">
							<input type="radio" name="months" id={linkId} aria-controls={panelId} checked={selected} />
							<label htmlFor={linkId}>
								{tab}
							</label>
						</div>
					</>;
				})}
				</div>

			{/* tab panels */}
			<div className="ui-product-view__tab-panels">
				{productTabs.map((tab, i) => {
					const panelId = `tab-panel${i + 1}`;

					return <>
						<div className="ui-product-view__tab-panel" id={panelId}>
							{tab == ProductTab.About && <>
								<ProductAbout title={`About this product`} product={product} currentVariant={currentVariant} />
							</>}

							{tab == ProductTab.Details && <>
								<ProductAttributes title={`Product details`} product={product} currentVariant={currentVariant} />
							</>}

							{tab == ProductTab.FAQs && <>
								{`:: TODO ::`}
							</>}
						</div>
					</>;
				})}
			</div>
			
			{/* price info */}
			<div className="ui-product-view__price-info">

				{/* approved? */}
				{isApproved && <>
					<div className="ui-product-view__price-info-approved">
						<i className="fr fr-thumbs-up"></i>
						{`Approved`}
					</div>
				</>}

				{/* product variants */}
				{hasVariants && <>
					<Button sm variant="primary" outlined className="ui-product-view__price-info-variants" popoverTarget="product_variants">
						<i className="fr fr-tasks"></i>
						<span>
							{`Options`}
						</span>
					</Button>
					<div id="product_variants" popover="auto">
						<div className="ui-product-view__price-info-variants-header">
							<span>
								{`Please select options`}
							</span>
						</div>
						<ProductVariants product={product} currentVariant={currentVariant} onChange={variant => {

							// Change the URL if needed. This triggers a re-render at this upper level which then ultimately
							// collects the variant and anything else necessary.
							// This way it is driven by url state and also pretty minimal, 
							// ensuring that the selected product is shareable.
							if (variant?.sku == variantSku) {
								return;
							}

							var nextQuery = new URLSearchParams(query);
							if (variant) {
								nextQuery.set("sku", variant.sku || '');
							} else {
								nextQuery.delete("sku");
							}

							const currentUrl = url.split('?')[0];

							var qs = nextQuery.toString();
							let nextUrl = currentUrl;

							if (qs && qs.length) {
								nextUrl += '?' + qs;
							}

							// Todo: router needs the ability to change query string without
							// causing a refresh. This will primarily fix a weird jank that you'll experience 
							// if you edit the dropdowns from an ?sku= page 
							// (it will cause a page load and the set of dropdowns will probably all be empty) 
							setPage(nextUrl);
						}} />
					</div>
				</>}

				{/* price */}
				<ProductPrice product={currentVariant || product} override={hasVariants && !currentVariant ? cheapestPrice : undefined} isFrom={!!hasVariants && !currentVariant} />

				{/* quantity / add to order, only present if there is no variants or a variant is selected. */}
				{(!(product.variants?.length) || currentVariant) && <ProductQuantity product={currentVariant || product} />}
			</div>

		</div>
	</>;
}

export default View;
import { Product } from 'Api/Product';
import Canvas from 'UI/Canvas';
import Quantity from 'UI/Product/Quantity';
import ProductCarousel from 'UI/Product/Carousel';
import ProductSpecifications from 'UI/Product/Specifications';
import Promotion from 'UI/Promotion';
import ProductAbout from 'UI/Product/About';
import ProductAttributes from 'UI/Product/Attributes';
import ProductPrice from 'UI/Product/Price';
import ProductStock from 'UI/Product/Stock';
import ProductSubtitle from 'UI/Product/Subtitle';
import ProductQuantity from 'UI/Product/Quantity';
import Button from 'UI/Button';

import { useState } from 'react';
import Breadcrumb from 'UI/Breadcrumb';
import ProductHeader from 'UI/Product/Header';


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

	enum ProductTab {
		About = `About this product`,
		Details = `Details & Specification`,
		FAQs = `FAQs`
	}
	const productTabs = Object.values(ProductTab);

	const [selectedTab, setSelectedTab] = useState(ProductTab.About);

	// TODO: approved?
	const isApproved = true;

	// TODO: check for associated info
	const infoTitle = `Note from Acticare`;
	const infoDescription = `This product has a lower max weight capacity than most bariatric beds as this is designed for residents who solely need more bed space.`;

	return <>
		<div className="ui-product-view">
			{/* breadcrumb links */}
			<Breadcrumb />

			{/* product images */}
			<ProductCarousel product={product} />

			{/* featured / title / stock info */}
			<ProductHeader product={product} />

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
								<ProductAbout title={`About this product`} product={product} />
							</>}

							{tab == ProductTab.Details && <>
								<ProductAttributes title={`Product details`} product={product} />
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

				{/* product info */}
				{infoDescription?.length > 0 && <>
					<Button sm variant="warning" className="ui-product-view__price-info-note" popoverTarget="product_note">
						<i className="fr fr-exclamation-circle"></i>
						<span>
							{infoTitle?.length > 0 ? infoTitle : `Note`}
						</span>
					</Button>
					<div id="product_note" popover="auto">
						<div className="ui-product-view__price-info-note-header">
							<i className="fr fr-exclamation-circle"></i>
							<span>
								{infoTitle?.length > 0 ? infoTitle : `Note`}
							</span>
						</div>
						{infoDescription}
					</div>
				</>}

				{/* price */}
				<ProductPrice product={product} />

				{/* quantity / add to order */}
				<ProductQuantity product={product} />
			</div>

		</div>
	</>;
}

export default View;
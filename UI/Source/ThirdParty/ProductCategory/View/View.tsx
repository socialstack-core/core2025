import productCategoryApi, { ProductCategory } from "Api/ProductCategory";
import CategoryFilters from 'UI/ProductCategory/Filters';
import ProductList from 'UI/Product/List';
import Loading from 'UI/Loading';
import productApi from 'Api/Product';
import useApi from "UI/Functions/UseApi";
import Input from 'UI/Input';
import DualRange from 'UI/DualRange';
import Promotion from 'UI/Promotion';
import { useState } from "react";

/**
 * Props for the View component.
 */
interface ViewProps {
	// Connected via a graph in the page, which is also where the includes are defined.
	// This component requires at least the following includes:
	// productCategories, productCategories.primaryUrl
	productCategory: ProductCategory
}

/**
 * The View React component.
 * @param props React props.
 */
const View: React.FC<ViewProps> = (props) => {
	const { productCategory } = props;
	const [showApprovedOnly, setShowApprovedOnly] = useState();
	const [showInStockOnly, setShowInStockOnly] = useState();
	const [viewStyle, setViewStyle] = useState('large-thumbs');
	const [sortOrder, setSortOrder] = useState('most-popular');
	const [pagination, setPagination] = useState('page1');

	// SSR compliant useApi call:
	const [products] = useApi(() => {
		return productApi.list({
			query: 'ProductCategories=?',
			args: [productCategory.id]
		}, [
			productApi.includes.primaryurl,
			productApi.includes.calculatedprice,
		])
	}, [
		productCategory
	]);

	// TODO: calculate lowest and highest price
	let lowestPrice = 5;
	let highestPrice = 5000;
	let step = 1;
	//let step = Math.round((highestPrice - lowestPrice) / 20);

	let fromPrice = 500;
	let toPrice = 3000;

	const [productCategories] = useApi(() => {
		return productCategoryApi.list({
			query: 'ParentId=?',
			args: [productCategory.id]
		}, [
			productCategoryApi.includes!.primaryurl
		])
	}, [
		productCategory
	]);

	if (!products || !productCategories) {
		return <Loading />;
	}

	let GBPound = new Intl.NumberFormat('en-GB', {
		style: 'currency',
		currency: 'GBP',
		minimumFractionDigits: 0,
		maximumFractionDigits: 0
	});

	return (
		<div className="ui-productcategory-view">
			<div className="ui-productcategory-view__filters-wrapper">
				<div className="ui-productcategory-view__filters">
					<fieldset>
						<legend>
							{`Show / hide products`}
						</legend>
						<Input type="checkbox" isSwitch label={`Only show approved`} value={showApprovedOnly} name="show-approved" noWrapper />
						<Input type="checkbox" isSwitch label={`Only show in stock`} value={showInStockOnly} name="show-in-stock" noWrapper />
					</fieldset>

					{productCategories?.results?.length > 0 && <>
						<fieldset>
							<legend>
								{`All product categories`}
							</legend>
							<CategoryFilters content={productCategories.results} />
							{/* TODO: limit list to 3 items with "show more" */}
						</fieldset>
					</>}

					<DualRange className="ui-productcategory-view__price" label={`Price`} numberFormat={GBPound}
						min={lowestPrice} max={highestPrice} step={step} defaultFrom={fromPrice} defaultTo={toPrice} />

				</div>
				<Promotion title={`Get 10% Off Our Bedroom Bestsellers`} description={`Save now on top-rated beds and accessories - Limited time offer`} url={`#`} />
			</div>

			<header className="ui-productcategory-view__header">
				<Input type="select" aria-label={`Sort by`} value={sortOrder} noWrapper>
					<option value="most-popular">
						{`Most popular`}
					</option>
				</Input>

				{/* replace this with more typical pagination below product list? */}
				<Input type="select" aria-label={`Show`} value={pagination} noWrapper>
					<option value="page1">
						{`20 out of 1,500 products`}
					</option>
				</Input>

				<div className="btn-group" role="group" aria-label={`Select view style`}>
					<Input type="radio" noWrapper label={`List`} groupIcon="fr-list" groupVariant="primary" value={viewStyle == 'list'} onChange={() => setViewStyle('list')} name="view-style" />
					<Input type="radio" noWrapper label={`Small thumbnails`} groupIcon="fr-th-list" groupVariant="primary" value={viewStyle == 'small-thumbs'} onChange={() => setViewStyle('small-thumbs')} name="view-style" />
					<Input type="radio" noWrapper label={`Large thumbnails`} groupIcon="fr-grid" groupVariant="primary" value={viewStyle == 'large-thumbs'} onChange={() => setViewStyle('large-thumbs')} name="view-style" />
				</div>
			</header>

			<ProductList content={products.results} viewStyle={viewStyle} />
		</div>
	);
}

export default View;
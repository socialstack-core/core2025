import {ProductCategory} from "Api/ProductCategory";
import CategoryFilters from 'UI/ProductCategory/Filters';
import ProductList from 'UI/Product/List';
import Loading from 'UI/Loading';
import productApi from 'Api/Product';
import useApi from "UI/Functions/UseApi";
import Input from 'UI/Input';
import DualRange from 'UI/DualRange';
import Promotion from 'UI/Promotion';
import {useState} from "react";
import searchApi, {ProductSearchType, SortDirection} from "Api/ProductSearchController";
import {ProductAttributeValue } from "Api/ProductAttributeValue";
import {useRouter} from "UI/Router";
import {AttributeFacetGroup, AttributeValueFacet, ProductCategoryFacet} from "UI/Product/Search/Facets";
import FilterList from "UI/Product/Search/FilterList";
import Breadcrumb from "UI/Breadcrumb";

const ROOT_CATEGORY_ID: uint = 1 as uint;

/**
 * Props for the View component.
 */
interface ViewProps {
	// Connected via a graph in the page, which is also where the includes are defined.
	// This component requires at least the following includes:
	// productCategories, productCategories.primaryUrl
	productCategory: ProductCategory
}

type SecondaryIncludes = {
	attributeValueFacets?: {
		results: AttributeValueFacet,
	}
	productCategoryFacets?: {
		results: ProductCategoryFacet[]
	}
}

/**
 * The View React component.
 * @param props React props.
 */
const View: React.FC<ViewProps> = (props) => {
	const { productCategory } = props;
	
	const [showInStockOnly, setShowInStockOnly] = useState(false);
	
	const [viewStyle, setViewStyle] = useState('large-thumbs');
	const [sortOrder, setSortOrder] = useState('relevance');
	const [pagination, setPagination] = useState('page1');
	const [minPrice, setMinPrice] = useState<double>(1);
	const [maxPrice, setMaxPrice] = useState<double>(5000);
	
	const { pageState } = useRouter();
	const { query } = pageState;
	const [selectedFacets, setSelectedFacets] = useState<ProductAttributeValue[]>([]);

	
	// TODO: calculate lowest and highest price
	let lowestPrice = 1;
	let highestPrice = 5000;
	let step = 1;
	//let step = Math.round((highestPrice - lowestPrice) / 20);


	var initialPageStr = query?.get("page") || "";
	var initialPageOffset = (parseInt(initialPageStr) || 1) - 1;
	const searchText = query?.get("q") ?? '';

	const [products] = useApi(() => {
		const appliedFacets = [
			{
				mapping: "productcategories",
				ids: [productCategory.id]
			}
		];

		if (selectedFacets.length) {
			// Group them up by attribute. This is important because ultimately the server wants to make a query of the form..
			// (Red or Blue or Green) and (200mm or 100mm)
			// The server provides this or/and behaviour via or'ing inside each appliedFacet
			// whilst and'ing the appliedFacets together.

			// Map of attribId -> attribValueIds.
			const uniqueAttributeMap : Map<int, int[]> = new Map<int, int[]>();

			selectedFacets.forEach(attribValue => {
				const attributeId = attribValue.productAttributeId;
				let attributeValueSet = uniqueAttributeMap.get(attributeId);

				if (!attributeValueSet) {
					attributeValueSet = [];
					uniqueAttributeMap.set(attributeId, attributeValueSet);
				}

				attributeValueSet.push(attribValue.id);
			});

			// Next for each attribute value group, add that appliedFacet.
			uniqueAttributeMap.forEach((attribValueIds) => {
				appliedFacets.push({
					mapping: 'attributes',
					ids: attribValueIds
				});
			});
		}

		return searchApi.faceted({
			query: searchText,
			pageOffset: initialPageOffset as int,
			searchType: ProductSearchType.Expansive,
			pageSize: 20 as uint,
			minPrice: minPrice,
			maxPrice: maxPrice,
			inStockOnly: showInStockOnly,
			sortOrder: {
				field: "relevance",
				direction: SortDirection.DESC
			},
			appliedFacets
		}, [
			productApi.includes.calculatedprice,
			productApi.includes.primaryCategory,

			// Plus then includes on the facets (the attribute and category selectors)
			productApi.includes.productCategoryFacets,
			productApi.includes.productCategoryFacets.category,
			productApi.includes.productCategoryFacets.category.primaryurl,
			productApi.includes.attributeValueFacets.value.attribute.attributeGroup
		])
	}, [selectedFacets, minPrice, maxPrice, showInStockOnly, searchText]);


	if (!products) {
		return <Loading />;
	}

	const facets = (products.secondary as SecondaryIncludes);
	const { attributeValueFacets, productCategoryFacets } = facets;

	
	const categoryFacets = (productCategoryFacets?.results || []) as ProductCategoryFacet[];
	const attributeFacets = (attributeValueFacets?.results || []) as AttributeValueFacet[];
	
	
	var attributeMap = new Map<uint, AttributeFacetGroup>();

	attributeFacets.forEach(facet => {
		if (!(facet.value?.attribute)) {
			return;
		}

		var attribId = facet.value.productAttributeId;
		var grouping = attributeMap.get(attribId);

		if (!grouping) {
			grouping = {
				attribute: facet.value.attribute,
				facetValues: []
			} as AttributeFacetGroup;

			attributeMap.set(attribId, grouping);
		}

		grouping.facetValues.push(facet);
	});

	const attributeFacetGroups = Array.from(attributeMap.values());

	let GBPound = new Intl.NumberFormat('en-GB', {
		style: 'currency',
		currency: 'GBP',
		minimumFractionDigits: 0,
		maximumFractionDigits: 0
	});
	
	let queryString = pageState?.query?.toString();
	
	// query string can be null, this should only run
	// when query string is not null (empty strings are falsy, so we
	// check that it's not null)
	if (queryString !== null && !queryString.startsWith("?")) {
		queryString = '?' + queryString;
	}
	// if its falsy, which means empty, null or undefined
	// we give it an empty value
	else if (!queryString) {
		queryString = '';
	}
	
	// moved breadcrumbs map function out of 
	// the component props and into a seperate
	// const up here, the ticket requires a home link
	// so added the requirements in below. 
	const breadcrumbs = [
		{
			name: 'Home',
			href: '/'
		}, 
		...(productCategory.breadcrumb ?? []).map(breadcrumb => {
			return ({
				name: breadcrumb.id === ROOT_CATEGORY_ID ? `All products` : breadcrumb.name,
				href: breadcrumb.primaryUrl
			})
		})
	]

	return (
		<>
			<Breadcrumb
				crumbs={breadcrumbs}
			/>
			<div className="ui-productcategory-view">
				<div className="ui-productcategory-view__filters-wrapper">
					<div className="ui-productcategory-view__filters">
						<fieldset>
							<legend>
								{`Show / hide products`}
							</legend>
							<Input type="checkbox" onChange={(ev) => setShowInStockOnly((ev.target as HTMLInputElement).checked)} isSwitch flipped label={`Only show in stock`} value={showInStockOnly} name="show-in-stock" noWrapper />
						</fieldset>
						<fieldset>
							<legend>
								{`All product categories`}
							</legend>
							<CategoryFilters collection={products} currentCategory={productCategory}/>
							{/* TODO: limit list to 3 items with "show more" */}
						</fieldset>
						<DualRange 
							className="ui-productcategory-view__price" 
							label={`Price`} 
							numberFormat={GBPound}
							min={lowestPrice} 
							max={highestPrice} 
							step={step} 
							defaultFrom={minPrice} 
							defaultTo={maxPrice} 
							onChange={(from: number, to: number) => {
								setMinPrice(from);
								setMaxPrice(to);
							}}
						/>
						
						{/* attributes */}
						{attributeFacetGroups.length > 0 && <>
							{
								attributeFacetGroups.map(facet => {
									// can include primaryUrl etc on facets as well if needed
									// here though it is (probably, I haven't looked at the designs recently) exclusively a button
									// which then restricts the search.
	
									return <>
										<fieldset key={facet.attribute.id}>
											<legend>
												{facet.attribute.name}
											</legend>
											<FilterList 
												selectedAttributeValues={selectedFacets}
												facets={facet.facetValues} 
												units={facet.attribute.units}
												maxVisible={4 as int}
												setSelectedAttributeValues={setSelectedFacets}
											/>
										</fieldset>
									</>;
								})
							}
						</>}
					</div>
					<Promotion title={`Get 10% Off Our Bedroom Bestsellers`} description={`Save now on top-rated beds and accessories - Limited time offer`} url={`#`} />
				</div>
	
				<header className="ui-productcategory-view__header">
					<Input type="select" aria-label={`Sort by`} value={sortOrder} noWrapper>
						<option value="relevance">
							{`Relevance`}
						</option>
					</Input>
	
					{/* replace this with more typical pagination below product list? */}
					<Input type="select" aria-label={`Show`} value={pagination} noWrapper>
						<option value="page1">
							{`20 out of 1,500 products`}
						</option>
					</Input>
	
					<div className="btn-group ui-btn-group" role="group" aria-label={`Select view style`}>
						<Input type="radio" noWrapper label={`List`} groupIcon="fr-list" groupVariant="primary" value='list' checked={viewStyle == 'list'} onChange={() => setViewStyle('list')} name="view-style" />
						<Input type="radio" noWrapper label={`Small thumbnails`} groupIcon="fr-th-list" groupVariant="primary" value='small-thumbs' checked={viewStyle == 'small-thumbs'} onChange={() => setViewStyle('small-thumbs')} name="view-style" />
						<Input type="radio" noWrapper label={`Large thumbnails`} groupIcon="fr-grid" groupVariant="primary" value='large-thumbs' checked={viewStyle == 'large-thumbs'} onChange={() => setViewStyle('large-thumbs')} name="view-style" />
					</div>
				</header>
	
				<ProductList content={products.results} viewStyle={viewStyle} />
			</div>
		</>
	);
}

export default View;
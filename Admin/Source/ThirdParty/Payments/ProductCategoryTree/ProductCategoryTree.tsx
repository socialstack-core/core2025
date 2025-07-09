import { useEffect, useState } from "react";
import { useRouter } from "UI/Router";
import SubHeader from "Admin/SubHeader";
import TreeView, { buildBreadcrumbs } from "Admin/TreeView";
import Input from "UI/Input";
import Image from "UI/Image";
import Loading from "UI/Loading";
import { MultiSelectBox } from "./MultiSelect";

import productCategoryApi, {ProductCategory} from "Api/ProductCategory";
import searchApi, {ProductSearchAppliedFacet, ProductSearchType, SortDirection} from "Api/ProductSearchController";
import { ProductIncludes} from "Api/Includes";
import ProductApi, { Product } from "Api/Product";
import { ApiList } from "UI/Functions/WebRequest";
import { ProductAttribute } from "Api/ProductAttribute";
import { ProductAttributeValue } from "Api/ProductAttributeValue";
import Link from "UI/Link";
import Button from "UI/Button";
import {SortField} from "Admin/AutoList";
import Time from "UI/Time";

/**
 * Props for the ProductCategoryTree component
 * @typedef {Object} ProductCategoryTreeProps
 * @property {boolean} noCreate - Determines whether the "New category" and "New product" buttons are shown
 */
export type ProductCategoryTreeProps = {
	noCreate: boolean;
};

/**
 * View type for the main page
 * @typedef {"list" | "tree"} PageViewType
 */
type PageViewType = "list" | "tree";

/**
 * Displays the product category tree or product list view with search and filter functionality.
 *
 * @component
 * @param {ProductCategoryTreeProps} props - Component props
 * @returns {JSX.Element}
 */
export default function ProductCategoryTree({ noCreate }: ProductCategoryTreeProps) {


	const { pageState } = useRouter();
	
	const [viewType, setViewType] = useState<PageViewType>(pageState.query?.get("query") ? "list" : "tree");
	const [searchQuery, setSearchQuery] = useState(pageState.query?.get("query") ?? '');
	const path = pageState.query?.get("path") || "";


	const breadcrumbs = buildBreadcrumbs("/en-admin/product", "Products", path, "/en-admin/product");

	const addProductUrl = "/en-admin/product/add";
	const addCategoryUrl = "/en-admin/productcategory/add";

	return (
		<>
			<SubHeader title="Edit Products" breadcrumbs={breadcrumbs} />

			<div className="sitemap__wrapper product-category-tree">
				<div className="page-controls">
					<div className="btn-group view-toggle" role="group" aria-label="Select view style">
						<Input
							type="radio"
							noWrapper
							label="Tree"
							groupIcon="fr-grid"
							groupVariant="primary"
							value={viewType === "tree"}
							onChange={() => setViewType("tree")}
							name="view-style"
						/>
						<Input
							type="radio"
							noWrapper
							label="List"
							groupIcon="fr-th-list"
							groupVariant="primary"
							value={viewType === "list"}
							onChange={() => setViewType("list")}
							name="view-style"
						/>
					</div>
					<div className="product-search">
						<Input
							type="search"
							defaultValue={searchQuery}
							onInput={(ev) => {
								setSearchQuery((ev.target as HTMLInputElement).value)
								setViewType("list")
							}}
							onFocus={() => {
								setViewType("list");
							}}
							placeholder="Filter products"
						/>
					</div>
				</div>
				<div className="sitemap__internal">
					{viewType === "tree" ? (
						<TreeView
							onLoadData={(path) => productCategoryApi.getTreeNodePath(path).then((resp) => resp)}
						/>
					) : (
						<ProductListView query={searchQuery} />
					)}
				</div>

				{!noCreate && (
					<footer className="admin-page__footer">
						<a href={addCategoryUrl} className="btn btn-primary">
							New category
						</a>
						<a href={addProductUrl} className="btn btn-primary">
							New product
						</a>
					</footer>
				)}
			</div>
		</>
	);
}

/**
 * Props for ProductListView
 * @typedef {Object} ProductListViewProps
 * @property {string} [query] - Optional search query
 */
type ProductListViewProps = {
	query?: string;
};

const hydrateFacets = (): ProductSearchAppliedFacet[] => {
	var req = parseProductUrl(location.href);
	
	var facets: ProductSearchAppliedFacet[] = [];
	
	Object.keys(req.facet).map((mapping: string) => {
		if (!req.facet[mapping]) {
			return;
		}
		facets.push({ mapping, ids: req.facet[mapping] })
	})
	
	return facets;
}

/**
 * Displays a list view of products, including filtering and search functionality.
 *
 * @component
 * @param {ProductListViewProps} props - Component props
 * @returns {JSX.Element}
 */
const ProductListView: React.FC<ProductListViewProps> = (props: ProductListViewProps) => {
	const [searchResults, setSearchResults] = useState<ApiList<Product>>();
	const [pageOffset, setPageOffset] = useState(0);
	const [attributeFacets, setAttributeFacets] = useState<ProductSearchAppliedFacet[]>(hydrateFacets());
	const [loading, setLoading] = useState<boolean>(false);
	
	const [sortOrder, setSortOrder] = useState<SortField>({ field: '_id', direction: 'asc' });

	const { pageState } = useRouter();
	
	let categoryFacets = attributeFacets.filter(item => item.mapping == 'productcategories').flatMap(attr => attr.ids) as ulong[];
	
	if (categoryFacets.length == 0) {
		categoryFacets.push(1 as uint); // top level category.
	}

	const queryPayload = {
		query: props.query,
		pageOffset: pageOffset as uint,
		appliedFacets: [
			{
				mapping: "attributes",
				ids: attributeFacets.filter(item => item.mapping != 'productcategories').flatMap(attr => attr.ids) as ulong[]
			},
			{
				mapping: "productcategories",
				ids: categoryFacets
			}
		],
		searchType: ProductSearchType.Reductive,
		sortOrder: {
			field: sortOrder.field,
			direction: sortOrder.direction === 'asc' ? SortDirection.ASC : SortDirection.DESC
		}
	};

	useEffect(() => {
		if (props.query !== pageState.query?.get("query")) {
			setAttributeFacets([]);
		}
	}, [props.query]);

	useEffect(() => {

		let facetStr = '';

		attributeFacets.forEach(facet => {
			if (!facet.ids) {
				return;
			}
			facetStr += '&facet[' + facet.mapping + ']=' + facet.ids.join(',');
		})
		
		const url = location.pathname + '?query=' + props.query + facetStr;
		history.replaceState(null, '',url);
		
		searchApi
			.faceted(queryPayload, [
				new ProductIncludes().attributeValueFacets,
				new ProductIncludes().productCategoryFacets,
				new ProductIncludes().attributes.attribute,
				new ProductIncludes().productcategories,
			])
			.then((response) => {
				setSearchResults(response);
				setLoading(false);
			});
	}, [props.query, attributeFacets, sortOrder]);

	useEffect(() => {
		
		if (!props.query) {
			const GET = parseProductUrl(location.href);

			if (GET.query) {
				// update all from URL.
				
				const facets: ProductSearchAppliedFacet[] = [];
				
				Object.keys(GET.facet).forEach((mapping: string) => {
					facets.push({ mapping, ids:GET.facet[mapping] });
				})
				
				setAttributeFacets(facets);
				
			}
		}
		
	}, [props.query]);
	
	const changeSortField = (field: string) => {
		if (field == sortOrder.field) {
			sortOrder.direction = sortOrder.direction === 'asc' ? 'desc' : 'asc';
			setSortOrder({...sortOrder});
			return;
		}
		setSortOrder({
			field, direction: 'asc'
		})
	}

	return (
		<div className="admin-page__internal">
			{loading ? (
				<Loading />
			) : (
				<div className="product-collection">
					{searchResults?.secondary && (
						<SearchAttributeFilter
							results={searchResults}
							onFilterChange={(mappingName, values) => {
								const facet = attributeFacets.find((facet) => facet.mapping === mappingName);

								if (!facet) {
									attributeFacets.push({ mapping: mappingName, ids: values });
								} else {
									facet.ids = values;
								}
								setAttributeFacets([...attributeFacets]);
							}}
							value={attributeFacets}
						/>
					)}
					<table className="table products-table">
						<thead>
						<tr>
							<th 
								onClick={() => changeSortField('_id')}
								className={sortOrder.field === '_id' ? 'active' : ''}
							>
								Id
								{sortOrder.field === '_id' ? <i className={'fas fa-chevron-' + (sortOrder.direction == 'asc' ? 'up' : 'down')}/> : null}
							</th>
							<th>Image</th>
							<th 
								onClick={() => changeSortField('Name.en')}
								className={sortOrder.field === 'name.en' ? 'active' : ''}
							>
								Name
								{sortOrder.field === 'name.en' ? <i className={'fas fa-chevron-' + (sortOrder.direction == 'asc' ? 'up' : 'down')}/> : null}
							</th>
							<th 
								onClick={() => changeSortField('Sku')}
								className={sortOrder.field === 'Sku' ? 'active' : ''}
							>
								SKU
								{sortOrder.field === 'Sku' ? <i className={'fas fa-chevron-' + (sortOrder.direction == 'asc' ? 'up' : 'down')}/> : null}
							</th>
							<th
								onClick={() => changeSortField('CreatedUtc')}
								className={sortOrder.field === 'CreatedUtc' ? 'active' : ''}
							>
								Created
								{sortOrder.field === 'CreatedUtc' ? <i className={'fas fa-chevron-' + (sortOrder.direction == 'asc' ? 'up' : 'down')}/> : null}
							</th>
							<th
								onClick={() => changeSortField('EditedUtc')}
								className={sortOrder.field === 'EditedUtc' ? 'active' : ''}
							>
								Created
								{sortOrder.field === 'EditedUtc' ? <i className={'fas fa-chevron-' + (sortOrder.direction == 'asc' ? 'up' : 'down')}/> : null}
							</th>

							<th>Actions</th>
						</tr>
						</thead>
						<tbody>
						{searchResults?.results.map((product) => (
							<tr key={product.id}>
								<td>{product.id}</td>
								<td>{product.featureRef ? <Image fileRef={product.featureRef} /> : "No image available"}</td>
								<td>{product.name}</td>
								<td>{product.sku}</td>
								<td><Time date={product.createdUtc} /></td>
								<td><Time date={product.editedUtc} /></td>
								<td>
									<Link href={'/en-admin/product/' + product.id}>
										<Button>{`Edit product`}</Button>
									</Link>
								</td>
							</tr>
						))}
						</tbody>
					</table>
				</div>
			)}
		</div>
	);
};

/**
 * Props for SearchAttributeFilter component
 * @typedef {Object} ProductAttributeFilterProps
 * @property {ApiList<Product>} results - Search results with includes
 * @property {(mappingName: string, values: ulong[]) => void} onFilterChange - Called when filter is changed
 * @property {ProductSearchAppliedFacet[]} value - Current filter values
 */
type ProductAttributeFilterProps = {
	results: ApiList<Product>;
	onFilterChange: (mappingName: string, values: ulong[]) => void;
	value: ProductSearchAppliedFacet[];
};

/**
 * Renders multi-select filters for product attributes.
 *
 * @component
 * @param {ProductAttributeFilterProps} props - Component props
 * @returns {JSX.Element}
 */
const SearchAttributeFilter: React.FC<ProductAttributeFilterProps> = (props: ProductAttributeFilterProps) => {
	const { results, onFilterChange, value } = props;

	const attributeValueIds = results.secondary?.attributeValueFacets?.results;
	if (!attributeValueIds) {
		throw new Error("Missing attributeValueFacets in includes.");
	}

	const attributeValues: ProductAttributeValue[] | undefined = results.includes.find((include: { field: string }) => include.field === "attributes")?.values;
	const attributes: ProductAttribute[] | undefined = results.includes.find((include: { field: string }) => include.field === "attribute")?.values;

	if (!attributeValues || !attributes) {
		throw new Error("Missing attribute or attributeValues in includes.");
	}

	return (
		<div className="attribute-filters">
			<CategoryFilter 
				results={results} 
				value={props.value?.find(facet => facet.mapping === 'category')?.ids ?? []} 
				onCategoryFilterChange={(values: ulong[]) => {
					onFilterChange('category', values);
				}} 
			/>
			{uniqueAttributes(attributes).map((productAttribute: ProductAttribute) => {
				const values = attributeValues.filter((val) => val.productAttributeId == productAttribute.id);
				const selectedValues = value.find((facet) => facet.mapping == productAttribute.key);
				const isInUse = value.find((item) => item.mapping === productAttribute.key);

				return (
					<div key={productAttribute.id} className={`attribute-filter${isInUse ? " in-use" : ""}`}>
						<MultiSelectBox
							onChange={(values: ulong[]) => {
								onFilterChange(productAttribute.key!, values);
							}}
							defaultText={productAttribute.name}
							value={selectedValues?.ids ?? []}
							options={uniqueAttributeValues(values).map((val: ProductAttributeValue) => {
								return {
									value: val.value + (productAttribute.units ?? ""),
									valueId: val.id,
									count: attributeValueIds.find((attr: any) => attr.attributeValueId == val.id)?.count ?? 0
								};
							})}
						/>
					</div>
				);
			})}
		</div>
	);
};

type CategoryFilterProps = {
	results: ApiList<Product>;
	onCategoryFilterChange: (values: ulong[]) => void;
	value: ulong[]
}

const CategoryFilter: React.FC<CategoryFilterProps> = (props) => {
	
	const { results } = props;
	
	if (!results) {
		return;
	}
	
	let categories = results?.includes.find((include: { field: string }) => include.field === "productCategories")?.values ?? [];
	categories = uniqueCategories(categories);
	
	if (categories.filter(Boolean).length == 0) {
		return;
	}


	const secondaryFacets = results.secondary?.productCategoryFacets.results ?? [];
	
	return (
		<div className={'attribute-filter'}>
			<MultiSelectBox 
				onChange={(values: ulong[]) => {
					props.onCategoryFilterChange(values)
				}} 
				defaultText={`Categories`} 
				value={props.value} 
				options={categories.map((category: ProductCategory) => {
					
					return {
						value: category.name,
						count: secondaryFacets.find((cat: any) => cat.productCategoryId === category.id)?.count ?? 0,
						valueId: category.id,
					}
				})}
			/>
		</div>
	)
}

/**
 * Filters and returns a list of unique attributes by name.
 *
 * @param {ProductAttribute[]} attrs - List of attributes
 * @returns {ProductAttribute[]} Unique attributes
 */
const uniqueAttributes = (attrs: ProductAttribute[]): ProductAttribute[] => {
	const seen = new Set();
	return attrs.filter((attr) => {
		if (seen.has(attr.name)) return false;
		seen.add(attr.name);
		return true;
	});
};


/**
 * Filters and returns a list of unique categories.
 *
 * @param {ProductCategory[]} categories - List of attributes
 * @returns {ProductCategory[]} Unique attributes
 */
const uniqueCategories = (categories: ProductCategory[]): ProductCategory[] => {
	const unique: ProductCategory[] = [];
	
	categories.forEach((category) => {
		if (unique.find(unq => unq.id === category.id)) {
			return;
		}
		unique.push(category);
	})
	return unique.sort((a, b) => a.name!.localeCompare(b.name!, undefined, { numeric: true }));;
};


/**
 * Filters and returns unique attribute values, sorted by value.
 *
 * @param {ProductAttributeValue[]} values - List of attribute values
 * @returns {ProductAttributeValue[]} Unique, sorted attribute values
 */
const uniqueAttributeValues = (values: ProductAttributeValue[]): ProductAttributeValue[] => {
	const seen = new Set<string>();
	return values
		.filter((v) => !seen.has(v.value!) && seen.add(v.value!))
		.sort((a, b) => a.value!.localeCompare(b.value!, undefined, { numeric: true }));
};

type ParsedQuery = {
	query?: string;
	facet: Record<string, ulong[]>;
};

/**
 * Parses a URL and extracts the 'query' string and 'facet' parameters.
 *
 * @param {string} url - The full URL to parse
 * @returns {ParsedQuery} - Parsed object containing query and facet map
 */
function parseProductUrl(url: string): ParsedQuery {
	const parsed = new URL(url);
	const params = new URLSearchParams(parsed.search);
	const result: ParsedQuery = { facet: {} };

	// Extract query
	const query = params.get("query");
	if (query) {
		result.query = query;
	}

	// Extract facet[key]=... values
	for (const [key, value] of params.entries()) {
		const match = key.match(/^facet\[(.+?)\]$/);
		if (match) {
			const facetKey = match[1];
			const idList = value
				.split(',')
				.map((str: string) => parseInt(str, 10))
				.filter((n: number) => !isNaN(n));
			result.facet[facetKey] = idList;
		}
	}

	return result;
}

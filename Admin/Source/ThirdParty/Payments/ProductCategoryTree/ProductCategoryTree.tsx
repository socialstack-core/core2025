import { useEffect, useState } from "react";
import { useRouter } from "UI/Router";
import SubHeader from "Admin/SubHeader";
import TreeView, { buildBreadcrumbs } from "Admin/TreeView";
import Input from "UI/Input";
import Image from "UI/Image";
import Loading from "UI/Loading";
import { MultiSelectBox } from "./MultiSelect";

import productCategoryApi from "Api/ProductCategory";
import searchApi, { ProductSearchAppliedFacet } from "Api/ProductSearchController";
import { ProductIncludes } from "Api/Includes";
import ProductApi, { Product } from "Api/Product";
import { ApiList } from "UI/Functions/WebRequest";
import { ProductAttribute } from "Api/ProductAttribute";
import { ProductAttributeValue } from "Api/ProductAttributeValue";

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
	const [viewType, setViewType] = useState<PageViewType>("tree");
	const [searchQuery, setSearchQuery] = useState("");

	const { pageState } = useRouter();
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
							onInput={(ev) => setSearchQuery((ev.target as HTMLInputElement).value)}
							placeholder="Filter products"
						/>
					</div>
				</div>
				<div className="sitemap__internal">
					{viewType === "tree" && !searchQuery ? (
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

/**
 * Displays a list view of products, including filtering and search functionality.
 *
 * @component
 * @param {ProductListViewProps} props - Component props
 * @returns {JSX.Element}
 */
const ProductListView: React.FC<ProductListViewProps> = (props: ProductListViewProps) => {
	const [defaultProductList, setDefaultProductList] = useState<ApiList<Product>>();
	const [searchResults, setSearchResults] = useState<ApiList<Product>>();
	const [pageOffset, setPageOffset] = useState(0);
	const [attributeFacets, setAttributeFacets] = useState<ProductSearchAppliedFacet[]>([]);
	const [loading, setLoading] = useState<boolean>(false);

	useEffect(() => {
		if (!defaultProductList) {
			setLoading(true);
			ProductApi.list({ pageSize: 20 as int, pageIndex: pageOffset as int }).then((result: ApiList<Product>) => {
				setDefaultProductList(result);
				setLoading(false);
			});
		}
	}, [defaultProductList]);

	const queryPayload = {
		query: props.query,
		pageOffset: pageOffset as uint,
		appliedFacets: attributeFacets
	};

	useEffect(() => {
		searchApi
			.faceted(queryPayload, [
				new ProductIncludes().attributeValueFacets,
				new ProductIncludes().productCategoryFacets,
				new ProductIncludes().attributes.attribute
			])
			.then((response) => {
				setSearchResults(response);
				setLoading(false);
			});
	}, [props.query, attributeFacets]);

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
					<table className="table">
						<thead>
						<tr>
							<th>Id</th>
							<th>Image</th>
							<th>Name</th>
							<th>SKU</th>
							<th>Actions</th>
						</tr>
						</thead>
						<tbody>
						{(searchResults ?? defaultProductList)?.results.map((product) => (
							<tr key={product.id}>
								<td>{product.id}</td>
								<td>{product.featureRef ? <Image fileRef={product.featureRef} /> : "No image available"}</td>
								<td>{product.name}</td>
								<td>{product.sku}</td>
								<td>-</td>
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

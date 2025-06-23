import { useEffect, useState } from "react";
import { useRouter } from "UI/Router";
import SubHeader from "Admin/SubHeader";
import TreeView, { buildBreadcrumbs } from "Admin/TreeView";
import Input from "UI/Input";
import Image from "UI/Image";
import Loading from "UI/Loading";
import { MultiSelectBox } from "./MultiSelect";

import productCategoryApi from "Api/ProductCategory";
import searchApi from "Api/ProductSearchController";
import { ProductIncludes } from "Api/Includes";
import { Product } from "Api/Product";
import { ApiList } from "UI/Functions/WebRequest";
import { ProductAttribute } from "Api/ProductAttribute";
import { ProductAttributeValue } from "Api/ProductAttributeValue";

export type ProductCategoryTreeProps = {
	noCreate: boolean;
};

type PageViewType = "list" | "tree";

type SearchResultIncludes<T> = {
	field: string;
	name: string;
	values: T[];
};

type AttributeFilter = {
	mapping: string,
	ids: ulong[]
}

export default function ProductCategoryTree({ noCreate }: ProductCategoryTreeProps) {
	const [viewType, setViewType] = useState<PageViewType>("tree");
	const [originalViewType, setOriginalViewType] = useState<PageViewType>();
	const [searchQuery, setSearchQuery] = useState("");
	const [pageOffset, setPageOffset] = useState(0);
	const [loading, setLoading] = useState(false);
	const [searchResults, setSearchResults] = useState<ApiList<Product>>();

	const [attributeFilter, setAttributeFilter] = useState<Record<uint, AttributeFilter>>({});

	const { pageState } = useRouter();
	const path = pageState.query?.get("path") || "";

	const breadcrumbs = buildBreadcrumbs("/en-admin/product", "Products", path, "/en-admin/product");

	const addProductUrl = "/en-admin/product/add";
	const addCategoryUrl = "/en-admin/productcategory/add";

	useEffect(() => {
		if (searchQuery.length === 0) {
			setViewType(originalViewType ?? viewType);
			setOriginalViewType(undefined);
			return;
		}

		if (viewType !== "list") {
			setOriginalViewType(viewType);
			setViewType("list");
			return;
		}

		setLoading(true);

		searchApi
			.faceted(
				{ 
					query: searchQuery, 
					pageOffset: pageOffset as uint,
					appliedFacets: Object.values(attributeFilter)
				},
				[
					new ProductIncludes().attributeValueFacets,
					new ProductIncludes().productCategoryFacets,
					new ProductIncludes().attributes.attribute,
				]
			)
			.then((response) => {
				setSearchResults(response);
				setLoading(false);
			});
	}, [searchQuery, pageOffset, attributeFilter]);

	// useEffect(() => {
	// 	if (!attributeFilter) {
	// 		setAttributeFilter({});
	// 		return;
	// 	}
	// 	if (searchQuery.length === 0) {
	// 		setAttributeFilter({});
	// 	}
	// }, [attributeFilter, searchQuery]);

	const searchResultAttributes: SearchResultIncludes<ProductAttribute> | undefined =
		searchResults?.includes?.find((inc: SearchResultIncludes<ProductAttribute>) => inc.field === "attribute");

	const searchResultAttributeValues: SearchResultIncludes<ProductAttributeValue> | undefined =
		searchResults?.includes?.find((inc: SearchResultIncludes<ProductAttributeValue>) => inc.field === "attributes");

	if (searchResultAttributes) {
		searchResultAttributes.values = searchResultAttributes.values
			.filter((attr, idx, self) => self.findIndex((t) => t.name === attr.name) === idx)
			.sort((a, b) => {
				const aHasFilter = attributeFilter[a.id]?.ids?.length > 0;
				const bHasFilter = attributeFilter[b.id]?.ids?.length > 0;
				return aHasFilter === bHasFilter ? 0 : aHasFilter ? -1 : 1;
			});
	}
	
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
					) : loading ? (
						<Loading />
					) : (
						<div className="admin-page__internal">
							{searchResultAttributes && (
								<div className="attribute-filters">
									{searchResultAttributes.values.map((attribute) => {
										const valueFacets = searchResults?.secondary?.attributeValueFacets.results ?? [];

										const options = searchResultAttributeValues?.values
											.filter((val) => val.productAttributeId === attribute.id)
											.map((val) => ({
												value: val.value + (attribute.units ?? ""),
												count: valueFacets.find((f: { attributeValueId: uint }) => f.attributeValueId === val.id)?.count ?? 0,
												valueId: val.id
											}))
											.reduce((acc, curr) => {
												if (!acc.find((opt) => opt.value === curr.value)) {
													acc.push(curr);
												}
												return acc;
											}, [] as { value: string; count: number, valueId: number }[])
											.sort((a, b) => a.value.localeCompare(b.value, undefined, { numeric: true }));

										return (
											<div
												key={attribute.id}
												className={`attribute-filter${attributeFilter[attribute.id] ? " in-use" : ""}`}
											>
												<MultiSelectBox
													onChange={(values) =>
														setAttributeFilter({
															...attributeFilter,
															[attribute.id]: {
																mapping: attribute.key,
																ids: values
															}
														})
													}
													key={'attribute-' + attribute.id}
													defaultText={attribute.name}
													value={attributeFilter[attribute.id]?.ids ?? []}
													options={options ?? []}
												/>
											</div>
										);
									})}
								</div>
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
								{searchResults?.results?.map((product) => (
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

import TreeView, { buildBreadcrumbs } from 'Admin/TreeView';
import SubHeader from 'Admin/SubHeader';
import { useRouter } from 'UI/Router';
import productCategoryApi from 'Api/ProductCategory';
import {useEffect, useState} from "react";
import Input from "UI/Input";
import searchApi from "Api/ProductSearchController";
import Image from "UI/Image";
import {useSession} from "UI/Session";
import Loading from "UI/Loading";
import {Product} from "Api/Product";
import {ApiList} from "UI/Functions/WebRequest";
import {ProductAttributeIncludes, ProductIncludes} from "Api/Includes";

export type ProductCategoryTreeProps = {
	noCreate: boolean
}

export type PageViewType = 'list' | 'tree';

export default function ProductCategoryTree(props: ProductCategoryTreeProps) {
	
	const [viewType, setViewType] = useState<PageViewType>('tree');
	const [originalViewType, setOriginalViewType] = useState<PageViewType | undefined>('tree');
	
	const [searchQuery, setSearchQuery] = useState<string>('');
	const [pageOffset, setPageOffset] = useState<number>(0);
	const [loading, setLoading] = useState<boolean>(false);
	
	const [searchResults, setSearchResults] = useState<ApiList<Product> | undefined>();
	
	const addProductUrl = '/en-admin/product/add';
	const addCategoryUrl = '/en-admin/productcategory/add';
	const { pageState } = useRouter();
	const { query } = pageState;
	const path = query?.get("path") || "";

	
	const breadcrumbs = buildBreadcrumbs(
		'/en-admin/product',
		`Products`,
		path,
		'/en-admin/product'
	);

	useEffect(() => {
		
		if (searchQuery.length == 0)
		{
			setViewType(originalViewType ?? viewType);
			setOriginalViewType(undefined);
			return;
		}
		if (viewType !== 'list')
		{
			setOriginalViewType(viewType);
			setViewType('list');
		}
		
		setLoading(true);
		
		// lets pull some results in
		searchApi.faceted({
			query: searchQuery,
			pageOffset: pageOffset as uint,
		}, [
			new ProductIncludes().attributeValueFacets,
			new ProductIncludes().productCategoryFacets,
			new ProductIncludes().attributes.attribute,
		])
		.then(response => {
			setSearchResults(response);
			setLoading(false);
		})
		
	}, [searchQuery, pageOffset]);

	return (
		<>
			<SubHeader 
				title={`Edit Products`} 
				breadcrumbs={breadcrumbs}
			/>
			<div className="sitemap__wrapper product-category-tree">
				<div className={'page-controls'}>
					<div className="btn-group view-toggle" role="group" aria-label={`Select view style`}>
						<Input type="radio" noWrapper label={`Tree`} groupIcon="fr-grid" groupVariant="primary" value={viewType == 'tree'} onChange={() => setViewType('tree')} name="view-style" />
						<Input type="radio" noWrapper label={`List`} groupIcon="fr-th-list" groupVariant="primary" value={viewType == 'list'} onChange={() => setViewType('list')} name="view-style" />
					</div>
					<div className={'product-search'}>
						<Input
							type="search"
							defaultValue={searchQuery}
							onInput={(ev) => {
								setSearchQuery((ev.target as HTMLInputElement).value);
							}}
							placeholder={'Filter products'}
						/>
					</div>
				</div>
				<div className="sitemap__internal">
					{viewType == 'tree' && searchQuery.length == 0 ? (
						<TreeView onLoadData={(path) => {
							return productCategoryApi
								.getTreeNodePath(path)
								.then(resp => {
									return resp;
								});
							}} 
						/>
						) : (
							loading ? <Loading /> :
							<div className={'admin-page__internal'}>	
								<table className={'table'}>
									<thead>
										<tr>
											<th>{`Id`}</th>
											<th>{`Image`}</th>
											<th>{`Name`}</th>
											<th>{`SKU`}</th>
											<th>{`Actions`}</th>
										</tr>
									</thead>
									<tbody>
									{searchResults?.results?.map((product, idx) => {
										
										return (
											<tr>
												<td>{product.id}</td>
												<td>
													{product.featureRef ? <Image fileRef={product.featureRef}/> : `No image available`}
												</td>
												<td>{product.name}</td>
												<td>{product.sku}</td>
												<td> - </td>
											</tr>
										)
									})}
									</tbody>
								</table>
							</div>
						)
					}
				</div>
				{!props.noCreate && <>
					<footer className="admin-page__footer">
						<a href={addCategoryUrl} className="btn btn-primary">
							{`New category`}
						</a>
						<a href={addProductUrl} className="btn btn-primary">
							{`New product`}
						</a>
					</footer>
				</>}
			</div>
		</>
	);
}
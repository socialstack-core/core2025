import TreeView, { buildBreadcrumbs } from 'Admin/TreeView';
import SubHeader from 'Admin/SubHeader';
import { useRouter } from 'UI/Router';
import Default from 'Admin/Templates/BaseAdminTemplate';
import { useState, useEffect } from 'react';
import productCategoryApi, { ProductCategory } from 'Api/ProductCategory';

export default function ProductCategoryTree(props) {
	var addProductUrl = '/en-admin/product/add';
	var addCategoryUrl = '/en-admin/productcategory/add';
	const { pageState } = useRouter();
	const { query } = pageState;
	var path = query?.get("path") || "";

	var breadcrumbs = buildBreadcrumbs(
		'/en-admin/product',
		`Products`,
		path,
		'/en-admin/product'
	);

	return (
		<Default>
			<SubHeader title={`Edit Products`} breadcrumbs={breadcrumbs} />
			<div className="sitemap__wrapper">
				<div className="sitemap__internal">
					<TreeView onLoadData={(path) => {

						return productCategoryApi
							.getTreeNodePath(path)
							.then(resp => {
								return resp;
							});

					}} />
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
		</Default>
	);
}

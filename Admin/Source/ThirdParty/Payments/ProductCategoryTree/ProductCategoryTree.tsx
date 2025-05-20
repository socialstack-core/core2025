import TreeView from 'Admin/TreeView';
import SubHeader from 'Admin/SubHeader';
import Default from 'Admin/Templates/BaseAdminTemplate';
import { useState, useEffect } from 'react';
import productCategoryApi, { ProductCategory } from 'Api/ProductCategory';

export default function ProductCategoryTree(props) {
	var addProductUrl = '/en-admin/product/add';
	var addCategoryUrl = '/en-admin/productcategory/add';

	return (
		<Default>
			<SubHeader title={`Edit Products`} breadcrumbs={[
				{
					title: `Products`
				}
			]} />
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

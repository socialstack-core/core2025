import TreeView from 'Admin/TreeView';
import SubHeader from 'Admin/SubHeader';
import Default from 'Admin/Templates/BaseAdminTemplate';
import { useState, useEffect } from 'react';
import productAttributeApi, { ProductAttribute } from 'Api/ProductAttribute';

export default function ProductCategoryTree(props) {
	var addUrl = '/en-admin/productattribute/add';
	var addGroupUrl = '/en-admin/productattributegroup/add';
	
	return (
		<Default>
			<SubHeader title={`Edit Product Attributes`} breadcrumbs={[
				{
					title: `Product Attributes`
				}
			]} />
			<div className="sitemap__wrapper">
				<div className="sitemap__internal">
					<TreeView onLoadData={(path) => {

						return productAttributeApi
							.getTreeNodePath(path)
							.then(resp => {
								return resp;
							});

					}} />
				</div>
				{!props.noCreate && <>
					<footer className="admin-page__footer">
						<a href={addGroupUrl} className="btn btn-primary">
							{`New group`}
						</a>
						<a href={addUrl} className="btn btn-primary">
							{`New attribute`}
						</a>
					</footer>
				</>}
			</div>
		</Default>
	);
}

import TreeView, { buildBreadcrumbs } from 'Admin/TreeView';
import SubHeader from 'Admin/SubHeader';
import { useRouter } from 'UI/Router';
import { useState, useEffect } from 'react';
import productAttributeApi, { ProductAttribute } from 'Api/ProductAttribute';

export default function ProductAttributeTree(props) {
	var addUrl = '/en-admin/productattribute/add';
	var addGroupUrl = '/en-admin/productattributegroup/add';
	const { pageState } = useRouter();
	const { query } = pageState;
	var path = query?.get("path") || "";

	var breadcrumbs = buildBreadcrumbs(
		'/en-admin/productattribute',
		`Product Attributes`,
		path,
		'/en-admin/productattribute'
	);
	
	return (
		<>
			<SubHeader title={`Edit Product Attributes`} breadcrumbs={breadcrumbs} />
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
		</>
	);
}

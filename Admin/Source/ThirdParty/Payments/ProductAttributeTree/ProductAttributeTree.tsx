import TreeView from 'Admin/TreeView';
import SubHeader from 'Admin/SubHeader';
import { useRouter } from 'UI/Router';
import Default from 'Admin/Templates/BaseAdminTemplate';
import { useState, useEffect } from 'react';
import productAttributeApi, { ProductAttribute } from 'Api/ProductAttribute';

function buildPath(pathParts: string[], max: number) {
	var result = '';

	for (var i = 0; i < max; i++) {
		if (i != 0) {
			result += "/";
		}
		result += pathParts[i];
	}

	return result;
}

export default function ProductAttributeTree(props) {
	var addUrl = '/en-admin/productattribute/add';
	var addGroupUrl = '/en-admin/productattributegroup/add';
	const { pageState } = useRouter();
	const { query } = pageState;
	var path = query?.get("path") || "";

	var breadcrumbs = [
		{
			url: '/en-admin/productattribute',
			title: `Product Attributes`
		}
	];

	if (path) {
		var pathParts = path.split('/');
		for (var i = 0; i < pathParts.length; i++) {

			breadcrumbs.push({
				url: '/en-admin/productattribute?path=' + buildPath(pathParts, i + 1),
				title: pathParts[i]
			});

		}
	}
	
	return (
		<Default>
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
		</Default>
	);
}

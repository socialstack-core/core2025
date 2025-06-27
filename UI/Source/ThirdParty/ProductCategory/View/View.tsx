import productCategoryApi, { ProductCategory } from "Api/ProductCategory";
import ProductList from 'UI/Product/List';
import Loading from 'UI/Loading';
import useApi from "UI/Functions/UseApi";
import Input from 'UI/Input';
import DualRange from 'UI/DualRange';
import Promotion from 'UI/Promotion';
import { useEffect, useMemo, useState, useRef } from "react";
import searchApi, { ProductSearchType } from 'Api/ProductSearchController';

interface ViewProps {
	productCategory: ProductCategory;
}

const View: React.FC<ViewProps> = ({ productCategory }) => {
	const [showApprovedOnly, setShowApprovedOnly] = useState(false);
	const [showInStockOnly, setShowInStockOnly] = useState(false);
	const [viewStyle, setViewStyle] = useState<'list' | 'small-thumbs' | 'large-thumbs'>('large-thumbs');
	const [sortOrder, setSortOrder] = useState('most-popular');
	const [pagination, setPagination] = useState('page1');
	const [selectedDirectChildren, setSelectedDirectChildren] = useState<number[]>([]);
	const [showAllCategories, setShowAllCategories] = useState(false);

	const [allDescendants] = useApi(() => {
		return productCategoryApi.getDescendants(productCategory.id);
	}, [productCategory]);

	// Build parent → children map
	const descendantMap = useMemo(() => {
		const map: Record<number, number[]> = {};
		if (!allDescendants?.results) return map;

		allDescendants.results.forEach(cat => {
			if (!map[cat.parentId!]) map[cat.parentId!] = [];
			map[cat.parentId!].push(cat.id);
		});
		return map;
	}, [allDescendants]);

	// Get all IDs under a category (including self)
	const getDescendantsAndSelfIds = (categoryId: number): number[] => {
		const ids: number[] = [];
		const stack = [categoryId];
		while (stack.length) {
			const current = stack.pop();
			if (current === undefined) continue;
			ids.push(current);
			const children = descendantMap[current] || [];
			stack.push(...children);
		}
		return ids;
	};

	// Default select all direct children
	useEffect(() => {
		if (allDescendants?.results && selectedDirectChildren.length === 0) {
			const directChildren = allDescendants.results.filter(cat => cat.parentId === productCategory.id);
			setSelectedDirectChildren(directChildren.map(c => c.id));
		}
	}, [allDescendants, selectedDirectChildren, productCategory]);

	// Categories to send to the search API
	const selectedCategoryIds = useMemo(() => {
		const allSelected = selectedDirectChildren.flatMap(id => getDescendantsAndSelfIds(id));
		const unique = new Set<number>([productCategory.id, ...allSelected]);
		return Array.from(unique);
	}, [selectedDirectChildren, descendantMap, productCategory.id]);

	// Product results
	const [products] = useApi(() => {
		return searchApi.faceted({
			searchType: ProductSearchType.Expansive,
			appliedFacets: [
				{
					mapping: "productcategories",
					ids: selectedCategoryIds as int[]
				}
			],
			query: "",
			pageSize: 20 as uint,
			pageOffset: 0 as uint
		});
	}, [selectedCategoryIds]);

	// Build flat count map
	const categoryCountMap = useMemo(() => {
		const map: Record<number, number> = {};
		const facets = products?.secondary?.productCategoryFacets?.results ?? [];
		for (const res of facets) {
			if (res.count > 0) map[res.productCategoryId] = res.count;
		}
		return map;
	}, [products]);

	// Recursively sum counts for category + all children
	const getTotalProductCount = (categoryId: number): number => {
		const allIds = getDescendantsAndSelfIds(categoryId);
		return allIds.reduce((total, id) => total + (categoryCountMap[id] ?? 0), 0);
	};

	// Direct children only for display
	const directChildren = useMemo(() => {
		return allDescendants?.results?.filter(cat => cat.parentId === productCategory.id) || [];
	}, [allDescendants, productCategory]);

	// Track last non-zero counts per direct child category
	const lastNonZeroCounts = useRef<Record<number, number>>({});

	// Update lastNonZeroCounts on products or selection change
	useEffect(() => {
		directChildren.forEach(cat => {
			const count = getTotalProductCount(cat.id);
			if (count > 0) {
				lastNonZeroCounts.current[cat.id] = count;
			}
		});
	}, [products, selectedDirectChildren, directChildren]);

	if (!products) return <Loading />;

	const GBPound = new Intl.NumberFormat('en-GB', {
		style: 'currency',
		currency: 'GBP',
		minimumFractionDigits: 0,
		maximumFractionDigits: 0
	});

	return (
		<div className="ui-productcategory-view">
			<div className="ui-productcategory-view__filters-wrapper">
				<div className="ui-productcategory-view__filters">
					<fieldset>
						<legend>Show / hide products</legend>
						<Input
							type="checkbox"
							isSwitch
							label="Only show approved"
							value={showApprovedOnly}
							name="show-approved"
							noWrapper
							onChange={(e) => setShowApprovedOnly((e.target as HTMLInputElement).checked)}
						/>
						<Input
							type="checkbox"
							isSwitch
							label="Only show in stock"
							value={showInStockOnly}
							name="show-in-stock"
							noWrapper
							onChange={(e) => setShowInStockOnly((e.target as HTMLInputElement).checked)}
						/>
					</fieldset>

					{directChildren.length > 0 && (
						<fieldset>
							<legend>All product categories</legend>
							{directChildren.map((category, idx) => {
								if (idx > 2 && !showAllCategories) return null;

								const isChecked = selectedDirectChildren.includes(category.id);

								const currentCount = getTotalProductCount(category.id);
								const displayedCount = currentCount > 0
									? currentCount
									: lastNonZeroCounts.current[category.id] ?? 0;

								return (
									<Input
										key={category.id}
										type="checkbox"
										label={`${category.name}${displayedCount > 0 ? ` (${displayedCount})` : ''}`}
										checked={isChecked}
										onChange={(e) => {
											const checked = (e.target as HTMLInputElement).checked;
											setSelectedDirectChildren(prev =>
												checked
													? [...prev, category.id]
													: prev.filter(id => id !== category.id)
											);
										}}
									/>
								);
							})}

							{directChildren.length > 3 && !showAllCategories && (
								<button type="button" onClick={() => setShowAllCategories(true)}>
									Show more categories
								</button>
							)}
						</fieldset>
					)}

					<DualRange
						className="ui-productcategory-view__price"
						label="Price"
						numberFormat={GBPound}
						min={5}
						max={5000}
						step={1}
						defaultFrom={500}
						defaultTo={3000}
					/>
				</div>

				<Promotion
					title="Get 10% Off Our Bedroom Bestsellers"
					description="Save now on top-rated beds and accessories - Limited time offer"
					url="#"
				/>
			</div>

			<header className="ui-productcategory-view__header">
				<Input type="select" aria-label="Sort by" value={sortOrder} noWrapper>
					<option value="most-popular">Most popular</option>
				</Input>

				<Input type="select" aria-label="Show" value={pagination} noWrapper>
					<option value="page1">20 out of 1,500 products</option>
				</Input>

				<div className="btn-group" role="group" aria-label="Select view style">
					<Input
						type="radio"
						noWrapper
						label="List"
						groupIcon="fr-list"
						groupVariant="primary"
						checked={viewStyle === 'list'}
						onChange={() => setViewStyle('list')}
						name="view-style"
					/>
					<Input
						type="radio"
						noWrapper
						label="Small thumbnails"
						groupIcon="fr-th-list"
						groupVariant="primary"
						checked={viewStyle === 'small-thumbs'}
						onChange={() => setViewStyle('small-thumbs')}
						name="view-style"
					/>
					<Input
						type="radio"
						noWrapper
						label="Large thumbnails"
						groupIcon="fr-grid"
						groupVariant="primary"
						checked={viewStyle === 'large-thumbs'}
						onChange={() => setViewStyle('large-thumbs')}
						name="view-style"
					/>
				</div>
			</header>

			<ProductList content={products.results} viewStyle={viewStyle} />
		</div>
	);
};

export default View;

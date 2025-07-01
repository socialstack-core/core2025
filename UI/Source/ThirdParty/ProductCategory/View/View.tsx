import {useRouter} from 'UI/Router';
import useApi from 'UI/Functions/UseApi';
import searchApi, {ProductSearchType} from 'Api/ProductSearchController';
import ProductList from 'UI/Product/List';
import Loading from 'UI/Loading';
import Html from 'UI/Html';
import Input from 'UI/Input';
import Button from 'UI/Button';
import FilterList from 'UI/Product/Search/FilterList';
import {useEffect, useState} from "react";
import {AttributeFacetGroup, AttributeValueFacet, ProductCategoryFacet} from 'UI/Product/Search/Facets';
import Link from "UI/Link";
import {ProductIncludes, SecondaryIncludes} from "Api/Includes";
import {ProductCategory} from "Api/ProductCategory";

const MAX_VISIBLE_CATEGORIES = 3;
const MAX_VISIBLE_ATTRIBUTE_OPTIONS = 4;

/**
 * Props for the Search component.
 */
interface ViewProps {
	productCategory: ProductCategory
}

/**
 * The Search React component.
 * @param props React props.
 */
const View: React.FC<ViewProps> = (props) => {
	const { pageState } = useRouter();
	const { query } = pageState;
	
	const { productCategory } = props;
	
	// acticare design shows these as toggles - see corresponding commented UI below
	//const [showApprovedOnly, setShowApprovedOnly] = useState();
	//const [showInStockOnly, setShowInStockOnly] = useState();

	// list, thumb or grid view
	const [viewStyle, setViewStyle] = useState('grid');

	// TODO: confirm sort options
	const [sortOrder, setSortOrder] = useState('most-popular');

	// TODO: make pagination great again
	const [pagination, setPagination] = useState('page1');

	const [selectedFacets, setSelectedFacets] = useState<Record<uint, uint[]>>({});


	const [initialSearch, setInitialSearch] = useState(query?.get("q") || "");


	var initialPageStr = query?.get("page") || "";
	var initialPageOffset = (parseInt(initialPageStr) || 1) - 1;

	
	useEffect(() => {

		const evListener = (ev: CustomEvent) => {
			const detail = ev.detail as { query: string };
			if (detail.query) {
				setInitialSearch(detail.query);
			}
		}

		window.addEventListener('search', evListener);

		return () => {
			window.removeEventListener('search', evListener);
		}

	}, []);


	const [resultSet] = useApi(() => {
		return searchApi.faceted({
			query: initialSearch,
			pageOffset: initialPageOffset as int,
			searchType: ProductSearchType.Expansive,
			pageSize: 20 as uint,
			appliedFacets: [
				{
					mapping: "productcategories",
					ids: [productCategory.id]
				},
				...(Object.keys(selectedFacets ?? {}) ?? []).map((attrId => {

					return {
						mapping: "attributes",
						ids: selectedFacets[parseInt(attrId) as uint] || []
					};
				}))
			]
		}, [
		// Any normal includes you might need here
		// 'price',

		// Plus then includes on the facets (the attribute and category selectors)
		new ProductIncludes().productCategoryFacets,
		new ProductIncludes().productCategoryFacets.category,
		new ProductIncludes().attributeValueFacets.value.attribute.attributeGroup
	])}, [initialSearch, selectedFacets]);

	const resetFilters = () => {
		
	};

	const showSearchResults = () => {

		if (!resultSet) {
			return <Loading />;
		}

		const facets = resultSet.secondary;
		const { attributeValueFacets, productCategoryFacets } = facets;

		const categoryFacets = (productCategoryFacets?.results || []) as ProductCategoryFacet[];
		const attributeFacets = (attributeValueFacets?.results || []) as AttributeValueFacet[];

		// Group attribute facets ("blue (12)") up by the attribute ("Colour") they are for.
		// Could group again by attribute.attributeGroup ("Material & Design") if necessary.

		var attributeMap = new Map<uint, AttributeFacetGroup>();

		attributeFacets.forEach(facet => {
			if (!(facet.value?.attribute)) {
				return;
			}

			var attribId = facet.value.productAttributeId;
			var grouping = attributeMap.get(attribId);

			if (!grouping) {
				grouping = {
					attribute: facet.value.attribute,
					facetValues: []
				} as AttributeFacetGroup;

				attributeMap.set(attribId, grouping);
			}

			grouping.facetValues.push(facet);
		});

		var attributeFacetGroups = Array.from(attributeMap.values());

		return <>
			<div className="ui-product-search__internal">
				<form className="ui-product-search__filters">

					<Button type="reset" variant="secondary" onClick={() => resetFilters()}>
						{`Reset all filters`}
					</Button>

					{/* switch toggle example group */}
					{/*
					<fieldset>
						<legend>
							{`Toggle`}
						</legend>
						<div className="fieldset-content fieldset-content--no-border">
							<Input type="checkbox" sm isSwitch label={`Toggle 1`} noWrapper />
							<Input type="checkbox" sm isSwitch label={`Toggle 2`} noWrapper />
						</div>
					</fieldset>
					*/}

					{/* categories */}
					{categoryFacets.length > 0 && <>
						<fieldset>
							<legend>
								{`Categories`}
							</legend>
							<div className="fieldset-content">
								<CategoryListRenderer facets={categoryFacets} />
							</div>
						</fieldset>
					</>}

					{/* attributes */}
					{attributeFacetGroups.length > 0 && <>
						{
							attributeFacetGroups.map(facet => {
								// can include primaryUrl etc on facets as well if needed
								// here though it is (probably, I haven't looked at the designs recently) exclusively a button
								// which then restricts the search.

								return <>
									<fieldset key={facet.attribute.id}>
										<legend>
											{facet.attribute.name}
										</legend>
										<FilterList 
											facets={facet.facetValues} 
											maxVisible={MAX_VISIBLE_ATTRIBUTE_OPTIONS}
											onChange={(values: ulong[]) => {
												selectedFacets[facet.attribute.id] = values;
												setSelectedFacets({...selectedFacets});
											}} 
										/>
									</fieldset>
								</>;
							})
						}
					</>}
				</form>
				<ProductList content={resultSet.results} viewStyle={viewStyle} />
			</div>
		</>;
	};

	return (
		<div className="ui-product-search">
			<header className="ui-product-search__header">
				<Html tag="h1" className="ui-product-search__title">
					{productCategory.name}
				</Html>
				<fieldset className="ui-product-search__view-options">
					<Input type="select" aria-label={`Sort by`} value={sortOrder} noWrapper>
						<option value="most-popular">
							{`Most popular`}
						</option>
					</Input>

					{/* replace this with more typical pagination below product list? */}
					<Input type="select" aria-label={`Show`} value={pagination} noWrapper>
						<option value="page1">
							{`20 out of 1,500 products`}
						</option>
					</Input>

					<div className="btn-group ui-btn-group" role="group" aria-label={`Select view style`}>
						{/* design shows list, small and large thumb views
						ref: https://www.figma.com/design/VYLC1be2OJRmymw5C0qc7J/Acticare---UX-Designs?node-id=160-3241&t=1CQfqx9zIXjEdPjX-0
							<Input type="radio" noWrapper label={`List`} groupVariant="primary" value={viewStyle == 'list'} onChange={() => setViewStyle('list')} name="view-style" />
							<Input type="radio" noWrapper label={`Small thumbnails`} groupVariant="primary" value={viewStyle == 'small-thumbs'} onChange={() => setViewStyle('small-thumbs')} name="view-style" />
							<Input type="radio" noWrapper label={`Large thumbnails`} groupVariant="primary" value={viewStyle == 'large-thumbs'} onChange={() => setViewStyle('large-thumbs')} name="view-style" />
						*/}
						<Input type="radio" noWrapper label={`List`} groupIcon="fr-th-list" groupVariant="primary" value={viewStyle == 'list'} onChange={() => setViewStyle('list')} name="view-style" />
						<Input type="radio" noWrapper label={`Grid`} groupIcon="fr-grid" groupVariant="primary" value={viewStyle == 'grid'} onChange={() => setViewStyle('grid')} name="view-style" />
					</div>
				</fieldset>
			</header>
			{showSearchResults()}
		</div>
	);
}

const CategoryListRenderer = (props: { facets: ProductCategoryFacet[] }) => {
	
	const [showMore, setShowMore] = useState(false);
	
	const { facets } = props;

	const categories = uniqueCategories(facets.map(facet => facet.category));
	
	return (
		<div className={'categories'}>
			{categories.map((category, idx) => {
				if (idx > 2 && !showMore) {
					return;
				}

				const facet = facets.find(f => f.category.id === category.id);

				return (
					<Link href={'/category/' + category.slug}>
						{category.name} ({facet?.count ?? 0})
					</Link>
				)
			})}
			{categories.length > 2 && <Button
				type={'button'}
				onClick={() => setShowMore(!showMore)}
			>
				<span>{showMore ? `Show less` : `Show more`}</span>
			</Button>}
		</div>
	)
}

export default View;


/**
 * Filters and returns a list of unique categories.
 *
 * @param {ProductCategory[]} categories - List of attributes
 * @returns {ProductCategory[]} Unique attributes
 */
const uniqueCategories = (categories: ProductCategory[]): ProductCategory[] => {
	const unique: ProductCategory[] = [];
	
	categories.forEach((category) => {
		if (unique.find(unq => unq.name === category.name)) {
			return;
		}
		unique.push(category);
	})
	return unique.sort((a, b) => a.name!.localeCompare(b.name!, undefined, { numeric: true }));;
};

import { useRouter } from 'UI/Router';
import useApi from 'UI/Functions/UseApi';
import searchApi from 'Api/ProductSearchController';
import productApi from 'Api/Product';
import ProductList from 'UI/Product/List';
import Loading from 'UI/Loading';
import Html from 'UI/Html';
import Input from 'UI/Input';
import Button from 'UI/Button';
import { ProductCategory } from 'Api/ProductCategory';
import { ProductAttributeValue } from 'Api/ProductAttributeValue';
import { ProductAttribute } from 'Api/ProductAttribute';
import { useState } from "react";

/**
 * Props for the Search component.
 */
interface SearchProps {
	/**
	 * An example optional fileRef prop.
	 */
	// logoRef?: FileRef
}

interface ProductCategoryFacet {
	count: int;
	category: ProductCategory
}

interface AttributeValueFacet {
	count: int;
	value: ProductAttributeValue
}

interface AttributeFacetGroup {
	attribute: ProductAttribute,
	facetValues: AttributeValueFacet[]
}

/**
 * The Search React component.
 * @param props React props.
 */
const Search: React.FC<SearchProps> = (props) => {
	const { pageState } = useRouter();
	const { query } = pageState;
	const [showApprovedOnly, setShowApprovedOnly] = useState();
	const [showInStockOnly, setShowInStockOnly] = useState();
	const [viewStyle, setViewStyle] = useState('grid');
	const [sortOrder, setSortOrder] = useState('most-popular');
	const [pagination, setPagination] = useState('page1');

	var initialSearch = query?.get("q") || "";
	var initialPageStr = query?.get("page") || "";
	var initialPageOffset = (parseInt(initialPageStr) || 1) - 1;

	const [resultSet] = useApi(() => searchApi.faceted({
		query: initialSearch,
		pageOffset: initialPageOffset as int,
	}, [
		// Any normal includes you might need here
		// 'price',

		// Plus then includes on the facets (the attribute and category selectors)
		'secondary.productCategoryFacets.category',
		'secondary.attributeValueFacets.value.attribute.attributeGroup'
	]), [initialSearch]);

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
		console.log(attributeFacetGroups);

		return <>
			<div className="ui-product-search__internal">
				<form className="ui-product-search__filters">
					<Button type="reset" variant="secondary">
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
					{/* NB: design calls for "show more" expander - TODO */}
					{categoryFacets.length > 0 && <>
						<fieldset>
							<legend>
								{`Categories`}
							</legend>
							<div className="fieldset-content">
								<Input type="search" placeholder={`Search for ...`} noWrapper />
								{
									categoryFacets.map(facet => {

										if (!facet.category) {
											return null;
										}

										// can include primaryUrl etc on facets as well if needed
										// here though it is (probably, I haven't looked at the designs recently) exclusively a button
										// which then restricts the search.

										return <Input type="checkbox" sm label={`${facet.category.name} (${facet.count})`} noWrapper />;
									})
								}
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
									<fieldset>
										<legend>
											{facet.attribute.name}
										</legend>
										<div className="fieldset-content">
											{
												facet.facetValues.map(facetVal => {
													return <Input type="checkbox" sm label={`${facetVal.value.value} (${facetVal.count})`} noWrapper />;
												})
											}
										</div>
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
					{initialSearch?.length > 0 ? `Results for <strong>${initialSearch}</strong>` : `Results`}
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

export default Search;
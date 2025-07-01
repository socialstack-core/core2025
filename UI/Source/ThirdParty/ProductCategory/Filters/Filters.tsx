import { ProductCategory } from "Api/ProductCategory";
import Input from 'UI/Input';
import {useSession} from "UI/Session";
import {useEffect, useState} from "react";

/**
 * Props for the List component.
 */
interface FiltersProps {
	/**
	 * The categories to list. Must have included 'primaryUrl'.
	 */
	content?: ProductCategory[],

	/**
	 * When altered, this sets the selected categories. 
	 * @param selectedCategories
	 */
	onChange: (selectedCategories: uint[]) => void,
}

/**
 * The Filters React component.
 * @param props React props.
 */
const Filters: React.FC<FiltersProps> = (props) => {
	const { content } = props;
	
	if (!content) {
		return null;
	}
	
	const { session } = useSession();
	const { locale } = session;
	
	// TODO: need an individual filter flag for each category;
	//       currently 'include' just triggers the checkbox rendering, but knows nothing about state
	return (
		<div className="ui-productcategory-filters">
			<Input type="search" placeholder={`Search for a category`} noWrapper />
			{content.map(category => {
				// TODO: filter state required for each category
				return (
					<Input 
						type="checkbox" 
						label={getLocalizedValue(category.name as unknown as LocalizedField, locale?.code ?? 'en')} 
						noWrapper 
						onChange={(ev) => {
							const target = ev.target as HTMLInputElement;
							
							if (target.checked) {
								props.onChange([...((content ?? []).map(cat => cat.id)), category.id]);
							} else {
								props.onChange([...((content ?? []).filter(existing => existing.id != category.id).map(cat => cat.id))]);
							}
						}}
					/>
				)
			})}
		</div>
	);
}

type LocalizedField = {
	values: Record<string, string>
} 

const getLocalizedValue = (field: LocalizedField, locale: string): string => {
	return field.values[locale];
}

export default Filters;
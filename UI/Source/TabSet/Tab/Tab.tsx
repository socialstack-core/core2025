import { useState, useEffect, ReactElement, useCallback } from 'react';

const COMPONENT_PREFIX = 'ui-tabset';

interface TabProps {
	/**
	 * tab label
	 */
	label: string,
	/**
	 * URL hash to associate with this tab
	 */
	hash: string,
	/**
	 * unique ID
	 */
	id: string,
	/**
	 * radio group name
	 */
	name: string,
	/**
	 * true if rendering tab pane, false if rendering tab link
	 */
	panel?: boolean,
	/**
	 * tab index
	 */
	index: number,
	/**
	 * current selected tab index
	 */
	selectedIndex: number,
	/**
	 * optional additional classes
	 */
	className?: string,
	/**
	 * callback function triggered on selection change
	 */
	onChange: Function
}

/**
 * Tab component
 */
const Tab: React.FC<React.PropsWithChildren<TabProps>> = ({ label, hash, id, name, panel, index, selectedIndex, className, onChange, children }) => {
	const linkId = `${id}_link_${index}`;
	const panelId = `${id}_panel_${index}`;
	const [tabLink, setTabLink] = useState<ReactElement>();

	let tabHash = hash && hash.length ? hash : label.toLowerCase();

	const handleClick = useCallback((e: React.MouseEvent, idx: number) => {
		const element: HTMLAnchorElement = (e.target as HTMLAnchorElement);

		if (element.nodeName == "A") {
			e.preventDefault();
			window.history.pushState(null, "", element.href);
		}

		if (onChange instanceof Function) {
			onChange(idx);
		}
	}, [onChange]);

	useEffect(() => {
		// javascript available - rework component to use links
		setTabLink(<>
			<a href={`#${tabHash}`} onClick={(e) => handleClick(e, index)}>{label}</a>
		</>)
	}, [index, label, tabHash, handleClick]);

	

	let tabLinkClasses = [`${COMPONENT_PREFIX}__link`];

	if (index == selectedIndex) {
		tabLinkClasses.push(`${COMPONENT_PREFIX}__link--selected`);
	}

	return panel ? <>
		{/* render tab panel */}
		<section id={panelId} className={`${COMPONENT_PREFIX}__panel`}>
			{children}
		</section>
	</> : <>
		{/* render tab link */}
		{/* checked / defaultChecked? */}
		{/* TODO: use Input/Radio */}
		<div className={tabLinkClasses.join(' ')}>
			{tabLink ? tabLink : <>
				<input type="radio" name={name} id={linkId} checked={index == selectedIndex ? true : undefined} aria-controls={panelId} onClick={(e) => handleClick(e, index)} />
				<label htmlFor={linkId}>{label}</label>
			</>}
		</div>
	</>;
}

export default Tab;
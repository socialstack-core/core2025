import { useEffect, useId } from 'react';

const COMPONENT_PREFIX = 'tabset';
//const DEFAULT_VARIANT = 'info';

//export type AlertType = 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info';

interface TabSetProps {
	/**
	 * unique name for tab collection (NB: will be used to group underlying radio button controls)
	 */
	name: string,
	/**
	 * unique ID
	 */
	id?: string,
	/**
	 * true if tab links should have no background
	 */
	links?: boolean,
	/**
	 * optional additional classes
	 */
	className?: string,
	/**
	 * current selected tab
	 */
	selectedIndex?: number,
	/**
	 * callback function triggered on selection change
	 */
	onChange?: Function
}

/**
 * TabSet component
 */
const TabSet: React.FC<React.PropsWithChildren<TabSetProps>> = ({ name, id, links, className, selectedIndex, onChange, children }) => {
	let _id = id || useId();
	let componentClasses = [COMPONENT_PREFIX];

	useEffect(() => {
		// javascript available - rework component to use links

	}, []);

	if (links) {
		componentClasses.push(`${COMPONENT_PREFIX}--links`);
	}

	if (className) {
		componentClasses.push(className);
	}

	return (
		<section id={_id} className={componentClasses.join(' ')}>
			{children.map((child, index) =>
				React.cloneElement(child, { name, id: _id, index, selectedIndex: selectedIndex || 0, onChange })
			)}
			<div className={`${COMPONENT_PREFIX}__panels`}>
				{children.map((child, index) =>
					React.cloneElement(child, { id: _id, panel: true, index })
				)}
			</div>
		</section>
	);
}

export default TabSet;
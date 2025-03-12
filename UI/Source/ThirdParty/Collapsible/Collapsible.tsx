import Dropdown, { DropdownItem } from 'UI/Dropdown';
import { useRef, useState, useEffect } from 'react';

/**
 * Props for the collapsible component.
 */
interface CollapsibleProps {
	/**
	 * Additional optional class names.
	 */
	className?: string,
	/**
	 * True if the collapsible is as compact as possible.
	 */
	compact?: boolean,
	/**
	 * True if the collapsible does not apply a min width.
	 */
	noMinWidth?: boolean,
	
	defaultClick?: (e: React.MouseEvent<HTMLElement, MouseEvent>) => void,
	onClick?: () => void,

	/**
	 * True if the collapsible remains always open.
	 */
	alwaysOpen?: boolean,

	/**
	 * True if the collapsible is hidden.
	 */
	hidden?: boolean,

	/**
	 * Title to display
	 */
	dropdownTitle?: string,

	/**
	 * Initial open state.
	 */
	open?: boolean,

	/**
	 * An optional <Icon>
	 */
	icon?: React.ReactNode,

	/**
	 * A title for the collapsible.
	 */
	title?: React.ReactNode,

	/**
	 * A subtitle for the collapsible.
	 */
	subtitle?: React.ReactNode,

	/**
	 * An information region near the collapsible buttons.
	 */
	info?: React.ReactNode,

	/**
	 * Depr: use info instead.
	 * Puts some react elements in the info region
	 */
	jsx?: React.ReactNode,

	/**
	 * 
	 */
	expanderLeft?: boolean,

	/**
	 * Called when the collapsible is opened.
	 */
	onOpen?: React.ToggleEventHandler<HTMLDetailsElement>,

	/**
	 * Called when the collapsible is closed.
	 */
	onClose?: React.ToggleEventHandler<HTMLDetailsElement>,

	/**
	 * An optional set of buttons to display inside the collapsible header.
	 */
	buttons?: CollapsibleButton[]
};

interface CollapsibleButton {

	/**
	 * An optional <Icon>
	 */
	icon?: React.ReactNode,

	/**
	 * True if the text should be visible as a label, otherwise it is title only.
	 */
	showLabel?: boolean

	/**
	 * Optional button text.
	 */
	text?: string,

	/**
	 * Optional link target.
	 */
	target?: string,

	/**
	 * True if this button is disabled
	*/
	disabled?: boolean

	/**
	 * Button style variant. Primary is assumed if not set.
	 */
	variant?: string

	/**
	 * Runs when the button is clicked.
	 * @param e
	 * @returns
	 */
	onClick?: (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void,

	/**
	 * If specified, the button will instead be a dropdown containing these items.
	 */
	children?: DropdownItem[]
}

/**
 * A collapsible region.
 * @param props
 * @returns
 */
const Collapsible: React.FC<React.PropsWithChildren<CollapsibleProps>> = props => {
	var {
		className,
		compact,
		noMinWidth,
		defaultClick,
		alwaysOpen,
		hidden,
		dropdownTitle,
		expanderLeft
	} = props;
	var noContent = !props.children;
	var [isOpen, setOpen] = useState(noContent ? false : !!props.open);

	var hasInfo = props.info;
	var hasJsx = props.jsx;
	var hasButtons = props.buttons && props.buttons.length;

	// NB: include "open" class in addition to [open] attribute as we may be using a polyfill to render this
	var detailsClass = isOpen ? "collapsible open" : "collapsible";
	var summaryClass = noContent ? "btn collapsible-summary no-content" : "btn collapsible-summary";
	var iconClass = expanderLeft || hasButtons ? "collapsible-icon collapsible-icon-left" : "collapsible-icon";

	if (compact) {
		detailsClass += " collapsible--compact";
	}

	if (hidden) {
		detailsClass += " collapsible--hidden";
	}

	if (noContent) {
		iconClass += " invisible";
	}

	if (className) {
		detailsClass += " " + className;
	}

	let largeIcon = props.icon; // must be an <Icon>

	function toggleEvent(e: React.ToggleEvent<HTMLDetailsElement>) {

		if (props.onOpen && e.newState == "open") {
			props.onOpen(e);
		}

		if (props.onClose && e.newState == "closed") {
			props.onClose(e);
		}

	}

	return <details className={detailsClass} onToggle={toggleEvent} open={isOpen} onClick={(e: React.MouseEvent<HTMLElement, MouseEvent>) => {
		if (e.defaultPrevented) {
			return;
		}
		var name = (e.target as HTMLElement).nodeName;
		if (name != 'SUMMARY' && name != 'DETAILS') {
			return;
		}
		if (!alwaysOpen) {
			props.onClick && props.onClick();
			e.preventDefault();
			e.stopPropagation();
			!noContent && setOpen(!isOpen);
		} else {
			e.preventDefault();
			e.stopPropagation();
		}

	}}>
		<summary className={summaryClass} onClick={defaultClick && !alwaysOpen ? (e : React.MouseEvent<HTMLElement, MouseEvent>) => {
			if (e.defaultPrevented) {
				return;
			}
			var name = (e.target as HTMLElement).nodeName;
			if (name != 'SUMMARY' && name != 'DETAILS') {
				return;
			}
			defaultClick && defaultClick(e);
		} : undefined}>
			{(expanderLeft || hasButtons) && !alwaysOpen &&
				<div className={iconClass}>
					{/* NB: icon classes injected dynamically via CSS */}
					<i className="far fa-fw"></i>
				</div>
			}
			{largeIcon}
			<h4 className="collapsible-title">
				{props.title}
				{props.subtitle &&
					<small>
						{props.subtitle}
					</small>
				}
			</h4>
			{!expanderLeft && !hasButtons &&
				<div className={iconClass}>
					<i className="far fa-chevron-down"></i>
				</div>
			}
			{(hasInfo || hasJsx || hasButtons) &&
				<div className="buttons">
					{hasInfo && <span className="info">{props.info}</span>}
					{hasJsx && <span className="jsx">
						{props.jsx}
					</span>}
					{props.buttons?.map(button => {
							var variant = button.variant || 'primary';
							var btnClass = 'btn btn-sm btn-outline-' + variant;

							// split button
							if (button.children && button.children.length) {
								var dropdownJsx = <>
									{button.icon}
									<span className={button.showLabel ? '' : 'sr-only'}>
										{button.text}
									</span>
								</>;

								return <Dropdown label={dropdownJsx} variant={'outline-' + variant} isSmall noMinWidth={noMinWidth}
									disabled={button.disabled} splitCallback={button.onClick} title={dropdownTitle} items={button.children} />;

							}

							// standard button
							if (button.onClick instanceof Function) {
								return <button type="button" className={btnClass} onClick={button.onClick} title={button.text} disabled={button.disabled}>
									{button.icon}
									<span className={button.showLabel ? '' : 'sr-only'}>
										{button.text}
									</span>
								</button>;
							}

							return <a href={button.onClick} className={btnClass} title={button.text} target={button.target}>
								{button.icon}
								<span className={button.showLabel ? '' : 'sr-only'}>
									{button.text}
								</span>
							</a>;

						})
					}
				</div>
			}
		</summary>
		{!noContent &&
			<div className="collapsible-content">
				{props.children}
			</div>
		}
	</details>;
}

export default Collapsible;
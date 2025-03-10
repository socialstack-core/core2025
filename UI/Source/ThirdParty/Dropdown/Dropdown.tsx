/* Bootstrap dropdown
 * ref: https://getbootstrap.com/docs/5.0/components/dropdowns/
  
  params:
  
  align:            alignment of menu with respect to the button (left / right if vertical, top / bottom if horizontal)
  position:         which side of the button the menu will appear on (top, bottom, left or right)
  menuTag:          tag to use to wrap the dropdown contents (defaults to <ul>)
  noMinWidth:		render compact menu width


  example usage:

	<Dropdown label="Dropdown test" variant="link">
		{
			groups.map(group => {
				return (
					<li key={group.id}>
						{href && <>
							<a href={href} className="dropdown-item">
								{group.industryName}
							</a>
						</>}
						{!href && <>
							<button type="button" className="btn dropdown-item" onClick={() => switchCategory(group.id)}>
								{group.industryName}
							</button>
						</>}
					</li>
				);
			})
				  
		}
	</Dropdown>
 
	to insert a heading in the dropdown, use:
	<li>
		<h6 class="dropdown-header">
			Dropdown header
		</h6>
	</li>
 
	to insert a divider in the dropdown, use:
	<li>
		<hr class="dropdown-divider">
	</li>

 */

import { useState, useEffect, useRef } from "react";

// TODO: popper support
//import { Manager, Reference, Popper } from 'UI/Popper';

let lastId = 0;

function newId() {
	lastId++;
	return `dropdown${lastId}`;
}

interface DropdownProps {
	/**
	 * Optional custom classname.
	 */
	className?: string,

	/**
	 * Style variant e.g. primary, secondary etc. If not specified primary is assumed.
	 */
	variant?: string,

	/**
	 * A title which appears when hovering the trigger button.
	 */
	title?: string,

	/**
	 * The label of the trigger button.
	 */
	label?: React.ReactNode,

	/**
	 * Optional custom arrow icon.
	 */
	arrow?: React.ReactElement,

	/**
	 * True if the dropdown should use the outline style.
	 */
	isOutline?: boolean,

	/**
	 * True if it should be a large button.
	 */
	isLarge?: boolean,

	/**
	 * True if it should be a small button.
	 */
	isSmall?: boolean,

	/**
	 * optional function to be called when pressing button (dropdown controlled by a separate button)
	 */
	splitCallback?: (event: React.MouseEvent<HTMLButtonElement>) => void,

	/**
	 * True if the menu starts open.
	 */
	initialState?: boolean,

	/**
	 * keeps the dropdown open after selecting an option (disabled by default)
	 */
	stayOpenOnSelection?: boolean,

	/**
	 * alignment of menu with respect to the button (left / right if vertical, top / bottom if horizontal)
	 */
	align?: string,

	/**
	 * which side of the button the menu will appear on (top, bottom, left or right)
	 */
	position?: string,

	/**
	 * tag to use to wrap the dropdown contents (defaults to <ul>)
	 */
	menuTag?: React.ElementType,

	/**
	 * True if the menu is disabled.
	 */
	disabled?: boolean,

	/**
	 * True if the menu should be as compact as possible.
	 */
	noMinWidth?: boolean
}

/**
 * Dropdown component
 */
const Dropdown: React.FC<React.PropsWithChildren<DropdownProps>> = (props) => {
	var { className, variant, title, label, arrow, isOutline, isLarge, isSmall,
		splitCallback, children, initialState,
		stayOpenOnSelection, align, position, disabled, menuTag, noMinWidth } = props;
	var dropdownClasses = ['dropdown'];

	if (className) {
		dropdownClasses.push(className);
	}

	if (splitCallback) {
		dropdownClasses.push('dropdown--split');
		dropdownClasses.push('btn-group');
		align = "Right";
	}

	// default to dropping down
	if (!position || position.length == 0) {
		position = "Bottom";
	}
	position = position.toLowerCase();

	if (!initialState) {
		initialState = false;
	}

	if (!align) {
		align = "";
	}

	align = align.toLowerCase();

	const MenuTag = menuTag || 'ul';

	// check for invalid alignment
	// (top/bottom position only supports L/R; left/right position only supports T/B)
	switch (position) {
		case 'top':
		case 'bottom':

			if (align == "top" || align == "bottom") {
				align = "";
			}

			break;

		case 'left':
		case 'right':

			if (align == "left" || align == "right") {
				align = "";
			}

			break;
	}

	// set default alignment
	if (!align || align.length == 0) {

		switch (position) {
			case 'top':
			case 'bottom':
				align = splitCallback ? "Right" : "Left";
				break;

			case 'left':
			case 'right':
				align = "Top";
				break;
		}

	}

	dropdownClasses.push('dropdown--align-' + align);
	dropdownClasses.push('dropdown--position-' + position);

	const [open, setOpen] = useState(initialState);
	const dropdownWrapperRef = useRef<HTMLDivElement>(null);
	const toggleRef = useRef(null);
	const dropdownRef = useRef(null);
	var menuItems, firstMenuItem, lastMenuItem;

	if (!variant) {
		variant = "primary";
	}

	var btnClass = [isOutline ? "btn btn-outline-" + variant : "btn btn-" + variant];

	if (isSmall) {
		btnClass.push("btn-sm");
	}

	if (isLarge) {
		btnClass.push("btn-lg");
	}

	var btnClassSplit = btnClass;
	btnClassSplit.push("dropdown-toggle");

	if (splitCallback) {
		btnClassSplit.push("dropdown-toggle-split");
	}

	const [dropdownId] = useState(newId());

	if (!label) {
		label = <>
			Dropdown
		</>;
	}

	// default arrow icon
	if (!arrow) {
		arrow = <svg className="dropdown__chevron" xmlns="http://www.w3.org/2000/svg" overflow="visible" viewBox="0 0 58 34">
			<path d="M29 34c-1.1 0-2.1-.4-2.9-1.2l-25-26c-1.5-1.6-1.5-4.1.1-5.7 1.6-1.5 4.1-1.5 5.7.1l22.1 23 22.1-23c1.5-1.6 4.1-1.6 5.7-.1s1.6 4.1.1 5.7l-25 26c-.8.8-1.8 1.2-2.9 1.2z" fill="currentColor" />
		</svg>;
	}

	function handleClick(event: MouseEvent) {
		// toggle dropdown
		if (toggleRef && toggleRef.current == event.target) {

			// using if (open) proved unreliable
			if (dropdownRef && dropdownRef.current) {
				closeDropdown();
			} else {
				setOpen(true);
			}

			return;
		}

		// clicked a form control within a dropdown?
		// assume we want to keep the menu open
		var target = event.target as HTMLElement;

		if (!target) {
			return;
		}

		var targetInput = target as HTMLInputElement;

		var isFormControl = targetInput?.type == 'radio' || targetInput?.type == 'checkbox' ||
			target.nodeName.toUpperCase() == 'LABEL' && target.classList.contains("form-check-label");

		// if we want to close even if we selected an item in the dropdown (default)
		if (!stayOpenOnSelection && !isFormControl) {
			closeDropdown();
		} else {

			// only close if we clicked outside of the dropdown
			// (leave dropdown open after making a selection)
			if (dropdownWrapperRef && !dropdownWrapperRef.current?.contains(target)) {
				closeDropdown();
			}
		}
	}

	function closeDropdown() {
		setOpen(false);
	}

	useEffect(() => {
		window.document.addEventListener("click", handleClick);
		
		return () => {
			window.document.removeEventListener("click", handleClick);
		};

	}, []);

	let dropdownMenuClass = ['dropdown-menu'];

	if (noMinWidth) {
		dropdownMenuClass.push('dropdown-menu--no-minwidth');
	}

	return (
		<div className={dropdownClasses.join(' ')} ref={dropdownWrapperRef}>
			{/* TODO: popper support
            <Manager>
                <Reference>
                    {() => (
                        */}

			{/* standard dropdown button */}
			{!splitCallback && (
				<button
					className={btnClassSplit.join(' ')}
					type="button"
					id={dropdownId}
					aria-expanded={open}
					aria-label={title}
					ref={toggleRef}
					disabled={disabled}
					title={title}>

					{position == "left" && <>
						<span className="dropdown__arrow">
							{arrow}
						</span>
						<span className="dropdown__label">
							{label}
						</span>
					</>}

					{position != "left" && <>
						<span className="dropdown__label">
							{label}
						</span>
						<span className="dropdown__arrow">
							{arrow}
						</span>
					</>}
				</button>
			)}

			{/* split dropdown button */}
			{splitCallback && <>
				<button
					className={btnClass.join(' ')}
					type="button"
					id={dropdownId}
					onClick={splitCallback}
					onAuxClick={splitCallback} // for middle mouse button support
					disabled={disabled}
					title={title}>
					<span className="dropdown__label">
						{label}
					</span>
				</button>
				<button
					className={btnClassSplit.join(' ')}
					type="button"
					aria-expanded={open}
					ref={toggleRef}
					disabled={disabled}
					title={`Options`}>
					<span className="dropdown__arrow">
						{arrow}
					</span>
				</button>
			</>}

			{/* dropdown contents */}
			{open && (
				<MenuTag className={dropdownMenuClass.join(' ')} data-source={className} aria-labelledby={dropdownId} ref={dropdownRef}>
					{children}
				</MenuTag>
			)}
			{/* TODO: popper support
                    )}
                </Reference>
                {open && (
                    <Popper
                        placement="top-start"
                        positionFixed={false}
                        modifiers={modifiers}>
                        {({ ref, style, placement, arrowProps }) => (
                            <ul className="dropdown-menu" aria-labelledby={dropdownId}>
                                {children}
                            </ul>
                        )}
                    </Popper>
                )}
            </Manager>
                */}
		</div>
	);
}

export default Dropdown;

/*
Dropdown.propTypes = {
	stayOpenOnSelection: 'bool',
	align: ['Top', 'Bottom', 'Left', 'Right'],
	position: ['Top', 'Bottom', 'Left', 'Right']
};

Dropdown.defaultProps = {
	stayOpenOnSelection: false,
	align: 'Left',
	position: 'Bottom'
}

Dropdown.icon = 'caret-square-down';
*/
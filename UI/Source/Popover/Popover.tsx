import { useEffect, useRef } from 'react';
//import { toggleFocusable } from 'UI/Functions/ToggleFocusable';

export type PopoverAlignment = 'left' | 'right' | 'top' | 'bottom' | 'center';
const DEFAULT_METHOD = 'auto';
const DEFAULT_ALIGNMENT = 'left';

/**
 * Props for the Popover component.
 */
interface PopoverProps {
	/**
	 * popover alignment (see PopoverAlignment for supported options) 
	 */
	alignment?: PopoverAlignment,

	/**
	 * auto (default)
	 * - allow closing via clicking elsewhere / pressing esc
	 * 
	 * manual
	 * - can only be displayed / closed using declarative buttons or JavaScript
	 * 
	 * hint (DON'T USE)
	 * - doesn't close auto popovers but will close other hints
	 * NB: not to be used until we find a workaround for Firefox/Safari which currently don't support this
	 * 
	 */
	method?: string,

	/**
	 * optional additional classes
	 */
	className?: string,

	/**
	 * unique ID
	 */
	id: string,

	/**
	 * set true if background should blur when popover is open
	 */
	blurBackground?: boolean,

	/**
	 * set true if top should be aligned underneath header
	 */
	underHeader?: boolean,

	/**
	 * set true if background should remain accessible
	 */
	backgroundActive?: boolean,

	/**
	 * optional flags to indicate when popover should appear without trigger
	 * (e.g. filters - could be behind a trigger for mobile, but visible on desktop)
	 */
	tabletPortraitVisible?: boolean,	// 753px +
	tabletLandscapeVisible?: boolean,	// 1024px +
	desktopVisible?: boolean,			// 1360px +
}

/**
 * The Popover React component.
 * @param props React props.
 */
const Popover: React.FC<PopoverProps> = (props) => {
	const { className, id, children, blurBackground, underHeader, backgroundActive,
		tabletPortraitVisible, tabletLandscapeVisible, desktopVisible } = props;
	const method = props.method || DEFAULT_METHOD;
	const alignment = props.alignment || DEFAULT_ALIGNMENT;
	const popoverRef = useRef();

	useEffect(() => {
		let minWidth = 0;

		if (tabletPortraitVisible) {
			minWidth = 753;
		}
		if (tabletLandscapeVisible) {
			minWidth = 1024;
		}
		if (desktopVisible) {
			minWidth = 1360;
		}

		// for those wondering "but ... why?!" with respect to CSS embedded in the component;
		// this is so that behaviour can be controlled on a per-popover basis
		// (e.g. some popovers may blur the background, some may not)
		const mediaQuery = (minWidth == 0) ? '' : `
			@media only screen and (min-width: ${minWidth}px) {
				#${id} {
					position: static;
					visibility: visible;
					transform: none;
					padding: 0;
					display: block;
					box-shadow: none;
					opacity: 1;
					width: 100%;
				}

				#${id}::backdrop {
					background-color: transparent !important;
				}

				[popovertarget="${id}"] {
					display: none;
				}
			}`;

		const backgroundRules = `
			filter: blur(2px) grayscale(25%);
			pointer-events: none;
		`;

		// NB: odd-looking ".\:popover-open" references are required by the popover API polyfill
		const styleEl = document.createElement('style');
		styleEl.textContent = `
			body:has(#${id}.ui-popover--blur-bg.\:popover-open) {
				#content {
					${backgroundRules}

					~ footer {
						${backgroundRules}
					}
				}
			}
			body:has(#${id}.ui-popover--blur-bg:popover-open) {
				#content {
					${backgroundRules}

					~ footer {
						${backgroundRules}
					}
				}
			}

			body:has(#${id}.ui-popover--full.ui-popover--blur-bg.\:popover-open) {
				#wrapper > header {
					${backgroundRules}
				}
			}
			body:has(#${id}.ui-popover--full.ui-popover--blur-bg:popover-open) {
				#wrapper > header {
					${backgroundRules}
				}
			}

			${mediaQuery}
		`;
		document.head.appendChild(styleEl);

		if (popoverRef.current) {
			//popoverRef.current.addEventListener("beforetoggle", toggleHandler);
			popoverRef.current.addEventListener("focusout", focusHandler);
		}

		return () => {

			if (popoverRef.current) {
				//popoverRef.current.removeEventListener("beforetoggle", toggleHandler);
				popoverRef.current.removeEventListener("focusout", focusHandler);
			}

			document.head.removeChild(styleEl);
		};
	}, []);

/*
	// disable background while popover is open (unless backgroundActive set),
	// - while we could use <dialog> to gain modal support, this can only be controlled via JavaScript
	// - it's also an all or nothing approach (e.g. we can't disable everything *except* the header)
	// - we use the Popover API to allow panels to be toggled without relying on JavaScript support
	// - can't use HTML inert attribute as this is liable to disable the contents of the popover itself
	// - note that CSS pointer-events rules are used to disable mouse interaction
	const toggleHandler = (event) => {

		if (backgroundActive) {
			return;
		}

		var reactRoot = window.SERVER ? undefined : document.querySelector("#react-root");

		if (!reactRoot) {
			return;
		}

		const header = reactRoot.querySelector("#wrapper > header");
		const content = reactRoot.querySelector("#wrapper > #content");
		const footer = reactRoot.querySelector("#wrapper > #content ~ footer");

		if (event.newState === "open") {

			if (header && !underHeader) {
				toggleFocusable(header, false);
			}

			if (content) {
				toggleFocusable(content, false);
			}

			if (footer) {
				toggleFocusable(footer, false);
			}

		} else {

			if (header && !underHeader) {
				toggleFocusable(header, true);
			}

			if (content) {
				toggleFocusable(content, true);
			}

			if (footer) {
				toggleFocusable(footer, true);
			}

		}

	};
*/
	// checks for focus leaving the popover - close if this happens, otherwise we run the risk of focusing a blurred background element
	const focusHandler = () => {

		if (backgroundActive) {
			return;
		}

		setTimeout(() => {
			if (!popoverRef.current.contains(document.activeElement)) {
				popoverRef.current.hidePopover();
			}
		}, 0);
	};

	let popoverClasses = ['ui-popover'];
	popoverClasses.push(`ui-popover--${alignment}`);

	if (blurBackground) {
		popoverClasses.push("ui-popover--blur-bg");
	}

	if (underHeader) {
		popoverClasses.push("ui-popover--under-header");
	} else {
		popoverClasses.push("ui-popover--full");
	}

	if (className) {
		popoverClasses.push(className);
	}

	return (
		<div className={popoverClasses.join(' ')} popover={method} id={id} ref={popoverRef}>
			{children}
		</div>
	);
}

export default Popover;
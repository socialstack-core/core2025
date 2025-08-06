import { useEffect, useRef } from 'react';

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

		const mediaQuery = (minWidth == 0) ? '' : `
			@media only screen and (min-width: ${minWidth}px) {
				#${id} {
					visibility: visible;
					transform: none;
				}

				#${id}::backdrop {
					background-color: transparent !important;
				}

				[popovertarget="${id}"] {
					display: none;
				}
			}`;

		const styleEl = document.createElement('style');
		styleEl.textContent = `
			body:has(#${id}.ui-popover--blur-bg.\:popover-open) {
				#content {
					filter: blur(2px) grayscale(25%);

					~ footer {
						filter: blur(2px) grayscale(25%);
					}
				}
			}
			body:has(#${id}.ui-popover--blur-bg:popover-open) {
				#content {
					filter: blur(2px) grayscale(25%);

					~ footer {
						filter: blur(2px) grayscale(25%);
					}
				}
			}

			body:has(#${id}.ui-popover--full.ui-popover--blur-bg.\:popover-open) {
				#wrapper > header {
					filter: blur(2px) grayscale(25%);
				}
			}
			body:has(#${id}.ui-popover--full.ui-popover--blur-bg:popover-open) {
				#wrapper > header {
					filter: blur(2px) grayscale(25%);
				}
			}

			${mediaQuery}
		`;
		document.head.appendChild(styleEl);

		if (popoverRef.current) {
			popoverRef.current.addEventListener("beforetoggle", toggleHandler);
		}

		return () => {

			if (popoverRef.current) {
				popoverRef.current.removeEventListener("beforetoggle", toggleHandler);
			}

			document.head.removeChild(styleEl);
		};
	}, []);

	// toggle inert status on background while popover is open (unless backgroundActive set),
	// NB: although the Popover API does support true modals, these are only accessible via JavaScript
	//     doing it in this way allows us to support popovers without relying on JS,
	//     while using progressive enhancement to disable the background when JS is available
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
				header.setAttribute("inert", "");
			}

			if (content) {
				content.setAttribute("inert", "");
			}

			if (footer) {
				footer.setAttribute("inert", "");
			}

		} else {

			if (header && !underHeader) {
				header.removeAttribute("inert");
			}

			if (content) {
				content.removeAttribute("inert");
			}

			if (footer) {
				footer.removeAttribute("inert");
			}

		}

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
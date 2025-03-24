import Summary from './Summary';
import Content from './Content';
import { useEffect, useRef } from 'react';

export default function Details(props) {
	const { name, label, summaryChildren, open, className, children } = props;
	const detailsRef = useRef();
	const CSS_ANIM_SUPPORTED = CSS.supports("interpolate-size", "allow-keywords");
	let animation = null;
	let isClosing = false;
	let isExpanding = false;

	let componentClasses = ['details'];

	if (className) {
		componentClasses.push(className);
	}

	useEffect(() => {

		if (detailsRef.current) {
			detailsRef.current.addEventListener("click", clickHandler);
			detailsRef.current.addEventListener("toggle", toggleHandler);
		}

		return () => {

			if (detailsRef.current) {
				detailsRef.current.removeEventListener("click", clickHandler);
				detailsRef.current.removeEventListener("toggle", toggleHandler);
			}

		};

	});

	function clickHandler(e) {

		// fallback for when we don't support the CSS-only animation method
		// (Firefox / Safari, as of March 2025)
		if (!CSS_ANIM_SUPPORTED) {
			e.preventDefault();

			detailsRef.current.style.overflow = 'hidden';

			if (isClosing || !detailsRef.current.open) {
				expand();
			} else if (isExpanding || detailsRef.current.open) {
				contract();
			}

		}

	}

	function expand() {
		detailsRef.current.style.height = `${detailsRef.current.offsetHeight}px`;
		detailsRef.current.open = true;

		// wait for the next frame to trigger the expansion
		window.requestAnimationFrame(() => expand2());
	}

	function expand2() {
		isExpanding = true;
		let summary = detailsRef.current.querySelector("summary");
		let content = detailsRef.current.querySelector(".details__content");

		if (!summary || !content) {
			return;
		}

		// Get the current fixed height of the element
		const startHeight = `${detailsRef.current.offsetHeight}px`;

		// Calculate the open height of the element (summary height + content height)
		const endHeight = `${summary.offsetHeight + content.offsetHeight}px`;

		// cancel if animation is already running
		if (animation) {
			animation.cancel();
		}

		// Start a WAAPI animation
		animation = detailsRef.current.animate({
			// Set the keyframes from the startHeight to endHeight
			height: [startHeight, endHeight]
		}, {
			duration: 600,
			easing: 'ease'
		});

		animation.onfinish = () => onAnimationFinish(true);
		animation.oncancel = () => isExpanding = false;
	}

	function contract() {
		isClosing = true;
		let summary = detailsRef.current.querySelector("summary");

		if (!summary) {
			return;
		}

		// Store the current height of the element
		const startHeight = `${detailsRef.current.offsetHeight}px`;

		// Calculate the height of the summary
		const endHeight = `${summary.offsetHeight}px`;

		// cancel if animation is already running
		if (animation) {
			animation.cancel();
		}

		// Start a WAAPI animation
		animation = detailsRef.current.animate({
			// Set the keyframes from the startHeight to endHeight
			height: [startHeight, endHeight]
		}, {
			duration: 600,
			easing: 'ease'
		});

		animation.onfinish = () => onAnimationFinish(false);
		animation.oncancel = () => isClosing = false;
	}
	
	function onAnimationFinish(open) {
		detailsRef.current.open = open;

		animation = null;
		isClosing = false;
		isExpanding = false;

		detailsRef.current.style.height = '';
		detailsRef.current.style.overflow = '';
	}

	function toggleHandler(e) {
		/*

		if (e.target.open && isOpening) {
			return;
		}

		if (!e.target.open && isClosing) {
			return;
		}

		if (e.target.open) {
			console.log("open: ", e.target);
		} else {
			console.log("closed: ", e.target);
		}

		if (!CSS_ANIM_SUPPORTED) {
			e.preventDefault();

			clickHandler(e);
		}

		*/
	}

	return (
		<details className={componentClasses.join(' ')} name={name} open={open === true ? true : undefined} ref={detailsRef}>
			{/* assume <Summary> and <Content> are already supplied */}
			{!label && !summaryChildren && <>
				{children}
			</>}

			{(label || summaryChildren) && <>
				<Summary label={label}>
					{summaryChildren}
				</Summary>
				<Content>
					{children}
				</Content>
			</>}
		</details>
	);
}

Details.propTypes = {
	name: 'string',
	label: 'string',
	open: 'boolean',
	className: 'string',
	summaryChildren: 'jsx'
};

Details.defaultProps = {
}

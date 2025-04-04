import { useState } from 'react';
import Button from 'UI/Button';

interface ToggleBootstrapProps {
}

export default function ToggleBootstrap({ }: ToggleBootstrapProps) {
	const [included, setIncluded] = useState(false);

	function toggleBootstrapStyles() {
		let bootstrapCdn = document.getElementById("bootstrap_cdn");

		if (bootstrapCdn) {
			bootstrapCdn.remove();
			setIncluded(false);
		} else {
			let origin = 'http://localhost:5050';
			let injectBootstrapURL = `${origin}/inject-bootstrap.css`;

			// any third-party styling should be referenced directly after the `<meta charset />` tag in the head
			// this ensures our classes get a chance to override
			//document.head.querySelector("meta[charset]").insertAdjacentHTML('afterend', `<link id="bootstrap_cdn" type="text/css" crossorigin="anonymous" rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">`);
			document.head.querySelector("meta[charset]").insertAdjacentHTML('afterend', `<link id="bootstrap_cdn" type="text/css" rel="stylesheet" href=${injectBootstrapURL}>`);
			setIncluded(true);
		}

	}

	let classNames = ['ui-toggle-bootstrap'];

	if (!included) {
		classNames.push('ui-btn--danger');
	} else {
		classNames.push('ui-btn--success');
	}

	return (
		<Button className={classNames.join(' ')} onClick={() => toggleBootstrapStyles()}>
			<svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 16 16">
				<path d="M5.062 12h3.475c1.804 0 2.888-.908 2.888-2.396 0-1.102-.761-1.916-1.904-2.034v-.1c.832-.14 1.482-.93 1.482-1.816 0-1.3-.955-2.11-2.542-2.11H5.062zm1.313-4.875V4.658h1.78c.973 0 1.542.457 1.542 1.237 0 .802-.604 1.23-1.764 1.23zm0 3.762V8.162h1.822c1.236 0 1.887.463 1.887 1.348 0 .896-.627 1.377-1.811 1.377z"></path>
				<path d="M0 4a4 4 0 0 1 4-4h8a4 4 0 0 1 4 4v8a4 4 0 0 1-4 4H4a4 4 0 0 1-4-4zm4-3a3 3 0 0 0-3 3v8a3 3 0 0 0 3 3h8a3 3 0 0 0 3-3V4a3 3 0 0 0-3-3z"></path>
			</svg>
			{included && `Bootstrap ON`}
			{!included && `Bootstrap OFF`}
		</Button>
	);
}


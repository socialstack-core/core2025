import { useState } from 'react';
import Button from 'UI/Button';

interface ToggleBootstrapRevertProps {
}

export default function ToggleBootstrapRevert({ }: ToggleBootstrapRevertProps) {
	const [included, setIncluded] = useState(false);

	function toggleBootstrapRevertStyles() {
		let bootstrapRevert = document.getElementById("bootstrap_revert");

		if (bootstrapRevert) {
			setIncluded(!included);
			bootstrapRevert.setAttribute("media", !included ? "screen" : "print");
		}
	}

	let classNames = ['ui-toggle-bootstrap-revert'];

	if (!included) {
		classNames.push('ui-btn--danger');
	} else {
		classNames.push('ui-btn--success');
	}

	return (
		<Button className={classNames.join(' ')} onClick={() => toggleBootstrapRevertStyles()}>
			<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
				<path d="M21 17a9 9 0 0 0-15-6.7L3 13" />
				<path d="M3 7v6h6" />
				<circle cx="12" cy="17" r="1" />
			</svg>
			{included && `Bootstrap reversion ON`}
			{!included && `Bootstrap reversion OFF`}
		</Button>
	);
}

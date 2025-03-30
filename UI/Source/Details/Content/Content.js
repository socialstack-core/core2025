const COMPONENT_PREFIX = 'details__content';

export default function Content(props) {
	const { children, className } = props;

	let componentClasses = [COMPONENT_PREFIX];

	if (className) {
		componentClasses.push(className);
	}

	return (
		<div className={componentClasses.join(' ')}>
			{children}
		</div>
	);
}

Content.propTypes = {
	label: 'string'
};

Content.defaultProps = {
}

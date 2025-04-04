const COMPONENT_PREFIX = 'ui-dialog';

export default function Body(props) {
	const { children, className } = props;

	let componentClasses = [`${COMPONENT_PREFIX}__body`];

	if (className) {
		componentClasses.push(className);
	}

	return (
		<div className={componentClasses.join(' ')}>
			{children}
		</div>
	);
}

Body.propTypes = {
};

Body.defaultProps = {
}

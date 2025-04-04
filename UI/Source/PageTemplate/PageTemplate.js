export default function PageTemplate(props) {
	const { className, children } = props;

	let componentClasses = ['ui-page-template'];

	if (className) {
		componentClasses.push(className);
	}

	return (
		<div className={componentClasses.join(' ')}>
			{children}
		</div>
	);
}

PageTemplate.propTypes = {
};

PageTemplate.defaultProps = {
}

PageTemplate.icon='align-center';

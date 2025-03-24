export default function PageRegion(props) {
	const { landmark, tag, sticky, className, children } = props;
	const Tag = tag;

	let componentClasses = ['page-region'];

	if (sticky) {
		componentClasses.push('page-region--sticky');
	}

	if (className) {
		componentClasses.push(className);
	}

	return (
		<Tag className={componentClasses.join(' ')} role={landmark}>
			{children}
		</Tag>
	);
}

PageRegion.propTypes = {
	landmark: 'string',
	tag: 'string',
	sticky: 'boolean'
};

PageRegion.defaultProps = {
}

PageRegion.icon='align-center';

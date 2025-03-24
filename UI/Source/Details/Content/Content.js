export default function Content(props) {
	const { children } = props;

	return (
		<div className="details__content">
			{children}
		</div>
	);
}

Content.propTypes = {
	label: 'string'
};

Content.defaultProps = {
}

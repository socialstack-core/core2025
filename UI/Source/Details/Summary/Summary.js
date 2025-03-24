export default function Summary(props) {
	const { label, children } = props;

	return (
		<summary className="details__summary">
			{children || label}
		</summary>
	);
}

Summary.propTypes = {
	label: 'string'
};

Summary.defaultProps = {
}

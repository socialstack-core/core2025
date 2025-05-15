/**
 * Props for the Signpost component.
 */
interface SignpostProps {
	/**
	 * An example optional fileRef prop.
	 */
	// logoRef?: FileRef
}

/**
 * The Signpost React component.
 * @param props React props.
 */
const Signpost: React.FC<SignpostProps> = (props) => {
	return (
		<div className="ui-product-signpost">
		</div>
	);
}

export default Signpost;
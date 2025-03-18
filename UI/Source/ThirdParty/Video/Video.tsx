/**
 * Props for the Video component.
 */
interface VideoProps {
	/**
	 * An example optional fileRef prop.
	 */
	// logoRef?: FileRef
}

/**
 * The Video React component.
 * @param props React props.
 */
const Video: React.FC<VideoProps> = (props) => {
	return (
		<div className="video">
		</div>
	);
}

export default Video;
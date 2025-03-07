import Icon from 'UI/Icon';

/**
 * Props for the Loading component.
 */
interface LoadingProps extends React.HTMLAttributes<HTMLSpanElement> {
	/**
	 * Optionally change the message being displayed.
	 */
	message?: string
}

/**
 * Standalone component which displays a loader (typically a spinner).
*/
const Loading: React.FC<LoadingProps> = (props) => {
	let message = props.message || `Loading ... `;
	
	return (
		<div className="alert alert-info loading">
			{message}
			<Icon type="fa-spinner" solid spin />
		</div>
	);
}

export default Loading;
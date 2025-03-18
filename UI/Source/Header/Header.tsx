import Image from 'UI/Image';

/**
 * Props for the Header component.
 */
interface HeaderProps {
	/**
	 * The website logo.
	 */
	logoRef?: FileRef
}

/**
 * The website header React component.
 * @param props React props.
 */
const Header: React.FC<HeaderProps> = (props) => {
	return (
		<div className="header">
			Hello, header world! 
			<Image fileRef={props.logoRef} />
		</div>
	);
}

export default Header;
import getRef from 'UI/Functions/GetRef';

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
			{props.logoRef && getRef(props.logoRef)}
		</div>
	);
}

export default Header;
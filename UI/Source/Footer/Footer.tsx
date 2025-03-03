/**
 * Props for the Footer component.
 */
interface FooterProps {

}

/**
 * The Footer React component.
 * @param props React props.
 */
const Footer: React.FC<FooterProps> = (props) => {
	return <div className="footer">
		Hello, footer world!
	</div>;
}

export default Footer;
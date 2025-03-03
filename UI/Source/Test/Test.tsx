import Header from 'UI/Header';
import Footer from 'UI/Footer';
import testImageRef from './test.jpg';

/**
 * Props for the Test component.
 */
interface TestProps {
	/**
	 * An example optional fileRef prop.
	 */
	// logoRef?: FileRef
}

/**
 * The Test React component.
 * @param props React props.
 */
const Test: React.FC<TestProps> = (props) => {
	return (
		<div className="test">
			<Header logoRef={testImageRef} />
			<p>
				Other assorted test content
			</p>
			<Footer />
		</div>
	);
}

export default Test;
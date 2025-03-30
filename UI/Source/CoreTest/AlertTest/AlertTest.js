import { useState, useEffect, useRef } from 'react';
import Alert from 'UI/Alert';
import RequestFullscreen from 'UI/RequestFullscreen';

export default function AlertTest(props) {
	//const { title, size, width } = props;
	//const [width, setWidth] = useState(window.innerWidth);
	const parentRef = useRef();

	/* runs only after component initialisation (comparable to legacy componentDidMount lifecycle method)
	useEffect(() => {
		// ...
	}, []);
	*/

	return <div className="alert-test" ref={parentRef}>
		<RequestFullscreen elementRef={parentRef} />

		<section className="component-test">
			<Alert>
				Alerts defined without a <code>variant</code> supplied default to the <code>info</code> style.
			</Alert>
		</section>

		<h2 className="component-test-category">Alert variants</h2>
		<section className="component-test">
			<Alert variant="primary" title={`Primary`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>

			<Alert variant="secondary" title={`Secondary`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>

			<Alert variant="success" title={`Success`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>

			<Alert variant="danger" title={`Danger`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>

			<Alert variant="warning" title={`Warning`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>

			<Alert variant="info" title={`Info`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>
		</section>

		<h2 className="component-test-category">Dismissable alerts</h2>
		<section className="component-test">
			<Alert dismissable variant="primary" title={`Primary`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>

			<Alert dismissable variant="secondary" title={`Secondary`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>

			<Alert dismissable variant="success" title={`Success`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>

			<Alert dismissable variant="danger" title={`Danger`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>

			<Alert dismissable variant="warning" title={`Warning`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>

			<Alert dismissable variant="info" title={`Info`}>
				Lorem ipsum dolor sit amet consectetur adipisicing elit. Illo alias <a href="#">aspernatur aut</a>, eaque odio numquam molestiae nihil quas aperiam non quod vero accusantium ipsum tempore corporis nulla, mollitia minus. Quos?
			</Alert>
		</section>
	</div>;
}

AlertTest.propTypes = {
};

AlertTest.defaultProps = {
}

AlertTest.icon = 'align-center';

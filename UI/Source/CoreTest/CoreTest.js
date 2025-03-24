import PageTemplate from 'UI/PageTemplate';
import PageRegion from 'UI/PageRegion';
import ThemeSwitcher from 'UI/ThemeSwitcher';
import Details from 'UI/Details';
import Summary from 'UI/Details/Summary';
import Content from 'UI/Details/Content';
import Alert from 'UI/Alert';
import Button from 'UI/Button';

export default function CoreTest(props) {
	// reference propTypes
	//const { title, size, width } = props;

	/* access session info, such as the currently logged-in user:
	const { session } = useSession();
	// session.user
	// session.locale
	*/

	/* runs only after component initialisation (comparable to legacy componentDidMount lifecycle method)
	useEffect(() => {
		// ...
	}, []);
	*/

	/* runs after both component initialisation and each update (comparable to legacy componentDidMount / componentDidUpdate lifecycle methods)
	useEffect(() => {
		// ...
	});
	*/

	/* to handle window events such as resize / scroll, etc:
	const [width, setWidth] = useState(window.innerWidth);
	useEffect(() => {
		const handleResize = () => setWidth(window.innerWidth);
		window.addEventListener('resize', handleResize);
		
		// optional return used to clean up
		return () => {
			window.removeEventListener('resize', handleResize);
		};
		
	});


	/* reference images in the same component folder:
	var vectorUrl = getRef(myVectorImage, { url: true });
	var rasterUrl = getRef(myRasterImage, { size: 128, url: true }); // where size represents the closest size required (see Api\ThirdParty\Uploader\UploaderConfig.cs for supported sizes)
	// omit size parameter to return original image resolution
	*/

	var summaryTest = <>
		<h4>
			Testing <u>summary</u> content
		</h4>
	</>;

	return (
		<PageTemplate className="core-test">
			<PageRegion landmark="banner" tag="header" sticky>
				<nav className="site-header">
					<ThemeSwitcher />
				</nav>
			</PageRegion>
			<PageRegion name="content" tag="div">
				<PageTemplate>
					{/*
					<PageRegion landmark="navigation" tag="nav">
						<menu>
							<li>
								<a href="#" onclick="loadPage('/pages/alerts.html'); return false;">Alerts</a>
							</li>
						</menu>
					</PageRegion>
					*/}
					<PageRegion landmark="main" tag="main">
						<div className="themed-panel">
							<h1>
								SocialStack CSS Test
							</h1>
							<p>
								Select a category from the list to preview
							</p>
							<hr />

							{/* details example with custom summary / content
							<Details name="demo">
								<Summary>
									{summaryTest}
								</Summary>
								<Content>
									Lorem, ipsum dolor sit amet consectetur adipisicing elit. Ea harum, dignissimos ut minus nesciunt modi corrupti, similique laboriosam doloribus animi, quibusdam dolor esse reiciendis quaerat voluptatem. Ut dolore sit aliquam.
								</Content>
							</Details>
							*/}

							{/* details example with custom summary
							<Details name="demo" summaryChildren={summaryTest}>
								Lorem, ipsum dolor sit amet consectetur adipisicing elit. Ea harum, dignissimos ut minus nesciunt modi corrupti, similique laboriosam doloribus animi, quibusdam dolor esse reiciendis quaerat voluptatem. Ut dolore sit aliquam.
							</Details>
							*/}

							<Details name="demo" label={`Alerts`}>
								<section className="component-test">
									<Alert>
										Alerts defined without a <code>variant</code> supplied default to the <code>info</code> style.
									</Alert>

									<hr/>

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
							</Details>
							<Details name="demo" label={`Buttons`}>

								<section className="component-test component-spacing">
									<Button close />
									<Button close outline />

									&nbsp;

									<Button close disabled />
									<Button close outline disabled />
								</section>

								<section className="component-test component-spacing">
									<Button variant="primary" close />
									<Button variant="secondary" close />
									<Button variant="success" close />
									<Button variant="danger" close />
									<Button variant="warning" close />
									<Button variant="info" close />

									&nbsp;

									<Button variant="primary" close disabled />
									<Button variant="secondary" close disabled />
									<Button variant="success" close disabled />
									<Button variant="danger" close disabled />
									<Button variant="warning" close disabled />
									<Button variant="info" close disabled />
								</section>

								<section className="component-test component-spacing">
									<Button variant="primary" outline close />
									<Button variant="secondary" outline close />
									<Button variant="success" outline close />
									<Button variant="danger" outline close />
									<Button variant="warning" outline close />
									<Button variant="info" outline close />

									&nbsp;

									<Button variant="primary" outline close disabled />
									<Button variant="secondary" outline close disabled />
									<Button variant="success" outline close disabled />
									<Button variant="danger" outline close disabled />
									<Button variant="warning" outline close disabled />
									<Button variant="info" outline close disabled />
								</section>

								<hr />

								<section className="component-test component-spacing">
									<Button variant="primary">
										Primary
									</Button>
									<Button variant="secondary">
										Secondary
									</Button>
									<Button variant="success">
										Success
									</Button>
									<Button variant="danger">
										Danger
									</Button>
									<Button variant="warning">
										Warning
									</Button>
									<Button variant="info">
										Info
									</Button>
								</section>

								<section className="component-test component-spacing">
									<Button variant="primary" disabled>
										Primary
									</Button>
									<Button variant="secondary" disabled>
										Secondary
									</Button>
									<Button variant="success" disabled>
										Success
									</Button>
									<Button variant="danger" disabled>
										Danger
									</Button>
									<Button variant="warning" disabled>
										Warning
									</Button>
									<Button variant="info" disabled>
										Info
									</Button>
								</section>

								<hr/>

								<section className="component-test component-spacing">
									<Button variant="primary" outline>
										Primary
									</Button>
									<Button variant="secondary" outline>
										Secondary
									</Button>
									<Button variant="success" outline>
										Success
									</Button>
									<Button variant="danger" outline>
										Danger
									</Button>
									<Button variant="warning" outline>
										Warning
									</Button>
									<Button variant="info" outline>
										Info
									</Button>
								</section>

								<section className="component-test component-spacing">
									<Button variant="primary" outline disabled>
										Primary
									</Button>
									<Button variant="secondary" outline disabled>
										Secondary
									</Button>
									<Button variant="success" outline disabled>
										Success
									</Button>
									<Button variant="danger" outline disabled>
										Danger
									</Button>
									<Button variant="warning" outline disabled>
										Warning
									</Button>
									<Button variant="info" outline disabled>
										Info
									</Button>
								</section>
							</Details>
							{/*
							<Details name="demo" label={`Dialog`}>
								Lorem, ipsum dolor sit amet consectetur adipisicing elit. Ea harum, dignissimos ut minus nesciunt modi corrupti, similique laboriosam doloribus animi, quibusdam dolor esse reiciendis quaerat voluptatem. Ut dolore sit aliquam.
								Lorem, ipsum dolor sit amet consectetur adipisicing elit. Ea harum, dignissimos ut minus nesciunt modi corrupti, similique laboriosam doloribus animi, quibusdam dolor esse reiciendis quaerat voluptatem. Ut dolore sit aliquam.
								Lorem, ipsum dolor sit amet consectetur adipisicing elit. Ea harum, dignissimos ut minus nesciunt modi corrupti, similique laboriosam doloribus animi, quibusdam dolor esse reiciendis quaerat voluptatem. Ut dolore sit aliquam.
							</Details>
							*/}
						</div>
					</PageRegion>
				</PageTemplate>
			</PageRegion>
			<PageRegion landmark="contentinfo" tag="footer">
				<nav className="site-footer">
					Footer
				</nav>
			</PageRegion>
		</PageTemplate>
	);
}

CoreTest.propTypes = {
};

CoreTest.defaultProps = {
}

CoreTest.icon='align-center';

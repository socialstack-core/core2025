import PageTemplate from 'UI/PageTemplate';
import PageRegion from 'UI/PageRegion';
import ThemeSwitcher from 'UI/ThemeSwitcher';
import Details from 'UI/Details';
//import Summary from 'UI/Details/Summary';
//import Content from 'UI/Details/Content';
import AlertTest from './AlertTest';
import ButtonTest from './ButtonTest';
import CloseButtonTest from './CloseButtonTest';
import DialogTest from './DialogTest';
import EmbedTest from './EmbedTest';
import FormTest from './FormTest';

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
								<AlertTest />
							</Details>
							<Details name="demo" label={`Buttons`}>
								<ButtonTest />
							</Details>
							<Details name="demo" label={`Close buttons`}>
								<CloseButtonTest />
							</Details>
							<Details name="demo" label={`Dialogs`}>
								<DialogTest />
							</Details>
							<Details name="demo" label={`Embeds`}>
								<EmbedTest />
							</Details>
							<Details name="demo" label={`Forms`}>
								<FormTest />
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

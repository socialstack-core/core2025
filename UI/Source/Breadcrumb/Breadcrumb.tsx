import Link from 'UI/Link';

/**
 * Props for the Breadcrumb component.
 */
interface BreadcrumbProps {
	/**
	 * Show current page
	 */
	includeCurrent?: boolean,
}


/**
 * The Breadcrumb React component.
 * @param props React props.
 */
const Breadcrumb: React.FC<BreadcrumbProps> = (props) => {
	const { includeCurrent } = props;

	return <nav className="site-breadcrumb">
		{/* TODO: hardcoded for now to get _something_ on the UI :P */}
		<menu>
			<li>
				<Link href="/" className="site-breadcrumb__item">
					<i className="fr fr-arrow-90"></i>
					<span>
						{`Profiling Beds`}
					</span>
				</Link>
			</li>
			<li>
				<Link href="/" className="site-breadcrumb__item">
					<i className="fr fr-arrow-90"></i>
					<span>
						{`Standard Profiling Beds`}
					</span>
				</Link>
			</li>
			{includeCurrent && <>
				<li>
					<span className="site-breadcrumb__item site-breadcrumb__item--current">
						{`Current page`}
					</span>
				</li>
			</>}
		</menu>
	</nav>;
}

export default Breadcrumb;
import Link from 'UI/Link';

export type Crumb = {
	name: string,
	href: string
};

/**
 * Props for the Breadcrumb component.
 */
interface BreadcrumbProps {
	/**
	 * Show current page
	 */
	includeCurrent?: boolean,

	crumbs?: Crumb[]
}

/**
 * The Breadcrumb React component.
 * @param props React props.
 */
const Breadcrumb: React.FC<BreadcrumbProps> = (props) => {
	const { includeCurrent, crumbs } = props;

	if (!crumbs) {
		return null;
	}

	return <nav className="site-breadcrumb">
		<menu>
			{
				crumbs.map(crumb => <li>
					<Link href={crumb.href} className="site-breadcrumb__item">
						<i className="fr fr-arrow-90"></i>
						<span>
							{crumb.name}
						</span>
					</Link>
				</li>)
			}
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
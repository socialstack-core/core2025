import Link from 'UI/Link';

/**
 * Props for the Breadcrumb component.
 */
interface BreadcrumbProps {
}


/**
 * The Breadcrumb React component.
 * @param props React props.
 */
const Breadcrumb: React.FC<BreadcrumbProps> = ({ ...props }) => {

	return <nav className="site-breadcrumb">
		{/* TODO: hardcoded for now to get _something_ on the UI :P */}
		<menu>
			<li>
				<Link href="/" className="site-breadcrumb__item">
					{`Home`}
				</Link>
			</li>
			<li>
				<span className="site-breadcrumb__item">
					{`Shop for products`}
				</span>
			</li>
		</menu>
	</nav>;
}

export default Breadcrumb;
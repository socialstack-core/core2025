import { useSession } from 'UI/Session';
import Dropdown from 'UI/Dropdown';
import Icon from 'UI/Icon';
import userApi from 'Api/User';
import { Page } from 'Api/Page';

/**
 * Props for the admin trigger menu component.
 */
interface AdminTriggerProps {
	page?:Page
};

/**
 * Admin menu
 * @param props
 * @returns
 */
const AdminTrigger: React.FC<React.PropsWithChildren<AdminTriggerProps>> = props => {
	
	const { session } = useSession();
	var { user, realUser, role } = session;
	
	// not logged in
	if (!user || !props.page) {
		return null;
	}
	
	var editUrl = '/en-admin/page/' + props.page.id + '/?context=' + encodeURIComponent(window.location.pathname);
	
	// impersonating?
	var isImpersonating = realUser && (user.id != realUser.id);

	// not an admin
	if ((!role || !role.canViewAdmin) && !isImpersonating) {
		return <>
			{props.children}
		</>;
	}

	/*
	// viewing admin page
	if (url.startsWith('/en-admin')) {
		return <>
			{props.children}
		</>;
	}
    */

	var triggerLabelJsx = <>
		<Icon type="fa-cog" light />
	</>;

	function endImpersonation() {
		return userApi.unpersonate().then(response => {
			window.location.reload();
		});
	}
	
	return <>
		<div id="admin-trigger">
			<Dropdown isSmall title={`Administration`}
				label={triggerLabelJsx} variant="dark" align="Right" position="Top">
				<li className="admin-trigger__env admin-trigger__env--stage">
					<h6 className="dropdown-header">
						<Icon type="fa-exclamation-triangle" light /> {`STAGE`}
					</h6>
				</li>
				<li className="admin-trigger__env admin-trigger__env--uat">
					<h6 className="dropdown-header">
						<Icon type="fa-exclamation-triangle" light /> {`UAT`}
					</h6>
				</li>
				<li className="admin-trigger__env admin-trigger__env--prod">
					<h6 className="dropdown-header">
						<Icon type="fa-exclamation-triangle" light /> {`PRODUCTION`}
					</h6>
				</li>

				{isImpersonating && <>
					<li>
						<button type="button" className="btn dropdown-item" onClick={() => endImpersonation()}>
							{`End impersonation`}
						</button>
					</li>
					<li>
						<hr className="dropdown-divider" />
					</li>
				</>}
				{editUrl && <>
					<li>
						<a href={editUrl} className="btn dropdown-item">
							<Icon type="fa-edit" light /> {`Edit this page`}
						</a>
					</li>
					<li>
						<hr className="dropdown-divider" />
					</li>
				</>}
				<li>
					<a href="/en-admin" className="btn dropdown-item">
						{`Return to admin`}
					</a>
				</li>
			</Dropdown>
			{isImpersonating && <Icon type="fa-mask" light />}
		</div>
		{props.children}
	</>;
};

export default AdminTrigger;
import { useSession, useRouter } from 'UI/Session';
import Dropdown from 'UI/Dropdown';
import Icon from 'UI/Icon';
import webRequest from 'UI/Functions/WebRequest';

export default function AdminTrigger(props) {
	
	const { session, setSession } = useSession();

	var { user, realUser, role } = session;
	
	// not logged in
	if (!user || !props.page) {
		return null;
	}
	
	var editUrl = '/en-admin/page/' + props.page.id + '/?context=' + encodeURIComponent(window.location.pathname);
	
	// impersonating?
	console.log("USER: ", user);
	console.log("REALUSER: ", realUser);
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
		<Icon name="fa-cog" light />
	</>;

	function endImpersonation() {
		return webRequest('user/unpersonate').then(response => {
			window.location.reload();
		});
	}
	
	return <>
		<div id="admin-trigger">
			<Dropdown isSmall title={`Administration`}
				label={triggerLabelJsx} variant="dark" align="Right" position="Top">
				<li className="admin-trigger__env admin-trigger__env--stage">
					<h6 className="dropdown-header">
						<Icon name="fa-exclamation-triangle" light /> {`STAGE`}
					</h6>
				</li>
				<li className="admin-trigger__env admin-trigger__env--uat">
					<h6 className="dropdown-header">
						<Icon name="fa-exclamation-triangle" light /> {`UAT`}
					</h6>
				</li>
				<li className="admin-trigger__env admin-trigger__env--prod">
					<h6 className="dropdown-header">
						<Icon name="fa-exclamation-triangle" light /> {`PRODUCTION`}
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
							<Icon name="fa-edit" light /> {`Edit this page`}
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
			{isImpersonating && <Icon name="fa-mask" light />}
		</div>
		{props.children}
	</>;
};
import Button from 'UI/Button';
import NotificationBadge from 'UI/NotificationBadge';
import { useSession } from 'UI/Session';
import { useRouter } from 'UI/Router';
import logout from 'UI/Functions/Logout';
import Link from 'UI/Link';
import Popover from 'UI/Popover';

/**
 * Props for the Account component.
 */
interface AccountProps {
}

type AccountMenuItem = {
	icon?: string,
	label?: string,
	action?: () => void,
	link?: string,
	divider?: boolean
};

/**
 * The Account React component. This component is used when a user is known to be at least logged in.
 * @param props React props.
 */
const Account: React.FC<AccountProps> = ({ ...props }) => {
	const { session, setSession } = useSession();
	const { setPage } = useRouter();
	const { user, business, role } = session;

	const notificationBellSVG = <>
		<svg viewBox="0 0 12 16" fill="none" xmlns="http://www.w3.org/2000/svg">
			<path d="M4.67333 14.1116C4.67333 14.8791 5.26667 15.5 6 15.5C6.73333 15.5 7.32667 14.8791 7.32667 14.1116H4.67333ZM6 3.63953C7.84 3.63953 9.33333 5.20233 9.33333 7.12791V12.0116H2.66667V7.12791C2.66667 5.20233 4.16 3.63953 6 3.63953ZM6 0.5C5.44667 0.5 5 0.967442 5 1.54651V2.36279C2.90667 2.83721 1.33333 4.7907 1.33333 7.12791V11.314L0 12.7093V13.407H12V12.7093L10.6667 11.314V7.12791C10.6667 4.7907 9.09333 2.83721 7 2.36279V1.54651C7 0.967442 6.55333 0.5 6 0.5ZM5.33333 5.03488H6.66667V7.82558H5.33333V5.03488ZM5.33333 9.22093H6.66667V10.6163H5.33333V9.22093Z" fill="currentColor" />
		</svg>
	</>;

	let notificationCount = 0;

	if (!notificationCount) {
		notificationCount = 0;
	}

	if (!user || !role) {
		return null;
	}

	const { jobTitle } = user;

	const menuItems = [] as AccountMenuItem[];

	// If the role can manage carehomes (10 or 12) then display the management links.
	if (role.id < 3 || role.id == 10 || role.id == 12) {
		// head office admin | carehome manager
		menuItems.push(
			{
				icon: 'fr fr-fw fr-cog',
				label: `Dashboard`,
				link: '/business/manage'
			},
			{
				divider: true
			},
			{
				icon: 'fr fr-fw fr-tasks',
				label: `Order management`,
				link: '/business/manage/orders'
			},
			{
				icon: 'fr fr-fw fr-credit-card',
				label: `Budget management`,
				link: '/business/manage/budgets'
			},
			{
				icon: 'fr fr-fw fr-box',
				label: `Product management`,
				link: '/business/manage/products'
			},
			{
				divider: true
			},
			{
				icon: 'fa fa-fw fa-hotel',
				label: `Care homes`,
				link: '/business/manage/carehomes'
			},
			{
				icon: 'fr fr-fw fr-users',
				label: `Users`,
				link: '/business/manage/users'
			},
			{
				icon: 'fr fr-fw fr-list',
				label: `Budget categories`,
				link: '/business/manage/budget-categories'
			},
			{
				icon: 'fr fr-fw fr-file',
				label: `Order forms`,
				link: '/business/manage/order-forms'
			},
			{
				divider: true
			}
			// log
			// business profile
			// account profile 
			// { switch profile if impersonating }
		);
	} else if (role.id == 11) {
		// Head office approver - order related management only.
		menuItems.push(
			{
				icon: 'fr fr-fw fr-tasks',
				label: `Order management`,
				link: '/business/manage/orders'
			},
			{
				divider: true
			},
			{
				icon: 'fr fr-fw fr-file',
				label: `Order forms`,
				link: '/business/manage/order-forms'
			}
		);
	}

	menuItems.push({
		label: `Sign out`,
		action: () => {
			logout('/', setSession, setPage);
		}
	});

	return <>
		{/* toggle account menu */}
		<Button sm outlined className="site-nav__actions-account" popoverTarget="account_popover">
			{/*
		<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 28 36" fill="none">
			<path d="M22.5106 9C22.5106 4.035 18.4756 0 13.5106 0C8.54556 0 4.51056 4.035 4.51056 9C4.51056 13.965 8.54556 18 13.5106 18C18.4756 18 22.5106 13.965 22.5106 9ZM7.51056 9C7.51056 5.685 10.1956 3 13.5106 3C16.8256 3 19.5106 5.685 19.5106 9C19.5106 12.315 16.8256 15 13.5106 15C10.1956 15 7.51056 12.315 7.51056 9Z" fill="currentColor" />
			<path d="M0.0105591 33V34.5C0.0105591 35.325 0.685559 36 1.51056 36C2.33556 36 3.01056 35.325 3.01056 34.5V33C3.01056 28.035 7.04556 24 12.0106 24H15.0106C19.9756 24 24.0106 28.035 24.0106 33V34.5C24.0106 35.325 24.6856 36 25.5106 36C26.3356 36 27.0106 35.325 27.0106 34.5V33C27.0106 26.385 21.6256 21 15.0106 21H12.0106C5.39556 21 0.0105591 26.385 0.0105591 33Z" fill="currentColor" />
		</svg>
		*/}
			<i className="fr fr-user"></i>
			<span className="sr-only">
				{`Account`}
			</span>
			{notificationCount > 0 && <>
				<NotificationBadge icon={notificationBellSVG} />
			</>}
		</Button>

		{/* account popover */}
		<Popover method="auto" id="account_popover" alignment="right" blurBackground={true} className="site-nav__account-wrapper">
			<header>
				<h2 className="site-nav__account-title">
					<i className="fr fr-user"></i>
					{user.fullName || user.username}
				</h2>
				{jobTitle && <>
					<p className="ui-page__subtitle">
						{jobTitle}
					</p>
				</>}
			</header>
			<ul className="site-nav__account-list">
				{menuItems.map(item => {
					const { action, icon, label, link, divider } = item;

					if (divider) {
						return <li className="site-nav__account-item">
							<hr className="site-nav__account-divider" />
						</li>;
					}

					return <li className="site-nav__account-item">
						{action ?
							<Button onClick={() => action()}>
								{icon && <i className={icon} />}
								<span>{label}</span>
							</Button> :
							<Link href={link}>
								{icon && <i className={icon} />}
								<span>{label}</span>
							</Link>
						}
					</li>;

				})}
			</ul>
		</Popover>
	</>;
};

export default Account;

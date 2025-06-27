/**
 * Props for the Notification Badge component.
 */
interface NotificationBadgeProps {
	/**
	 * notification count
	 */
	count?: number,

	/**
	 * icon
	 */
	icon?: React.ReactNode,
}

/**
 * Notificaton Badge component
 */
const NotificationBadge: React.FC<NotificationBadgeProps> = (props) => {
	var { count, icon } = props;
	const MAX_COUNT = 99;

	if (!icon && count <= 0) {
		return;
	}

	var notificationClass: string[] = ['notification-badge'];

	if (!icon && count > MAX_COUNT) {
		notificationClass.push('notification-badge--small');
	}

	return <>
		<span className={notificationClass.join(' ')}>
			{icon}
			{!icon && count > 0 && Math.min(count, MAX_COUNT)}
		</span>
	</>;
}

export default NotificationBadge;
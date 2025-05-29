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

	if (!icon && count <= 0) {
		return;
	}

	var notificationClass: string[] = ['notification-badge'];

	if (!icon && count > 99) {
		notificationClass.push('notification-badge--small');
	}

	return <>
		<span className={notificationClass.join(' ')}>
			{icon}
			{!icon && count > 0 && count}
		</span>
	</>;
}

export default NotificationBadge;
/**
 * Return a contact link suitable for use as an href.
 * @param number contact number (e.g. "(0123) 456 7890")
 * @param link (optional) relative contact link (e.g. "/contact-us")
 * @returns given contact number in "tel:+1234567890" format if available, falling back to link
 */
function getContactLink(number: string, link?: string) {
	let input = number?.trim();

	if (!input?.length) {
		return link;
	}

	const hasPlus = input.startsWith("+");
	const digits = input.replace(/\D/g, '');

	return hasPlus ? `tel:+${digits}` : `tel:+${digits.replace(/^0/, '')}`;
}

export {
	getContactLink
};

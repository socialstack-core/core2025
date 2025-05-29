import Html from 'UI/Html';
import { getContactLink } from 'UI/Functions/ContactTools';
import { useState, useEffect } from "react";

/**
 * Props for the Footer component.
 */
interface FooterProps {

	/** 
	 * contact number (e.g. "0123 456 7890")
	 */
	contactNumber?: string,

	/** 
	 * contact link (e.g. "/contact-us")
	 */
	contactLink?: string,

	/** 
	 * address (use embedded <br/> to split lines)
	 */
	address?: string,

	/** 
	 * copyright year from (defaults to current year)
	 */
	copyrightFrom?: int,

	/** 
	 * copyright year to (defaults to current year)
	 */
	copyrightTo?: int,

	/** 
	 * copyright name (prefixed with (C) symbol and year(s))
	 */
	copyright?: string
}

/**
 * Props for each footer link.
 */
interface FooterLinkProps {

	/** 
	 * label
	 */
	label: string,

	/** 
	 * URL
	 */
	url: string
}

/**
 * The Footer React component.
 * @param props React props.
 */
const Footer: React.FC<FooterProps> = ({ contactNumber, contactLink, address, copyrightFrom, copyrightTo, copyright, ...props }) => {

	if (!contactNumber) {
		contactNumber = "0808 189 2044";
	}

	const contactHref = getContactLink(contactNumber, contactLink);
	const [column2Links, setColumn2Links] = useState<FooterLinkProps[]>([]);
	const [column3Links, setColumn3Links] = useState<FooterLinkProps[]>([]);

	useEffect(() => {
		// TODO: retrieve footer links from DB
		setColumn2Links([
			{
				label: `About`,
				url: `/about`
			},
			{
				label: `What We Offer`,
				url: `/what-we-offer`
			},
			{
				label: `All products`,
				url: `/products`
			},
			{
				label: `Testimonials`,
				url: `/testimonials`
			},
			{
				label: `Contact us`,
				url: `/contact-us`
			},
		]);

		setColumn3Links([
			{
				label: `Privacy Policy`,
				url: `/privacy-policy`
			},
			{
				label: `Cyber Security Certificate`,
				url: `/cyber-security-certificate`
			},
			{
				label: `Terms & Conditions`,
				url: `/terms-and-conditions`
			},
			{
				label: `Delivery Information`,
				url: `/delivery`
			},
			{
				label: `Returns Policy`,
				url: `/returns`
			},
		]);

	}, [])

	// calculate copyright period, either:
	// - only from year supplied = [from]-[currentYear]
	// - from and to years supplied = [from]-[to]
	// - only to year supplied = [to]
	// - neither from or to year supplied = [currentYear]
	const currentYear: number = new Date().getFullYear();
	let fromYear: number = validateYear(copyrightFrom);
	let toYear: number = validateYear(copyrightTo);

	// ignore invalid date ranges
	if (toYear <= fromYear) {
		toYear = 0;
	}

	let copyrightYear = currentYear.toString();

	if (fromYear && !toYear) {
		copyrightYear = fromYear >= currentYear ? fromYear.toString() : `${fromYear}&mdash;${currentYear}`;
	}

	if (!fromYear && toYear) {
		copyrightYear = toYear.toString();
	}

	if (fromYear && toYear) {
		copyrightYear = `${fromYear}&mdash;${toYear}`;
	}

	function validateYear(year: number): number {

		if (isNaN(year) || year < 1900) {
			return 0;
		}

		return year;
	}

	return <div className="site-footer">
		<div class="site-footer__internal">

			<div class="site-footer__contact">
				{contactHref && <>
					<a href={contactHref} class="site-footer__contact-link">
						{contactHref.startsWith("tel:") && <>
							<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 45 44" fill="none">
								<path d="M41.24 27.9l-5.92-2.94a5.45 5.45 0 00-7.26 2.27c-1.87-.73-5-2.92-6.82-4.68-1.76-1.83-3.92-4.89-4.68-6.76a5.46 5.46 0 002.39-7.31l-2.9-5.81C14.38-.2 9.8-.3 7.84.28c-2.8.83-5.23 2.8-6.66 5.41-1.9 3.5-1.14 7.74-.13 11.95a37.69 37.69 0 009.98 15.27 37.94 37.94 0 0015.49 10.1c2.22.53 4.51.99 6.71.99 1.8 0 3.52-.31 5.1-1.17a11.26 11.26 0 005.4-6.66c.58-1.98.48-6.56-2.49-8.26zm-1.04 7.25a7.5 7.5 0 01-3.65 4.45c-2.37 1.28-5.85.62-9.04-.13-5.08-1.7-9.74-4.76-13.92-9.16-4.32-4.1-7.39-8.76-9.05-13.68-.8-3.35-1.47-6.83-.19-9.2A7.62 7.62 0 018.81 3.8c.31-.1.73-.13 1.19-.13 1.14 0 2.51.27 2.77.73l2.84 5.7c.22.46.26.95.1 1.43-.17.48-.52.84-.98 1.05L13.7 13c-.7.29-1.14.95-1.14 1.7.02 3.5 4.73 9.1 6.03 10.43 1.3 1.26 6.88 5.96 10.4 6 .7 0 1.34-.4 1.65-1.03 0 0 .24-.5.57-1.12a1.82 1.82 0 012.42-.79l5.83 2.9c.75.44 1 2.95.7 4.01l.05.04z" fill="currentColor" />
							</svg>
						</>}
						{contactNumber?.length > 0 ? contactNumber : contactLink}
					</a>
				</>}

				{address?.length > 0 && <>
					<Html tag="address">
						{address}
					</Html>
				</>}

				{copyright?.length > 0 && <>
					&copy; {copyrightYear} {copyright}
				</>}

			</div>

			{column2Links?.length > 0 && <>
				<menu class="site-footer__column2">
					{column2Links.map(link => {
						return <li>
							<a href={link.url}>
								{link.label}
							</a>
						</li>;
					})}
				</menu>
			</>}

			{column3Links?.length > 0 && <>
				<menu class="site-footer__column3">
					{column3Links.map(link => {
						return <li>
							<a href={link.url}>
								{link.label}
							</a>
						</li>;
					})}
				</menu>
			</>}

		</div>
	</div>;
}

export default Footer;
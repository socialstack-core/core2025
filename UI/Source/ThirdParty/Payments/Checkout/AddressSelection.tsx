import Input from 'UI/Input';
import Form from 'UI/Form';
import Button from 'UI/Button';
import Link from 'UI/Link';
import Modal from 'UI/Modal';
import { useState } from 'react';
import addressApi, { Address } from 'Api/Address';
import CheckoutSection from './CheckoutSection';
import { ApiList } from 'UI/Functions/WebRequest';
import AddressCard from 'UI/Payments/Checkout/AddressCard';

/**
 * Props for the Checkout component.
 */
interface AddressSelectionProps {

	value?: Address;

	setValue: (val: Address) => void;
	setSameAs?: (val: boolean) => void;

	selectedTitle: string;
	unselectedTitle: string;

	name: string;

	hasSame?: boolean;

	isSame?: boolean;

	savedAddresses: Address[];

	addressType: 'delivery' | 'billing';

	enabled: boolean;
}

const AddressSelection: React.FC<AddressSelectionProps> = (props) => {
	const { selectedTitle, unselectedTitle, value, setValue, name, hasSame, isSame,
		setSameAs, addressType, savedAddresses, enabled } = props;
	const [showNewAddressModal, setShowNewAddressModal] = useState(false);

	const addressToLines = (addr: Address) => {
		var current = [];
		addr.line1 && current.push(addr.line1);
		addr.line2 && current.push(addr.line2);
		addr.line3 && current.push(addr.line3);
		addr.city && current.push(addr.city);
		addr.postcode && current.push(addr.postcode);
		return current;
	};

	const renderAddress = (address: Address) => {
		return addressToLines(address).map(line => <div>{line}</div>);
	};

	const addNewAddress = () => {

		return <>
			<Form action={addressApi.create}
				onValues={values => {
					if (addressType == 'delivery') {
						values!.isDefaultDeliveryAddress = true;
					} else if (addressType == 'billing') {
						values!.isDefaultBillingAddress = true;
					}

					return values;
				}}

				onSuccess={
					addr => {
						setValue(addr);
						setShowNewAddressModal(false);
					}
				}>
				<Input type='text' name='line1' label='Address Line 1' />
				<Input type='text' name='line2' label='Address Line 2' />
				<Input type='text' name='line3' label='Address Line 3' />
				<Input type='text' name='city' label='City' />
				<Input type='text' name='postcode' label='Postcode' />

				<div className="payment-checkout__address-modal-footer">
					<Button outlined onClick={() => setShowNewAddressModal(false)}>
						{`Cancel`}
					</Button>
					<Button type="submit">
						{`Add address`}
					</Button>
				</div>
			</Form>
		</>;

	};

	const renderContents = () => {
		return <>
			{hasSame && <div>
				<Input type='checkbox' name={name + '_same'} defaultChecked={isSame} onChange={(e) => {
					setSameAs && setSameAs((e.target as HTMLInputElement).checked);
				}} label={`Same as delivery address`} />
			</div>}
			{(!hasSame || !isSame) && <>
				<div className="payment-checkout__address-selection">
					{savedAddresses.map(savedAddress => {
						return <AddressCard address={savedAddress} selectedAddress={value} onChange={() => setValue(savedAddress)} name={name} />
					})}
				</div>
				<div className="payment-checkout__address-selection-footer">
					<Link outlined href="/address_book">
						{`Edit addresses`}
					</Link>
					<Button onClick={() => setShowNewAddressModal(true)}>
						{`Add new address`}
					</Button>
				</div>
			</>}

			{showNewAddressModal && <>
				<Modal
					title={`Add address`}
					className={"payment-checkout__address-modal"}
					onClose={() => setShowNewAddressModal(false)}
					visible={true}>
					{addNewAddress()}
				</Modal>
			</>}
		</>;
	};

	return <CheckoutSection title={value ? selectedTitle : unselectedTitle} enabled={enabled}>
		{renderContents()}
	</CheckoutSection>;
};

export default AddressSelection;
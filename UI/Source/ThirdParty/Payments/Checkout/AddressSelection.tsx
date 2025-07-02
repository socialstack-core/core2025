import Input from 'UI/Input';
import Form from 'UI/Form';
import Button from 'UI/Button';
import { useState, useEffect } from 'react';
import addressApi, { Address } from 'Api/Address';
import CheckoutSection from './CheckoutSection';

/**
 * Props for the Checkout component.
 */
interface AddressSelectionProps {

	value?: Address;

	setValue: (val: Address) => void;
	setSameAs?: (val: boolean) => void;

	title: string;

	name: string;

	same?: boolean;

	isSame?: boolean;

	addressType: 'delivery' | 'billing';
}

const AddressSelection: React.FC<AddressSelectionProps> = (props) => {
	const { title, value, setValue, name, same, isSame, setSameAs, addressType } = props;
	const [adding, setAdding] = useState(false);

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
			<Form action={addressApi.create} submitLabel={`Add address`}
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
						setAdding(false);
						setValue(addr);
					}
				}
			>
				<Input type='text' name='line1' label='Address Line 1' />
				<Input type='text' name='line2' label='Address Line 2' />
				<Input type='text' name='line3' label='Address Line 3' />
				<Input type='text' name='city' label='City' />
				<Input type='text' name='postcode' label='Postcode' />
			</Form>
		</>;

	};

	const renderContents = () => {

		if (value && !adding) {

			return <>
				{renderAddress(value)}
				<p>
					<Button onClick={() => setAdding(true)}>{`Change address`}</Button>
				</p>
			</>;

		}

		// Todo: selecting an address from savedAddresses if there is options present and !sameAddress

		return <>
			{same && <div>
				<Input type='checkbox' name={name + '_same'} defaultChecked={isSame} onChange={(e) => {
					setSameAs && setSameAs((e.target as HTMLInputElement).checked);
				}} label={`Same as delivery address`} />
			</div>}
			{(!same || !isSame) && addNewAddress()}
		</>;
	};

	return <CheckoutSection title={title} enabled>
		{renderContents()}
	</CheckoutSection>;
};

export default AddressSelection;
import Input from 'UI/Input';
import Form from 'UI/Form';
import Button from 'UI/Button';
import { useState, useEffect } from 'react';
import addressApi, { Address } from 'Api/Address';
import CheckoutSection from './CheckoutSection';
import { ApiList } from 'UI/Functions/WebRequest';

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

	savedAddresses: ApiList<Address>;

	addressType: 'delivery' | 'billing';
}

const AddressSelection: React.FC<AddressSelectionProps> = (props) => {
	const { title, value, setValue, name, same, isSame,
		setSameAs, addressType, savedAddresses } = props;
	const [changing, setChanging] = useState(false);

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
						setChanging(false);
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

		if (value && !changing) {

			return <>
				{renderAddress(value)}
				<p>
					<Button onClick={() => setChanging(true)}>{`Change address`}</Button>
				</p>
			</>;

		}

		return <>
			{same && <div>
				<Input type='checkbox' name={name + '_same'} defaultChecked={isSame} onChange={(e) => {
					setSameAs && setSameAs((e.target as HTMLInputElement).checked);
				}} label={`Same as delivery address`} />
			</div>}
			{(!same || !isSame) && <>
				<Input type='select' defaultValue={0} onChange={(e) => {
					var id = parseInt((e.target as HTMLSelectElement).value);
					var addr = savedAddresses.results.find(addr => addr.id == id);

					if (addr) {
						setValue(addr);
						setChanging(false);
					}
				}}>
					<option value={0}>{`Add a new address`}</option>
					{savedAddresses.results.map(savedAddress => <option value={savedAddress.id}>
						{renderAddress(savedAddress)}
					</option>)}
				</Input>
				{addNewAddress()}
			</>}
		</>;
	};

	return <CheckoutSection title={title} enabled>
		{renderContents()}
	</CheckoutSection>;
};

export default AddressSelection;
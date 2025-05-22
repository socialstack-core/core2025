import addressApi, { Address } from 'Api/Address';
import useApi from 'UI/Functions/UseApi';
import { useSession } from 'UI/Session';
import Loading from 'UI/Loading';
import Form from 'UI/Form';
import Input from 'UI/Input';
import { useState } from 'react';
import Button from 'UI/Button';
import ConfirmModal from 'UI/Modal/ConfirmModal';

/**
 * Props for the AddressBook component.
 */
interface AddressBookProps {
	/**
	 * An example optional fileRef prop.
	 */
	// logoRef?: FileRef
}

/**
 * The AddressBook React component.
 * @param props React props.
 */
const AddressBook: React.FC<AddressBookProps> = (props) => {
	const { session } = useSession();
	const { user } = session;

	if (!user) {
		// Page login rules block this, but just in case.
		return null;
	}

	// A counter which is used to cause the address list to update with new entries.
	const [createCounter, setCreateCounter] = useState<number>(0);
	const [confirmDelete, setConfirmDelete] = useState<Address | null>(null);

	const [ addressList ] = useApi(() => addressApi.list({
		query: 'UserId=?',
		args: [
			user.id
		],
		sort: null
	}), [createCounter]);

	if (!addressList) {
		return <Loading />;
	}

	const addressToLines = (addr: Address) => {
		var current = [];
		addr.line1 && current.push(addr.line1);
		addr.line2 && current.push(addr.line2);
		addr.line3 && current.push(addr.line3);
		addr.city && current.push(addr.city);
		addr.postcode && current.push(addr.postcode);
		return current;
	};

	return (
		<div className="ui-payments-address-book">
			<div className="ui-payments-address-book__list">
				{
					addressList.results.map(address => {

						return <div>
							<p>
								Is default billing: {address.isDefaultBillingAddress ? 'Yes' : 'No'}
							</p>
							<p>
								Is default delivery: {address.isDefaultDeliveryAddress ? 'Yes' : 'No'}
							</p>
							<Button onClick={() => setConfirmDelete(address)}>{`Delete`}</Button>
							{addressToLines(address).map(line => <div>{line}</div>)}
						</div>

					})
				}
			</div>
			<div className="ui-payments-address-book__add">
				<Form action={addressApi.create} submitLabel={`Add address`} onSuccess={
					() => setCreateCounter(createCounter+1)
				}>
					<Input type='text' name='line1' label='Address Line 1' />
					<Input type='text' name='line2' label='Address Line 2' />
					<Input type='text' name='line3' label='Address Line 3' />
					<Input type='text' name='city' label='City' />
					<Input type='text' name='postcode' label='Postcode' />
					<Input type='checkbox' name='isDefaultBillingAddress' label='Set as default billing address' />
					<Input type='checkbox' name='IsDefaultDeliveryAddress' label='Set as default shipping address' />
				</Form>
			</div>
			{
				confirmDelete && <ConfirmModal
					confirmVariant={'danger'}
					confirmCallback={() => {
						return addressApi.delete(confirmDelete.id).then(() => {
							setConfirmDelete(null);
							setCreateCounter(createCounter + 1);
						});
					}}
					cancelCallback={() => setConfirmDelete(null)}
					confirmText={`Yes, delete the address`}
				>
					{`Are you sure you want to delete this address?`}
				</ConfirmModal>
			}
		</div>
	);
}

export default AddressBook;
import { Price } from 'Api/Price';
import { useEffect, useState } from "react";
import Modal from 'UI/Modal';
import AutoForm from 'Admin/AutoForm';
import { useSession } from 'UI/Session';
import { formatCurrency, formatPOA } from "UI/Functions/CurrencyTools";

const PriceTable: React.FC = (props) => {
	const [inputHandle, setInputHandle] = useState(null);
	const { session } = useSession();
	const { locale } = session;
	
	const [value, setValue] = useState(() => {
		var initVal = (props.value || props.defaultValue || []).filter(t => t!=null).sort((a, b)=> a.minimumQuantity - b.minimumQuantity);
		return initVal;
	});
	const [showModal, setShowModal] = useState(false);
	const [entityToEdit, setEntityToEdit] = useState();

	const onSetValue = (prices: Price[]) => {
		prices = prices.sort((a, b)=> a.minimumQuantity - b.minimumQuantity);
		setValue(prices);
	}
	
	const onRemove = (price: Price) => {
		var newValue = value.filter(v => v != price)
		onSetValue(newValue);
		props.onChange && props.onChange({target: {value: newValue.map(e => e.id)}, fullValue: newValue});
	};
	
    return <div className="price-tiers">

		{props.label && !props.hideLabel && (
			<label className="form-label">
				{props.label}
			</label>
		)}
		
		<table className="table">
			<thead>
				<tr>
					<th>
						{`Minimum Quantity`}
					</th>
					<th>
						{`Amount (${locale.currencyCode})`}
					</th>
				</tr>
			</thead>
			<tbody>
				{
					value.map(price => {
						return <tr>
							<td>
								{price.minimumQuantity}
							</td>
							<td>
								{`${formatCurrency(price.amount, locale)}`}
							</td>
							<td>
								<button className="btn btn-sm btn-outline-primary btn-entry-select-action btn-view-entry" title={`Edit`}
									onClick={e => {
										e.preventDefault();
										setEntityToEdit(price.id);
										setShowModal(true);
									}}>
									<i className="fal fa-fw fa-edit"></i> <span className="sr-only">{`Edit`}</span>
								</button>
							</td>
							<td>
								<button className="btn btn-sm btn-outline-danger btn-entry-select-action btn-remove-entry" title={`Remove`}
									onClick={() => onRemove(price)}>
									<i className="fal fa-fw fa-times"></i> <span className="sr-only">{`Remove`}</span>
								</button>
							</td>
						</tr>
					})
				}
			</tbody>
		</table>
		<footer className="admin-multiselect__footer">
			<button type="button" className="btn btn-sm btn-outline-primary btn-entry-select-action btn-new-entry"
				onClick={e => {
					e.preventDefault();
					setEntityToEdit(null);
					setShowModal(true);
				}}
			>
				{/*<i className="fal fa-fw fa-plus"></i> {`New ${this.props.label}...`}*/}
				<i className="fal fa-fw fa-plus"></i> {`New`}
			</button>
		</footer>
		<input type="hidden" name={props.name} ref={ele => {
			this.input = ele;

			if (ele != null) {
				ele.onGetValue = (v, input, e) => {

					if (input != this.input) {
						return v;
					}

					return value.map(entry => entry.id);
				}
			}
		}} />
		{showModal &&
			<Modal
				title={entityToEdit ? `Edit ${props.contentType}` : `Create New ${props.contentType}`}
				visible
				isExtraLarge
				onClose={() => {
					setShowModal(false);
					setEntityToEdit(null);
				}}
			>
				<AutoForm
					canvasContext={props.canvasContext ? props.canvasContext : props.currentContent}
					modalCancelCallback={() => {
						setShowModal(false);
						setEntityToEdit(null);
					}}
					endpoint={props.contentType}
					singular={props.contentType}
					plural={props.contentType + "s"}
					id={entityToEdit ? entityToEdit : null}
					onActionComplete={entity => {
						var newValue = value;
						var valueIndex = value.findIndex(
							(checkIndex) => checkIndex.id === entity.id
						)

						if (valueIndex !== -1) {
							newValue[valueIndex] = entity
						} else {
							newValue.push(entity)
						}
						
						setShowModal(false);
						setEntityToEdit(null);
						onSetValue(newValue);

						onChange && onChange({ target: { value: value.map(e => e.id) }, fullValue: value });
					}} 
				/>
			</Modal>
		}
    </div>;
};

export default PriceTable;

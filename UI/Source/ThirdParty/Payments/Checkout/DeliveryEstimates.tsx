import Button from 'UI/Button';
import deliveryOptionApi, { DeliveryOption } from 'Api/DeliveryOption';
import { ApiList } from 'UI/Functions/WebRequest';
import { formatCurrency } from "UI/Functions/CurrencyTools";
import { addMinutes } from "UI/Functions/DateTools";

/**
 * Props for the Checkout component.
 */
interface DeliveryEstimatesProps {
	estimates: ApiList<DeliveryOption>;

	value?: DeliveryOption;

	setValue: (val: DeliveryOption) => void;
}

const DeliveryEstimates: React.FC<DeliveryEstimatesProps> = (props) => {

	const { value, setValue, estimates } = props;

	if (!estimates?.results?.length) {
		return <>
			{`Unfortunately we are unable to deliver this order. Please get in touch for more details.`}
		</>;	
	}

	return <>
		{
			estimates.results.map(estimate => {

				// Unpack the estimate:
				var estimateDetail = JSON.parse(estimate.informationJson || '');

				// A singular delivery option can contain 1 or more 
				// (usually 1) delivery on different days.

				// For example, you can choose to get things as soon as possible with 5/6 items showing up tomorrow
				// and 1/6 the day after. Or you can choose to get all 6 the day after. 

				return <div>
					{
						estimateDetail.deliveries.map(deliveryInfo => {
							// DeliveryInfo C# type, in DeliveryEstimate.cs

							// If there are >1 deliveries, then deliveryInfo.Products is
							// set indicating what is present in *this* proposed delivery.

							if (!deliveryInfo.slotStartUtc) {
								// A date isn't known for this service. 
								// It happens on JIT style delivery where the product is ordered from an upstream usually foreign supplier.
								// "We'll let you know when a date is confirmed" etc.
							} else {

								var date = new Date(deliveryInfo.slotStartUtc);

								if (deliveryInfo.timeWindowLength) {
									// It's a time slot range. Date is the start of the slot.
									var slotEnd = addMinutes(date, deliveryInfo.timeWindowLength);

									// The time slot is between date and slotEnd.

								} else {
									// Only use the day component. Time must be ignored.
								}
							}

							return <div>
								<p>{deliveryInfo.deliveryName}</p>
								<p>{deliveryInfo.deliveryNotes}</p>
							</div>

						})
					}
					<p>
						{formatCurrency(estimateDetail.price, {currencyCode: estimateDetail.currency})}
					</p>
					{estimate == value ? `Selected` : <Button onClick={() => setValue(estimate)}>{`Choose this option`}</Button>}
				</div>;

			})
		}
	</>;

};

export default DeliveryEstimates;
import {useEffect, useId, useState } from "react";

/**
 * Props for the DualRange component.
 * 
 * NB: WIP - needs work to bring into line with functional demo here:
 * https://codepen.io/cablewhite/pen/myJPJXx/1eff2161802d5c5f0c0b00a88c1d44d6
 */
interface DualRangeProps {
	label: string,
	min?: number,
	max?: number,
	step?: number,
	defaultFrom?: number,
	defaultTo?: number,
	numberFormat?: Intl.NumberFormat,
	onChange: (min: number, max: number) => void,
}

const DEFAULT_MIN_RANGE = 0;
const DEFAULT_MAX_RANGE = 100;

/**
 * The DualRange React component.
 * @param props React props.
 */
const DualRange: React.FC<DualRangeProps> = (props) => {
	const { label, min, max, step, defaultFrom, defaultTo, numberFormat, onChange } = props;

	const minValue = min || DEFAULT_MIN_RANGE;
	const maxValue = max || DEFAULT_MAX_RANGE;
	const stepValue = step || 1;

	const [fromValue, setFromValue] = useState(defaultFrom || minValue);
	const [toValue, setToValue] = useState(defaultTo || maxValue);
	
	useEffect(() => {
		props.onChange(fromValue, toValue);
	}, [fromValue, toValue]);

	const id = useId();
	const fromId = `from_${id}`;
	const toId = `to_${id}`;
	const labelFromId = `lfrom_${id}`;
	const labelToId = `lto_${id}`;

	const rangeDistance = maxValue - minValue;
	const fromPosition = Number(fromValue) - minValue;
	const toPosition = Number(toValue) - minValue;

	const rangeBackground = `linear-gradient(
      to right,
      var(--range-track-background) 0%,
      var(--range-track-background) ${(fromPosition) / (rangeDistance) * 100}%,
      var(--range-track-fill) ${((fromPosition) / (rangeDistance)) * 100}%,
      var(--range-track-fill) ${(toPosition) / (rangeDistance) * 100}%, 
      var(--range-track-background) ${(toPosition) / (rangeDistance) * 100}%, 
      var(--range-track-background) 100%)`;

	function changeFromSlider(e) {
		const newValue = parseInt(e.target.value, 10);
		setFromValue(newValue > toValue ? toValue : newValue);
	}

	function changeToSlider(e) {
		const newValue = parseInt(e.target.value, 10);
		setToValue(newValue < fromValue ? fromValue : newValue);
	}

	return (
		<div className="ui-dual-range">
			<label id={id} htmlFor={fromId}>
				{label}
			</label>
			<div role="group" aria-labelledby={id} className="ui-dual-range__internal">
				<label htmlFor={fromId} id={labelFromId}>
					{/*numberFormat ? numberFormat.format(min) : min*/}
					{numberFormat ? numberFormat.format(fromValue) : fromValue}
				</label>
				<label id={labelToId}>
					{/*numberFormat ? numberFormat.format(max) : max*/}
					{numberFormat ? numberFormat.format(toValue) : toValue}
				</label>

				<div className="ui-dual-range__gradient" style={{ 'background': rangeBackground }} />

				{/* ideally this should reference <Input type="range" />, but this will do for now */}
				<input type="range" className="ui-dual-range__from" id={fromId}
					min={minValue} max={maxValue}
					aria-valuemin={minValue} aria-valuemax={toValue} aria-valuenow={fromValue} aria-labelledby={`${id} ${labelFromId}`}
					step={stepValue} value={fromValue} onChange={changeFromSlider} />

				<input type="range" className="ui-dual-range__to" id={toId}
					min={minValue} max={maxValue}
					aria-valuemin={fromValue} aria-valuemax={maxValue} aria-valuenow={toValue} aria-labelledby={`${id} ${labelToId}`}
					step={stepValue} value={toValue} onChange={changeToSlider} />
			</div>
		</div>
	);
}

export default DualRange;
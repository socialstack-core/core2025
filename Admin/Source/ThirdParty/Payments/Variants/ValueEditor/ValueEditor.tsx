import { useEffect, useState } from "react";
import Loop from "UI/Loop";
import ProductAttributeApi, { ProductAttribute } from "Api/ProductAttribute";
import ProductAttributeValueApi from "Api/ProductAttributeValue";
import Input from "UI/Input";
import Button from "UI/Button";
import Image from "UI/Image";
import Icon from "UI/Icon";
import Link from "UI/Link";
import Video from "UI/Video";
import Col from "UI/Column";
import Row from "UI/Row";
import AttributeSelect from "Admin/Payments/AttributeSelect";
import Container from "UI/Container";

type VariantEditorProps = {
	variant: Product
};

const VariantEditor: React.FC<VariantEditorProps> = (props) => {
	
	// NB this can't be a form as it's surrounded by the main admin form.
	const {variant} = props;
	const { additionalAttributes } = variant;
	
	return <div className="variants-variant-editor">
	{/*
		<Row>
			<Col>
				Img editor todo
			</Col>
			<Col>
	*/}
				<AttributeSelect value={additionalAttributes} label={`Additional attributes`}/>
				<Input type='text' name='sku' defaultValue={variant.sku} placeholder={`SKU`}/>
				<div>
					{variant.id && <Link target='_blank' href={'/en-admin/product/' + variant.id}>{`Edit more details`}</Link>}
					<Button danger onClick={() => props.onRemove(variant)}><Icon type='fa-trash' /></Button>
				</div>
		{/*
		</Col>
	</Row>*/}
	</div>;
	
};

const ValueEditor: React.FC = (props) => {
	
	const [inputHandle, setInputHandle] = useState(null);
	
	const [value, setValue] = useState(() => {
		var initVal = (props.value || props.defaultValue || []).filter(t => t!=null);
		return initVal;
	});
	
	const onRemove = (variant: Product) => {
		setValue(value.filter(v => v != variant));
	};
	
    return <div className="variants-value-editor">
		{props.label && !props.hideLabel && (
			<label className="form-label">
				{props.label}
			</label>
		)}
		<ul className="variants-value-editor__entries">
			{value.map(variant => <VariantEditor variant={variant} onRemove={onRemove}/>)}
		</ul>
		<Button onClick={() => {
			setValue([...value, {}]);
		}}>{`Add variant`}</Button>
		
		<input type="hidden" name={props.name} ref={ele => {
			setInputHandle(ele);

			if (ele != null) {
				ele.onGetValue = (v, input, e) => {
					if (input != inputHandle) {
						return v;
					}
					
					return value.map(entry => entry.id).filter(id => id != undefined);
				}
			}
		}} />
    </div>;
};

export default ValueEditor;

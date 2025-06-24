import Input from 'UI/Input';


export default function ProductTypes(props) {

	return <Input {...props} type='select'>
			<option value='0'>Physical product</option>
			<option value='1'>Digital product (no delivery necessary)</option>
		</Input>;
	
}

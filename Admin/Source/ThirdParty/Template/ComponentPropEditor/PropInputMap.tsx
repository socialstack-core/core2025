import { CodeModuleType, CodeModuleTypeField, UnionType } from "Admin/Functions/GetPropTypes"
import { Scalar } from "Admin/Template/RegionEditor"
import Input from "UI/Input"

export type PropInputProps = {
    // the field.
    type: CodeModuleTypeField,
    onInput: (value: Scalar) => void,
    value: Scalar
}

/**
 * This component displays an input component to set a property based off of the type of 
 * field it is.
 * @param {PropInputProps} props 
 * @returns {React.ReactElement}
 */
const PropInput: React.FC<PropInputProps> = (props:PropInputProps): React.ReactElement => {

    // TODO: Pad this out with inputs.

    switch(props.type.fieldType.name) {

        case "union":
            return (
                <Input
                    type={'select'}
                    onChange={(ev: React.ChangeEvent) => {
                        const target: HTMLSelectElement = (ev.target as HTMLSelectElement)
                        props.onInput(target.value)
                    }}
                    value={props.value as string}
                    help={(props.type.optional ? ' (optional)' : undefined)}
                    label={props.type.name}
                >
                    <option value={''}>{`Please choose an option`}</option>
                    {props.type.fieldType.types?.map((value: UnionType) => {
                        return (
                            <option value={value.value}>{value.value}</option>
                        )
                    })}
                </Input>
            );
        case "bool":
            return (
                <Input
                    type={'select'}
                    onChange={(ev: React.ChangeEvent) => {
                        const target: HTMLSelectElement = (ev.target as HTMLSelectElement)
                        props.onInput(target.value)
                    }}
                    value={props.value as string}
                    help={(props.type.optional ? ' (optional)' : undefined)}
                    label={props.type.name}
                >
                    <option value={'false'}>{`Choose Yes or No`}</option>
                    <option value={"false"}>{`No`}</option>
                    <option value={"true"}>{`Yes`}</option>
                </Input>
            );
        default:
            return (
                <Input
                    type={'text'}
                    onInput={(ev: React.FormEvent<HTMLInputElement>) => {
                        const target: HTMLInputElement = ev.target as HTMLInputElement;
                        props.onInput(target.value)
                    }}
                    help={(props.type.optional ? ' (optional)' : undefined)}
                    label={props.type.name}
                    value={props.value as string}
                />
            )
    }
}


export default PropInput;
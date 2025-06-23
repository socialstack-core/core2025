import { useEffect, useRef, useState} from "react";
import Input from 'UI/Input'

export type MultiSelectOption = {
    value: string,
    count: number,
    valueId: number
}

export type MultiSelectBoxProps = {
    onChange: (values: ulong[]) => void;
    defaultText: string;
    value: ulong[]
    options: MultiSelectOption[]
}

export const MultiSelectBox = (props: MultiSelectBoxProps) => {
    
    const [isOpen, setIsOpen] = useState(false);
    const [value, setValue] = useState<ulong[]>();

    useEffect(() => {
        if (!value && props.value) {
            // will only happen mount only.
            setValue(props.value);
            return;
        }
        
        if (value && value != props.value) {
            props.onChange(value);
        }
    }, [value]);
    
    const ref = useRef<HTMLDivElement>(null);

    useEffect(() => {
        
        const clickListener = (ev: MouseEvent) => {
            const target = ev.target as HTMLDivElement;
            
            if (ref.current && ref.current.contains(target)) {
                // noop.
                return;
            }
            if (ref.current == target) {
                setIsOpen(!isOpen);
                return;
            }
            setIsOpen(false);
        }
        
        window.addEventListener('click', clickListener);
        
        return () => {
            window.removeEventListener('click', clickListener)
        }
        
    }, []);
    
    return (
        <div 
            className={'multi-select ' + (value?.length != 0 ? ' in-use' : '')} 
            ref={ref}
            onClick={() => {
                if (!isOpen) {
                    setIsOpen(true);
                }
            }}
        >
            <div onClick={() => setIsOpen(!isOpen)} className={'multi-select-title'}>
                {props.defaultText} {props.value.length != 0 ? "(" + props.value.length + ")" : ''}

                {isOpen ? <i className={'fas fa-chevron-up'}/> : <i className={'fas fa-chevron-down'}/>}
            </div>
            {isOpen && (
                <div className={'multi-select-options'}>
                    {(!props.options || props.options.length == 0) && <p>{`No options available`}</p>}
                    {props.options && props.options.map((option: MultiSelectOption) => {
                        return (
                            <li>
                               <Input 
                                    type={'checkbox'} 
                                    label={option.value + ' (' + option.count + ')'}
                                    onChange={(event) => {
                                        const newValue: ulong[] = value?.filter((existing) => existing !== option.valueId) ?? [];
                                        
                                        if ((event.target as HTMLInputElement).checked) {
                                            newValue.push(option.valueId as ulong);
                                        }
                                        setValue(newValue);
                                    }}
                                    checked={value?.includes(option.valueId as ulong)}
                               />
                            </li>
                        )
                    })}
                </div>
            )}
        </div>
    )
}
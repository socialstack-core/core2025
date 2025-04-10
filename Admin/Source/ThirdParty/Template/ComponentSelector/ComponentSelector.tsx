import { CodeModuleMeta, getAll, TypeMeta } from "Admin/Functions/GetPropTypes";
import { Scalar } from "Admin/Template/RegionEditor";
import { useEffect, useState } from "react";
import Alert from "UI/Alert";
import Input from "UI/Input";
import Modal from "UI/Modal";
import ComponentGroup from "./ComponentGroup";


export type ComponentSelectorProps = {
    onClose?: () => void,
    title: string,
    onComponentSelected: (componentPath:string) => void
}

const ComponentSelector: React.FC<ComponentSelectorProps> = (props: ComponentSelectorProps): React.ReactElement => {


    // this holds all the components that follow certain criteria.
    const [components, setComponents] = useState<Record<string, Record<string, CodeModuleMeta>> | null>(null)

    // allow components to be filtered
    const [filter, setFilter] = useState<string>()

    useEffect(() => {
        // we need the components available to actually choose one. 
        if (!components) {

            getAll().then((results:TypeMeta) => {
                const { codeModules } = results;

                const componentGroups:Record<string, Record<string, CodeModuleMeta>> = {};

                Object.keys(codeModules).forEach(key => {
                    // a couple of directories need removing from this, Api/ is a typescript generated directory so we don't need to include that
                    // Admin/ is full of admin components that shouldn't be on the frontend
                    // UI/Templates are templates so hide them, we should also remove UI/Functions from that list 
                    // And also remove email templates too
                    if (key.startsWith("Admin/") || key.startsWith("Api/") || key.startsWith("UI/Templates") || key.startsWith("UI/Functions") || key.startsWith("Email/Templates")) {
                        return;
                    }

                    const codeModuleMeta = codeModules[key];

                    if (codeModuleMeta.types.length != 0) {

                        for(const type of codeModuleMeta.types) {

                            if (type.fields?.find(field => field.fieldType.name === 'function')) {
                                // we can't specify JS/TS using this so skip any that have functions as required
                                // properties.
                                return;
                            }

                        }

                    }

                    const path = key.substring(0, key.lastIndexOf('/'))

                    if (!componentGroups[path]) {
                        componentGroups[path] = {};
                    }

                    componentGroups[path][key] = codeModuleMeta;
                })

                setComponents(componentGroups)
            });
        }

    }, [components])

    return (
        <Modal 
            visible={true}
            onClose={props.onClose}
            title={props.title}
            className='region-component-choice'
            noFooter={true}
        >
            {!components ? <Alert type='info'>{`Loading`}</Alert> : 
                <div className='component-selection'>
                    <div className='component-list'>
                        <Input 
                            type='text' 
                            onInput={(ev) => setFilter((ev.target as HTMLInputElement).value)}
                            placeholder="Filter components"
                        />
                        {Object.keys(components).map(group => {
                            return (
                                <ComponentGroup 
                                    filter={filter} 
                                    onComponentSelected={(name) => {
                                        props.onComponentSelected(name)
                                    }} 
                                    groupName={group} 
                                    group={components[group]}
                                />
                            )
                        })}
                    </div>
                </div>
            }
        </Modal>
    )
}



export default ComponentSelector;
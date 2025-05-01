import { CodeModuleMeta, getAll, TypeMeta } from "Admin/Functions/GetPropTypes";
import { Scalar } from "Admin/Template/RegionEditor";
import { useEffect, useState } from "react";
import Alert from "UI/Alert";
import Input from "UI/Input";
import Modal from "UI/Modal";
import { useSession } from "UI/Session";
import ComponentGroupApi, { ComponentGroup } from "Api/ComponentGroup";
import { ApiList } from "UI/Functions/WebRequest";
import ComponentGroupRenderer from "./ComponentGroup";


export type ComponentSelectorProps = {
    onClose?: () => void,
    title: string,
    onComponentSelected: (componentPath:string, componentProps?: Record<string, Scalar | Scalar[]>) => void,
    extra?: Record<string, Record<string, Scalar | Scalar[]>>,
    extraLabel?: string,
    permitted?: string[]
}

const ComponentSelector: React.FC<ComponentSelectorProps> = (props: ComponentSelectorProps): React.ReactElement => {

    const { session } = useSession();

    // this holds all the components that follow certain criteria.
    const [components, setComponents] = useState<Record<string, Record<string, CodeModuleMeta>> | null>(null)

    // if allowed components is empty, no rules limiting the allowed components have been 
    // created, therefore can use them all.
    const [allowedComponents, setAllowedComponents] = useState<string[]>();

    // allow components to be filtered
    const [filter, setFilter] = useState<string>()

    useEffect(() => {
        if (!allowedComponents)
        {
            ComponentGroupApi.list({
                query: "Role = ?",
                args: [session?.role?.id ?? 0]
            })
            .then((result: ApiList<ComponentGroup>) => {
                if (result.totalResults == 0)
                {
                    setAllowedComponents([]);
                    return;
                }
                // this can be the only one.
                const group = result.results[0];

                if (group.allowedComponents)
                {
                    var parsed = JSON.parse(group.allowedComponents!);
                    // if the JSON is invalid, prevent an error.
                    setAllowedComponents(Array.isArray(parsed) ? parsed : []);

                    console.log(allowedComponents);

                    console.error("The allowedComponentsJson property is invalid, expected string[]", group);
                }
                else
                {
                    console.error('Incorrect response for component group')
                    setAllowedComponents([]);
                }
            })
            .catch((err) => {
                console.error('An error occured', err);
                setAllowedComponents([]);
            })
        }
    }, [allowedComponents, session])

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

                    // we can reduce the options shown to what is actively available
                    if (props.permitted && Array.isArray(props.permitted) && props.permitted.length != 0 && !props.permitted.includes(key)) {
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

    }, [components, props.permitted])

    const { extra } = props;

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
                        {extra && (
                            <div className='component-group'>
                                <div className='group-head'>{props.extraLabel ?? `Other`}</div>
                                <div className='group-content'>
                                    {Object.keys(extra).map(componentName => {
                                        const componentProps = extra[componentName];

                                        if (allowedComponents && allowedComponents.length != 0)
                                        {
                                            // there's some rules in place, lets see if the current
                                            // user can access each component.

                                            if (!allowedComponents.includes(componentName))
                                            {
                                                console.log('Skipped')
                                                // omit the item
                                                return;
                                            }
                                        }

                                        return (
                                            <div title={componentName} className='group-item' onClick={() => props.onComponentSelected(componentName, componentProps)}>
                                                <i className="fa fa-puzzle-piece" />
                                                <label>{
                                                    componentName.includes('/') ? 
                                                        componentName.substring(componentName.lastIndexOf('/') + 1, componentName.length) :
                                                        componentName
                                                }</label>
                                            </div>
                                        )
                                    })}
                                </div>
                            </div>
                        )}
                        {Object.keys(components).map(group => {
                            return (
                                <ComponentGroupRenderer 
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
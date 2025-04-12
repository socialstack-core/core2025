import { CodeModuleTypeField, TemplateModule } from "Admin/Functions/GetPropTypes";
import ComponentPropEditor from "Admin/Template/ComponentPropEditor";
import ComponentSelector from "Admin/Template/ComponentSelector";
import { Template } from "Api/Template";
import { createRef, useEffect, useState } from "react";
import Alert from "UI/Alert";
import Modal from "UI/Modal";
import { canAddChildren, templateConfigToCanvasJson } from "./Functions";
import PropInput from "../ComponentPropEditor/PropInputMap";


export type Scalar = string | number | boolean | null | undefined;

export type RegionEditorFields = {
    $isLocked?: boolean,
    permitted?: string[],
    $editorLabel?: string
}

export type TreeComponentItem = {
    // the component type (i.e UI/Functions/SomeComponent)
    t: string,
    // the initialising props, don't change this to "any" or "unknown", just 
    // adjoin it to the accepted V types below
    d: Record<string, Scalar | Scalar[]> & RegionEditorFields,
    // roots, these are different to children, these tell the canvas
    // when a prop is a React Component to render the component 
    r?: Record<string, TreeComponentItem>,
    // these are children that exist within non-root components.
    c?: TreeComponentItem[]
}

export type RegionEditorProps = {
    templateJson: TreeComponentItem,
    currentTemplate?: Template,
    currentLayout: TemplateModule,
    onChange?: (newTree: TreeComponentItem) => void,
    existing?: Template
}

const assignParentLock = (child: TreeComponentItem, isExisting: boolean = false) => {
    
    if (!isExisting) {
        if (child.d.$isLocked) {
            child.d.isLockedByParent = true;
        }

        child.d.isFromParent = true;
    }
    child.c?.forEach(subChild => {
        assignParentLock(subChild, isExisting);
    })
}

/*
* When choosing ReactNode props, this should re-create the whole tree but only
* affect the changed item, lets say a user changes a header element, anything else
* should be left alone. Any descendants of the header will then be removed. Although
* the header property should be assigned to the prop, it will of course have to be added to the
* children tree. 
*/

const RegionEditor: React.FC<RegionEditorProps> = (props: RegionEditorProps): React.ReactNode => {

    const { currentLayout, currentTemplate, templateJson } = props;
    const { types } = currentLayout.types;

    // make sure we can find the props:MyProps type first, if we can't show an error
    const layoutProps = types.find(type => type.instanceName?.includes('Props') && (type.name == 'interface' || type.name === 'type' || type.name === 'union'));

    // this exists purely for the template root component
    const [templateConfig, setTemplateConfig] = useState<Record<string, Scalar>>({});
    const [error, setError] = useState<string | undefined>();

    if (!layoutProps) {
        return (
            <Alert type='error'>{`Invalid template: Cannot find a props type in this template, please ensure the template's props use`}</Alert>
        )
    }


    // get the fields, from here we iterate over them
    // and then these directly reflect on the template
    const { fields } = layoutProps;

    // this holds the config for all fields, when this is updated
    // the full template is updated.
    const [fullConfig, setFullConfig] = useState<Record<string, CoreRegionConfig & Record<string, Scalar | TreeComponentItem[]>>>({});

    const onChange = (update: CoreRegionConfig & Record<string, Scalar | TreeComponentItem[]>) => {
        
        setError(undefined);
        fullConfig[update.propName] = {...update};

        setFullConfig({...fullConfig});

        // now to valid any rules 
        try
        {
            const convertedJson = templateConfigToCanvasJson(fullConfig, currentLayout)
            Object.assign(convertedJson.d, templateConfig);

            props.onChange && props.onChange(convertedJson)
        }
        catch(e)
        {
            setError((e as Error).message);
        }

        console.log(fullConfig)
    }

    // this only fires when the "inherits" field is changed. 
    useEffect(() => {

        const json: TreeComponentItem = JSON.parse(currentTemplate?.bodyJson ?? '{}') ?? {};

        if (json.d) {
            // sets the initial config
            setTemplateConfig(json.d as Record<string, Scalar>);

            if (json.r) {
                Object.keys(json.r).forEach((componentKeyName: string) => {

                    const cfg = json.r![componentKeyName]

                    fullConfig[componentKeyName] = {
                        enabled: true,
                        propName: componentKeyName,
                        components: cfg.c,
                        isLockedByParent: !props.existing && Boolean(cfg.d.$isLocked),
                        $isLocked: Boolean(cfg.d.$isLocked) 
                    }

                    cfg.c?.forEach((child) => {
                        assignParentLock(child, Boolean(props.existing))
                    })
                });
            }
        }

        json.c?.forEach((child: TreeComponentItem) => {
            assignParentLock(child);
        })

    }, [currentTemplate])

    const configFields = (currentLayout as any).types.types[0].fields?.filter((field:CodeModuleTypeField) => !['React.ReactNode', 'React.ReactElement'].includes(field?.fieldType?.instanceName!))

    return (
        <div className='region-editor'>
            {error && <Alert variant='danger'>{error}</Alert>}
            {fields?.map(field => field.fieldType.instanceName === 'React.ReactNode' && <RegionLevelEditor setError={setError} config={fullConfig[field.name]} onChange={onChange} field={field} />)}

            {currentLayout && templateJson && 
                configFields && configFields.length != 0 && 
                // Optimization Level: Interstellar
                // By skipping this block when there are no configurable fields,
                // we prevent React from allocating ~2KB of memory for elements, fibers, and closure baggage.
                // That’s enough to store ~300 cat emojis 🐱
                // You’re welcome, planet Earth.
                (
                    <div className='template-component-configuration'>
                        <h4>Configuration</h4>
                        <div className='configurable-items'>
                            {(currentLayout as any).types.types[0].fields.map((layoutConfig: CodeModuleTypeField) => {
                                if (['React.ReactNode', 'React.ReactElement'].includes(layoutConfig?.fieldType?.instanceName!))
                                {
                                    return;
                                }

                                return (
                                    <PropInput
                                        type={layoutConfig}
                                        onInput={(value: Scalar) => {
                                            templateConfig[layoutConfig.name] = value;
                                            
                                            setTemplateConfig({...templateConfig})

                                            const convertedJson = templateConfigToCanvasJson(fullConfig, currentLayout)
                                            Object.assign(convertedJson.d, templateConfig);

                                            props.onChange && props.onChange(convertedJson)
                                        }}
                                        value={templateConfig[layoutConfig.name] as string}
                                    />
                                )
                            })}
                        </div>
                    </div>
                )
            }
        </div>
    )

}

type RegionLevelEditorProps = {
    field: CodeModuleTypeField,
    config: CoreRegionConfig & Record<string, any>, 
    onChange: (config: CoreRegionConfig & Record<string, Scalar>) => void,
    setError: Function
}

export type CoreRegionConfig = {
    enabled: boolean,
    propName: string,
    components?: TreeComponentItem[],
    isLockedByParent?: boolean
}

const RegionLevelEditor: React.FC<RegionLevelEditorProps> = (props: RegionLevelEditorProps): React.ReactElement => {

    // holds the state as to whether the select component modal should show
    // this one specifically exists to top level props component selection
    const [showSelectComponentModal, setShowSelectComponentModal] = useState(false)

    const { field } = props;
    let { config } = props;

    // takes the checked state whenever emitChange is called, as opposed to storing the value
    // and having to sync states.
    const enabledRef: React.RefObject<HTMLInputElement | null> = createRef<HTMLInputElement>();

    // this config is a sub item of the actual full config. This is a per-component
    // level, this isn't the full template config that will properly create the template.
    if (!config) {
        config = {
            enabled: !field.optional,
            propName: field.name
        }
    }

    // When a change is made, this needs to be called, this then
    // passes back the changes to the parent component (which should only be RegionEditor)
    // the parent component will then pass the new config back to here where it will render accordingly.
    const emitChange = () => {

        if (!config.components) {
            config.components = []
        }

        // get the checked status before passing to parent handler.
        props.onChange(config);
    }

    // this is for the meta components (i.e ones that don't physically exist)
    // but act as things like a placeholder etc...
    const extraComponents: Record<string, Record<string, Scalar | Scalar[]>> = {
        'Admin/Template/Slot': {
            // the name is editable, so we start with untitled slot
            // $editorLabel is purely a UX feature, it allows areas to 
            // be effectively named as to remind a user what its purpose is.
            $editorLabel: 'Untitled-Slot',
            // creates this value by default,
            // this is just a marker that its not a real component.
            $isMeta: true,
            // start off with it being optional, can always enforce it when ready
            optional: true,
            // whether the slot can hold multiple child components
            multipleAllowed: false,
            // what components are permitted?
            permitted: [],
            // not locked by default
            $isLocked: false,
        }
    }

    return (
        <div className='region'>
            <div className='enablement'>
                <input 
                    type='checkbox' 
                    ref={enabledRef} 
                    onClick={() => {

                        // when the input element has mounted, and is checked
                        // check if there is a component, if there isn't 
                        // the user should be prompted to choose one,
                        // if one isn't chosen, this should be reset back to false
                        if (enabledRef.current && enabledRef.current.checked) {
                            if (!config.component) {
                                setShowSelectComponentModal(true)
                                return;
                            }
                        }

                        // if the checkbox is de-checked, this should remove any
                        // configuration tied to it.
                        if (enabledRef.current && !enabledRef.current.checked) {
                            config.component = undefined;
                            config.enabled = false;
                            config.componentProps = {};

                            emitChange();
                        }
                    }} 
                    checked={Boolean(config['enabled'])}
                />
            </div>
            <div 
                className='main-info'
                onClick={() => {

                    if (config.isLockedByParent) {
                        return;
                    }
                    setShowSelectComponentModal(true)
                }}
            >
                <h3>{field.name} {config.$isLocked && <i className='lock-status'>(locked)</i>}</h3>
                {!config.isLockedByParent && (
                    <button  
                        type={'button'}
                    >+</button>
                )}
                
                {!config.isLockedByParent && (
                    <button 
                        onClick={(e) => {
                            e.preventDefault();
                            e.stopPropagation();

                            config.$isLocked = !config.$isLocked;
                            emitChange();
                        }}
                    >
                        <i className={'fas fa-lock' + (!config.$isLocked ? '-open' : '')}/>
                    </button>
                )}
            </div>
            <div className='child-regions'>
                {config.components?.map((component, idx) => {
                    return (
                        <ChildRegionEditor 
                            deleteFunc={() => {
                                const components: TreeComponentItem[] = [];

                                config.components?.forEach(child => {
                                    if (child === component) {
                                        return;
                                    }

                                    components.push(child);
                                })

                                config.components = components;

                                emitChange();
                            }} 
                            onChange={emitChange}
                            item={component}
                        />
                    )
                })}
            </div>
            {
                showSelectComponentModal && (
                    <ComponentSelector
                        title={`Choose component for ${field.name}`}
                        onClose={() => {
                            if (!config.components || config.components.length == 0) {
                                config.enabled = false;
                                config.component = undefined;
                            }
                            emitChange();
                            setShowSelectComponentModal(false)
                        }}
                        extra={extraComponents}
                        extraLabel={`Structure`}
                        onComponentSelected={(component, props) => {

                            config.enabled = true;

                            if (!config.components) {
                                config.components = [];
                            }
                            config.components.push({
                                t: component,
                                d: props ?? {},
                                c: []
                            })

                            emitChange();
                            setShowSelectComponentModal(false)
                        }}
                    />
                )
            }
        </div>
    )

}

type ChildRegionEditorProps = {
    item: TreeComponentItem,
    name?: string,
    deleteFunc: () => void,
    onChange: Function
}

const ChildRegionEditor: React.FC<ChildRegionEditorProps> = (props:ChildRegionEditorProps): React.ReactElement => {
    
    // destructuring
    const { item } = props;

    // useStates
    const [isRenaming, setIsRenaming] = useState(false)
    const [isConfigureMode, setIsConfigureMode] = useState(false)
    const [isAddModalOpen, setIsAddModalOpen] = useState(false);

    const [canHaveChildren, setCanHaveChildren] = useState<boolean>();


    // useful vars
    const itemName: string = (
        props.name ?? 
        item.d.$editorLabel as string ?? 
        (
            item.t.includes('/') ? 
                item.t.substring(item.t.lastIndexOf('/') + 1, item.t.length) : 
                item.t
        )
    );
    
    useEffect(() => {
        canAddChildren(item.t)
            .then(result => {
                setCanHaveChildren(result)
            })
            .catch((err) => {
                console.error(err);
            })
    }, [canHaveChildren])

    return (
        <div className='child-section'>
            
            <div className='main'>
                {isRenaming ? 
                    <input 
                        type='text' 
                        onInput={(ev) => {
                            const target: HTMLInputElement = ev.target as HTMLInputElement;
                            item.d.$editorLabel = target.value.length == 0 ? `Untitled-Component` : target.value.replaceAll(" ", "-");
                        }}
                        onKeyDown={(ev: React.KeyboardEvent<HTMLInputElement>) => {
                            if (['Enter', 'Escape'].includes(ev.key)) {
                                setIsRenaming(false)
                                props.onChange()
                            }
                        }}
                        onBlur={() => { 
                            setIsRenaming(false)
                            props.onChange() 
                        }}
                        defaultValue={itemName}
                    />
                : 
                    <h4 onClick={(e) => {
                        e.detail == 2 && !item.d.isLockedByParent && setIsRenaming(true) 
                    }}>
                        {itemName} 
                        {
                            item.d.$isLocked && 
                            <i className='lock-status'>(locked)</i>
                        }
                    </h4>
                }
                {!isRenaming && !item.d.isLockedByParent && (
                    <>
                        {!item.d.isFromParent && <i onClick={() => setIsRenaming(true)} className='fas fa-pencil'/>}
                        {!item.d.isFromParent && <i onClick={() => setIsConfigureMode(true)} className='fas fa-cog'/>}
                        {canHaveChildren && <i onClick={() => setIsAddModalOpen(true)} className='fas fa-plus'/>}
                        {!item.d.isFromParent && <i className='fas fa-trash' onClick={() => {
                            props.deleteFunc()
                        }}/>}
                        <i
                            onClick={() => {
                                item.d.$isLocked = !item.d.$isLocked;
                                props.onChange();
                            }} 
                            className={'fas fa-lock' + (!item.d.$isLocked ? '-open' : '')}
                        />
                    </>
                )}
            </div>
            <div className='child-children'>
                {item.r && Object.keys(item.r).map((key, idx) => {
                    const child = item.r![key];

                    return (
                        <ChildRegionEditor 
                            deleteFunc={() => {
                                const newR: Record<string, TreeComponentItem> = {};

                                Object.keys(item.r!).forEach(existingKey => {
                                    if (existingKey === key) {
                                        return;
                                    }
                                    newR[key] = item.r![existingKey];
                                })

                                item.r = newR;
                                props.onChange();
                            }} 
                            item={child} 
                            onChange={props.onChange}
                            name={child.d.$editorLabel as string ?? key}
                        />
                    )
                })}

                {item.c && item.c.map((child) => {
                    return (
                        <ChildRegionEditor 
                            deleteFunc={() => {
                                const newItems: TreeComponentItem[] = [];

                                item.c?.forEach(item => {
                                    if (item === child) {
                                        return;
                                    }
                                    newItems.push(item);
                                })

                                item.c = newItems;
                                props.onChange();
                            }} 
                            item={child} 
                            onChange={props.onChange}
                            name={child.d.$editorLabel as string ?? child.t}
                        />
                    )
                })}
            </div>
            {isConfigureMode && (
                <Modal
                    visible={true}
                    title={`Configuration for ${itemName}`}
                    onClose={() => {
                        setIsConfigureMode(false)
                        props.onChange();
                    }}
                    noFooter
                >
                    <ComponentPropEditor
                        item={item}
                    />
                </Modal>
            )}
            {isAddModalOpen && (
                <ComponentSelector
                    title={`Add component to ${itemName}`}
                    onClose={() => setIsAddModalOpen(false)}
                    permitted={item.d.permitted}
                    onComponentSelected={(component, d) => {
                        if (!item.c) {
                            item.c = [];
                        }
                        item.c.push({
                            t: component, 
                            d: d ?? {},
                            c: [],
                            r: {} 
                        })

                        props.onChange();
                        setIsAddModalOpen(false)
                    }}
                />
            )}
        </div>
    )
}

export default RegionEditor;
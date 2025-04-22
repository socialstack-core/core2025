import { CodeModuleMeta, getAll } from "Admin/Functions/GetPropTypes";
import { Scalar, TreeComponentItem } from "Admin/Template/RegionEditor"
import { useEffect, useReducer, useState } from "react";
import Input from "UI/Input";
import { getPropsForComponent } from "../RegionEditor/Functions";
import Alert from "UI/Alert";
import PropInput from "./PropInputMap";
import Modal from "UI/Modal";
import { sortComponentOrder } from "../Functions";

export type ComponentPropEditorProps = {
    item: TreeComponentItem
    modules?: Record<string, CodeModuleMeta>,
    onChange?: (item: TreeComponentItem) => void
}

/*
* A facade component that shows a different type of editor for both meta & physical components. 
*/
const ComponentPropEditor: React.FC<ComponentPropEditorProps> = (props:ComponentPropEditorProps) : React.ReactNode => {

    const { item } = props;

    // this can be null when configuring non-physical components. 
    const [codeModule, setCodeModule] = useState<CodeModuleMeta | null>(null);
    const [allModules, setAllModules] = useState<Record<string, CodeModuleMeta> | null>(null);

    useEffect(() => {

        if (!allModules) {
            getAll().then(result => {

                // this will then trigger this same useEffect
                // again, setting the current codeModule
                setAllModules(result.codeModules)
            })
            return
        }

        setCodeModule(allModules[item.t])

    }, [allModules])

    if (item.d.$isMeta && allModules) {
        // if allModules is null, it will flow down to !codeModule if statement and return loading.
        return <NonPhysicalPropEditor {...props} modules={allModules!}/>
    }

    if (!codeModule) {
        // as a temporary solution, a good old <p> tag is being added here
        // but that is terrible, we need a gucci spinner, with lights and backup dancers.

        // look at this fine art
        return <p>Loading</p>
    }
    // here we can assume that all the necessary loading has happened and we don't need to do any more
    // as mentioned above getAll is cached, so we probably wont even see the fancy loader above, unless
    // the user is running this on a toaster. 

    return <PhysicalComponentPropEditor 
                {...props} 
                tsInfo={codeModule} 
                onChange={props.onChange}
            />
}

const NonPhysicalPropEditor: React.FC<ComponentPropEditorProps> = (props: ComponentPropEditorProps): React.ReactElement => {

    // this is a pretty much hardcoded component, meta components only need certain information.
    const { item } = props;

    return (
        <div className='component-prop-editor'>
            <Input 
                type='text'
                label={`Name`}
                onInput={(ev: React.KeyboardEvent<HTMLInputElement>) => {
                    item.d.$editorLabel = (ev.target as HTMLInputElement).value;
                }} 
                defaultValue={item.d.$editorLabel as string}
            />
            <Input
                type='checkbox'
                label={`Is optional?`}
                defaultChecked={Boolean(item.d.isOptional)}
                onChange={(ev: React.ChangeEvent) => {
                    const target: HTMLInputElement = (ev.target as HTMLInputElement);
                    item.d.isOptional = target.checked;
                }}
            />
            <Input
                type='checkbox'
                label={`Allow multiple children?`}
                defaultChecked={Boolean(item.d.multipleAllowed)}
                onChange={(ev: React.ChangeEvent) => {
                    const target: HTMLInputElement = (ev.target as HTMLInputElement);
                    item.d.multipleAllowed = target.checked;
                }}
            />
            <PermittedChildEditor item={item}/>
        </div>
    )
}

export type PhysicalComponentProps = ComponentPropEditorProps & {
    tsInfo: CodeModuleMeta
}

const PhysicalComponentPropEditor: React.FC<PhysicalComponentProps> = (props: PhysicalComponentProps): React.ReactElement => {

    const { tsInfo, item, onChange } = props;

    const fields = getPropsForComponent(tsInfo);

    if (!fields) {
        return <Alert variant="danger">{`There was an issue reading this component`}</Alert>
    }

    const value = JSON.stringify(item);

    return (
        <Input
            defaultValue={value}
            type='canvas'
            key={value}
            onCanvasChange={(source:string) => {
                var edited = JSON.parse(source);

                console.log({ edited, item })

                onChange && onChange(canvasNodeToTreeComponent(item,edited))
            }}
            label={`Component preview`} 
            className='component-preview'
        />
    )
}

const canvasNodeToTreeComponent = (item: TreeComponentItem, node: any):TreeComponentItem => {
    
    const newConfig:TreeComponentItem = {
        t: item.t,
        d: node.c.d ?? {},
        c: []
    };
    
    Object.assign(newConfig.d, item.d)

    return newConfig;
}

type PermittedChildEditorProps = {
    item: TreeComponentItem
}

// this component is used to restrict what components can go inside the current one
const PermittedChildEditor: React.FC<PermittedChildEditorProps> = (props: PermittedChildEditorProps): React.ReactElement => {

    const { item } = props;
    const [allComponents, setAllComponents] = useState<Record<string, CodeModuleMeta>>()
    const [isSelectingModal, setIsSelectingModal] = useState<boolean>();
    const [filter, setFilter] = useState<string>()
    const [_, forceUpdate] = useReducer(x => x + 1, 0)
    
    // load all components in.
    useEffect(() => {

        if (!allComponents) {
            getAll().then(result => setAllComponents(result.codeModules))
        }

    }, [allComponents])


    
    if (!item.d.permitted) {
        // lets initialise the array, so long as its empty, ALL components can be placed under here
        item.d.permitted = [];
    }

    return (
        <div className='permitted-child-editor'>
            {(item.d.permitted as []).length == 0 ? 
                <p>{`All components can currently be placed in this component`}</p> :
                <div className='component-list'>
                    {item.d.permitted.map(permitted => {
                        return (
                            <div 
                                className={'component-item listed'}
                            >
                                {permitted}
                                <i 
                                    className='fas fa-trash delete-icon'
                                    onClick={() => {
                                        if (item.d.permitted!.includes(permitted)) {
                                            // remove it. 
                                            item.d.permitted!.splice(
                                                item.d.permitted!.indexOf(permitted), 
                                                1
                                            )
                                        } else {
                                            // add it
                                            item.d.permitted!.push(permitted)
                                        }
                                        forceUpdate();
                                    }}     
                                />
                            </div>
                        )
                    })}
                </div>
            }
            <button 
                onClick={() => setIsSelectingModal(true)} 
                type='button'
            >
                {`Edit restrictions`}
            </button>
            {isSelectingModal && (
                <Modal
                    visible={true}
                    title={`Enabled components for ${item.d.$editorLabel ?? item.t}`}
                    onClose={() => setIsSelectingModal(false)}
                    noFooter
                >
                    <Input
                        type='text'
                        onInput={(ev) => {
                            const target: HTMLInputElement = (ev.target as HTMLInputElement)

                            setFilter(target.value)
                        }}
                        placeholder={`Search components`}
                        defaultValue={filter}
                    />
                    {allComponents ? sortComponentOrder(Object.keys(allComponents)).map((componentName => {

                        const ignoreStartsWith = ["Api/", "Admin/", "UI/Templates", "UI/Functions", "Email/Templates"];

                        if (ignoreStartsWith.find(a => componentName.startsWith(a))) {
                            return null;
                        }
                        if (componentName.includes(".")) {
                            // no component should include a .
                            // the ones this filter removes 
                            // will show items not meant to be used 
                            // as components.
                            return null;
                        }

                        if (filter && filter.length != 0) {
                            if (!componentName.toLowerCase().includes(filter.toLowerCase())) {
                                return null;
                            }
                        }

                        return (
                            <div 
                                onClick={() => {
                                    if (item.d.permitted!.includes(componentName)) {
                                        // remove it. 
                                        item.d.permitted!.splice(
                                            item.d.permitted!.indexOf(componentName), 
                                            1
                                        )
                                    } else {
                                        // add it
                                        item.d.permitted!.push(componentName)
                                    }
                                    forceUpdate();
                                }} 
                                className={'component-item' + (item.d.permitted!.includes(componentName) ? ' active' : '')}
                            >
                                <i className={'fas fa-' + (item.d.permitted!.includes(componentName) ? 'toggle-on' : 'toggle-off')}/>
                                {componentName}
                            </div>
                        )
                    })) : <p>Loading...</p>}
                </Modal>
            )}
        </div>
    )

} 

export default ComponentPropEditor;
import { CodeModuleMeta, getAll } from "Admin/Functions/GetPropTypes";
import { TreeComponentItem } from "Admin/Template/RegionEditor"
import { useEffect, useState } from "react";
import Input from "UI/Input";

export type ComponentPropEditorProps = {
    item: TreeComponentItem
    modules?: Record<string, CodeModuleMeta>
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

    return <PhysicalComponentPropEditor {...props} tsInfo={codeModule}/>
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
        </div>
    )
}

export type PhysicalComponentProps = ComponentPropEditorProps & {
    tsInfo: CodeModuleMeta
}

const PhysicalComponentPropEditor: React.FC<PhysicalComponentProps> = (props: PhysicalComponentProps): React.ReactElement => {

    return (
        <div className='component-prop-editor'>

        </div>
    )
}

export default ComponentPropEditor;
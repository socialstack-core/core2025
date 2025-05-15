import { useEffect, useState } from "react";
import Input from "UI/Input";
import Alert from "UI/Alert";
import { CodeModuleMeta, getAll } from "Admin/Functions/GetPropTypes";
import { EditorCanvasTreeNode, RegionDefaultRC } from "../RegionEditor/types";
import { getPropsForComponent } from "../RegionEditor/Functions";


export type ComponentPropEditorProps = {
    item: EditorCanvasTreeNode
    onChange?: (replaceItem: EditorCanvasTreeNode) => void, 
    isRegion: boolean
}

/*
* A facade component that shows a different type of editor for both meta & physical components. 
*/
const ComponentPropEditor: React.FC<ComponentPropEditorProps> = (props:ComponentPropEditorProps) : React.ReactNode => {

    const { item } = props;

    // this can be null when configuring non-physical components. 
    const [codeModule, setCodeModule] = useState<CodeModuleMeta>();
    const [allModules, setAllModules] = useState<Record<string, CodeModuleMeta>>();

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

    }, [item, allModules])

    if (props.isRegion) {
        return (
            <PhysicalComponentPropEditor 
                {...props} 
                onChange={props.onChange}
                allModules={allModules}
            />
        );
    }

    if (!codeModule) {
        // as a temporary solution, a good old <p> tag is being added here
        // but that is terrible, we need a gucci spinner, with lights and backup dancers.

        // look at this fine art
        return <Alert variant="info">{`Loading...`}</Alert>
    }
    // here we can assume that all the necessary loading has happened and we don't need to do any more
    // as mentioned above getAll is cached, so we probably wont even see the fancy loader above, unless
    // the user is running this on a toaster. 

    return <PhysicalComponentPropEditor 
                {...props} 
                tsInfo={codeModule} 
                onChange={props.onChange}
                allModules={allModules}
            />
}

export type PhysicalComponentProps = ComponentPropEditorProps & {
    tsInfo?: CodeModuleMeta,
    allModules?: Record<string, CodeModuleMeta>
}

const PhysicalComponentPropEditor: React.FC<PhysicalComponentProps> = (props: PhysicalComponentProps): React.ReactElement => {

    const { tsInfo, item, onChange, allModules } = props;
    
    let fields;

    if (tsInfo) {

        fields = getPropsForComponent(tsInfo);

        if (!fields) {
            return <Alert variant="danger">{`There was an issue reading this component`}</Alert>
        }
    }

    const value = JSON.stringify(item);

    return (
        <div className='configurer component-prop-editor'>
            <Input
                type='checkbox'
                label={`Is optional?`}
                defaultChecked={Boolean(item.rc?.isOptional)}
                onChange={(ev: React.ChangeEvent) => {
                    const target: HTMLInputElement = (ev.target as HTMLInputElement);

                    if (!item.rc) {
                        item.rc = {...RegionDefaultRC}
                        item.rc.editorLabel = item.t;
                    }

                    item.rc.isOptional = target.checked;
                }}
            />
            <Input
                type='checkbox'
                label={`Allow multiple children?`}
                defaultChecked={Boolean(item.rc?.multipleChildrenAllowed)}
                onChange={(ev: React.ChangeEvent) => {
                    const target: HTMLInputElement = (ev.target as HTMLInputElement);

                    if (!item.rc) {
                        item.rc = {...RegionDefaultRC}
                        item.rc.editorLabel = item.t;
                    }

                    item.rc.multipleChildrenAllowed = target.checked;
                }}
            />
            {allModules && (
                <PermittedModuleEditor
                    allModules={allModules}
                    node={item}
                />
            )}
            {!props.isRegion && (<Input
                defaultValue={value}
                type='canvas'
                key={value}
                onCanvasChange={(source:string) => {
                    var edited: EditorCanvasTreeNode = JSON.parse(source);

                    if (!edited.rc) {
                        edited.rc = {...RegionDefaultRC}
                        edited.rc.editorLabel = item.t;
                    }

                    if (!edited.rc.childrenAllowed) {
                        edited.c = []; // reset.
                    }

                    onChange && onChange(edited)
                }}
                label={`Component preview`} 
                className='component-preview'
            />)}
        </div>
    )
}

type PermittedModuleEditorProps = {
    allModules: Record<string, CodeModuleMeta>,
    node: EditorCanvasTreeNode
}

const PermittedModuleEditor = (props: PermittedModuleEditorProps) => {
    
    return (
        <div className='permitted-editor'>
            
        </div>
    )
}

export default ComponentPropEditor;
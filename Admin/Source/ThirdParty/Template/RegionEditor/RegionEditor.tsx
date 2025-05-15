import { TemplateModule } from "Admin/Functions/GetPropTypes";
import { CanvasTreeNode, DocumentRootTreeNode, EditorCanvasTreeNode, RegionCanvasTreeNode, RegionDefaultRC } from "./types";
import Loading from "UI/Loading";
import Alert from "UI/Alert";
import { Template } from "Api/Template";
import { Children, useEffect, useState } from "react";
import ComponentSelector from "Admin/Template/ComponentSelector";
import { canAddChildren } from "./Functions";
import Modal from "UI/Modal";
import Input from "UI/Input";
import ComponentPropEditor from "Admin/Template/ComponentPropEditor";

/**
 * This is the props for the region editor.
 */
export type RegionEditorProps = {
    /**
     * This holds the template document root tree node, this is as far up the tree as we can go.
     */
    templateDocument: DocumentRootTreeNode,
    /**
     * When a change is made to the template, this will be called with the previous and latest version of the template.
     * @param previous - The previous version of the template.
     * @param latest - The latest version of the template.
     * @returns {void}
     */
    onChange?: (previous: DocumentRootTreeNode, latest: DocumentRootTreeNode) => void,
    
    /**
     * This holds the base file, the file feeds the region editor with the base rules,
     * the template then "layers" on top of this.
     */
    layoutFile?: TemplateModule,
    /**
     * This marks the template it extends, we can take the config from this and ensure all rules are being followed.
     */
    extends?: Template
}

/**
 * The region editor works with <c>DocumentRootTreeNode</c> only, this renders the full editor.
 * @param props 
 * @returns 
 */
const RegionEditor: React.FC<RegionEditorProps> = (props: RegionEditorProps) => {

    // templateDocument is the actual template's JSON structure but parsed.
    // layoutFile is the file inside UI/Templates that ixt implements.
    const { templateDocument, layoutFile } = props;

    // this is the region in which a component is being added.
    const [isSelectingComponent, setIsSelectingComponent] = useState<string>();

    const [isConfiguring, setIsConfiguring] = useState<boolean>();

    // no hooks beyond this point

    if (!layoutFile) {
        return (<Loading />)
    }

    // if fields is null, return an error.
    if (!layoutFile.types.types[0].fields) {
        return (
            <Alert type="danger">{`This is an invalid template`}</Alert>
        )
    }

    // regions can be empty, though they shouldn't be
    const regions = layoutFile.types.types[0].fields?.filter(field => field.fieldType.instanceName == "React.ReactNode");
    

    return (
        <div className='region-editor'>
            {regions.map((region, idx) => {
                
                // lets check if the region is in the template document
                let correspondingNode: RegionCanvasTreeNode = templateDocument.r![region.name] as RegionCanvasTreeNode;

                if (!correspondingNode) {
                    // the region has not been created yet, so lets create an empty minimal node.
                    const newRegion: RegionCanvasTreeNode = {
                        t: 'Admin/Template/Region',
                        c: [],
                        d: {},
                        rc: {...RegionDefaultRC}
                    };
                    newRegion.rc!.editorLabel = region.name;
                    
                    // lets spawn a new document
                    const newDocument = {...templateDocument}
                    
                    // set the newly generated region up
                    newDocument.r![region.name] = newRegion;

                    // and call onChange
                    props.onChange && props.onChange(templateDocument, newDocument);
                    return;

                }
                
                // if the region has no region config, create a default one.
                if (!correspondingNode.rc) {
                    correspondingNode.rc = {...RegionDefaultRC};
                    correspondingNode.rc.editorLabel = region.name;

                    props.onChange && props.onChange(templateDocument, templateDocument);
                    return;
                }
                
                const { rc } = correspondingNode;

                return (
                    <div key={idx} className='region'>
                        <div className='main-info'>
                            <h3>{region.name} {rc.isLockedByParent && <i>{`Locked by parent`}</i>}</h3>
                            {
                                // if the region is locked by the parent, we can't do anything to it. 
                                !rc?.isLockedByParent && (
                                    <button
                                        type='button'
                                        onClick={() => {

                                            const newDocument = {...templateDocument};

                                            const targetRegion: RegionCanvasTreeNode = newDocument.r![region.name]! as RegionCanvasTreeNode;

                                            if (!targetRegion.rc) {
                                                targetRegion.rc = {...RegionDefaultRC};
                                                targetRegion.rc.editorLabel = region.name;
                                            }

                                            targetRegion.rc.isLocked = !targetRegion.rc?.isLocked;

                                            props.onChange && props.onChange(templateDocument, newDocument);

                                        }}
                                    >
                                        {rc.isLocked ? <i className='fas fa-lock'></i> : <i className='fas fa-lock-open'></i>}
                                    </button>
                                )
                            }
                            
                            {!rc?.isLocked && (
                                <button 
                                    type='button'
                                    onClick={() => setIsConfiguring(true)}
                                >
                                    <i className='fas fa-cog' />
                                </button>   
                            )}
                            {!rc.isLocked && (
                                <button
                                    type='button'
                                    title={`Add component`}
                                    onClick={() => {
                                        setIsSelectingComponent(region.name)
                                    }}
                                >
                                    <i className='fas fa-plus' />
                                </button>
                            )}
                            {isConfiguring && (
                                <Modal
                                    visible
                                    title={`Configure ${correspondingNode.rc?.editorLabel ?? correspondingNode.t}`}
                                    onClose={() => {
                                        setIsConfiguring(false)

                                        const newDocument = {...templateDocument};

                                        const targetRegion: RegionCanvasTreeNode = newDocument.r![region.name]! as RegionCanvasTreeNode;

                                        if (!targetRegion.rc) {
                                            targetRegion.rc = {...RegionDefaultRC};
                                            targetRegion.rc.editorLabel = region.name;
                                        }

                                        props.onChange && props.onChange(templateDocument, newDocument);
                                    }}
                                    noFooter
                                >
                                    <ComponentPropEditor
                                        item={correspondingNode}
                                        onChange={() => {

                                            const newDocument = {...templateDocument};

                                            const targetRegion: RegionCanvasTreeNode = newDocument.r![region.name]! as RegionCanvasTreeNode;

                                            if (!targetRegion.rc) {
                                                targetRegion.rc = {...RegionDefaultRC};
                                                targetRegion.rc.editorLabel = region.name;
                                            }

                                            props.onChange && props.onChange(templateDocument, newDocument);
                                        }}
                                        isRegion={true}
                                    />
                                </Modal>
                            )}
                        </div>
                        

                        {Array.isArray(correspondingNode.c) && correspondingNode.c.length != 0 && (
                            <div className="child-regions">
                                {correspondingNode.c.map((childNode, idx) => {
                                    return (
                                        <CanvasNodeEditor 
                                            key={'child-' + idx + '-' + childNode.t} 
                                            node={childNode} 
                                            onChange={() => {
                                                const newDocument = {...templateDocument};

                                                const targetRegion: RegionCanvasTreeNode = newDocument.r![region.name]! as RegionCanvasTreeNode;

                                                if (!targetRegion.rc) {
                                                    targetRegion.rc = {...RegionDefaultRC};
                                                    targetRegion.rc.editorLabel = region.name;
                                                }

                                                props.onChange && props.onChange(templateDocument, newDocument);
                                            }}
                                            onDelete={(toDelete) => {
                                                const newDocument = {...templateDocument};

                                                const targetRegion: RegionCanvasTreeNode = newDocument.r![region.name]! as RegionCanvasTreeNode;

                                                if (!targetRegion.rc) {
                                                    targetRegion.rc = {...RegionDefaultRC};
                                                    targetRegion.rc.editorLabel = region.name;
                                                }

                                                targetRegion.c!.splice(
                                                    targetRegion.c!.indexOf(toDelete), 1
                                                )

                                                props.onChange && props.onChange(templateDocument, newDocument);
                                            }}
                                        />
                                    )
                                })}
                            </div>
                        )} 
                    </div>
                )
            })}
            {isSelectingComponent && (
                // select a component to add to the region.
                <ComponentSelector
                    title={`Add component to ${isSelectingComponent}`}
                    onComponentSelected={(path, componentProps) => {
                        
                        // clone the current document
                        const newDocument = {...templateDocument};
                        
                        // grab the current region
                        const targetRegion: RegionCanvasTreeNode = newDocument.r![isSelectingComponent]! as RegionCanvasTreeNode;

                        if (!Array.isArray(targetRegion.c)) {
                            targetRegion.c = [];
                        }

                        // add the new component to the region
                        targetRegion.c.push({
                            t: path,
                            d: componentProps,
                            c: []
                        })

                        props.onChange && props.onChange(templateDocument, newDocument);

                    }}
                    onClose={() => {
                        setIsSelectingComponent(undefined)
                    }}
                />
            )}
        </div>
    )
}

type CanvasNodeEditorProps = {
    node: RegionCanvasTreeNode,
    onChange: () => void,
    onDelete: (toDelete: EditorCanvasTreeNode) => void
}

/**
 * This is for editing the non-root components of the template, its used recursively
 * @param {CanvasNodeEditorProps} props 
 * @returns 
 */
const CanvasNodeEditor = (props: CanvasNodeEditorProps) => {

    const { node } = props;

    const [supportsChildren, setSupportsChildren] = useState<boolean>();
    const [isSelectingComponent, setIsSelectingComponent] = useState<boolean>();
    const [isRenaming, setIsRenaming] = useState<boolean>();
    const [isConfiguring, setIsConfiguring] = useState<boolean>();

    useEffect(() => {
        if (typeof supportsChildren === "undefined") {
            canAddChildren(node.t).then((result) => {
                setSupportsChildren(result);
            })
        }
    }, [supportsChildren])

    const toggleLock = () => {
        if (!node.rc) {
            node.rc = {...RegionDefaultRC};
            node.rc.editorLabel = node.t;
        }
        
        node.rc.isLocked = !node.rc.isLocked;
        
        props.onChange();
    }

    return (
        <div className='child-section region-inner'>
            <div className='main-info'>
                <h4>{node.rc?.editorLabel ?? node.t}</h4>
                {
                    // if the component is locked by the parent, we can't do anything to it. 
                    !node.rc?.isLockedByParent && (
                        node.rc?.isLocked ? <i onClick={() => toggleLock()} className='fas fa-lock'></i> : <i onClick={() => toggleLock()} className='fas fa-lock-open'></i>
                    )
                }

                {!node.rc?.isLocked && supportsChildren && node.rc?.childrenAllowed !== false && (
                    // if the component is locked, we cannot add a component to it.
                    <i className='fas fa-plus' onClick={() => setIsSelectingComponent(true)} />
                )}
                {isSelectingComponent && (node.rc?.childrenAllowed !== false) && (
                    <ComponentSelector
                        title={`Add component to ${isSelectingComponent}`}
                        onComponentSelected={(path, componentProps) => {
                            
                            if (!Array.isArray(node.c)) {
                                node.c = [];
                            }

                            node.c.push({
                                t: path,
                                d: componentProps ?? {},
                                c: []
                            })

                            props.onChange && props.onChange();
                            
                        }}
                        onClose={() => {
                            setIsSelectingComponent(false)
                        }}
                    />
                )}
                {!node.rc?.isLocked && (
                    <i className='fas fa-edit' onClick={() => setIsRenaming(true)}  />
                )}
                {!node.rc?.isLocked && (
                    <i className='fas fa-cog' onClick={() => setIsConfiguring(true)} />   
                )}
                {!node.rc?.isLocked && (
                    <i className='fas fa-trash' onClick={() => props.onDelete(node)}/>
                )}
            </div>
            <div className="child-regions">
                {node.c && node.c.map((childNode, idx) => {
                    return (
                        <CanvasNodeEditor
                            node={childNode}
                            key={idx}
                            onChange={() => {
                                props.onChange();
                            }}
                            onDelete={(toDelete) => {
                                node.c!.splice(
                                    node.c!.indexOf(toDelete), 1
                                )
                                props.onChange();
                            }}
                        />
                    )
                })}
            </div>
            {isConfiguring && (
                <Modal
                    visible
                    title={`Configure ${node.rc?.editorLabel ?? node.t}`}
                    onClose={() => {
                        setIsConfiguring(false)
                        props.onChange();
                    }}
                    noFooter
                >
                    <ComponentPropEditor
                        item={node}
                        onChange={() => {
                            props.onChange();
                        }}
                        isRegion={false}
                    />
                </Modal>
            )}
            {isRenaming && (
                <Modal
                    visible
                    title={`Rename ${node.rc?.editorLabel ?? node.t}`}
                    onClose={() => {
                        setIsRenaming(false)
                        props.onChange();
                    }}
                    noFooter
                >
                    <Input
                        type='text'
                        value={node.rc?.editorLabel ?? node.t}
                        onInput={(ev) => {
                            const target = ev.target as HTMLInputElement;

                            if (!node.rc) {
                                node.rc = {...RegionDefaultRC};
                                node.rc.editorLabel = node.t;
                            }
                            node.rc.editorLabel = target.value;
                        }}
                        onKeyDown={(ev) => {
                            if (ev.keyCode == 13) {
                                ev.stopPropagation();
                                ev.preventDefault();

                                setIsRenaming(false);
                                props.onChange();
                            }
                        }}
                    />
                </Modal>
            )}
        </div>
    )
}

export default RegionEditor;
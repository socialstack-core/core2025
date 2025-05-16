/**
 * A recursive React component that renders a visual representation of a CanvasNode
 * with editing, configuration, and child component management capabilities.
 * 
 * This includes support for:
 * - Renaming nodes
 * - Adding/removing child nodes (conditionally allowed)
 * - Configuring allowed children
 * - Editing raw node properties
 */

import { useEffect, useReducer, useState } from "react";
import { CanvasNode } from "../types";
import { canAddChildren } from "../Region/functions";
import Modal from "UI/Modal";
import Input from "UI/Input";
import CanvasNodeComponentSelector from "Admin/Template/Components/Editor/CanvasNodeComponentSelector";
import { CodeModuleMeta } from "Admin/Functions/GetPropTypes";
import Collapsible from "UI/Collapsible";
import PermittedComponents from "Admin/Template/Components/Editor/PermittedComponents";

/**
 * Props for the CanvasNodeRenderer component.
 * 
 * @typedef {Object} CanvasNodeRendererProps
 * @property {CanvasNode} node - The canvas node to render and manage.
 * @property {() => void} onChange - Callback triggered when the node is modified.
 */
export type CanvasNodeRendererProps = {
    node: CanvasNode,
    onChange: () => void
}

/**
 * Main component that renders a single CanvasNode and recursively its children.
 * Supports editing features like renaming, adding children, and configuration modals.
 *
 * @component
 * @param {CanvasNodeRendererProps} props - The input props including node and onChange.
 * @returns {React.ReactNode} A rendered canvas node interface with editing controls.
 */
const CanvasNodeRenderer: React.FC<CanvasNodeRendererProps> = (props: CanvasNodeRendererProps): React.ReactNode => {

    const { node, onChange } = props;

    /**
     * Whether the current node supports children based on its type metadata.
     */
    const [nodeSupportsChildren, setNodeSupportsChildren] = useState(false);

    /**
     * Modal visibility states for various UI controls.
     */
    const [isRenaming, setIsRenaming] = useState(false);
    const [isAddingChild, setIsAddingChild] = useState(false);
    const [isConfiguring, setIsConfiguring] = useState(false);

    /**
     * Force component re-rendering manually when needed (used for UI state updates).
     */
    const [, forceUpdate] = useReducer((x) => x + 1, 0);

    /**
     * Determine if the current node supports adding more children based on its config.
     */
    const canAddChild = nodeSupportsChildren && (
        node.ec.multipleChildrenAllowed ||
        (!node.ec.multipleChildrenAllowed && node.c.length === 0)
    );

    /**
     * Fetch whether this node type supports children from backend metadata.
     */
    useEffect(() => {
        canAddChildren(node.t)
            .then(setNodeSupportsChildren)
            .catch(console.error);
    }, [node.t]);

    return (
        <div className='canvas-node'>
            {/* Label and action bar for the current node */}
            <div className='label'>
                {/* Display the label or fallback to the component type */}
                {node.ec.editorLabel ?? node.t}

                <div className='actions'>
                    {/* Toggle lock if not locked by a parent */}
                    {!node.ec.isLockedByParent && (
                        <i 
                            onClick={() => {
                                node.ec.isLocked = !node.ec.isLocked;
                                onChange();
                                forceUpdate(); // update UI
                            }}
                            className={'fas fa-lock' + (node.ec.isLocked ? '' : '-open')}
                        />
                    )}

                    {/* Editing actions if the node isn't locked */}
                    {!node.ec.isLocked && (
                        <>
                            {canAddChild && (
                                <i 
                                    onClick={() => setIsAddingChild(true)} 
                                    className='fas fa-plus' 
                                    title="Add child"
                                />
                            )}
                            <i 
                                className='fas fa-pencil' 
                                onClick={() => setIsRenaming(true)} 
                                title="Rename"
                            />
                            <i 
                                className='fas fa-cog' 
                                onClick={() => setIsConfiguring(true)} 
                                title="Configure"
                            />
                        </>
                    )}
                </div>
            </div>

            {/* Recursively render children */}
            <div className='children'>
                {node.c.map((childNode, index) => (
                    <CanvasNodeRenderer
                        key={index}
                        node={childNode}
                        onChange={onChange}
                    />
                ))}
            </div>

            {/* Modal: Rename node */}
            {isRenaming && (
                <Modal
                    visible
                    title={`Rename ${node.ec.editorLabel ?? node.t}`}
                    noFooter
                    onClose={() => {
                        onChange();
                        setIsRenaming(false);
                    }}
                >
                    <Input
                        type='text'
                        defaultValue={node.ec.editorLabel ?? node.t}
                        label={`Rename component`}
                        onInput={(ev) => {
                            node.ec.editorLabel = (ev.target as HTMLInputElement).value;
                        }}
                        onKeyDown={(ev) => {
                            if (ev.key === 'Enter') {
                                ev.preventDefault();
                                ev.stopPropagation();
                                onChange();
                                setIsRenaming(false);
                            }
                        }}
                    />
                </Modal>
            )}

            {/* Modal: Add child component */}
            {isAddingChild && (
                <Modal
                    visible
                    title={`Add child to ${node.ec.editorLabel ?? node.t}`}
                    noFooter
                    onClose={() => setIsAddingChild(false)}
                >
                    <CanvasNodeComponentSelector
                        permitted={node.ec.allowedComponents}
                        onSelect={(componentName: string, component: CodeModuleMeta) => {
                            // Push new child node to current node
                            node.c.push({
                                t: componentName,
                                ec: {
                                    isLocked: false,
                                    isLockedByParent: false,
                                    multipleChildrenAllowed: true,
                                    allowedComponents: []
                                },
                                d: {},
                                c: []
                            });
                            setIsAddingChild(false);
                            onChange();
                        }}
                    />
                </Modal>
            )}

            {/* Modal: Configuration (allowed components + raw editing) */}
            {isConfiguring && (
                <Modal
                    visible
                    title={`Edit configuration for '${node.ec.editorLabel ?? node.t}'`}
                    noFooter
                    onClose={() => setIsConfiguring(false)}
                >
                    {/* Collapsible: Allowed components */}
                    <Collapsible
                        title={`Permitted components (${node.ec.allowedComponents.length === 0 ? 'All' : node.ec.allowedComponents.length})`}
                    >
                        <PermittedComponents
                            node={node}
                            key={'permitted-components-' + (node.ec.editorLabel ?? node.t)}
                            onChange={(permitted) => {
                                node.ec.allowedComponents = permitted;
                                onChange();
                            }}
                        />
                    </Collapsible>

                    {/* Collapsible: Raw JSON editing */}
                    <Collapsible title={`Edit properties`}>
                        <Input
                            type='canvas'
                            defaultValue={JSON.stringify(props.node)}
                            onCanvasChange={(updated: string) => {
                                const newNode = JSON.parse(updated);

                                if (!Array.isArray(newNode.c)) {
                                    node.d = newNode.d;
                                    onChange();
                                    return;
                                }

                                node.c = newNode.c;
                                node.t = newNode.t;

                                // Validate and fix up config recursively
                                ensureConfigAndDeleteNonNodes(node.c);

                                onChange();
                            }}
                        />
                    </Collapsible>
                </Modal>
            )}
        </div>
    );
};

/**
 * Recursively ensure each child node has a valid config object.
 * Remove non-object children (e.g., string placeholders).
 *
 * @param {CanvasNode[]} children - The list of children to validate and sanitize.
 */
const ensureConfigAndDeleteNonNodes = (children: CanvasNode[]) => {
    children.forEach((child, idx) => {
        // Remove string children (invalid structure)
        if (typeof child === 'string') {
            children.splice(idx, 1);
            return;
        }

        // Ensure editor config exists
        if (!child.ec) {
            child.ec = {
                isLocked: false,
                isLockedByParent: false,
                allowedComponents: [],
                multipleChildrenAllowed: true
            };
        }

        // Recursively sanitize nested children
        ensureConfigAndDeleteNonNodes(child.c);
    });
};

export default CanvasNodeRenderer;

import { useState } from "react";
import { CanvasNode } from "../types";
import Modal from "UI/Modal";
import PermittedComponents from "Admin/Template/Components/Editor/PermittedComponents";
import Collapsible from "UI/Collapsible";
import ComponentSelector from "Admin/Template/Components/Editor/RegionComponentSelector";
import { CodeModuleMeta } from "Admin/Functions/GetPropTypes";
import CanvasNodeRenderer from "Admin/Template/Components/Editor/CanvasNodeRenderer";

/**
 * Props definition for the RegionEditor component.
 */
type RegionEditorProps = {
    /** The display name of the region (e.g., "Header", "Footer", etc.) */
    name: string;
    /** The region node object that contains configuration and structure */
    region: CanvasNode;
    /** Callback to notify parent when the region is updated */
    onChange: (newRegion: CanvasNode) => void;
};

/**
 * RegionEditor
 *
 * A specialized editor for managing "regions" within a UI builder canvas.
 * Regions are treated similarly to nodes but are top-level/root descendants
 * and may be subject to additional constraints.
 *
 * This editor allows:
 * - Locking/unlocking the region configuration
 * - Changing the main component type
 * - Configuring allowed components
 * - Rendering the region’s child structure via `CanvasNodeRenderer`
 *
 * @param {RegionEditorProps} props - The props required to render and interact with the region.
 * @returns {JSX.Element} A rendered React component to edit a canvas region.
 */
const RegionEditor: React.FC<RegionEditorProps> = (props: RegionEditorProps): React.ReactNode => {
    const { name, region } = props;

    // Editor configuration state for the current region
    const config = region.ec;

    // UI State: Toggles for modal visibility
    const [isConfiguring, setIsConfiguring] = useState(false);
    const [isChangingComponent, setIsChangingComponent] = useState(false);

    return (
        <div className='region'>
            <div className='main'>

                {/* Region title + currently selected component */}
                <h3>
                    {name} ({region.t === 'Admin/Template/Wrapper' ? 'empty' : region.t})
                </h3>

                {/* Actions for locking, changing, or configuring the region */}
                <div className='actions'>
                    {!config.isLockedByParent && (
                        <i
                            className={'fas fa-lock' + (config.isLocked ? '' : '-open')}
                            onClick={() => {
                                // Toggle lock and notify change
                                config.isLocked = !config.isLocked;

                                const newRegion: CanvasNode = {
                                    ...region,
                                    d: {},
                                    c: [],
                                    ec: { ...config }
                                };
                                props.onChange(newRegion);
                            }}
                        />
                    )}

                    {!config.isLocked && (
                        <>
                            <i
                                title='Change component'
                                onClick={() => setIsChangingComponent(true)}
                                className='fas fa-retweet'
                            />
                            <i
                                title='Edit component'
                                onClick={() => setIsConfiguring(true)}
                                className='fas fa-cog'
                            />
                        </>
                    )}
                </div>

                {/* Render region's internal child structure, if it’s not a placeholder */}
                {region.t && region.t !== "Admin/Template/Wrapper" && (
                    <div className='region-children'>
                        <CanvasNodeRenderer
                            node={region}
                            onChange={() => {
                                // Notify parent that children structure changed
                                props.onChange({ ...region });
                            }}
                        />
                    </div>
                )}
            </div>

            {/* -------- Modal: Component Selector -------- */}
            {isChangingComponent && !config.isLocked && (
                <Modal
                    visible={true}
                    title={`Change region '${name}' component`}
                    onClose={() => setIsChangingComponent(false)}
                    noFooter
                >
                    <ComponentSelector
                        node={region}
                        onChange={(componentName: string, component: CodeModuleMeta) => {
                            const newRegion: CanvasNode = {
                                ...region,
                                t: componentName,
                                d: {},
                                c: []
                            };
                            props.onChange(newRegion);
                            setIsChangingComponent(false);
                        }}
                    />
                </Modal>
            )}

            {/* -------- Modal: Configuration Editor -------- */}
            {isConfiguring && (
                <Modal
                    visible={true}
                    title={`Edit configuration for '${name}'`}
                    onClose={() => setIsConfiguring(false)}
                    noFooter
                >
                    <Collapsible
                        title={`Permitted components (${config.allowedComponents.length === 0 ? 'All' : config.allowedComponents.length})`}
                    >
                        <PermittedComponents
                            node={region}
                            key={'permitted-components-' + name}
                            onChange={(components) => {
                                // Update allowed components list and notify parent
                                const newRegion: CanvasNode = {
                                    ...region,
                                    ec: {
                                        ...region.ec,
                                        allowedComponents: components
                                    }
                                };
                                props.onChange(newRegion);
                            }}
                        />
                    </Collapsible>
                </Modal>
            )}
        </div>
    );
};

export default RegionEditor;

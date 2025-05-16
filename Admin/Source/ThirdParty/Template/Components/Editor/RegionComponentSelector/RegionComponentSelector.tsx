/**
 * A React component that displays a list of grouped UI components.
 * Users can filter and select a component from an allowed list.
 * 
 * This is typically used when changing a component type for a canvas region.
 *
 * Key features:
 * - Fetches available components grouped by category on mount
 * - Filters components by user input
 * - Only displays components allowed by node configuration
 */

import { useEffect, useState } from "react";
import Loading from "UI/Loading";
import Input from "UI/Input";
import { ComponentGroup } from "./types";
import { CanvasNode } from "../types";
import { getAllGroupedComponents } from "../PermittedComponents/functions";
import { CodeModuleMeta } from "Admin/Functions/GetPropTypes";

/**
 * Props for the RegionComponentSelector.
 */
export type RegionComponentSelectorProps = {
    /** The canvas node to configure; contains metadata such as allowed components. */
    node: CanvasNode;
    /** Callback triggered when a component is selected. */
    onChange: (name: string, component: CodeModuleMeta) => void;
};

/**
 * RegionComponentSelector
 *
 * Allows users to select a valid component from a grouped, filterable list,
 * respecting the component restrictions defined in the provided node.
 *
 * @component
 * @param {RegionComponentSelectorProps} props - Component props
 * @returns {React.ReactNode} Rendered UI for selecting a region component
 */
const RegionComponentSelector: React.FC<RegionComponentSelectorProps> = (
    props: RegionComponentSelectorProps
): React.ReactNode => {

    const { ec } = props.node;
    const { allowedComponents } = ec;

    /** Fetched and grouped list of components available to select. */
    const [groups, setGroups] = useState<ComponentGroup[]>();

    /** Text-based filter input for narrowing down the component list. */
    const [filter, setFilter] = useState<string>();

    /** Fetch all components on mount (if not already loaded). */
    useEffect(() => {
        if (!Array.isArray(groups)) {
            getAllGroupedComponents()
                .then(setGroups)
                .catch(console.error);
        }
    }, [groups]);

    return (
        <div className='permitted-components'>

            {/* Show loading state while fetching component metadata */}
            {!groups && <Loading />}

            {/* Show list once components are loaded */}
            {groups && (
                <div className='group-collection'>

                    {/* Input field for filtering components */}
                    <div className='mb-3 search-field'>
                        <Input
                            type='text'
                            placeholder='Filter components'
                            onInput={(ev) =>
                                setFilter((ev.target as HTMLInputElement).value)
                            }
                            onKeyDown={(ev) => {
                                if (ev.key === 'Enter') {
                                    ev.preventDefault();
                                    ev.stopPropagation();
                                }
                            }}
                        />
                    </div>

                    {/* Display each group of components */}
                    {groups.map(group => {
                        // Skip group if no items match the filter
                        if (filter?.length) {
                            const hasMatches = group.items.some(c =>
                                c.path.toLowerCase().includes(filter.toLowerCase())
                            );
                            if (!hasMatches) return;
                        }

                        // Skip group if none of the components are allowed
                        if (
                            allowedComponents.length &&
                            !group.items.some(item => allowedComponents.includes(item.path))
                        ) {
                            return;
                        }

                        return (
                            <div className='group-item' key={group.groupName}>
                                {/* Group label */}
                                <div className='group-item-head'>
                                    <h3>{group.groupName}</h3>
                                </div>

                                {/* Render group items */}
                                <div className='group-item-body'>
                                    {group.items.map(item => {
                                        // Skip item if it doesn't match filter
                                        if (
                                            filter?.length &&
                                            !item.path.toLowerCase().includes(filter.toLowerCase())
                                        ) {
                                            return null;
                                        }

                                        // Skip item if it's not in the allowed list
                                        if (
                                            allowedComponents.length &&
                                            !allowedComponents.includes(item.path)
                                        ) {
                                            return;
                                        }

                                        return (
                                            <div
                                                key={item.path}
                                                className='group-component'
                                                onClick={() =>
                                                    props.onChange(item.path, item.component)
                                                }
                                            >
                                                <i className='fas fa-puzzle-piece' />
                                                <span>{item.name}</span>
                                            </div>
                                        );
                                    })}
                                </div>
                            </div>
                        );
                    })}
                </div>
            )}
        </div>
    );
};

export default RegionComponentSelector;

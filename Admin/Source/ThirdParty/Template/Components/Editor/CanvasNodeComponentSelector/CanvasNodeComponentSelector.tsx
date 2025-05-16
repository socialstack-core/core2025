/**
 * A React component used for selecting components (grouped by module path)
 * that are allowed as children of a canvas node. It filters the list by both
 * user input and a list of permitted component paths.
 *
 * Key features:
 * - Fetches available component groups on mount
 * - Filters groups by text and permissions
 * - Allows selecting a component via callback
 */

import { useEffect, useState } from "react";
import Loading from "UI/Loading";
import Input from "UI/Input";
import { CodeModuleMeta } from "Admin/Functions/GetPropTypes";
import { getAllGroupedComponents } from "../PermittedComponents/functions";
import { ComponentGroup } from "../RegionComponentSelector/types";

/**
 * Props for the CanvasNodeComponentSelector component.
 */
export type CanvasNodeComponentSelectorProps = {
    /**
     * A list of permitted component module paths.
     * Only these will be shown in the selector. If empty, all are shown.
     */
    permitted: string[];

    /**
     * Callback triggered when a user selects a component.
     *
     * @param name - The module path of the selected component.
     * @param component - The metadata describing the selected component.
     */
    onSelect: (name: string, component: CodeModuleMeta) => void;
};

/**
 * A UI component that allows users to select a component from a list of grouped modules,
 * optionally filtered by allowed component paths and user-provided search input.
 *
 * @component
 * @param {CanvasNodeComponentSelectorProps} props - Component input props.
 * @returns {React.ReactNode} Rendered component selector interface.
 */
const CanvasNodeComponentSelector: React.FC<CanvasNodeComponentSelectorProps> = (
    props: CanvasNodeComponentSelectorProps
): React.ReactNode => {

    const { permitted, onSelect } = props;

    /**
     * Holds the grouped components returned from the backend.
     */
    const [groups, setGroups] = useState<ComponentGroup[]>();

    /**
     * A text-based filter used to narrow down displayed components.
     */
    const [filter, setFilter] = useState<string>();

    /**
     * Effect hook: fetches grouped components on mount, unless already fetched.
     */
    useEffect(() => {
        if (!Array.isArray(groups)) {
            getAllGroupedComponents()
                .then(setGroups)
                .catch(console.error);
        }
    }, [groups]);

    return (
        <div className='permitted-components'>

            {/* Show a loading indicator while groups are being fetched */}
            {!groups && <Loading />}

            {/* Once fetched, render the grouped component UI */}
            {groups && (
                <div className='group-collection'>

                    {/* Search input to filter displayed components by name or path */}
                    <div className='mb-3 search-field'>
                        <Input
                            type='text'
                            placeholder={`Filter components`}
                            onInput={(ev) => {
                                setFilter((ev.target as HTMLInputElement).value);
                            }}
                            onKeyDown={(ev) => {
                                if (ev.key === 'Enter') {
                                    ev.stopPropagation();
                                    ev.preventDefault();
                                }
                            }}
                        />
                    </div>

                    {/* Loop through each component group */}
                    {groups.map(group => {

                        // Filter out entire group if no items match the search filter
                        if (filter && filter.length !== 0) {
                            const hasMatches = group.items.some(c =>
                                c.path.toLowerCase().includes(filter.toLowerCase())
                            );
                            if (!hasMatches) return;
                        }

                        // If permitted list is present, skip groups with no allowed components
                        if (permitted.length !== 0 && !group.items.some(item => permitted.includes(item.path))) {
                            return;
                        }

                        return (
                            <div className='group-item' key={group.groupName}>

                                {/* Display the group label/header */}
                                <div className='group-item-head'>
                                    <h3>{group.groupName}</h3>
                                </div>

                                {/* Render all matching items in this group */}
                                <div className='group-item-body'>
                                    {group.items.map(item => {

                                        // Skip item if it doesn't match the filter string
                                        if (filter && filter.length !== 0 &&
                                            !item.path.toLowerCase().includes(filter.toLowerCase())) {
                                            return null;
                                        }

                                        // Skip item if it's not included in the permitted list
                                        if (permitted.length !== 0 && !permitted.includes(item.path)) {
                                            return;
                                        }

                                        return (
                                            <div
                                                key={item.path}
                                                className='group-component'
                                                onClick={() => {
                                                    onSelect(item.path, item.component);
                                                }}
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

export default CanvasNodeComponentSelector;

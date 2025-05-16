/**
 * A React component that displays and manages a list of grouped UI components.
 * Users can:
 * - View available component groups
 * - Filter components by name
 * - Toggle individual components as "permitted" or not
 * 
 * This UI is typically used for configuring a parent node’s allowable children.
 */

import { useEffect, useReducer, useState } from "react";
import { PermittedComponentGroup, PermittedComponentsProps } from "./types";
import { getAllGroupedComponents } from "./functions";
import Loading from "UI/Loading";
import Input from "UI/Input";

/**
 * PermittedComponents
 *
 * Renders a filterable list of component groups.
 * Clicking a component toggles its inclusion in the "allowedComponents" list.
 *
 * @component
 * @param {PermittedComponentsProps} props - Props including the target node and optional onChange callback.
 * @returns {React.ReactNode} A full UI for configuring permitted components.
 */
const PermittedComponents: React.FC<PermittedComponentsProps> = (props: PermittedComponentsProps): React.ReactNode => {

    /**
     * Destructure the editor configuration from the passed-in node.
     * This contains the list of currently allowed components.
     */
    const { ec } = props.node;

    /**
     * Allowed components for this node. If empty, all components are implicitly allowed.
     */
    const { allowedComponents } = ec;

    /**
     * groups: The fetched list of all available components, organized by logical group.
     * Initially undefined until fetched.
     */
    const [groups, setGroups] = useState<PermittedComponentGroup[]>();

    /**
     * filter: The current search term used to filter displayed components by name/path.
     */
    const [filter, setFilter] = useState<string>();

    /**
     * forceUpdate: A dummy state update used to trigger re-renders after state mutations
     * like toggling a component's allowed state.
     */
    const [, forceUpdate] = useReducer((a) => a + 1, 0);

    /**
     * useEffect: On mount (and when `groups` is empty), fetch the full component list.
     * Avoids re-fetching on every render by checking if `groups` already exists.
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

            {/* Show loading UI until component groups are loaded */}
            {!groups && <Loading />}

            {/* Render the filter input and grouped component list once loaded */}
            {groups && (
                <div className='group-collection'>

                    {/* Input field for filtering components by name/path */}
                    <div className='mb-3 search-field'>
                        <Input
                            type='text'
                            placeholder={`Filter components`}
                            onInput={(ev) => {
                                // Update local state as the user types
                                setFilter((ev.target as HTMLInputElement).value);
                            }}
                            onKeyDown={(ev) => {
                                // Prevent Enter key from submitting forms or triggering global actions
                                if (ev.key === 'Enter') {
                                    ev.stopPropagation();
                                    ev.preventDefault();
                                    return;
                                }
                            }}
                        />
                    </div>

                    {/* Loop through each group and display if it has matching children */}
                    {groups.map(group => {
                        // If filtering is active and no components in this group match, skip rendering it
                        if (filter && filter.length !== 0) {
                            const hasMatches = group.items.some(c =>
                                c.path.toLowerCase().includes(filter.toLowerCase())
                            );
                            if (!hasMatches) {
                                return null;
                            }
                        }

                        return (
                            <div className='group-item' key={group.groupName}>
                                
                                {/* Group heading (e.g. "Layout", "Media", etc.) */}
                                <div className='group-item-head'>
                                    <h3>{group.groupName}</h3>
                                </div>

                                {/* List of components within the group */}
                                <div className='group-item-body'>
                                    {group.items.map(item => {

                                        // If filtering is active and this item doesn’t match, skip it
                                        if (filter && filter.length !== 0) {
                                            if (!item.path.toLowerCase().includes(filter.toLowerCase())) {
                                                return null;
                                            }
                                        }

                                        const isActive = allowedComponents.includes(item.path);

                                        return (
                                            <div
                                                key={item.path}
                                                className={
                                                    'group-component' + (isActive ? ' active' : '')
                                                }
                                                onClick={() => {
                                                    // Toggle the inclusion state for this component
                                                    if (!isActive) {
                                                        allowedComponents.push(item.path);
                                                    } else {
                                                        allowedComponents.splice(
                                                            allowedComponents.indexOf(item.path),
                                                            1
                                                        );
                                                    }

                                                    // Notify parent of change, if callback is provided
                                                    if (props.onChange) {
                                                        props.onChange(allowedComponents);
                                                    }

                                                    // Force UI update to reflect changes
                                                    forceUpdate();
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

export default PermittedComponents;

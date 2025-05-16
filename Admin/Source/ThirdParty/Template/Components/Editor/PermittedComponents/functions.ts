/**
 * @file getAllGroupedComponents.ts
 *
 * Provides a function to fetch and organize available components into named groups,
 * filtered by user role permissions. This is typically used for configuring which
 * components are available in a visual editor or builder interface.
 */

import { getAll } from "Admin/Functions/GetPropTypes";
import { PermittedComponentGroup } from "./types";
import ComponentGroupApi from 'Api/ComponentGroup';

/**
 * Fetches all components grouped by their module path, filtered by user role permissions.
 *
 * - Fetches all available code modules and groups them by folder/module structure.
 * - Uses the current user's role to restrict available components via ComponentGroup API.
 * - Filters out unwanted paths like Admin, API, Email templates, etc.
 *
 * @returns {Promise<PermittedComponentGroup[]>} Promise resolving to an array of grouped components.
 */
export const getAllGroupedComponents = (): Promise<PermittedComponentGroup[]> => {
    return new Promise((resolve, reject) => {
        /** 
         * The final result: an array of grouped components, each grouped by their module path.
         */
        const all: PermittedComponentGroup[] = [];

        // Step 1: Get full metadata for all code modules, including props and paths
        getAll().then(typeMeta => {

            /**
             * TODO: Future improvement: support fetching permitted components
             * from role-specific data without requiring ComponentGroup API.
             */

            // Step 2: Fetch role-specific permitted components from API
            ComponentGroupApi.list({
                query: "Role = ?",
                args: [
                    // Pull the current role ID from a global (hacky, but currently used)
                    (window as any).gsInit.role.result.id
                ]
            })
            .then((componentGroupResponse) => {

                /**
                 * `permitted` holds a flattened list of all allowed component paths
                 * derived from the component group response for this user's role.
                 */
                const permitted: string[] = [];

                componentGroupResponse.results.forEach(groupItem => {
                    const components = JSON.parse(groupItem.allowedComponents ?? '[]');
                    permitted.push(...components);
                });

                // Step 3: Iterate through all known module paths
                Object.keys(typeMeta.codeModules).forEach(modulePath => {
                    // Step 3a: Exclude non-template component modules
                    if (
                        modulePath.startsWith("UI/Template")    || 
                        modulePath.startsWith("Admin/")         ||
                        modulePath.startsWith("Email/Template") ||
                        modulePath.startsWith("Api/")           ||
                        modulePath.startsWith("UI/Functions")
                    ) {
                        return; // skip these modules
                    }

                    // Step 3b: If `permitted` list exists, only include listed paths
                    if (permitted.length !== 0 && !permitted.includes(modulePath)) {
                        return;
                    }

                    // Step 4: Extract grouping key (directory path) and component name
                    const path = modulePath.substring(0, modulePath.lastIndexOf('/'));
                    const name = modulePath.substring(modulePath.lastIndexOf('/') + 1);

                    // Step 5: Either find an existing group or create a new one
                    const group = all.find(grp => grp.groupName === path);

                    if (!group) {
                        // Create a new group with this component
                        all.push({
                            groupName: path,
                            items: [{
                                name,
                                path: modulePath,
                                component: typeMeta.codeModules[modulePath]
                            }]
                        });
                        return;
                    }

                    // Add component to existing group
                    group.items.push({
                        name,
                        path: modulePath,
                        component: typeMeta.codeModules[modulePath]
                    });

                    // Sort group items alphabetically for nicer display
                    group.items = group.items.sort((a, b) => a.name.localeCompare(b.name));
                });

                // Final sort of all groups by group name
                resolve(all.sort((a, b) => a.groupName.localeCompare(b.groupName)));
            })
            .catch(reject); // Handle failure from ComponentGroupApi

        }).catch(reject); // Handle failure from getAll()
    });
};

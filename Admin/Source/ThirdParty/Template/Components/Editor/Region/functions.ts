import { CodeModuleType, getAll } from "Admin/Functions/GetPropTypes";

/**
 * Determines whether a given component supports children (i.e., can contain child nodes).
 *
 * This is assessed by:
 * - Checking for a `children` field in the component's interface
 * - Checking if the default export includes a generic parameter like `React.PropsWithChildren`
 * - Always allowing Admin/Template components for internal builder compatibility
 *
 * @param {string} componentName - The full module path of the component to evaluate.
 * @returns {Promise<boolean>} Resolves to `true` if the component can have children; otherwise, `false`.
 */
export const canAddChildren = (componentName: string): Promise<boolean> => {
    return new Promise((resolve, reject) => {

        // Short-circuit: Always allow Admin/Template components to have children
        if (componentName.startsWith("Admin/Template")) {
            resolve(true);
            return;
        }

        // Fetch the full code module metadata
        getAll().then(codebase => {

            const module = codebase.codeModules[componentName];

            if (!module) {
                resolve(false); // Component not found in metadata
                return;
            }

            const moduleTypes: CodeModuleType[] = module.types;

            /**
             * Extract just the component name (e.g., "MyComponent" from "Some/Path/MyComponent")
             */
            const name: string = componentName.includes('/')
                ? componentName.split('/').pop()!
                : componentName;

            /**
             * Find the component's interface type that may declare `children` as a field.
             * Assumes convention that instanceName includes the component name.
             */
            const iface = moduleTypes.find(
                exp => exp.name === 'interface' && exp.instanceName?.includes(name)
            );

            // If the interface has a field named "children", it's child-capable
            let canAddChild = Boolean(
                iface?.fields?.find(field => field.name === 'children')
            );

            // If interface didn't indicate children, check the default export
            if (!canAddChild) {
                const defExport = moduleTypes.find(
                    entry => entry.name === 'export' && entry.instanceName === 'default'
                );

                // No default export? Can't determine child support
                if (!defExport?.detail) {
                    resolve(false);
                    return;
                }

                const { detail } = defExport.detail;

                const childrenEnabledPropTypes: string[] = [
                    "React.PropsWithChildren"
                    // Add more known child-enabling types here as needed
                ];

                // Generic info missing or unclear
                if (!detail?.genericParameters || detail.genericParameters.length === 0) {
                    resolve(false);
                    return;
                }

                /**
                 * If the first generic parameter is `React.PropsWithChildren`, assume support
                 */
                canAddChild = childrenEnabledPropTypes.includes(
                    detail.genericParameters[0].name
                );
            }

            resolve(canAddChild);
        }).catch(reject);
    });
};

import { CodeModuleMeta } from "Admin/Functions/GetPropTypes";

/**
 * Represents a logical grouping of components under a shared namespace or category.
 */
export type ComponentGroup = {
    groupName: string;      // e.g., "UI/Button" or "Shared/Forms"
    items: Component[];     // The components under this group
};

/**
 * Represents a single UI component and its metadata.
 */
export type Component = {
    path: string;           // Full import path (e.g., "UI/Button/PrimaryButton")
    name: string;           // Display name of the component "PrimaryButton"
    component: CodeModuleMeta; // Metadata including prop types, exports, etc.
};
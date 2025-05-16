import { CodeModuleMeta } from "Admin/Functions/GetPropTypes";
import { CanvasNode } from "../types";

/**
 * Props for the `PermittedComponents` React component.
 * 
 * This is used to configure which components are allowed as children
 * for a given `CanvasNode`.
 */
export type PermittedComponentsProps = {
    /**
     * The canvas node whose allowed components are being managed.
     */
    node: CanvasNode;

    /**
     * Optional callback triggered when the allowed components list is updated.
     * 
     * @param allowed - Array of strings representing component paths that are permitted.
     */
    onChange?: (allowed: string[]) => void;
};

/**
 * A group of components that are categorized under a shared `groupName`.
 * Typically used for display grouping in the UI (e.g. "Layout", "Media").
 */
export type PermittedComponentGroup = {
    /**
     * The display name of the group.
     */
    groupName: string;

    /**
     * The list of components that belong to this group.
     */
    items: PermittedComponent[];
};

/**
 * A single UI component that can be permitted as a child in a canvas node.
 */
export type PermittedComponent = {
    /**
     * The unique module path of the component (used for identification and import).
     */
    path: string;

    /**
     * The human-readable name of the component (used in UI display).
     */
    name: string;

    /**
     * Metadata about the component, such as prop types and runtime support.
     */
    component: CodeModuleMeta;
};

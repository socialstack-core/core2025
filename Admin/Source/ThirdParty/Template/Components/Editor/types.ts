/**
 * Configuration options for a canvas node.
 */
export type CanvasNodeConfig = {
    /**
     * A list of allowed component type identifiers that can be used within this node.
     */
    allowedComponents: string[];

    /**
     * Indicates whether this node is locked, preventing modifications.
     */
    isLocked: boolean;

    /**
     * Indicates whether this node is locked due to a restriction from its parent.
     */
    isLockedByParent: boolean;

    /**
     * Specifies if this node is allowed to have multiple children.
     */
    multipleChildrenAllowed: boolean;

    /**
     * Has this been renamed?
     */
    editorLabel?: string
};

/**
 * Represents a node within a canvas document.
 */
export type CanvasNode = {
    /**
     * The type identifier of this node (typically corresponds to a component type).
     */
    t: string;

    /**
     * The child nodes of this canvas node.
     */
    c: CanvasNode[];

    /**
     * Arbitrary data associated with this node, typically used for component properties or metadata.
     */
    d: Record<string, any>;

    /**
     * Configuration settings specific to this node.
     */
    ec: CanvasNodeConfig;
};

/**
 * Represents a complete canvas document, extending the base node with a registry.
 */
export type CanvasDocument = CanvasNode & {
    t?: string,
    /**
     * Roots...
     */
    r: Record<string, CanvasNode>;
};

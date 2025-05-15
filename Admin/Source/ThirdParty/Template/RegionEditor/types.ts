/**
 * The region editor minimum configuration, this is required to be able to enforce rules etc... in child templates.
 */
export type RegionConfiguration = {
    /**
     * True if the region has been locked in the current context, either by the parent or by the current config.
     */
    isLocked: boolean,
    /**
     * Only true if the template has a parent, if this is true, then isLocked is also true.
     */
    isLockedByParent: boolean,
    /**
     * The label that shows in the editor, this won't be visible in the actual page.
     */
    editorLabel?: string,
    /**
     * A list of components that are allowed to be added to this region.
     */
    allowedComponents?: string[],
    /**
     * If the current region supports children, if not then allowedComponents should be empty.
     */
    childrenSupported: boolean,

    /**
     * Are children allowed to be added to this region?
     */
    childrenAllowed: boolean,
    /**
     * If this is true, then the region is optional, if false then the region is required.
     */
    isOptional: boolean,

    /**
     * Are multiple children allowed in this region or just one?
     */
    multipleChildrenAllowed?: boolean
}

/**
 * Is the default structure of a node in the canvas tree. 
 */
export type CanvasTreeNode = {
    /**
     * The component, ex. UI/Alert, UI/Modal etc...
     */
    t: string,
    /**
     * The children of this node, if any.
     */
    c?: CanvasTreeNode[],
    /**
     * The props that are passed to the component.
     */
    d?: Record<string, any>
}
/**
 * The structure of the tree node within the region editor.
 */
export type RegionCanvasTreeNode = CanvasTreeNode & {
    /**
     * Holds the configuration of the above, this is used to enforce rules on the children.
     */
    rc?: RegionConfiguration,
    /**
     * All children of this node should be cast to follow this type, this then allows the children to have properties.
     */
    c?: RegionCanvasTreeNode[],
}

export type EditorCanvasTreeNode = CanvasTreeNode & {
    /**
     * Holds the configuration of the above, this is used to enforce rules on the children.
     */
    rc?: RegionConfiguration,
    /**
     * All children of this node should be cast to follow this type, this then allows the children to have properties.
     */
    c?: EditorCanvasTreeNode[],
}

/**
 * Represents the root most node
 */
export type DocumentRootTreeNode = RegionCanvasTreeNode &{
    /**
     * This is "roots", these are the root levels of the tree that match up with the chosen template file.
     */
    r?: Record<string, CanvasTreeNode>
}



/**
 * The default region configuration
 */
const RegionDefaultRC = {
    // by default no component should be locked.
    isLocked: false,
    // this also defaults to false, not guaranteed to have a parent template
    isLockedByParent: false,
    // when there's no label, assign one. For the most part
    // the label just defaults to the component type.
    editorLabel: `Unnamed region`,
    // This starts empty, empty means all components are allowed.
    allowedComponents: [],
    // children are supported by default, only if the component has the "children" property.
    // this is just a cached marker as canAddChildren is a promise, the promise is cached
    // but it could also be a first load, this sits in place for it.
    childrenSupported: true,
    // are children actually allowed in this component, it may support them, but are they allowed?
    childrenAllowed: true,
    // is this component optional, it defaults to true, all components should be optional by default.
    // isOptional should come into effect when in the canvas editor.
    isOptional: true
}

export {
    RegionDefaultRC
}
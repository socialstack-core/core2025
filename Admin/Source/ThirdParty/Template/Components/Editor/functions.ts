import { Template } from "Api/Template";
import { CanvasDocument, CanvasNode } from "./types";

/**
 * Creates a `CanvasDocument` from a parent template.
 * This function parses the `bodyJson` of the parent template into a `CanvasDocument` object.
 * It then recursively updates all regions and components to set the `isLockedByParent` flag 
 * to `true` if the `isLocked` flag is set.
 * 
 * @param {Template} parent - The parent template object that contains the `bodyJson` to be parsed.
 * @returns {CanvasDocument | null} - The created `CanvasDocument` or `null` if the `bodyJson` is invalid or missing.
 */
export const createDocumentFromParent = (parent: Template): CanvasDocument | null => {

    // If no bodyJson exists on the parent template, return null.
    if (!parent.bodyJson) {
        return null;
    }

    // Parse the bodyJson into a CanvasDocument.
    const existingDocument: CanvasDocument = JSON.parse(parent.bodyJson);

    // If parsing fails and no document is created, return null.
    if (!existingDocument) {
        return null;
    }

    // Recursively assign properties to all nodes in the document.
    Object.values(existingDocument.r).forEach(root => {
        recursivePropertyAssignment(root); // Start recursive property assignment on root nodes.
    })

    // Return the fully created and processed document.
    return existingDocument;
}

/**
 * Recursively assigns properties to each node in the CanvasDocument.
 * If a node is locked (`node.ec.isLocked`), it sets `node.ec.isLockedByParent` to true.
 * 
 * This function will traverse all the child nodes recursively.
 * 
 * @param {CanvasNode} node - The current node being processed. This could be a root node or a child node.
 */
const recursivePropertyAssignment = (node: CanvasNode) => {
    // If the current node is locked, mark it as locked by the parent.
    if (node.ec.isLocked) {
        node.ec.isLockedByParent = true;
    }

    // Recursively process each child node.
    node.c.forEach(child => {
        recursivePropertyAssignment(child); // Process the child node.
    })
}

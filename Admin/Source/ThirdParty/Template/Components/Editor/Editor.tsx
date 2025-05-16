import { useEffect, useReducer, useState } from "react";
import { CanvasDocument } from "./types";
import { TemplateModule } from "Admin/Functions/GetPropTypes";
import { Template } from "Api/Template";
import Alert from "UI/Alert";
import RegionEditor from "Admin/Template/Components/Editor/Region";
import Loading from "UI/Loading";
import { createDocumentFromParent } from "./functions";

/**
 * Props for the `Editor` component. This defines the structure of the props passed into the Editor.
 * 
 * @typedef {Object} CanvasDocumentEditorProps
 * @property {string | CanvasDocument} [value] - The value to load into the editor. It can either be a JSON string or an existing CanvasDocument.
 * @property {TemplateModule} uiTemplateFile - The template file used to define the structure and fields of the document being edited.
 * @property {Template} [dbParentTemplate] - An optional parent template from which the document structure can be inherited.
 */
export type CanvasDocumentEditorProps = {
    value?: string | CanvasDocument,
    uiTemplateFile: TemplateModule,
    dbParentTemplate?: Template
};

/**
 * Editor component for managing and editing a `CanvasDocument`. It is used in the context of a template-based form where a document's 
 * content is dynamically generated and edited based on a template.
 *
 * @component
 * @param {CanvasDocumentEditorProps} props - The props for the `Editor` component.
 * @returns {React.ReactNode} - The JSX to render the editor UI.
 */
const Editor: React.FC<CanvasDocumentEditorProps> = (props) => {

    /**
     * Holds the current document being edited. This document will be populated once loaded or created.
     * @type {CanvasDocument | undefined}
     */
    const [document, setDocument] = useState<CanvasDocument>();

    /**
     * A custom hook used to trigger re-rendering of the component. It’s used to force a re-render when the document is updated.
     * @type {React.Dispatch<React.SetStateAction<number>>}
     */
    const [, forceUpdate] = useReducer((x) => x + 1, 0);

    // Destructure the `uiTemplateFile` from props for easy access to the template data
    const { uiTemplateFile } = props;

    /**
     * Extracts the props type from the template that defines the structure of the document.
     * The `targetProp` holds the fields that are used to render editable regions in the document.
     * 
     * @type {Type | undefined}
     */
    const targetProp = uiTemplateFile.types.types.find(type =>
        type.instanceName?.includes('Props') && (type.name === 'interface' || type.name === 'type' || type.name === 'union')
    );

    /**
     * useEffect hook that is triggered when the component mounts or when the `value`, `document`, or `dbParentTemplate` props change.
     * It ensures that a valid `CanvasDocument` is loaded or created. If no document is provided, it creates a new blank document.
     * If a document is provided via the `value` prop, it loads it. If a parent template is provided, it inherits from it.
     */
    useEffect(() => {
        if (!document) {
            let loaded = false;

            // Load the document from the provided value (either as a string or a CanvasDocument)
            if (props.value) {
                if (typeof props.value === 'string') {
                    setDocument(JSON.parse(props.value));
                } else {
                    setDocument(props.value);
                }
                loaded = true;
            }
            // If no document is provided, inherit from the parent template if available
            else if (props.dbParentTemplate) {
                const parent = createDocumentFromParent(props.dbParentTemplate);
                if (parent) {
                    setDocument(parent);
                    loaded = true;
                }
            }

            // If no document is loaded, create a new blank document
            if (!loaded) {
                const newDocument: CanvasDocument = {
                    r: {},  // Regions will be populated later
                    t: '',   // Template type (empty initially)
                    c: [],   // Components (empty initially)
                    d: {},   // Custom data (empty initially)
                    ec: {    // Editor configuration
                        isLocked: false,
                        isLockedByParent: false,
                        allowedComponents: [],
                        multipleChildrenAllowed: true
                    }
                };

                // Add regions based on the fields in the target template property
                targetProp?.fields?.forEach(field => {
                    newDocument.r[field.name] = {
                        t: 'Admin/Template/Wrapper',  // Default type for regions
                        c: [],  // Components for the region
                        d: {},  // Custom data for the region
                        ec: {
                            isLocked: false,
                            isLockedByParent: false,
                            allowedComponents: [],
                            multipleChildrenAllowed: true
                        }
                    };
                });

                setDocument(newDocument);
            }
        }
    }, [props.value, document, props.dbParentTemplate, targetProp?.fields]);

    /**
     * If the template property (`targetProp`) is not found, show an error message.
     * This indicates that the template structure is not valid or cannot be found.
     */
    if (!targetProp) {
        return <Alert variant="danger">{`Unable to locate the template properties`}</Alert>;
    }

    /**
     * If the document is not yet loaded, show a loading spinner until the document is available.
     */
    if (!document) {
        return <Loading />;
    }

    // Filter to allow only fields with the React.ReactNode type to be editable as regions
    const fieldTypes = ["React.ReactNode", "ReactNode"];

    // Destructure the fields from the target property, which defines the editable regions
    const { fields } = targetProp;

    return (
        <div className='template-document-editor'>
            {/* Hidden input field containing the current document as a JSON string */}
            <input type='hidden' name='bodyJson' value={JSON.stringify(document)} />

            <div className='regions'>
                {/* Loop over each field in the template and render a RegionEditor if it's of the correct type */}
                {fields?.map(field => {
                    // Skip fields that are not of type React.ReactNode
                    if (!fieldTypes.includes(field.fieldType.instanceName!)) {
                        return null;
                    }

                    return (
                        <RegionEditor
                            key={field.name}  // Unique key for each RegionEditor
                            name={field.name}  // The name of the field, used as the region name
                            region={document.r[field.name]}  // The current state of the region
                            onChange={(newRegion) => {
                                // When the region is changed, update the document and force a re-render
                                document.r[field.name] = newRegion;
                                forceUpdate();  // Trigger re-render to reflect the changes
                            }}
                        />
                    );
                })}
            </div>
        </div>
    );
};

export default Editor;

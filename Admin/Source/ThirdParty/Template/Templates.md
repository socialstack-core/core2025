# Template Editor Documentation

This documentation provides an overview of the Template Editor module, including its components, structure, and functionality. The Template Editor enables users to create, manage, and modify templates through a user-friendly interface. Templates are designed using a collection of components that can be customized to fit various needs, such as web pages, emails, and PDFs.

## Overview

The **Template Editor** allows users to build and edit templates by arranging and configuring components in designated regions. These templates are later rendered dynamically within a page editor. The system supports creating new templates, editing existing ones, and managing template versions.

## Core Components

The Template Editor consists of several key components, each with a specific role in managing the template structure and content.

### 1. **TemplateForm Component**

The `TemplateForm` component is the central form that handles the creation and editing of templates. It collects essential metadata about the template and its settings, such as title, description, template type, and base template.

* **Template Creation and Editing**: This component allows users to create a new template or edit an existing one, depending on whether an `existing` template is provided.
* **Form Fields**: The form includes fields for the template's title, description, key, template type, and base template. It also features a `TemplateTypeSelector` to specify whether the template is for web, email, or PDF use.
* **Validation**: The form ensures that the template data is valid before submission. If there are missing or incorrect fields, such as an empty title or key, the form will display an error message.
* **Parent Template Selection**: For templates that inherit from another template, the `TemplateForm` allows users to choose a parent template. This creates a hierarchical relationship between templates.

### 2. **TemplateTypeSelector Component**

The `TemplateTypeSelector` component is a form element that allows users to select the type of template they are working with. This is crucial for organizing templates and ensuring they are used in the correct context.

The available template types are:

* **Web**: Templates for use on web pages.
* **Email**: Templates for email campaigns or communications.
* **PDF**: Templates for generating PDF documents.

By selecting the appropriate template type, users can ensure that the correct rendering and formatting rules are applied when the template is used.

### 3. **Editor Component**

The `Editor` component is responsible for managing the overall editing experience. It handles loading templates, rendering regions, and allowing users to interact with components within each region.

* **Loading and Editing Templates**: The editor can load an existing template for editing or create a new one. If an existing template is loaded, its structure is displayed, and users can modify it by adding, removing, or adjusting components within the template's regions.
* **Component Rendering**: The editor displays the components placed within each region of the template. Users can see a visual representation of their template as they work.
* **Dynamic Updates**: As users make changes to the template, the editor dynamically updates to reflect those changes. This ensures that users always see an up-to-date version of the template.
* **Template Preview**: The editor allows users to preview their template before saving or publishing it. This preview helps to ensure the template appears as expected when rendered.

### 4. **RegionEditor Component**

The `RegionEditor` is a child of the `Editor` component and is responsible for managing individual regions within the template. A region is a placeholder within the template where components can be added or modified.

* **Managing Regions**: The `RegionEditor` enables users to add, remove, or rearrange components within a specific region. This flexibility allows for highly customized template designs.
* **Component Placement**: Users can drag and drop components into regions, or select components from a list to add to the region.
* **Dynamic Rendering**: The `RegionEditor` ensures that changes made to the region are immediately reflected in the editor. This helps users visualize their template's structure in real time.

### 5. **TemplateApi**

The `TemplateApi` module is responsible for interacting with the backend to create, update, load, and manage templates.

* **Template Management**: The `TemplateApi` provides methods for creating new templates, updating existing ones, and loading templates from the database. It ensures that template data is correctly saved and retrieved.
* **Template Validation**: Before templates are submitted, the `TemplateApi` performs validation to ensure that all required fields (such as title, key, and template type) are filled out correctly.
* **Data Persistence**: The `TemplateApi` ensures that template data, including structure and content, is persistently stored in the backend, allowing for easy retrieval and modification.

### 6. **CanvasDocument and CanvasNode**

The `CanvasDocument` and `CanvasNode` represent the internal structure of a template. These objects define how the template is constructed and how it will be rendered.

* **CanvasDocument**: The `CanvasDocument` represents the entire template, including all regions and metadata. It acts as the primary data model for the template.
* **CanvasNode**: Each `CanvasNode` represents an individual component or a region within the template. A `CanvasNode` can contain other nodes, allowing for complex nested structures.

These objects are used to store and manage the template structure, ensuring that changes made in the editor are reflected in the template's final output.

### 7. **Validation and Error Handling**

Template creation and editing include built-in validation to ensure that all required fields are filled out and that the template structure is correct. If validation fails, an error message is displayed to inform the user of the issue.

* **Field Validation**: Ensures that required fields, such as the template title, key, and template type, are filled out before submission.
* **Template Structure Validation**: Ensures that the template's structure is valid, such as having the correct regions and components in place.
* **Error Handling**: If there is an error during the template creation or editing process (e.g., missing data or incorrect formatting), the system will display an appropriate error message.

### 8. **Dynamic Parent Template Loading**

Templates can inherit components and regions from a parent template. This feature enables users to create derivative templates based on an existing one, maintaining consistency across templates.

* **Parent Template Selection**: Users can choose a parent template during the template creation or editing process. This allows them to reuse components and regions from the parent template, reducing redundancy.
* **Template Inheritance**: When a template inherits from a parent template, any changes made to the parent template can propagate to its child templates, ensuring consistency across the system.

## How It All Works Together

### Template Creation Process

1. **Create New Template**: The user accesses the `TemplateForm` and provides basic information about the template, such as its title, description, and type.
2. **Configure Template Regions**: In the `Editor`, the user adds components to regions, adjusting the layout and structure as needed.
3. **Validation**: Once the template is configured, the system validates the template to ensure it meets the necessary requirements (e.g., title, key, template type).
4. **Save and Persist**: After successful validation, the template is saved using the `TemplateApi`, and its structure is stored in a `CanvasDocument` object.

### Template Editing Process

1. **Load Existing Template**: The user accesses an existing template by its ID. The `TemplateApi` loads the template data and presents it in the `Editor`.
2. **Modify Template**: The user modifies the template's structure by adjusting regions, adding/removing components, and making any necessary changes.
3. **Validation**: The system ensures the template is still valid after the changes are made.
4. **Save Changes**: The updated template is saved using the `TemplateApi`, and the changes are reflected in the `CanvasDocument`.

### Template Preview and Rendering

After the template is created or modified, users can preview how the template will render once applied. This preview helps ensure that the template looks and behaves as expected in its intended context (e.g., web, email, PDF).

## Conclusion

The Template Editor module provides a powerful and flexible interface for creating and managing templates. By using components and regions, users can build highly customizable templates that meet the specific needs of their content. The editor's validation, error handling, and parent-child template relationships ensure that templates are properly structured and maintained, enabling seamless content management and rendering.

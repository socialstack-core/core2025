import TemplateForm from "./Components/Form";
import { useEffect, useState } from "react";
import TemplateApi, { Template } from "Api/Template";
import Loading from "UI/Loading";

/**
 * Props for the SinglePage component. In this case, no props are passed, hence it’s an empty object.
 * @typedef {Object} SinglePageProps
 */

/**
 * The SinglePage component is responsible for displaying a template creation form or an edit form based on the URL. 
 * It handles loading an existing template when editing or presenting a blank form when creating a new template.
 * 
 * - When the URL path ends with 'add', it displays a new template form.
 * - When an ID is present in the URL, it attempts to load the corresponding template and displays the template form pre-filled with that template's data.
 * 
 * @component
 * 
 * @param {SinglePageProps} props - The props passed to the component (empty in this case).
 * 
 * @returns {React.ReactElement} - JSX element representing the template form or loading state based on the current URL and template state.
 * 
 * @example
 * // Example usage of SinglePage component
 * <SinglePage />
 */
const SinglePage: React.FC<{}> = (props): React.ReactElement => {

    // State to hold the existing template if editing
    const [existing, setExisting] = useState<Template>();

    // Parse the URL to extract the template ID
    const url = location.pathname.split('/');
    const last = url.pop()?.toLowerCase();
    const id = parseInt(last!);

    // Load the existing template when the component mounts or when the ID changes
    useEffect(() => {
        if (!Number.isNaN(id)) {
            TemplateApi.load(id as uint).then(tpl => setExisting(tpl));
        }
    }, [id]);

    // If the URL path ends with 'add', show the template creation form
    if (last === 'add') {
        return <TemplateForm />;
    }

    // If the template is still being loaded, display a loading state
    if (!existing) {
        return <Loading />;
    }

    // Log the existing template’s body JSON (for debugging purposes)
    console.log(JSON.parse(existing.bodyJson!));

    // If the existing template is loaded, show the template form with pre-filled data
    return <TemplateForm existing={existing} />;
}

/**
 * The AdminPage component serves as the wrapper layout for the SinglePage component.
 * It ensures that the SinglePage component is rendered within the Default admin layout.
 * 
 * @component
 * 
 * @returns {React.ReactElement} - JSX element representing the layout containing the SinglePage component.
 * 
 * @example
 * // Example usage of AdminPage component
 * <AdminPage />
 */
const AdminPage = () => {
    return <SinglePage />
}

export default AdminPage;

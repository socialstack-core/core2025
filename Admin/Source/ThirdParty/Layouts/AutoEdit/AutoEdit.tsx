import { useTokens } from 'UI/Token';
import AutoForm from 'Admin/AutoForm';
import Default from 'Admin/Layouts/Default';

interface AutoFormProps {
	// parent

	/**
	 * The entity type name as it appears in the API endpoint such as "User"
	 */
	contentType: string,

	/**
	 * The singular name of the content type such as "User".
	 */
	singular: string,

	/**
	 * The plural name of the content type such as "Users".
	 */
	plural: string,

	/**
	 * Optional previous page URL.
	 */
	previousPageUrl?: string,

	/**
	 * Optional previous page name.
	 */
	previousPageName?: string
}

interface AutoEditProps extends AutoFormProps {
	/**
	 * A string which can contain tokens, such as ${primary.id}.
	 */
	id: string,

	/**
	 * Optional react node to display before the form.
	 */
	afterForm?: React.ReactNode,

	/**
	 * Optional react node to display after the form.
	 */
	beforeForm?: React.ReactNode,

	/**
	 * A function which is given the ID of what is being displayed and returns a react node.
	 * The react content is displayed at the same location as beforeForm.
	 * @param id
	 * @returns
	 */
	beforeFormFunc?: (id:string) => React.ReactNode,
};

const AutoEdit: React.FC<React.PropsWithChildren<AutoEditProps>> = ({ children, ...props }) => {
	var id = useTokens(props.id) as string;

	return <Default>
		{props.beforeFormFunc && props.beforeFormFunc(id)}
		{props.beforeForm}
		<AutoForm {...props} id={id}>
			{props.afterForm}
		</AutoForm>
		{children}
	</Default>;
};

export default AutoEdit;

/*
AutoEdit.propTypes = {
	children: true,
	id: 'token',
	beforeForm:'jsx',
	afterForm: 'jsx',
	endpoint: 'string',
	deletePage: 'string',
	showExportButton: 'bool'
};
*/
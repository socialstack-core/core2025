import Tile from 'Admin/Tile';
import AutoList, { AutoListProps } from 'Admin/AutoList';
import Default from 'Admin/Layouts/Default';

export interface ListProps extends AutoListProps {
    noCreate?: boolean;
}

const List: React.FC<React.PropsWithChildren<ListProps>> = (props): React.ReactNode => {

    let { contentType, singular, plural, children, ...listProps } = props;

    let textPlural = plural;

    if (!props.fields || !Array.isArray(props.fields)) {
        return null;
    }

    const acceptedFields: string[] = ['title', 'name', 'email', 'username', 'description'];

    const defSearchFields: string[] = props.fields.filter(
        field => acceptedFields.includes(field)
    )

    if (!defSearchFields.length) {
        defSearchFields.push('title');
    }

    return (
        <Default>
            <AutoList 
                contentType={contentType} 
                singular={singular}
                plural={plural}
                {...listProps}
                title={`Edit or create ${textPlural}`}
                create={!props.noCreate}
                searchFields={props.searchFields || defSearchFields} 
            />
            {children}
        </Default>	
    )

}

export default List;
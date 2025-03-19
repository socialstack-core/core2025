import Tile from 'Admin/Tile';
import AutoList from 'Admin/AutoList';
import Loop from 'UI/Loop';
import Default from 'Admin/Layouts/Default';
import { useRouter } from 'UI/Router';

export type ListProps = {
    endpoint?: string;
    singular?: string;
      plural?: string;
      fields?: string[];
    children:  (React.ReactNode | string | number | boolean)[]; 
    searchFields?: string[];
    noCreate?: boolean;
}

const List: React.FC<ListProps> = (props: ListProps): React.ReactNode => {

    var router = useRouter();

    if (!router) {
        return null;
    }

    const { pageState } = router;
    let { endpoint, singular, plural } = props;

    if (!endpoint && Array.isArray(pageState.tokenNames)) {
        for(let i = 0;i < pageState.tokenNames.length;i++) {
            if (pageState.tokenNames[i] === 'entity' && pageState.tokens) {
                endpoint = pageState.tokens[i];
                singular = endpoint.replace(/([A-Z])/g, ' $1').trim();
                plural = singular + "s";
                break;
            }
        }
    }

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
                endpoint={endpoint} 
                singular={singular} 
                title={`Edit or create ${singular}`}
                create={!props.noCreate}
                searchFields={props.searchFields || defSearchFields} 
            />
            {props.children}
        </Default>	
    )

}
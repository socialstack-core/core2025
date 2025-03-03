import Tile from 'Admin/Tile';
import Loop from 'UI/Loop';
import {TokenResolver} from 'UI/Token';
import AutoForm from 'Admin/AutoForm';
import Default from 'Admin/Layouts/Default';
import omit from 'UI/Functions/Omit';
import {useRouter} from 'UI/Session';

export default class AutoEdit extends React.Component {
	
	constructor(props){
		super(props);
	}
	
	render(){
		var { pageState } = useRouter();
		var id = this.props.id;
		var parent = this.props.parent;
		var endpoint = this.props.endpoint;
		var entityName = this.props.singular;
		var entityNamePlural = this.props.plural;
		var previousPageUrl = this.props.previousPageUrl;
		var previousPageName = this.props.previousPageName;

		// legacy support - mapping old urlTokens:
		if(typeof id != 'string' && id.type == 'urlToken'){
			id = '${url.' + id.name + '}';
		}

		if (!endpoint && pageState && pageState.tokenNames) {
			for(var i = 0; i < pageState.tokenNames.length; i++) {
				if (pageState.tokenNames[i] == 'entity') {
					endpoint = pageState.tokens[i];
					entityName = endpoint.replace(/([A-Z])/g, ' $1').trim();
					entityNamePlural = entityName + "s";
				} else if (pageState.tokenNames[i] == 'id') {
					id = pageState.tokens[i];
				}
			}
		}

		return <TokenResolver value={id}>
			{id => 
				<Default>
					{this.props.beforeFormFunc && this.props.beforeFormFunc(id)}
					{this.props.beforeForm}
					<AutoForm {...(omit(this.props, ['children']))} id={id}
						parent={parent}  endpoint={endpoint}
						singular={entityName} plural={entityNamePlural}
						previousPageUrl={previousPageUrl} previousPageName={previousPageName}>
						{this.props.afterForm}
					</AutoForm>
					{this.props.children}
				</Default>	
			}
		</TokenResolver>;
	}
	
}

AutoEdit.propTypes = {
	children: true,
	id: 'token',
	beforeForm:'jsx',
	afterForm: 'jsx',
	endpoint: 'string',
	deletePage: 'string',
	showExportButton: 'bool'
};
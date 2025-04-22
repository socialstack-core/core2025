import { CodeModuleMeta } from "Admin/Functions/GetPropTypes";

type ComponentGroupProps = {
    groupName: string,
    group: Record<string, CodeModuleMeta>,
    onComponentSelected: (component: string) => void,
    filter: string | undefined
}

const ComponentGroupRenderer: React.FC<ComponentGroupProps> = (props: ComponentGroupProps): React.ReactElement => {

    const { group, groupName, filter } = props;

    return (
        <div className='component-group'>
            <div className='group-head'>{groupName}</div>
            <div className='group-content'>
                {Object.keys(group).map((componentName) => {

                    // first, check if there is a query applied
                    // people will want to search for components
                    if (filter && filter.length != 0) {

                        // do a check whether the group name contains the query
                        if (!componentName.toLowerCase().includes(filter.toLowerCase())) {
                            return;
                        }
                    }
                    return (
                        <div title={componentName} className='group-item' onClick={() => props.onComponentSelected(componentName)}>
                            <i className="fa fa-puzzle-piece" />
                            <label>{componentName.replace(groupName + "/", "")}</label>
                        </div>
                    )
                })}
            </div>
        </div>
    )

}

export default ComponentGroupRenderer;
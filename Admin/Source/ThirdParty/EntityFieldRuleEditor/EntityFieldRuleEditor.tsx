import { CodeModuleType, getEntities } from "Admin/Functions/GetPropTypes";
import ContentFieldAccessRuleApi, { ContentFieldAccessRule } from "Api/ContentFieldAccessRule";
import RoleApi, { Role } from "Api/Role";
import { useEffect, useState } from "react";
import Alert from "UI/Alert";
import Loading from "UI/Loading";

export type EntityFieldRuleEditorProps = {
    entity: string | CodeModuleType
}

const EntityFieldRuleEditor: React.FC<EntityFieldRuleEditorProps> = (props: EntityFieldRuleEditorProps) => {

    const [entity, setEntity] = useState<CodeModuleType>();
    const [error, setError] = useState<string>();
    const [roles, setRoles] = useState<Role[]>();
    const [currentRole, setCurrentRole] = useState<Role>();

    const [entityRules, setEntityRules] = useState<ContentFieldAccessRule[]>();

    useEffect(() => {

        if (!entity && !error)
        {
            if (typeof props.entity === 'string')
            {
                // get them all, find the match
                getEntities().then(allEntities => {
                    var target = allEntities.find(entity => entity.instanceName == props.entity)
                    if (!target)
                    {
                        setError('Cannot find the entity ' + props.entity);
                        return;
                    }
                    setEntity(target);
                })
            }
            else
            {
                setEntity(props.entity);
            }
        }

    }, [entity, props.entity, error])

    useEffect(() => {
        if (!roles)
        {
            RoleApi.listAll().then(roleCollection => setRoles(roleCollection.results));
        }
    }, [roles])

    useEffect(() => {

        if (currentRole && entity) {
            ContentFieldAccessRuleApi.list({
                query: "entityName = ? AND roleId = ?",
                args: [entity?.instanceName!, currentRole.id]
            })
            .then((result) => {
                setEntityRules(result.results);
            })
        }

    }, [currentRole, entity])

    if (!entity && error)
    {
        return (
            <Alert variant="danger">{error}</Alert>
        )
    }
    if (!entity && !error)
    {
        return (
            <Loading />
        )
    }

    const renderValue = (value: string) => {
        if (value == "true") {
            return <i className="fa fa-check" style={{ color: 'green'}} />;
        }
        if (value == "false") {
            return <i className="fa fa-minus-circle" style={{ color: 'red'}} />;
        }
        if (value == null) {
            // TODO: Resolve truly.
            return <i className="fa fa-check" style={{ color: 'orange'}} />;
        }
        
        return <> {value }<i className="fa fa-check" style={{ color: 'orange'}}/></>;
    }

    return (
        <div className='entity-field-rule-editor'>
            <h4>{`${entity?.instanceName} field rules`}</h4>
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th colSpan={3}>
                            <div className="admin_permission-grid__filter">
                                <label htmlFor="permission_filter" className="col-form-label">{`Roles`}</label>
                                <div className="admin_permission-grid__filter-field input-group">
                                    <select 
                                        className="form-control" 
                                        id="permission_filter"
                                        onChange={(ev) => {
                                            const el: HTMLSelectElement = ev.target;
                                            const role = roles?.find(role => role.id == parseInt(el.value));
                                            setCurrentRole(role);
                                        }}
                                        defaultValue={currentRole?.id}
                                    >
                                        <option value=''>{`Choose role`}</option>
                                        {roles?.map(role => {
                                            return (
                                                <option value={role.id}>{role.name}</option>
                                            )
                                        })}      
                                    </select>
                                </div>
                            </div>
                        </th>
                    </tr>
                    {currentRole && <tr>
                        <th>{`Field name`}</th>
                        <th>{`Can read`}</th>
                        <th>{`Can write`}</th>
                    </tr>}
                </thead>
                <tbody>
                    {!currentRole && (
                        <tr>
                            <td style={{ textAlign: 'center' }} colSpan={3}>{`No role selected`}</td>
                        </tr>
                    )}
                    {currentRole && 
                        entity?.fields?.map(field => {

                            var rule = entityRules?.find(rule => rule.fieldName == field.name);

                            return (
                                <tr>
                                    <td>{field.name}</td>
                                    <td>{renderValue(rule?.canRead!)}</td>
                                    <td>{renderValue(rule?.canWrite!)}</td>
                                </tr>
                            )
                        })
                    }
                </tbody>
            </table>
        </div>
    )
}


export default EntityFieldRuleEditor;
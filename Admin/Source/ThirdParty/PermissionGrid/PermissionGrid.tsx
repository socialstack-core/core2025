import Input from 'UI/Input';
import Modal from 'UI/Modal';
import permissionsApi from 'Api/Permission';
import { useState, useEffect } from 'react';

interface PermissionGridProps {
	editor: boolean;
}

/**
 * A grid of capabilities and the roles they're active on
 */
const PermissionGrid: React.FC<React.PropsWithChildren<PermissionGridProps>> = (props) => {

	const [role, setRoles] = useState([]);
	const [capabilities, setCapabilities] = useState([]);
	const [filteredCapabilities, setFilteredCapabilities] = useState([]);
	const [filter, setFilter] = useState('');
	const [grants, setGrants] = useState(null);
	const [dropdownType, setDropdownType] = useState(null);
	const [editingCell, setEditingCell] = useState(null);

	useEffect(() => {
		load();
	}, [props.editor, props.value, props.defaultValue]);

	const load = () => {
		permissionsApi.list().then(permissionInfo => {
			
			if(props.editor){
				var grants = {};
				try{
					grants = JSON.parse(props.value ||props.defaultValue);
					
					if(!grants || Array.isArray(grants)){
						grants = {};
					}
				}catch(e){
					console.log("Bad grant json: ", e);
				}

				setGrants(grants);
			}
			
			setRoles(permissionInfo.roles);
			setCapabilities(permissionInfo.capabilities);
			setFilteredCapabilities(permissionInfo.json.capabilities);
		});
	}

	const updateFilter = (filter : string) => {
		setFilter(filter);
		setFilteredCapabilities(
			capabilities.filter((capability) => capability.key.toLowerCase().includes(filter.toLowerCase()))
		);
	}

	const clearFilter = () => {
		setFilter('');
		setFilteredCapabilities(capabilities);
	}

	const renderFilter = () => {
		return <>
			<th>
				<div className="admin_permission-grid__filter">
					<label htmlFor="permission_filter" className="col-form-label">
						{`Capability`}
					</label>
					<div className="admin_permission-grid__filter-field input-group">
						<input type="text" className="form-control" id="permission_filter" placeholder={`Filter by`}
							value={filter} onKeyUp={(e : KeyboardEvent) => updateFilter(e.target.value)} />
						<button className="btn btn-outline-secondary" type="button" onClick={() => clearFilter()}>
							{`Clear`}
						</button>
					</div>
				</div>
			</th>
		</>;
	}

	const renderList = () => {
		if (!capabilities) {
			return null;
		}

		filteredCapabilities.sort(function(a, b) {
			if (a.key < b.key) return -1;
			if (a.key > b.key) return 1;
			return 0;
		});

		let noMatches = (!filteredCapabilities || filteredCapabilities.length == 0) && capabilities.length > 0;

        return (
            <div className={'admin_permission-grid'}>
				<table className="table table-striped">
					<thead>
						<tr>
							{renderFilter()}
							{roles.map(role => {
								return (
									<th>
										{role.name}
									</th>
								);
							})}
						</tr>
					</thead>
					<tbody>
						{noMatches && <>
							<tr>
								<td colSpan={roles.length + 1}>
									<span className="admin_permission-grid--nomatch">
										{`No matching capabilities`}
									</span>									
								</td>
							</tr>
						</>}

						{filteredCapabilities.map(cap => {
							var map = {};

							if (cap.grants) {
								cap.grants.forEach(grant => {
									map[grant.role.key] = grant;
								});
							}

							return (
								<tr>
									<td>
										{cap.key}
									</td>
									{roles.map(role => {
										var grant = map[role.key];

										if (!grant) {
											return (<td>
												<i className='fa fa-minus-circle' style={{ color: 'red' }} />
											</td>);
										}

										if (grant.ruleDescription && grant.ruleDescription.length) {
											return (
												<td>
													<i className='fa fa-check' style={{ color: 'orange' }} />
													<p style={{ fontSize: 'smaller' }}>
														{grant.ruleDescription}
													</p>
												</td>
											);
										}

										return (
											<td>
												<i className='fa fa-check' style={{ color: 'green' }} />
											</td>
										);
									})}
								</tr>
							);
						})}
					</tbody>
					{!noMatches && <>
						<tfoot>
							<tr>
								<td colSpan={roles.length + 1}>
								abc
									{!filter || filter.length == 0 && <>
										x
										{`Displaying ${capabilities.length} capabilities`}
									</>}
									{filter && filter.length > 0 && <>
										xx
										{`Displaying ${filteredCapabilities.length} of ${capabilities.length} capabilities`}
									</>}
								</td>
							</tr>
						</tfoot>
					</>}
				</table>
            </div>
        );
	}

	const renderCell = (grantInfo) => {

		if(grantInfo.value === false)
		{
			// red x
			return <i className='fa fa-minus-circle' style={{color: 'red'}}/>;
		}
		else if(typeof grantInfo.value === 'string')
		{
			// text assumed:
			return <div>
				<i className='fa fa-check' style={{color: 'orange'}}/>
				<p style={{fontSize: 'smaller'}}>
					{grantInfo.value}
				</p>
			</div>
		}
		
		//tick
		return <i className='fa fa-check' style={{color: 'green'}}/>;
	}
	
	const getGrantInfo = (capability) => {
		var content = getContent();
		var capGrant = null; // Not granted is the default
		if(capability.grants){
			var grantSet = capability.grants;
			for(var i=0;i<grantSet.length;i++){
				if(grantSet[i].role.key == content.key){
					capGrant = grantSet[i];
					break;
				}
			}
			
		}
		
		var current = {
			inherited: true
		};
		
		if(!capGrant){
			// Inherited grant is x:
			current.value = false;
		}else if(capGrant.ruleDescription && capGrant.ruleDescription.length){
			current.value = capGrant.ruleDescription;
		}else{
			// Inherited is a tick:
			current.value = true;
		}
		
		if(grants && grants[capability.key] !== undefined){
			current.inherited = false;
			current.value = grants[capability.key];
		}
		
		return current;
	}
	
	const getContent = () => {
		return props.currentContent || {};
	}
	
	const renderEditMode = () => {

		if(!grants || !filteredCapabilities){
			return null;
		}
		
		filteredCapabilities.sort(function(a, b) {
			if (a.key < b.key) return -1;
			if (a.key > b.key) return 1;
			return 0;
		});

		let noMatches = (!filteredCapabilities || filteredCapabilities.length == 0) && capabilities.length > 0;

		return [
			<Input type='hidden' inputRef={ref => {
				if(ref){
					ref.onGetValue = () => {
						return JSON.stringify(grants);
					};
				}
			}} label={`Grants`} name={props.name} />,
			<div className={'admin_permission-grid'}>
				<table className="table table-striped">
					<thead>
					<tr>
						{renderFilter()}
						{roles.map(role => {
							if(getContent().id == role.id) {
								return (
									<th>
										{role.name}
									</th>
								);
							}
						})}
					</tr>
					</thead>
					<tbody>
						{noMatches && <>
							<tr>
								<td colSpan={2}>
									<span className="admin_permission-grid--nomatch">
										{`No matching capabilities`}
									</span>
								</td>
							</tr>
						</>}

					{filteredCapabilities.map(cap => {
						
						// Is it overriden? If yes, we have a custom value (otherwise it's inherited).
						var grantInfo = getGrantInfo(cap);
						
						return (
							<tr>
								<td>
									{cap.key}
								</td>
								<td onClick = {() => {
									setEditingCell({
										key: cap.key,
										grantInfo
									});
									setDropdownType(getDropdownType(grantInfo));
								}}>
									{renderCell(grantInfo)}
								</td>
							</tr>);
					})}
					</tbody>
					{!noMatches && <>
						<tfoot>
							<tr>
								<td colSpan={2} >
									{filter.length == 0 && <>
										{`Displaying ${capabilities.length} capabilities`}
									</>}
									{filter.length > 0 && <>
										{`Displaying ${filteredCapabilities.length} of ${capabilities.length} capabilities`}
									</>}
								</td>
							</tr>
						</tfoot>
					</>}
				</table>

				{editingCell && renderEditModal()}
			</div>
		];
	}
	
	const getDropdownType = (grantInfo) => {
		if(grantInfo.inherited){
			return "inherited";
		}
		
		if(grantInfo.value === true){
			return "always";
		}
		
		if(grantInfo.value === false){
			return "never";
		}
		
		return "custom";
	}
	
	const renderEditModal = () => {
		var { grantInfo } = editingCell;
		var content = getContent();
		return <Modal title={editingCell.key + " for " + content.name} visible = {true} isExtraLarge onClose = {() => {
			setEditingCell(null);
		}}>
			<Input 
				label = "Rule"
				type = "select"
				name = "rule" 
				onChange = {(e) => {
					setDropdownType(e.target.value);
				}}
				value={dropdownType}
				defaultValue={dropdownType}
			>
				[
					<option value = {"inherited"}>
						Inherited
					</option>,
					<option value = {"never"}>
						Always denied
					</option>,
					<option value = {"always"}>
						Always granted
					</option>,
					<option value = {"custom"}>
						Custom rule
					</option>
				]
			</Input>
			{dropdownType == 'custom' && <Input inputRef={crRef => this.customRuleRef = crRef} defaultValue = {typeof grantInfo.value === 'string' ? grantInfo.value : ''} validate = {['Required']} label = "Custom Rule" type = "text" name = "customRule"/>}
			<Input type="button" onClick={(e) => {
				e.preventDefault();
				
				// Apply the change to grants now.
				var type = dropdownType;
				
				if(type === "inherited"){
					delete grants[cell.key];
				}else if(type === "never"){
					grants[cell.key] = false;
				}else if(type === "always"){
					grants[cell.key] = true;
				}else if(type === "custom"){
					grants[cell.key] = this.customRuleRef.value || "";
				}
				
				setEditingCell(null);
				
			}}>
				Apply
			</Input>
		</Modal>
	}
	
	if(props.editor) {
		return renderEditMode();
	}

	return  renderList();
	
}

export default PermissionGrid;
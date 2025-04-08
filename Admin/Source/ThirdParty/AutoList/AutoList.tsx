import Canvas from 'UI/Canvas';
import Search from 'UI/Search';
import Table from 'UI/Table';
import { Content } from 'Api/Content';
import ConfirmModal from 'UI/Modal/ConfirmModal';
import {useTokens} from 'UI/Token';
import { isoConvert } from "UI/Functions/DateTools";
import { useState, useEffect } from 'react';
import { ApiIncludes } from 'Api/ApiEndpoints';
import { ApiList } from 'UI/Functions/WebRequest';

type ScalarValue = string | number | boolean

/**
 * Props for the AutoList component.
 */
export interface AutoListProps {
	contentType: string; // e.g. "User" which must exist as "Api/User", a generated module exporting an ApiEndpoints instance.
	singular?: string;
	plural?: string;
	fields: string[];
	searchFields?: string[];
	customUrl?: string;
	title?: string;
	create?: boolean;
	beforeList?: React.ReactNode;
	afterList?: React.ReactNode;
	listEndpoint?: (where?: Partial<Content>, includes?: ApiIncludes) => Promise<Content[]>,
	deleteEndpoint?: (id: number) => Promise<Content>
}

export interface SortField {
	field: string;
	direction: 'asc' | 'desc';
}

const AutoList : React.FC<React.PropsWithChildren<AutoListProps>> = (props) => {

	const [bulkSelections, setBulkSelections] = useState<Record<number, boolean> | null>(null);
	const [searchText, setSearchText] = useState<string>('');
	const [confirmDelete, setConfirmDelete] = useState(false);
	const [deleting, setDeleting] = useState(false);
	const [deleteFailed, setDeleteFailed] = useState(false);
	const [sort, setSort] = useState<SortField | null>(() => {
		// If an id field is specified, that's the default sort
		return props.fields.find(field => field == 'id') ? { field: 'id', direction: 'desc' } : null;
	});

	useEffect(() => {

		if (sort && !props.fields.find(field => field == sort.field)) {
			// Restore to id sort:
			setSort(props.fields.find(field => field == 'id') ? { field: 'id', direction: 'desc' } : null);
		}

	}, [props.fields]);

	const renderEmpty = () => {
		return <tr>
			{searchText ? `No matching records for "${searchText}"` : `No data available`}
		</tr>;
	}

	const renderHeader = (allContent? : Content[]) => {
		// Header (Optional)
		var fields = props.fields.map(field => {
			
			// Class for styling the sort buttons:
			var sortingByThis = sort && sort.field == field;
			var className = '';
			
			if(sortingByThis){
				className = sort?.direction == 'desc' ? 'sorted-desc' : 'sorted-asc';
			}
			
			// Field name with its first letter uppercased:
			var ucFirstFieldName = field.length ? field.charAt(0).toUpperCase() + field.slice(1) : '';
			
			return (
				<th className={className}>
					{ucFirstFieldName} <i className="fa fa-caret-down" onClick={() => {
						// Sort desc
						setSort({
							field,
							direction: 'desc'
						});
					}}/> <i className="fa fa-caret-up" onClick={() => {
						// Sort asc
						setSort({
							field,
							direction: 'asc'
						});
					}}/>
				</th>
			);
		});
		
		// If everything in allContent is selected, mark this as selected as well.
		var checked = false;
		
		if(bulkSelections && allContent?.length){
			checked = true;
			allContent.forEach(e => {
				if(!bulkSelections[e.id]){
					checked = false;
				}
			});
		}
		
		return [
			<th>
				<input type='checkbox' checked={checked} onClick={() => {
					
					// Check or uncheck all things.
					if(checked){
						setBulkSelections(null);
					}else{
						var bs : Record<number, boolean> = {};
						allContent?.forEach(e => bs[e.id] = true);
						setBulkSelections(bs);
					}
					
				}} />
			</th>
		].concat(fields);
	}
	
	const renderColgroups = (allContent? : Content[]) => {
		var fields = props.fields.map(field => {
			var className = '';

			switch (field) {
				case 'id':
					className = 'col__id';
					break;
            }

			return (
				<col className={className}>
				</col>
			);
		});

		return [
			<col className='col__select'>
			</col>
		].concat(fields);
	}

	const getSelectedCount = () => {
		if(!bulkSelections){
			return 0;
		}
		var c = 0;
		for(var k in bulkSelections){
			c++;
		}
		return c;
	}
	
	const renderEntry = (entry: any) => {
		var path = props.customUrl 
			? '/en-admin/' + props.customUrl + '/'
			: '/en-admin/' + props.contentType.toLowerCase() + '/';
		var checked = bulkSelections ? !!bulkSelections[entry.id] : false;
		var checkbox = <td>
			<input type='checkbox' checked={checked} onChange={() => {
				var newBs : Record<number, boolean> | null = null;

				if (bulkSelections && bulkSelections[entry.id]) {
					newBs = {...bulkSelections};
					delete newBs[entry.id];
				}else{
					newBs = {};
					newBs[entry.id] = true;
				}
				
				setBulkSelections(newBs);
			}}/>
		</td>;
		
		// Each row
		return <tr>
			{[checkbox].concat(props.fields.map(field => {

				var fieldValue = entry[field];

				if (field.endsWith("Json") || (typeof fieldValue == "string" && fieldValue.toLowerCase().indexOf('"c":') >= 0)) {
					fieldValue = <Canvas>{fieldValue}</Canvas>;
				} else if (field == "id") {
					if (entry.isDraft) {
						fieldValue = <span>{fieldValue} <span className="is-draft">(Draft)</span></span>;
					}
				} else if (typeof fieldValue == "number" && field.toLowerCase().endsWith("utc")) {
					fieldValue = isoConvert(fieldValue).toUTCString();
				}

				return <td>
					<a href={path + '' + entry.id + (entry.revisionId ? '?revision=' + entry.revisionId : '')}>
						{
							fieldValue
						}
					</a>
				</td>
			}))}</tr>;
	}
	
	const renderBulkOptions = (selectedCount : number) => {
		var message = (selectedCount > 1) ? `${selectedCount} items selected` : `1 item selected`;
		
		return <div className="admin-page__footer-actions">
			<span className="admin-page__footer-actions-label">
				{message}
			</span>
			<button type="button" className="btn btn-danger" onClick={() => startDelete()}>
				{`Delete selected`}
			</button>
		</div>;
	}
	
	const startDelete = () => {
		setConfirmDelete(true);
	}

	const cancelDelete = () => {
		setConfirmDelete(false);
	}

	const confirmDeletion = () => {
		if (!bulkSelections) {
			return;
		}

		setConfirmDelete(false);
		setDeleting(true);

		// get the item IDs:
		var ids: number[] = Object.keys(bulkSelections).map(id => parseInt(id));

		if (typeof props.deleteEndpoint !== 'function') {
			console.warn("[AutoForm] - No delete functionality")
			return
		}
		
		// we checked above that this was a function. hence '!'
		var deletes = ids.map(id => props.deleteEndpoint!(id));

		Promise.all(deletes).then(response => {
			setBulkSelections(null);
		}).catch(e => {
			console.error(e);
			setDeleting(true);
			setDeleteFailed(true);
		});
	}
	
	const renderConfirmDelete = (count : number) => {
		return <ConfirmModal confirmCallback={() => confirmDeletion()} confirmVariant="danger" cancelCallback={() => cancelDelete()}>
			<p>
				{`Are you sure you want to delete ${count} item(s)?`}
			</p>
		</ConfirmModal>
	}

	const capitalise = (name? : string) => {
		return name && name.length ? name.charAt(0).toUpperCase() + name.slice(1) : "";
	}
	
	var {searchFields} = props;

	var searchFieldsDesc = searchFields ? searchFields.join(', ') : undefined;

	var combinedFilter : any = {sort};
	
	if(searchText && searchFields){
		var searchWhere : any[] = [];
		
		for(var i=0;i< searchFields.length;i++){
			var ob: Record<string, any> = {};
			var field = searchFields[i];
			var fieldNameUcFirst = field.charAt(0).toUpperCase() + field.slice(1);
				
			ob[fieldNameUcFirst] = {
				contains: searchText
			};
			
			searchWhere.push(ob);
		}
			
		combinedFilter.where = searchWhere;
	}

	if (!props.contentType) {
		return `Old page identified: please delete your en-admin pages and restart the API to regenerate them.`;
	}

	var addUrl = props.customUrl 
		? '/en-admin/' + props.customUrl + '/' + 'add'
		: '/en-admin/' + props.contentType.toLowerCase() + '/' + 'add';
	
	var selectedCount = getSelectedCount();

	// Api is expected to be an ApiEndpoints object.
	var api = require('Api/' + props.contentType).default;

	return <>
		<div className="admin-page">
			<header className="admin-page__subheader">
				<div className="admin-page__subheader-info">
					<h1 className="admin-page__title">
						{props.title}
					</h1>
					<ul className="admin-page__breadcrumbs">
						{/*this.props.previousPageUrl && this.props.previousPageName &&
						<p>
							<a href={this.props.previousPageUrl}>
								{this.props.previousPageName}
							</a> &gt; <span>
								{this.props.title}
							</span>
						</p>
					*/}

						<li>
							<a href={'/en-admin/'}>
								{`Admin`}
							</a>
						</li>
						<li>
							{capitalise(props.plural)}
						</li>
					</ul>
				</div>
				{searchFields && <>
					<Search 
						className="admin-page__search" 
						placeholder={`Search ${searchFieldsDesc}`}
						onQuery={(where, query : string) => {
							setSearchText(query);
						}}
					/>
				</>}
			</header>
			<div className="admin-page__content">
				<div className="admin-page__internal">
					{props.beforeList}
					<Table over={api} filter={combinedFilter}
						orNone={() => renderEmpty()}
						onResults={results => {
							// Either changed page or loaded for first time - clear bulk selects if there is any.
							if (bulkSelections) {
								setBulkSelections(null);
							}

							return results;
						}} onHeader={renderHeader}>
						{renderEntry}
					</Table>
					{confirmDelete && renderConfirmDelete(selectedCount)}
				</div>
				{/*feedback && <>
					<footer className="admin-page__feedback">
					</footer>
				</>*/}
				<footer className="admin-page__footer">
					{selectedCount > 0 ? renderBulkOptions(selectedCount) : null}
					{props.create && <>
						<a href={addUrl} className="btn btn-primary">
							{`Create`}
						</a>
					</>}
				</footer>
			</div>
		</div>
	</>;
}

export default AutoList;
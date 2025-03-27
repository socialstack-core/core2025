import Tile from 'Admin/Tile';
import Table from 'UI/Table';
import { Filter } from 'UI/Loop';
import Time from 'UI/Time';
import Default from 'Admin/Layouts/Default';
import automationsApi, { Automation } from 'Api/Automation';
import { useState } from 'react';
import { ApiIncludes } from 'Api/ApiEndpoints';
import { ApiList } from 'UI/Functions/WebRequest';

var _latest: Record<string, boolean> | null = null;

const Automations: React.FC<React.PropsWithChildren<{}>> = (props) => {
	
	var [running, setRunning] = useState<Record<string, boolean>>({});
	
	var runAutomation = (entry : Automation) => {
		
		entry.lastTrigger = new Date().toISOString();
		
		var newRunning = {...running};
		newRunning[entry.name] = true;
		_latest = newRunning;
		setRunning(newRunning);
		
		var doneRunning = () => {
			var run = {..._latest};
			delete run[entry.name];
			setRunning(run);
		};

		automationsApi.execute(entry.name).then(() => {
			doneRunning();
		}).catch(e => {
			console.error(e);
			doneRunning();
		});
		
	};
	
	var renderHeader = () => {
		
		return [
			<th>
				{`Name`}
			</th>,
			<th>
				{`Schedule`}
			</th>,
			<th>
				{`Last ran`}
			</th>,
			<th>
				{`Actions`}
			</th>
		];
		
	};

	var renderColgroups = () => {
		return [
			<col></col>,
			<col></col>,
			<col></col>
		];
	};
	
	var renderEntry = (entry: Automation) => {
		return <tr>
			<td>{entry.name}{entry.description && entry.description.length > 0 && <><br /><small>{entry.description}</small></>}</td>
			<td>{entry.cronDescription} ({entry.cron})</td>
			<td>{entry.lastTrigger ? <Time date={entry.lastTrigger}/> : `None since startup`}</td>
			<td>
				<button disabled={running[entry.name]} className="btn btn-primary" onClick={() => {
					
					runAutomation(entry);
					
				}}>Run Now</button>
			</td>
		</tr>;
	};
	
	var renderEmpty = () => {
		return <table className="table">
			<thead>
				<tr>
					{renderHeader()}
				</tr>
			</thead>
			<colgroup>
				{renderColgroups()}
			</colgroup>
			<tbody>
				<tr>
					<td colSpan={2} className="table__empty-message">
						{`No automations`}
					</td>
				</tr>
			</tbody>
		</table>;
	}

	return <Default>
		
		<div className="admin-page">
			<header className="admin-page__subheader">
				<div className="admin-page__subheader-info">
					<h1 className="admin-page__title">
						{`Automations`}
					</h1>
					<ul className="admin-page__breadcrumbs">
						<li>
							<a href={'/en-admin/'}>
								{`Admin`}
							</a>
						</li>
						<li>
							{`Automations`}
						</li>
					</ul>
				</div>
			</header>
			<div className="admin-page__content">
				<div className="admin-page__internal">
					<Table source={(filter?: Filter<Automation>, includes?: ApiIncludes[]) => {
						return automationsApi.get() as Promise<ApiList<Automation>>;
					}}
						orNone={() => renderEmpty()}
						onHeader={renderHeader}
					>
						{renderEntry}
					</Table>
					{props.children}
				</div>
				{/*
				<footer className="admin-page__footer">
					{selectedCount > 0 ? this.renderBulkOptions(selectedCount) : null}
					{this.props.create && <>
						<a href={addUrl} className="btn btn-primary">
							{`Create`}
						</a>
					</>}
				</footer>
				 */}
			</div>
		</div>
	</Default>;

}

export default Automations;
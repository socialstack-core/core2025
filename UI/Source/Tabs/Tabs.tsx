/**
 * recommended usage:
 * 
  
 import { useRouter } from 'UI/Router';
 const { pageState, updateQuery } = useRouter();
 const { query } = pageState;
 
 
 * define tabs via an enum:
  
 	enum UserTab {
		Details = `Details`,
		Permissions = `Permissions`,
		Actions = `Actions`
	}


* define a var to hold the current tab (optionally store and retrieve this as a query param to better support sharing)
	const { query } = pageState;
	const currentTab = query?.get("tab") || UserTab.Details.toLowerCase();


* define a method to update the URL without refreshing the page each time a tab is selected:

	const setCurrentTab = (target: string) => {
		updateQuery({
			tab: target
		});
	};


 * define a render function for each tab:

 const renderDetails = () => { ... }
 const renderPermissions = () => { ... }
 const renderActions = () => { ... }

 const renderTab = (tab: string) => {

   switch (tab) {
     case UserTab.Details:
		 return renderDetails();

     case UserTab.Permissions:
		 return renderPermissions();

     case UserTab.Actions:
		 return renderActions();

     default:
		 return;
   }

 }


 * reference via props on the component:

 <Tabs tabs={Object.values(UserTab)} renderPanel={renderTab} onChange={(tab) => setCurrentTab(tab)} />

 */

/**
 * Props for the Tabs component.
 */
interface TabsProps {
	tabs: string[],
	renderPanel: Function,
	currentTab?: string,
	onChange?: (tab: string) => void;
}

/**
 * The Tabs React component.
 * @param props React props.
 */
const Tabs: React.FC<TabsProps> = (props) => {
	const { tabs, renderPanel, onChange } = props;

	if (!tabs?.length || !renderPanel) {
		return;
	}

	const currentTab = props.currentTab || tabs[0].toLowerCase();

	return (
		<section className="ui-tabs">

			{/* tab links - rendered as radio buttons to allow functionality without reliance on JavaScript */}
			<div className="ui-page__tab-links">
				{tabs.map((tab, i) => {
					const linkId = `tab-link${i + 1}`;
					const panelId = `tab-panel${i + 1}`;
					const selected = currentTab === tab.toLowerCase();

					return (
						<div className="ui-page__tab-link" key={linkId}>
							<input
								type="radio"
								name="tabs"
								id={linkId}
								aria-controls={panelId}
								checked={selected}
								onClick={() => {
									if (onChange) {
										onChange(tab);
									}
								}}
							/>
							<label htmlFor={linkId}>{tab}</label>
						</div>
					);
				})}
			</div>

			{/* tab panels */}
			<div className="ui-page__tab-panels">
				{tabs.map((tab, i) => {
					const panelId = `tab-panel${i + 1}`;

					return (
						<div className="ui-page__tab-panel" id={panelId} key={panelId}>
							{renderPanel(tab)}
						</div>
					);
				})}
			</div>
		</section>
	);
}

export default Tabs;
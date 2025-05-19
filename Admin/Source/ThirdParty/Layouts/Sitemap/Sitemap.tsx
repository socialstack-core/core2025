import TreeView from 'Admin/TreeView';
import SubHeader from 'Admin/SubHeader';
import Default from 'Admin/Layouts/Default';
import { useState, useEffect } from 'react';
import pageApi, { Page, RouterTreeNodeDetail } from 'Api/Page';

export default function Sitemap(props) {
	const [ currentNode, setCurrentNode ] = useState<RouterTreeNodeDetail | null>(null);
	const [ showCloneModal, setShowCloneModal] = useState(false);
	const [ showConfirmModal, setShowConfirmModal ] = useState(false);
	
	function removePage(page : Page) {
		pageApi.delete(page.id).then(response => {
			window.location.reload();
		});
	}

	var addUrl = window.location.pathname.replace(/\/+$/g, '') + '/add';

	return (
		<Default>
			<div className="admin-page">
				<SubHeader title={`Edit Site Pages`} breadcrumbs={[
					{
						title: `Pages`
					}
				]} />
				<div className="sitemap__wrapper">
					<div className="sitemap__internal">
						{/*showCloneModal && <>
							<Modal visible onClose={() => setShowCloneModal(false)} title={`Save Page As`}>
								<p>
									<strong>{`Cloning from:`}</strong> <br />
									{getPageDescription(showCloneModal)}
								</p>
								<hr />
								<Form 
									onSuccess={(response) => {
										let clonedPage = structuredClone(showCloneModal);
										clonedPage.url = response.url;
										clonedPage.title = response.title;
										clonedPage.description = response.description;

										pageApi.create(clonedPage).then(response => {
											setShowCloneModal(false);
											reloadPages();
										});

									}}>

									<Input label={`Url`} id="sitemap__clone-url" type="text" name="url" required />
									<Input label={`Title`} id="sitemap__clone-title" type="text" name="title" />
									<Input label={`Description`} id="sitemap__clone-description" type="text" name="description" />

									<div className="sitemap__clone-modal-footer">
										<button type="button" className="btn btn-outline-danger" onClick={() => setShowCloneModal(false)}>
											{`Cancel`}
										</button>
										<input type="submit" className="btn btn-primary" value={`Save Copy`} />
									</div>
								</Form>
							</Modal>
						</>}
						{showConfirmModal && <>
							<ConfirmModal confirmCallback={() => removePage(showConfirmModal)} confirmVariant="danger" cancelCallback={() => setShowConfirmModal(false)}>
								<p>
									<strong>{`This will remove the following page:`}</strong> <br />
									{getPageDescription(showConfirmModal)}
								</p>
								<p>
									{`Are you sure you wish to do this?`}
								</p>
							</ConfirmModal>
						</>*/}
						<TreeView onLoadData={(path) => {

							return pageApi
								.getRouterTreeNodePath(path)
								.then(resp => {
									return resp;
								});

						}} />
					</div>
					{!props.noCreate && <>
						<footer className="admin-page__footer">
							<a href={addUrl} className="btn btn-primary">
								{`Create new`}
							</a>
						</footer>
					</>}
				</div>
			</div>
		</Default>
	);
}

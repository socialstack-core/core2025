import Collapsible from 'UI/Collapsible';
import Default from 'Admin/Layouts/Default';
import { useState, useEffect } from 'react';
import { useRouter } from 'UI/Router';
import ConfirmModal from 'UI/Modal/ConfirmModal';
import Modal from 'UI/Modal';
import Icon from 'UI/Icon';
import Link from 'UI/Link';
import Input from 'UI/Input';
import Form from 'UI/Form';
import Table from 'UI/Table';
import Loading from 'UI/Loading';
import pageApi, { Page, RouterTreeNodeDetail } from 'Api/Page';

export default function Sitemap(props) {
	const [ currentNode, setCurrentNode ] = useState<RouterTreeNodeDetail | null>(null);
	const [ showCloneModal, setShowCloneModal] = useState(false);
	const [ showConfirmModal, setShowConfirmModal ] = useState(false);
	const { setPage, pageState } = useRouter();

	useEffect(() => {
		// todo: query strings should originate from pageState. 
		// This is to enable SSR support on them in general.

		var urlParams = new URLSearchParams(location.search);
		var url = urlParams.get("url") || "";

		pageApi.getRouterTreeNodePath(url).then(resp => {
			setCurrentNode(resp);
		});
	}, [pageState]);
	
	function removePage(page : Page) {
		pageApi.delete(page.id).then(response => {
			window.location.reload();
		});
	}

	var addUrl = window.location.href.replace(/\/+$/g, '') + '/add';

	var renderCurrentNode = () => {
		if (!currentNode) {
			return null;
		}

		// Group up the ones with children first (the directories)
		var dirs = currentNode.children?.filter(child => child.hasChildren);
		var files = currentNode.children?.filter(child => {
			// Anything but pure intermediate nodes
			return child.type && child.type != "Group";
		});

		var currentPath = window.location.pathname;
		var baseUrl = currentPath + "?url=";

		return <table className="table">
			<tbody>
				{
					dirs && dirs.map(dir => {
						return <tr>
							<td>
								<Icon type='fa-folder' /> <Link href={baseUrl + dir.fullRoute}>{dir.childKey || (dirs!.length == 1 ? '* (anything)' : '* (anything else)')}</Link>
							</td>
						</tr>
					})
				}
				{
					files && files.map(file => {
						var fileName = file.childKey || (files!.length == 1 ? '* (anything)' : '* (anything else)');

						return <tr>
							<td>
								<h4>{fileName}</h4>
								<h6>{file.name}</h6>
							</td>
						</tr>
					})
				}
			</tbody>
		</table>

	};

	return (
		<Default>
			<div className="admin-page">
				<header className="admin-page__subheader">
					<div className="admin-page__subheader-info">
						<h1 className="admin-page__title">
							{`Edit Site Pages`}
						</h1>
						<ul className="admin-page__breadcrumbs">
							<li>
								<a href={'/en-admin/'}>
									{`Admin`}
								</a>
							</li>
							<li>
								{`Pages`}
							</li>
						</ul>
					</div>
				</header>
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
						{
							currentNode ? renderCurrentNode() : <Loading />
						}
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

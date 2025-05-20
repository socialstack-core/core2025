import { useState, useEffect } from 'react';
import { useRouter } from 'UI/Router';
import Icon from 'UI/Icon';
import Link from 'UI/Link';
import Input from 'UI/Input';
import Form from 'UI/Form';
import Table from 'UI/Table';
import Loading from 'UI/Loading';
import pageApi, { Page, RouterTreeNodeDetail } from 'Api/Page';

export default function TreeView(props) {
	const [currentNode, setCurrentNode] = useState < RouterTreeNodeDetail | null > (null);
	const [showCloneModal, setShowCloneModal] = useState(false);
	const [showConfirmModal, setShowConfirmModal] = useState(false);
	const { setPage, pageState } = useRouter();

	useEffect(() => {
		// todo: query strings should originate from pageState. 
		// This is to enable SSR support on them in general.

		var urlParams = new URLSearchParams(location.search);
		var path = urlParams.get("path") || "";

		props.onLoadData(path)
		.then(result => {
			setCurrentNode(result);
		});

	}, [pageState]);

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
		var baseUrl = currentPath + "?path=";

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
								<h4><Link href={file.editUrl}>{fileName}</Link></h4>
								<h6>{file.name}</h6>
							</td>
						</tr>
					})
				}
			</tbody>
		</table>

	};

	return currentNode ? renderCurrentNode() : <Loading />;
}

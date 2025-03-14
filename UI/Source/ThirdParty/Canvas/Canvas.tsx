import { expand, CanvasNode } from 'UI/Functions/CanvasExpand';
import Alert from 'UI/Alert';
import { useErrorBoundary, useEffect, useState } from 'react'; // useErrorBoundary is a preact function.

var uniqueKey = 1;

const loadJson = (props : CanvasProps) => {
	var content;
	
	if (props.bodyJson) {
		content = props.bodyJson;
	} else if(props.children) {
		try {
			content = JSON.parse(props.children);
		} catch (e) {
			console.log("Canvas failed to load JSON: ", props.children);
			console.error(e);
		}
	}

	if (content) {
		content = expand(content, props.onContentNode);
	}

	return content;
}

interface CanvasProps {
	/**
	 * Optional pre-parsed JSON.
	 */
	bodyJson?: any,

	/**
	 * Optional canvas content as a JSON string.
	 */
	children?: string,

	/**
	 * Optional callback which runs when a node is expanded.
	 * @param node
	 * @returns
	 */
	onContentNode?: (node : CanvasNode) => void
}

/**
 * This component renders canvas JSON. It takes canvas JSON as its child.
 */
const Canvas: React.FC<CanvasProps> = (props) => {

	const [content, setContent] = useState(() => loadJson(props));

	useEffect(() => {
		setContent(loadJson(props));
	}, [props.bodyJson, props.children]);

	const renderNodeSet = (set: CanvasNode[]) => {
		return set.map((n, i) => {
			if (n && !n.__key) {
				if (n.id) {
					n.__key = "_canvas:id_" + n.id;
				} else {
					n.__key = "_canvas_" + uniqueKey;
				}
				uniqueKey++;
			}
			return renderNode(n);
		});
	};

	const renderNode = (node: CanvasNode): React.ReactNode => {
		if (!node) {
			return null;
		}

		if (node.type == '#text') {
			return node.text;
		} else if (typeof node.type === 'string') {
			var childContent = null;

			if (node.content && node.content.length) {
				childContent = renderNodeSet(node.content);
			} else if (!node.isInline && node.type != 'br') {
				// Fake a <br> such that block elements still have some sort of height.
				//childContent = renderNode({type:'br', props: {'rte-fake': 1}});
				if (!node.props) {
					node.props = {};
				}
				var className = node.props.className ? node.props.className : "";
				if (!(className && className.length && className.includes("empty-canvas-node"))) {
					className += " empty-canvas-node";
				}
				node.props.className = className;
			}

			const NodeType = node.type as React.ElementType;

			return <NodeType key={node.__key} {...node.props}>{childContent}</NodeType>;
		} else if (node.type) {
			// Custom component
			var props = { ...node.props };
			const NodeType = node.type as React.ElementType;

			if (node.roots) {
				var children = null;

				for (var k in node.roots) {
					var root = node.roots[k];

					var isChildren = k == 'children';

					var rendered = root.content && root.content.length ? renderNodeSet(root.content) : null;

					if (isChildren) {
						children = rendered;
					} else {
						props[k] = rendered;
					}
				}

				return <ErrorCatcher node={node}>
					<NodeType key={node.__key} {...props}>{children}</NodeType>
				</ErrorCatcher>;
			} else {
				// It has no content inside it; it's purely config driven.
				// Either wrap it in a span (such that it only has exactly 1 DOM node, always), unless the module tells us it has one node anyway:
				return <ErrorCatcher node={node}>
					<NodeType key={node.__key} {...props} />
				</ErrorCatcher>;
			}
		} else if (node.content && node.content.length) {
			return renderNodeSet(node.content);
		}

		return null;
	};
	
	// Otherwise, render the (preprocessed) child nodes.
	if(!content){
		return null;
	}
	
	return renderNode(content);
}

export default Canvas;

interface ErrorCatcherProps {

	node: CanvasNode

}

const ErrorCatcher: React.FC<React.PropsWithChildren<ErrorCatcherProps>> = (({ node, children }) => {
	const [error] = useErrorBoundary((e : any) => console.warn(e));

	if (error) {
		var name = node ? node.typeName : `Unknown`;

		return <Alert variant='danger'>
			{`The component "${name}" crashed.`}
			<details>
				<summary>{`Error details`}</summary>
				{
					error.message
				}
			</details>
		</Alert>;
	}

	return <div>{children}</div>;
});

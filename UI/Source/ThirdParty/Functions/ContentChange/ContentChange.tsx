import Content from 'Api/Content';
import getEndpointType from 'UI/Functions/GetEndpointType';

interface ContentChangeOptions {
	deleted: boolean;
	updated: boolean;
	added: boolean;
	created: boolean;
}

interface ContentChangeDetail {
    deleted: boolean;
    updated: boolean;
    created: boolean;
    added: boolean;
    updatingId: int | undefined;
    change: any;
    endpoint: string;
    entity: Content;
	endpointType: string
}

/*
* Trigger a content change event for the given entity.
* Either it was edited, or is new.
*/
export default function contentChange(entity: Content, endpoint: string, changeDetail: ContentChangeOptions) {

	var detail = {} as ContentChangeDetail;

	var endpointInfo = getEndpointType(endpoint);
	
	detail.endpointType = endpointInfo.type;
	
	if(changeDetail){
		if(changeDetail.deleted){
			detail.deleted = true;
		}else if(changeDetail.updated){
			detail.updated = true;
		}else if(changeDetail.added || changeDetail.created){
			detail.created = detail.added = true;
		}
	}else{
		// Figure out if it was an update or created by default.
		if(endpointInfo.isUpdate){
			detail.updated = true;
			detail.updatingId = endpointInfo.updatingId;
		}else{
			detail.created = detail.added = true;
		}
	}
	
	detail.change = changeDetail || {updated: true};
	detail.endpoint = endpoint;
	detail.entity = entity;

	var e = new CustomEvent('contentchange', {
		detail
	});

	// Dispatch the event:
	document.dispatchEvent(e);
}
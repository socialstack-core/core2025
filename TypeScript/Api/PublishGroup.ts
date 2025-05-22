/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { PublishGroupIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {PublishGroup} (Api.PublishGroups.PublishGroup)
**/
export type PublishGroup = UserCreatedContent<uint> & {
    name?: string;
    isPublished?: boolean;
    readyForPublishing?: boolean;
    autoPublishTimeUtc?: Date | string | number;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class PublishGroupApi extends AutoController<PublishGroup,uint>{

    constructor(){
        super('/v1/publishGroup');
        this.includes = new PublishGroupIncludes();
    }

}

export default new PublishGroupApi();

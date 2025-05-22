/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ConfigurationIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Configuration} (Api.Configuration.Configuration)
**/
export type Configuration = VersionedContent<uint> & {
    name?: string;
    environments?: string;
    key?: string;
    configJson?: string;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class ConfigurationApi extends AutoController<Configuration,uint>{

    constructor(){
        super('/v1/configuration');
        this.includes = new ConfigurationIncludes();
    }

}

export default new ConfigurationApi();

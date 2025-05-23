/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiIncludes } from './Includes';
// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { PriceIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Price} (Api.Payments.Price)
**/
export type Price = VersionedContent<uint> & {
    name?: string;
    amount?: uint;
    currencyCode?: string;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class PriceApi extends AutoController<Price,uint>{

    constructor(){
        super('/v1/price');
        this.includes = new PriceIncludes();
    }

}

export default new PriceApi();

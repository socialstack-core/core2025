/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiIncludes } from './Includes';
// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { AddressIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Address} (Api.Payments.Address)
**/
export type Address = VersionedContent<uint> & {
    isDefaultDeliveryAddress?: boolean;
    isDefaultBillingAddress?: boolean;
    uprn?: ulong;
    latitude?: double;
    longitude?: double;
    line1?: string;
    line2?: string;
    line3?: string;
    city?: string;
    postcode?: string;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class AddressApi extends AutoController<Address,uint>{

    constructor(){
        super('/v1/address');
        this.includes = new AddressIncludes();
    }

}

export default new AddressApi();

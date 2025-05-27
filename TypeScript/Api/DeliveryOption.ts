/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { DeliveryOptionIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {DeliveryOption} (Api.Payments.DeliveryOption)
**/
export type DeliveryOption = VersionedContent<uint> & {
    informationJson?: string;
    addressId?: uint;
    shoppingCartId?: uint;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class DeliveryOptionApi extends AutoController<DeliveryOption,uint>{

    constructor(){
        super('/v1/deliveryoption');
        this.includes = new DeliveryOptionIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.DeliveryOptionController}::{Estimate}
     * @url 'v1/deliveryoption/estimate/cart/{shoppingCartId}'
     */
    estimate = (shoppingCartId: uint): Promise<ApiList<DeliveryOption>> => {
        return getJson<ApiList<DeliveryOption>>(this.apiUrl + '/estimate/cart/' + shoppingCartId +'')
    }

}

export default new DeliveryOptionApi();

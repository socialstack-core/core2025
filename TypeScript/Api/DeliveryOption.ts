/**
 * This file was automatically generated. DO NOT EDIT.
 */

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

}

export default new DeliveryOptionApi();

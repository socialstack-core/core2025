/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ShoppingCartIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {ShoppingCart} (Api.Payments.ShoppingCart)
**/
export type ShoppingCart = VersionedContent<uint> & {
    status?: uint;
    deliveryOptionId?: uint;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class ShoppingCartApi extends AutoController<ShoppingCart,uint>{

    constructor(){
        super('/v1/shoppingcart');
        this.includes = new ShoppingCartIncludes();
    }

}

export default new ShoppingCartApi();

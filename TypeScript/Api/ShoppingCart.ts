/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getOne, getText } from 'UI/Functions/WebRequest';

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
    addressId?: uint;
    deliveryOptionId?: uint;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}

/**
* This type was generated to reflect {CartItemChange} (Api.Payments.CartItemChange)
**/
export type CartItemChange = {
    productId?: uint;
    deltaQuantity?: int;
    quantity?: uint;
}

/**
* This type was generated to reflect {CartItemChanges} (Api.Payments.CartItemChanges)
**/
export type CartItemChanges = {
    shoppingCartId?: uint;
    items: CartItemChange[];
}
// ENTITY CONTROLLER

export class ShoppingCartApi extends AutoController<ShoppingCart,uint>{

    constructor(){
        super('/v1/shoppingcart');
        this.includes = new ShoppingCartIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.ShoppingCartController}::{ChangeItems}
     * @url 'v1/shoppingCart/change_items'
     */
    changeItems = (itemChanges: CartItemChanges, includes?: ApiIncludes[]): Promise<ShoppingCart> => {
        return getOne<ShoppingCart>(this.apiUrl + '/change_items' + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '', itemChanges)
    }

}

export default new ShoppingCartApi();

/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ProductQuantityIncludes } from 'Api/Includes';

import { Subscription } from 'Api/Subscription';

import { Purchase } from 'Api/Purchase';

import { ShoppingCart } from 'Api/ShoppingCart';

import { Product } from 'Api/Product';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {ProductQuantity} (Api.Payments.ProductQuantity)
**/
export type ProductQuantity = VersionedContent<uint> & {
    productId?: uint;
    quantity?: ulong;
    shoppingCartId?: uint;
    subscriptionId?: uint;
    purchaseId?: uint;
    // HasVirtualField() fields (5 in total)
    subscription?: Subscription;
    purchase?: Purchase;
    shoppingCart?: ShoppingCart;
    product?: Product;
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class ProductQuantityApi extends AutoController<ProductQuantity,uint>{

    constructor(){
        super('/v1/productUsage');
        this.includes = new ProductQuantityIncludes();
    }

}

export default new ProductQuantityApi();

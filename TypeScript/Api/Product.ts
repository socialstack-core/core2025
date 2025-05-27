/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { ProductIncludes } from 'Api/Includes';

import { Price } from 'Api/Price';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Product} (Api.Payments.Product)
**/
export type Product = VersionedContent<uint> & {
    sku?: string;
    name?: string;
    slug?: string;
    isBilledByUsage?: boolean;
    billingFrequency?: uint;
    minQuantity?: ulong;
    descriptionJson?: string;
    featureRef?: string;
    priceStrategy?: uint;
    priceId?: uint;
    stock?: uint;
    variantOfId?: uint;
    tierOfId?: uint;
    // HasVirtualField() fields (2 in total)
    price?: Price;
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class ProductApi extends AutoController<Product,uint>{

    constructor(){
        super('/v1/product');
        this.includes = new ProductIncludes();
    }

}

export default new ProductApi();

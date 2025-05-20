/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { PurchaseIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Purchase} (Api.Payments.Purchase)
**/
export type Purchase = VersionedContent<uint> & {
    status?: uint;
    couponId?: uint;
    authorise?: boolean;
    multiExecute?: boolean;
    localeId?: uint;
    paymentGatewayInternalId?: string;
    paymentGatewayId?: uint;
    paymentMethodId?: uint;
    currencyCode?: string;
    totalCost?: ulong;
    contentAntiDuplication?: ulong;
    contentType?: string;
    contentId?: uint;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}

/**
* This type was generated to reflect {PurchaseStatus} (Api.Payments.PurchaseStatus)
**/
export type PurchaseStatus = {
    status?: uint;
    nextAction?: string;
}
// ENTITY CONTROLLER

export class PurchaseApi extends AutoController<Purchase,uint>{

    constructor(){
        super('/v1/purchase');
        this.includes = new PurchaseIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.PurchaseController}::{Submit}
     * @url 'v1/purchase/submit'
     */
    submit = (purchaseOrder: Purchase): Promise<PurchaseStatus> => {
        return getJson<PurchaseStatus>(this.apiUrl + '/submit', purchaseOrder);
    }

}

export default new PurchaseApi();

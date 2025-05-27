/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { SubscriptionIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Subscription} (Api.Payments.Subscription)
**/
export type Subscription = VersionedContent<uint> & {
    lastChargeUtc?: Date | string | number;
    nextChargeUtc?: Date | string | number;
    willCancel?: boolean;
    timeslotFrequency?: uint;
    status?: uint;
    paymentMethodId?: uint;
    localeId?: uint;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}

/**
* This type was generated to reflect {CardUpdateStatus} (Api.Payments.CardUpdateStatus)
**/
export type CardUpdateStatus = {
    status?: uint;
}
// ENTITY CONTROLLER

export class SubscriptionApi extends AutoController<Subscription,uint>{

    constructor(){
        super('/v1/subscription');
        this.includes = new SubscriptionIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.SubscriptionController}::{UpdateCard}
     * @url 'v1/subscription/{id}/update-card'
     */
    updateCard = (id: uint, cardUpdate: Subscription): Promise<CardUpdateStatus> => {
        return getJson<CardUpdateStatus>(this.apiUrl + '/' + id +'/update-card', cardUpdate)
    }

}

export default new SubscriptionApi();

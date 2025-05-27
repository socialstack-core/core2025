/**
 * This file was automatically generated. DO NOT EDIT.
 */

// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { PaymentMethodIncludes } from 'Api/Includes';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {PaymentMethod} (Api.Payments.PaymentMethod)
**/
export type PaymentMethod = VersionedContent<uint> & {
    paymentGatewayId?: uint;
    gatewayToken?: string;
    issuer?: string;
    expiryUtc?: Date | string | number;
    lastUsedUtc?: Date | string | number;
    paymentMethodTypeId?: uint;
    name?: string;
    oneMonthExpiryNotice?: boolean;
    // HasVirtualField() fields (1 in total)
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class PaymentMethodApi extends AutoController<PaymentMethod,uint>{

    constructor(){
        super('/v1/paymentmethod');
        this.includes = new PaymentMethodIncludes();
    }

}

export default new PaymentMethodApi();

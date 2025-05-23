/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getOne, getText } from 'UI/Functions/WebRequest';

import { ApiIncludes } from './Includes';
// IMPORTS

import { Content, UserCreatedContent, VersionedContent, AutoController } from 'Api/Content';

import { CouponIncludes } from 'Api/Includes';

import { Price } from 'Api/Price';

import { User } from 'Api/User';

// TYPES

/**
* This type was generated to reflect {Coupon} (Api.Payments.Coupon)
**/
export type Coupon = VersionedContent<uint> & {
    token?: string;
    maxNumberOfPeople?: uint;
    disabled?: boolean;
    expiryDateUtc?: Date | string | number;
    subscriptionDelayDays?: uint;
    discountPercent?: uint;
    discountFixedAmount?: uint;
    freeDelivery?: boolean;
    minimumSpendAmount?: uint;
    // HasVirtualField() fields (3 in total)
    discountAmount?: Price;
    minSpendPrice?: Price;
    creatorUser?: User;
}
// ENTITY CONTROLLER

export class CouponApi extends AutoController<Coupon,uint>{

    constructor(){
        super('/v1/coupon');
        this.includes = new CouponIncludes();
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.CouponController}::{CheckCoupon}
     * @url 'v1/coupon/check/{couponcode}'
     */
    checkCoupon = (couponCode: string, includes?: ApiIncludes[]): Promise<Coupon> => {
        return getOne<Coupon>(this.apiUrl + '/check/' + couponCode +'' + (Array.isArray(includes) ? '?includes=' + includes.join(',') : '') + '');
    }

}

export default new CouponApi();

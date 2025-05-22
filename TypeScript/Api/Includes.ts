/**
 * This file was automatically generated. DO NOT EDIT.
 */

// TYPES
// INCLUDES
export class ApiIncludes {
    private text: string = '';

    constructor(existing: string = '', addition: string = ''){
        this.text = existing + (addition.length != 0 ? '.' + addition : '');
        if (this.text[0] && this.text[0] == '.'){
             this.text = this.text.substring(1, this.text.length);
        }
    }

    getText = () => this.text;
    get primaryurl() {
        return new ApiIncludes(this.getText(), 'primaryurl');
    }
    get emailaddress() {
        return new ApiIncludes(this.getText(), 'emailaddress');
    }
    get signedref128() {
        return new ApiIncludes(this.getText(), 'signedref128');
    }
    get signedref256() {
        return new ApiIncludes(this.getText(), 'signedref256');
    }
    get signedreforiginal() {
        return new ApiIncludes(this.getText(), 'signedreforiginal');
    }
    get rolepermits() {
        return new ApiIncludes(this.getText(), 'rolepermits');
    }
    get composition() {
        return new ApiIncludes(this.getText(), 'composition');
    }
    get tags() {
        return new ApiIncludes(this.getText(), 'tags');
    }
    get attributes() {
        return new ApiIncludes(this.getText(), 'attributes');
    }
    get productcategories() {
        return new ApiIncludes(this.getText(), 'productcategories');
    }
    get productquantities() {
        return new ApiIncludes(this.getText(), 'productquantities');
    }
    get tiers() {
        return new ApiIncludes(this.getText(), 'tiers');
    }
    get optionalextras() {
        return new ApiIncludes(this.getText(), 'optionalextras');
    }
    get accessories() {
        return new ApiIncludes(this.getText(), 'accessories');
    }
    get suggestions() {
        return new ApiIncludes(this.getText(), 'suggestions');
    }
    get subscriptions() {
        return new ApiIncludes(this.getText(), 'subscriptions');
    }
    get userpermits() {
        return new ApiIncludes(this.getText(), 'userpermits');
    }
    get customcontenttypefields() {
        return new ApiIncludes(this.getText(), 'customcontenttypefields');
    }
    get categories() {
        return new ApiIncludes(this.getText(), 'categories');
    }
    get productimages() {
        return new ApiIncludes(this.getText(), 'productimages');
    }
    get uploads() {
        return new ApiIncludes(this.getText(), 'uploads');
    }
}

export class TemplateIncludes extends ApiIncludes {
    get creatorUser() {
        return new TemplateIncludes(this.getText(), 'creatoruser');
    }
}

export class TagIncludes extends ApiIncludes {
    get creatorUser() {
        return new TagIncludes(this.getText(), 'creatoruser');
    }
}

export class PublishGroupIncludes extends ApiIncludes {
    get creatorUser() {
        return new PublishGroupIncludes(this.getText(), 'creatoruser');
    }
}

export class LocaleIncludes extends ApiIncludes {
    get creatorUser() {
        return new LocaleIncludes(this.getText(), 'creatoruser');
    }
}

export class TranslationIncludes extends ApiIncludes {
    get creatorUser() {
        return new TranslationIncludes(this.getText(), 'creatoruser');
    }
}

export class CouponIncludes extends ApiIncludes {
    get discountAmount() {
        return new CouponIncludes(this.getText(), 'discountamount');
    }
    get minSpendPrice() {
        return new CouponIncludes(this.getText(), 'minspendprice');
    }
    get creatorUser() {
        return new CouponIncludes(this.getText(), 'creatoruser');
    }
}

export class AddressIncludes extends ApiIncludes {
    get creatorUser() {
        return new AddressIncludes(this.getText(), 'creatoruser');
    }
}

export class DeliveryOptionIncludes extends ApiIncludes {
    get creatorUser() {
        return new DeliveryOptionIncludes(this.getText(), 'creatoruser');
    }
}

export class PaymentMethodIncludes extends ApiIncludes {
    get creatorUser() {
        return new PaymentMethodIncludes(this.getText(), 'creatoruser');
    }
}

export class PriceIncludes extends ApiIncludes {
    get creatorUser() {
        return new PriceIncludes(this.getText(), 'creatoruser');
    }
}

export class ProductAttributeIncludes extends ApiIncludes {
    get attributeGroup() {
        return new ProductAttributeIncludes(this.getText(), 'attributegroup');
    }
    get creatorUser() {
        return new ProductAttributeIncludes(this.getText(), 'creatoruser');
    }
}

export class ProductAttributeGroupIncludes extends ApiIncludes {
    get creatorUser() {
        return new ProductAttributeGroupIncludes(this.getText(), 'creatoruser');
    }
}

export class ProductAttributeValueIncludes extends ApiIncludes {
    get attribute() {
        return new ProductAttributeValueIncludes(this.getText(), 'attribute');
    }
    get creatorUser() {
        return new ProductAttributeValueIncludes(this.getText(), 'creatoruser');
    }
}

export class ProductCategoryIncludes extends ApiIncludes {
    get productCategory() {
        return new ProductCategoryIncludes(this.getText(), 'productcategory');
    }
    get creatorUser() {
        return new ProductCategoryIncludes(this.getText(), 'creatoruser');
    }
}

export class ProductIncludes extends ApiIncludes {
    get price() {
        return new ProductIncludes(this.getText(), 'price');
    }
    get creatorUser() {
        return new ProductIncludes(this.getText(), 'creatoruser');
    }
}

export class ProductQuantityIncludes extends ApiIncludes {
    get subscription() {
        return new ProductQuantityIncludes(this.getText(), 'subscription');
    }
    get purchase() {
        return new ProductQuantityIncludes(this.getText(), 'purchase');
    }
    get shoppingCart() {
        return new ProductQuantityIncludes(this.getText(), 'shoppingcart');
    }
    get product() {
        return new ProductQuantityIncludes(this.getText(), 'product');
    }
    get creatorUser() {
        return new ProductQuantityIncludes(this.getText(), 'creatoruser');
    }
}

export class ProductTemplateIncludes extends ApiIncludes {
    get creatorUser() {
        return new ProductTemplateIncludes(this.getText(), 'creatoruser');
    }
}

export class PurchaseIncludes extends ApiIncludes {
    get creatorUser() {
        return new PurchaseIncludes(this.getText(), 'creatoruser');
    }
}

export class ShoppingCartIncludes extends ApiIncludes {
    get creatorUser() {
        return new ShoppingCartIncludes(this.getText(), 'creatoruser');
    }
}

export class SubscriptionIncludes extends ApiIncludes {
    get creatorUser() {
        return new SubscriptionIncludes(this.getText(), 'creatoruser');
    }
}

export class SubscriptionUsageIncludes extends ApiIncludes {
    get creatorUser() {
        return new SubscriptionUsageIncludes(this.getText(), 'creatoruser');
    }
}

export class PasswordResetRequestIncludes extends ApiIncludes {
}

export class PageIncludes extends ApiIncludes {
    get creatorUser() {
        return new PageIncludes(this.getText(), 'creatoruser');
    }
}

export class PermalinkIncludes extends ApiIncludes {
    get creatorUser() {
        return new PermalinkIncludes(this.getText(), 'creatoruser');
    }
}

export class AdminNavMenuItemIncludes extends ApiIncludes {
}

export class NavMenuIncludes extends ApiIncludes {
    get creatorUser() {
        return new NavMenuIncludes(this.getText(), 'creatoruser');
    }
}

export class NavMenuItemIncludes extends ApiIncludes {
    get creatorUser() {
        return new NavMenuItemIncludes(this.getText(), 'creatoruser');
    }
}

export class EmailTemplateIncludes extends ApiIncludes {
    get creatorUser() {
        return new EmailTemplateIncludes(this.getText(), 'creatoruser');
    }
}

export class UserIncludes extends ApiIncludes {
    get userRole() {
        return new UserIncludes(this.getText(), 'userrole');
    }
    get creatorUser() {
        return new UserIncludes(this.getText(), 'creatoruser');
    }
}

export class CustomContentTypeIncludes extends ApiIncludes {
    get creatorUser() {
        return new CustomContentTypeIncludes(this.getText(), 'creatoruser');
    }
}

export class CustomContentTypeFieldIncludes extends ApiIncludes {
    get customContentType() {
        return new CustomContentTypeFieldIncludes(this.getText(), 'customcontenttype');
    }
    get creatorUser() {
        return new CustomContentTypeFieldIncludes(this.getText(), 'creatoruser');
    }
}

export class CustomContentTypeSelectOptionIncludes extends ApiIncludes {
    get customContentTypeField() {
        return new CustomContentTypeSelectOptionIncludes(this.getText(), 'customcontenttypefield');
    }
    get creatorUser() {
        return new CustomContentTypeSelectOptionIncludes(this.getText(), 'creatoruser');
    }
}

export class ConfigurationIncludes extends ApiIncludes {
    get creatorUser() {
        return new ConfigurationIncludes(this.getText(), 'creatoruser');
    }
}

export class ComponentGroupIncludes extends ApiIncludes {
    get role() {
        return new ComponentGroupIncludes(this.getText(), 'role');
    }
    get creatorUser() {
        return new ComponentGroupIncludes(this.getText(), 'creatoruser');
    }
}

export class DomainCertificateIncludes extends ApiIncludes {
    get creatorUser() {
        return new DomainCertificateIncludes(this.getText(), 'creatoruser');
    }
}

export class CategoryIncludes extends ApiIncludes {
    get creatorUser() {
        return new CategoryIncludes(this.getText(), 'creatoruser');
    }
}

export class UploadIncludes extends ApiIncludes {
    get creatorUser() {
        return new UploadIncludes(this.getText(), 'creatoruser');
    }
}

export class ContentFieldAccessRuleIncludes extends ApiIncludes {
    get creatorUser() {
        return new ContentFieldAccessRuleIncludes(this.getText(), 'creatoruser');
    }
}

export class RoleIncludes extends ApiIncludes {
    get creatorUser() {
        return new RoleIncludes(this.getText(), 'creatoruser');
    }
}

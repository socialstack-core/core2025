/**
 * This file was automatically generated. DO NOT EDIT.
 */

// TYPES
// INCLUDES
export class ApiIncludes {
    private text: string = '';

    constructor(existing: string = '', addition: string = ''){
        this.text = existing + (addition.length != 0 ? '.' + addition : '');
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
    get attributegroups() {
        return new ApiIncludes(this.getText(), 'attributegroups');
    }
    get childgroups() {
        return new ApiIncludes(this.getText(), 'childgroups');
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
        return new TemplateIncludes(this.getText(), 'creatorUser');
    }
}

export class TagIncludes extends ApiIncludes {
    get creatorUser() {
        return new TagIncludes(this.getText(), 'creatorUser');
    }
}

export class PublishGroupIncludes extends ApiIncludes {
    get creatorUser() {
        return new PublishGroupIncludes(this.getText(), 'creatorUser');
    }
}

export class LocaleIncludes extends ApiIncludes {
    get creatorUser() {
        return new LocaleIncludes(this.getText(), 'creatorUser');
    }
}

export class TranslationIncludes extends ApiIncludes {
    get creatorUser() {
        return new TranslationIncludes(this.getText(), 'creatorUser');
    }
}

export class CouponIncludes extends ApiIncludes {
    get discountAmount() {
        return new CouponIncludes(this.getText(), 'discountAmount');
    }
    get minSpendPrice() {
        return new CouponIncludes(this.getText(), 'minSpendPrice');
    }
    get creatorUser() {
        return new CouponIncludes(this.getText(), 'creatorUser');
    }
}

export class PaymentMethodIncludes extends ApiIncludes {
    get creatorUser() {
        return new PaymentMethodIncludes(this.getText(), 'creatorUser');
    }
}

export class PriceIncludes extends ApiIncludes {
    get creatorUser() {
        return new PriceIncludes(this.getText(), 'creatorUser');
    }
}

export class ProductAttributeIncludes extends ApiIncludes {
    get creatorUser() {
        return new ProductAttributeIncludes(this.getText(), 'creatorUser');
    }
}

export class ProductAttributeGroupIncludes extends ApiIncludes {
    get creatorUser() {
        return new ProductAttributeGroupIncludes(this.getText(), 'creatorUser');
    }
}

export class ProductAttributeValueIncludes extends ApiIncludes {
    get attribute() {
        return new ProductAttributeValueIncludes(this.getText(), 'attribute');
    }
    get creatorUser() {
        return new ProductAttributeValueIncludes(this.getText(), 'creatorUser');
    }
}

export class ProductCategoryIncludes extends ApiIncludes {
    get productCategory() {
        return new ProductCategoryIncludes(this.getText(), 'productCategory');
    }
    get creatorUser() {
        return new ProductCategoryIncludes(this.getText(), 'creatorUser');
    }
}

export class ProductIncludes extends ApiIncludes {
    get price() {
        return new ProductIncludes(this.getText(), 'price');
    }
    get creatorUser() {
        return new ProductIncludes(this.getText(), 'creatorUser');
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
        return new ProductQuantityIncludes(this.getText(), 'shoppingCart');
    }
    get product() {
        return new ProductQuantityIncludes(this.getText(), 'product');
    }
    get creatorUser() {
        return new ProductQuantityIncludes(this.getText(), 'creatorUser');
    }
}

export class ProductTemplateIncludes extends ApiIncludes {
    get creatorUser() {
        return new ProductTemplateIncludes(this.getText(), 'creatorUser');
    }
}

export class PurchaseIncludes extends ApiIncludes {
    get creatorUser() {
        return new PurchaseIncludes(this.getText(), 'creatorUser');
    }
}

export class ShoppingCartIncludes extends ApiIncludes {
    get creatorUser() {
        return new ShoppingCartIncludes(this.getText(), 'creatorUser');
    }
}

export class SubscriptionIncludes extends ApiIncludes {
    get creatorUser() {
        return new SubscriptionIncludes(this.getText(), 'creatorUser');
    }
}

export class SubscriptionUsageIncludes extends ApiIncludes {
    get creatorUser() {
        return new SubscriptionUsageIncludes(this.getText(), 'creatorUser');
    }
}

export class PasswordResetRequestIncludes extends ApiIncludes {
}

export class PageIncludes extends ApiIncludes {
    get creatorUser() {
        return new PageIncludes(this.getText(), 'creatorUser');
    }
}

export class PermalinkIncludes extends ApiIncludes {
    get creatorUser() {
        return new PermalinkIncludes(this.getText(), 'creatorUser');
    }
}

export class AdminNavMenuItemIncludes extends ApiIncludes {
}

export class NavMenuIncludes extends ApiIncludes {
    get creatorUser() {
        return new NavMenuIncludes(this.getText(), 'creatorUser');
    }
}

export class NavMenuItemIncludes extends ApiIncludes {
    get creatorUser() {
        return new NavMenuItemIncludes(this.getText(), 'creatorUser');
    }
}

export class EmailTemplateIncludes extends ApiIncludes {
    get creatorUser() {
        return new EmailTemplateIncludes(this.getText(), 'creatorUser');
    }
}

export class UserIncludes extends ApiIncludes {
    get userRole() {
        return new UserIncludes(this.getText(), 'userRole');
    }
    get creatorUser() {
        return new UserIncludes(this.getText(), 'creatorUser');
    }
}

export class CustomContentTypeIncludes extends ApiIncludes {
    get creatorUser() {
        return new CustomContentTypeIncludes(this.getText(), 'creatorUser');
    }
}

export class CustomContentTypeFieldIncludes extends ApiIncludes {
    get customContentType() {
        return new CustomContentTypeFieldIncludes(this.getText(), 'customContentType');
    }
    get creatorUser() {
        return new CustomContentTypeFieldIncludes(this.getText(), 'creatorUser');
    }
}

export class CustomContentTypeSelectOptionIncludes extends ApiIncludes {
    get customContentTypeField() {
        return new CustomContentTypeSelectOptionIncludes(this.getText(), 'customContentTypeField');
    }
    get creatorUser() {
        return new CustomContentTypeSelectOptionIncludes(this.getText(), 'creatorUser');
    }
}

export class ConfigurationIncludes extends ApiIncludes {
    get creatorUser() {
        return new ConfigurationIncludes(this.getText(), 'creatorUser');
    }
}

export class ComponentGroupIncludes extends ApiIncludes {
    get role() {
        return new ComponentGroupIncludes(this.getText(), 'role');
    }
    get creatorUser() {
        return new ComponentGroupIncludes(this.getText(), 'creatorUser');
    }
}

export class DomainCertificateIncludes extends ApiIncludes {
    get creatorUser() {
        return new DomainCertificateIncludes(this.getText(), 'creatorUser');
    }
}

export class CategoryIncludes extends ApiIncludes {
    get creatorUser() {
        return new CategoryIncludes(this.getText(), 'creatorUser');
    }
}

export class UploadIncludes extends ApiIncludes {
    get creatorUser() {
        return new UploadIncludes(this.getText(), 'creatorUser');
    }
}

export class ContentFieldAccessRuleIncludes extends ApiIncludes {
    get creatorUser() {
        return new ContentFieldAccessRuleIncludes(this.getText(), 'creatorUser');
    }
}

export class RoleIncludes extends ApiIncludes {
    get creatorUser() {
        return new RoleIncludes(this.getText(), 'creatorUser');
    }
}

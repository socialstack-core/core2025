import Default from "Admin/Templates/BaseAdminTemplate";
import {ChangeEvent, useState} from "react";
import Loop from "UI/Loop";
import ProductAttributeApi, {ProductAttribute} from "Api/ProductAttribute";
import {ListFilter} from "Api/Content";
import Modal from "UI/Modal";
import ProductAttributeValueApi, { ProductAttributeValue } from "Api/ProductAttributeValue";
import Input from "UI/Input";
import Button from "UI/Button";
import Image from "UI/Image";
import Video from "UI/Video";

const AttributeValueEditor: React.FC = () => {
    
    const [query, setQuery] = useState<string>();
    const [currentAttribute, setCurrentAttribute] = useState<ProductAttribute>();
    
    const filter: ListFilter = {
        sort:{
            direction: "desc",
            field: "id"
        },
        query: "productAttributeType != ?",
        args: [7],
        pageSize: 20 as int
    };
    
    if (query && query.length != 0)
    {
        filter.query += " and name contains ?";
        filter.args!.push(query);
    }
    
    
    return (
        <Default>
            <div className="admin-page">
                <header className="admin-page__subheader">
                    <div className="admin-page__subheader-info">
                        <h1 className="admin-page__title">
                            {`Manage product attribute values`}
                        </h1>
                        <ul className="admin-page__breadcrumbs">
                            <li>
                                <a href="/en-admin">{`Admin home`}</a>
                            </li>
                            <li>{`Product Attribute values`}</li>
                        </ul>
                    </div>
                    <div className="search admin-page__search" data-theme="search-theme">
                        <input 
                            autoComplete="false"
                            className="form-control"
                            placeholder="Search.."
                            type="text"
                            onInput={(ev: ChangeEvent<HTMLInputElement>) => {
                                setQuery(ev.target.value);
                            }}
                        />
                    </div>
                </header>
                <div className="admin-page__content">
                    <div className="admin-page__internal">
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>{`Attribute`}</th>
                                </tr>
                            </thead>
                            <tbody>
                                <Loop 
                                    over={ProductAttributeApi} 
                                    filter={filter} 
                                    paged={{
                                        pageSize: 20,
                                        showInput: true
                                    }}
                                >
                                    {(attribute: ProductAttribute) => {
                                        return (
                                            <tr 
                                                onClick={() => {
                                                    setCurrentAttribute(attribute);
                                                }} 
                                                className={'attribute-value-editor-row'}
                                            >
                                                <td>
                                                    {attribute.name}
                                                    <span className={'hover-msg'}>({`Click to edit`})</span>
                                                </td>
                                            </tr>
                                        )
                                    }}
                                </Loop>
                            </tbody>
                        </table>
                    </div>
                </div>
                {currentAttribute && (
                    <AttributeValueEditorModal attribute={currentAttribute} onClose={() => setCurrentAttribute(undefined)} />
                )}
            </div>
        </Default>
    )
}


export type AttributeValueEditorModalProps = {
    attribute: ProductAttribute;
    onClose: () => void;
}

type FileChangeEvent = {
    target: {
        value: string
    }
}

const AttributeValueEditorModal: React.FC<AttributeValueEditorModalProps> = (props: AttributeValueEditorModalProps): React.ReactElement => {

    const { attribute } = props;
    
    const [updateNo, setUpdateNo] = useState<number>(0);

    const keyDown: React.KeyboardEventHandler<HTMLInputElement> = (ev) => {
        if (ev.key === "Enter") {
            ev.preventDefault();
            ev.stopPropagation();
            // create
            const target: HTMLInputElement = ev.target as HTMLInputElement;

            ProductAttributeValueApi.create({
                value: target.value,
                productAttributeId: attribute.id
            })
                .then(() => {
                    setUpdateNo(updateNo + 1)
                })
        }
    }

    /**
     * 1=long, <Input type='number' and whatever html numeric input needs to be whole nums only
     * 2=double, <Input type='number' and whatever html numeric input needs to be..not whole nums only
     * 3=string, <Input type='text'
     * 4=image ref, <Input type='image'
     * 5=video ref, <Input type='video'
     * 6=file ref,  <Input type='file'
     * 7=boolean {none, Yes/ No values are created and readonly
     */
    const getInputType = (attrType: int) => {
        
        const getFileInput = (accept: string) => {
            return (
                <Input
                    type={'file'}
                    accept={accept}
                    key={updateNo}
                    onChange={(fileRef: FileChangeEvent) => {
                        
                        ProductAttributeValueApi.create({
                            value: fileRef.target.value,
                            productAttributeId: attribute.id
                        })
                            .then(() => {
                                setUpdateNo(updateNo + 1)
                            })
                    }}
                />
            )
        }

        switch(attrType) {
            case 1:
                return (
                    <Input type={'number'} value={''} onKeyDown={keyDown}/>
                );
            case 2:
                return (
                    <Input step={0.1} type={'number'} value={''} onKeyDown={keyDown}/>
                );
            case 3:
                return (
                    <Input type={'text'} onKeyDown={keyDown} value={''}/>
                )
            case 4:
                return getFileInput('image/*')
            case 5:
                return getFileInput('video/*')
            case 6:
                return getFileInput('*/*')
            case 7:
                return null;
            default:
                return <Input type={'text'} onKeyDown={keyDown} value={''}/>
        }
    }
    
    const isFile = [4,5,6].includes(attribute.productAttributeType as int);

    return (
        <Modal
            visible={true}
            title={`Manage values for ${attribute.name}`}
            noFooter={true}
            onClose={props.onClose}
        >
            <table className="table">
                <thead>
                    <tr>
                        <th colSpan={2}>Value</th>
                    </tr>
                </thead>
                <tbody>
                    {isFile ? (
                        <tr>
                            <td colSpan={2}>
                                {getInputType(attribute.productAttributeType!)}
                            </td>
                        </tr>
                    ) : <tr>
                            <td>{`Add value:`}</td>
                            <td>
                                {getInputType(attribute.productAttributeType!)}
                            </td>
                    </tr>}
                    <Loop
                        key={updateNo}
                        over={ProductAttributeValueApi}
                        filter={{
                            query: "productAttributeId = ?",
                            args: [attribute.id]
                        }}
                        orNone={() => <tr><td colSpan={2}>{`No values for this attribute`}</td></tr>}
                    >
                        {(value) => {
                            return (
                                <tr className={'attribute-value-value'}>
                                    {isFile ? (
                                        <td>
                                            {/* 4 = image */}
                                            {attribute.productAttributeType == 4 && <Image fileRef={value.value!}/>}


                                            {/* 5 = video */}
                                            {attribute.productAttributeType == 5 && <Video fileRef={value.value!}/>}
                                            
                                            {/* 6 = file */}
                                            {attribute.productAttributeType == 6 && <><i className={'fas fa-file'}/> {value.value!}</>}
                                        </td>
                                    ) : <td>{value.value}</td>}
                                    <td>
                                        <Button
                                            onClick={() => {
                                                ProductAttributeValueApi.delete(value.id).then(() => {
                                                    setUpdateNo(updateNo + 1)
                                                })
                                            }}
                                        >
                                            <i className={'fas fa-trash'}/> {`Remove`}
                                        </Button>
                                    </td>
                                </tr>
                            )
                        }}
                    </Loop>
                </tbody>
            </table>
        </Modal>
    )
}

export {
    AttributeValueEditorModal
}

export default AttributeValueEditor;
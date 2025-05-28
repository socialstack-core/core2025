import Default from "Admin/Templates/BaseAdminTemplate";
import { useEffect, useState } from "react";
import Loop from "UI/Loop";
import ProductAttributeApi, { ProductAttribute } from "Api/ProductAttribute";
import ProductAttributeValueApi from "Api/ProductAttributeValue";
import Input from "UI/Input";
import Button from "UI/Button";
import Image from "UI/Image";
import Video from "UI/Video";
import Container from "UI/Container";
import SubHeader from 'Admin/SubHeader';

type FileChangeEvent = {
    target: {
        value: string;
    };
};

type AttributeValueEditorProps = {
    attribute: ProductAttribute
};

const AttributeValueEditor: React.FC<AttributeValueEditorProps> = (props) => {
    const { attribute } = props;
    const [updateNo, setUpdateNo] = useState<number>(0);
    const [error, setError] = useState<string | null>(null);

    const isValidInput = (value: string): boolean => {
        if (value.trim().length === 0) {
            setError("Cannot add empty value");
            return false;
        }
        setError(null);
        return true;
    };

    const keyDown: React.KeyboardEventHandler<HTMLInputElement> = (ev) => {
        if (ev.key === "Enter") {
            ev.preventDefault();
            ev.stopPropagation();

            const target = ev.target as HTMLInputElement;
            const inputValue = target.value.trim();

            if (!isValidInput(inputValue)) return;

            ProductAttributeValueApi.create({
                value: inputValue,
                productAttributeId: attribute!.id,
            }).then(() => {
                setUpdateNo(updateNo + 1);
            });
        }
    };

    const getFileInput = (accept: string) => (
        <Input
            type="file"
            accept={accept}
            key={updateNo}
            onChange={(fileRef: FileChangeEvent) => {
                ProductAttributeValueApi.create({
                    value: fileRef.target.value,
                    productAttributeId: attribute!.id,
                }).then(() => {
                    setUpdateNo(updateNo + 1);
                });
            }}
        />
    );

    const getInputType = (attrType: number) => {
        switch (attrType) {
            case 1:
                return <Input type="number" onKeyDown={keyDown} value="" />;
            case 2:
                return <Input type="number" step={0.1} onKeyDown={keyDown} value="" />;
            case 3:
                return <Input type="text" onKeyDown={keyDown} value="" />;
            case 4:
                return getFileInput("image/*");
            case 5:
                return getFileInput("video/*");
            case 6:
                return getFileInput("*/*");
            case 7:
                return null; // Boolean types are read-only
            default:
                return <Input type="text" onKeyDown={keyDown} value="" />;
        }
    };

    const isFile = [4, 5, 6].includes(attribute.productAttributeType!);

    return (
        <Default>
            <SubHeader title={`Manage values for '${attribute.name}'`} breadcrumbs={[
                {
                    url: "/en-admin/productattribute/",
                    title: `Product Attributes`
                },
                {
                    url: "/en-admin/productattribute/" + attribute.id,
                    title: attribute.name!
                },
                {
                    title: `Values`
                }
            ]} />
            <Container>
                <table className="table">
                    <thead>
                    <tr>
                        <th colSpan={2}>Value</th>
                    </tr>
                    </thead>
                    <tbody>
                    {isFile ? (
                        <tr>
                            <td colSpan={2}>{getInputType(attribute.productAttributeType!)}</td>
                        </tr>
                    ) : (
                        <tr>
                            <td>{`Add value:`}</td>
                            <td>{getInputType(attribute.productAttributeType!)}</td>
                        </tr>
                    )}
                    {error && (
                        <tr>
                            <td colSpan={2} style={{color: "red"}}>{error}</td>
                        </tr>
                    )}
                    <Loop
                        key={updateNo}
                        over={ProductAttributeValueApi}
                        filter={{
                            query: "productAttributeId = ?",
                            args: [attribute.id],
                        }}
                        orNone={() => (
                            <tr>
                                <td colSpan={2}>{`No values for this attribute`}</td>
                            </tr>
                        )}
                    >
                        {(value) => (
                            <tr className="attribute-value-value">
                                {isFile ? (
                                    <td>
                                        {attribute.productAttributeType === 4 && (
                                            <Image fileRef={value.value!}/>
                                        )}
                                        {attribute.productAttributeType === 5 && (
                                            <Video fileRef={value.value!}/>
                                        )}
                                        {attribute.productAttributeType === 6 && (
                                            <>
                                                <i className="fas fa-file"/> {value.value!}
                                            </>
                                        )}

                                        <Button
                                            onClick={() => {
                                                ProductAttributeValueApi.delete(value.id).then(() =>
                                                    setUpdateNo(updateNo + 1)
                                                );
                                            }}
                                        >
                                            <i className="fas fa-trash"/> {`Remove`}
                                        </Button>
                                    </td>
                                ) : (
                                    <td>{value.value}</td>
                                )}
                                <td>
                                    <Button
                                        onClick={() => {
                                            ProductAttributeValueApi.delete(value.id).then(() =>
                                                setUpdateNo(updateNo + 1)
                                            );
                                        }}
                                    >
                                        <i className="fas fa-trash"/> {`Remove`}
                                    </Button>
                                </td>
                            </tr>
                        )}
                    </Loop>
                    </tbody>
                </table>
            </Container>
        </Default>
    );
};

export default AttributeValueEditor;

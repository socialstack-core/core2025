import { getTemplates, TemplateModule } from "Admin/Functions/GetPropTypes";
import Default from "Admin/Layouts/Default"
import PageApi, { Page } from "Api/Page";
import TemplateApi, { Template } from "Api/Template";
import { useEffect, useState } from "react";
import Button from "UI/Button";
import Column from "UI/Column";
import Container from "UI/Container";
import Form from "UI/Form";
import Input from "UI/Input";
import Row from "UI/Row";


const CreatePage: React.FC = (): React.ReactElement => {

    // holds file based layouts.
    const [layouts, setLayouts] = useState<TemplateModule[]>();

    // holds the selected layout from the array above
    const [chosenLayout, setChosenLayout] = useState<TemplateModule>();

    // holds the templates that use the layout
    const [templates, setTemplates] = useState<Template[]>();

    // holds the selected template from the array above
    const [chosenTemplate, setChosenTemplate] = useState<Template>();

    // fetch all templates that can be used.
    useEffect(() => {
        if (!layouts) {
            getTemplates().then((res) => {
                setLayouts(res)
            })
            .catch(err => console.error(err))
        }
    }, [layouts])

    // when a layout is chosen, fetch all templates that use the layout.
    useEffect(() => {
        if (chosenLayout) {
            TemplateApi.list({
                baseTemplate: chosenLayout?.name,
            })
            .then((templates) => {
                setTemplates(templates.results)
            })
            .catch(err => console.error(err))
        }
    }, [chosenLayout])

    return (
        <Default>
            <Container>
                <Row className='page-create'>
                    <Column size={"6"}>

                        <h3>{`Create new page`}</h3>

                        <Form
                            action={PageApi.create}
                            onSuccess={(res: Page) => {
                                // redirect to the page created.
                                window.location.href = `/en-admin/page/${res.id}`
                            }}
                            onValues={(values: Page): Page => {

                                // set the layout and template to the values.
                                values.bodyJson = chosenTemplate?.bodyJson;

                                return values;
                            }}
                        >
                            <Input
                                type={'text'}
                                name={'title'}
                                label={`Page Title`}
                            />
                            <Input
                                type={'text'}
                                name={'url'}
                                label={`Page URL`}
                            />
                            <Input
                                type={'textarea'}
                                name={'description'}
                                label={`Page Description`}
                            />
                            <Input
                                type={'select'}
                                label={`Page Layout`}
                                name={'layout'}
                                onChange={(ev) => {
                                    const layout = layouts?.find((layout) => layout.name === (ev.target as HTMLSelectElement).value);
                                    setChosenLayout(layout)
                                }}
                            >
                                <option value={""}>Select Layout</option>
                                {layouts?.map((layout) => {
                                    return (
                                        <option key={layout.name} value={layout.name}>{layout.name.includes('/') ? layout.name.split('/').pop() : layout.name}</option>
                                    )
                                })}
                            </Input>
                            {chosenLayout && (
                                <Input
                                    type={'select'}
                                    label={`Page Template`}
                                    name={'template'}
                                    onChange={(ev) => {
                                        const template = templates?.find((template) => template.id === parseInt((ev.target as HTMLSelectElement).value));

                                        setChosenTemplate(template);
                                    }}
                                >
                                    <option value={"0"}>Select Template</option>
                                    {templates?.map((template) => {
                                        return (
                                            <option key={template.id} value={template.id}>{template.title}</option>
                                        )
                                    })}
                                </Input>
                            )}
                            {
                                chosenLayout && chosenTemplate && (
                                    <Button buttonType='submit'>{`Create page`}</Button>
                                )
                            }
                        </Form>
                    </Column>
                </Row>
            </Container>
        </Default>
    )
}

export default CreatePage;
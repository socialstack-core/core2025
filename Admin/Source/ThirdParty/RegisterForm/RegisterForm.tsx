import UserApi from "Api/User"
import { useState } from "react"
import Alert from "UI/Alert"
import Form from "UI/Form"
import Input from 'UI/Input'
import Spacer from "UI/Spacer"

export type RegisterFormProps = {
    hasUsername: boolean
}

export type Policy = {
    message?: string
}

const RegisterForm: React.FC<RegisterFormProps> = (props: RegisterFormProps): React.ReactNode => {
    
    const [policy, setPolicy] = useState<Policy | null>(null);
    const { hasUsername } = props;
    const [success, setSuccess] = useState<boolean | null>();

    return (
        <Form
            action={UserApi.create}
            onSuccess={() => setSuccess(true)}
            onValues={(v) => {
                setPolicy(null)
                return v
            }}
            onFailed={(e) => {
                setPolicy(e)
            }}
            className='register-form'
        >
            <p>
                {`All fields are required`}
            </p>
            <div>
                <Input 
                    name="firstName" 
                    placeholder={`Your first name`} 
                    validate={['Required']} 
                    type={'text'}
                />
                <Input 
                    name="lastName" 
                    placeholder={`Your last name`} 
                    validate={['Required']} 
                    type={'text'}
                />
                <Input 
                    name="email" 
                    type="email" 
                    placeholder={`Email address`} 
                    validate={['Required', 'EmailAddress']} 
                />
                {hasUsername && 
                    <Input 
                        name="username" 
                        placeholder={`Username`} 
                        validate={['Required']} 
                        type={'text'}
                    />
                }
                <Input 
                    name="password" 
                    type="password" 
                    placeholder={`New Password`}
                    validate={['Required']} 
                />
            </div>
            {policy && (
                <Alert type="error">{
                    policy.message || `Unable to set your password - the request may have expired`
                }</Alert>
            )}
            {success ?
                <Alert type="success">
                    {`Account created! Please ask an existing admin to enable it for you.`}
                </Alert>
                :
                <div>
                    {`You'll need to ask to be authorised.`}
                    <Spacer height={20}/>
                    <Input type="submit" label="Create my account" />
                    {`Already got an account?`} <a href="/en-admin/login">{`Login here`}</a>
                </div>
            }
        </Form>
    )

}

export default RegisterForm;
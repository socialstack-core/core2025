import { useEffect, useState } from "react";
import Alert from "UI/Alert";
import { ApiContent } from "UI/Functions/WebRequest";
import PasswordResetRequestApi from "Api/PasswordResetRequest";
import Loading from "UI/Loading";
import { useTokens } from "UI/Token";

export type PasswordResetResponse = {
    url: string
}

const PasswordResetButton: React.FC<{}> = (): React.ReactNode => {

    const [url, setUrl] = useState<string|null>(null);
    const [loading, setLoading] = useState<boolean>(false);

    const userId: uint = parseInt(useTokens('${url.user.id}', undefined)) as uint;

    const generate = () => {
        PasswordResetRequestApi.generate(userId)
            .then((result) => {
                setUrl(
                    location.origin + 
                    result.url
                )
            })
    }

    return (
        <div className="password-reset-button">
            {loading ? (
                <Loading />
            ) : <>
                <button 
                    className="btn btn-secondary" 
                    onClick={() => generate()}
                    disabled={loading}
                >
                    Generate password reset link
                </button>
                {url && (
                    <div>
                        <Alert type="info">
                            Send this to the user - when they open it in a browser, they'll be able to set a password and login. 
                        </Alert>
                        <p>
                            {url}
                        </p>
                    </div>
                )}
                </>
            }
        </div>
    );

}

export default PasswordResetButton;
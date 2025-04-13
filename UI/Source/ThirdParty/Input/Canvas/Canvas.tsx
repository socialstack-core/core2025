import Canvas from "UI/Canvas"
import { DefaultInputType } from "UI/Input/Default";

declare global {
    interface InputPropsRegistry {
        'canvas': {
            className?: string,
            id?: string,
            onChange?: (e: React.FormEvent<HTMLInputElement>) => void,
            onBlur?: (e: React.FocusEvent) => void,
            defaultValue: {
                t: string
            }
        }
    }
}

export default Canvas;
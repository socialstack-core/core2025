import { FC, PropsWithChildren, ReactNode } from "react";

const Landing: FC<PropsWithChildren<void>> = (props: PropsWithChildren<void>): ReactNode => {
    return (
        <div id="content-root" className="body landing">
            <div className="main_container fullsize">
                <div className="landing_page fullsize">
                    <div className="landing_panel">
                        {props.children}
                    </div>
                </div>
            </div>
        </div>
    );    
}


export default Landing;
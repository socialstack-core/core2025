interface SidebarTemplateProps {
    header?: React.ReactNode,
    body?: React.ReactNode,
    sidebar?: React.ReactNode,
    footer?: React.ReactNode,
}

export default (props : SidebarTemplateProps) => {

     return (
          <div id="wrapper">
               {props.header && <header>{props.header}</header>}
               <div id="content">
                    {props.body && <main>{props.body}</main>}
                    {props.sidebar && <aside>{props.sidebar}</aside>}
               </div>
               {props.footer && <footer>{props.footer}</footer>}
          </div>
     )
};
interface SidebarTemplateProps {
    header?: React.ReactNode,
    body?: React.ReactNode,
    sidebar?: React.ReactNode,
    footer?: React.ReactNode,
    sidebarSide?: 'left' | 'right'
}

export default (props : SidebarTemplateProps) => {

     return (
          <div id="wrapper">
               {props.header && <header>{props.header}</header>}
               <div id="content">
                    {props.sidebar && props.sidebarSide == 'left' && <aside>{props.sidebar}</aside>}
                    {props.body && <main>{props.body}</main>}
                    {props.sidebar && props.sidebarSide == 'right' && <aside>{props.sidebar}</aside>}
               </div>
               {props.footer && <footer>{props.footer}</footer>}
          </div>
     )
};
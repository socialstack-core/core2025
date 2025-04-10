interface SidebarTemplateProps {
    header?: React.ReactNode,
    body?: React.ReactNode,
    sidebar?: React.ReactNode,
    footer?: React.ReactNode,
    sidebarSide?: 'left' | 'right'
}

const BaseWebTemplate: React.FC = (props : SidebarTemplateProps) => {

     return (
          <div id="wrapper">
               {props.header && <header>{props.header}</header>}
               <div id="content">
                    {props.body && <main>{props.body}</main>}
               </div>
               {props.footer && <footer>{props.footer}</footer>}
          </div>
     )
};

// BaseWebTemplate.propTypes = {}

export default BaseWebTemplate;
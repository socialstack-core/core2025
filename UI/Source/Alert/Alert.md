# UI/Alert

```tsx
import Alert from 'UI/Alert';
```

Produces a suitably-styled alert panel for the given title / message. Various themes are available:

- Primary
- Secondary
- Success
- Danger
- Warning
- Info

## Notes

- primary / secondary themes have no associated icon
- all other themes provide a suitable icon - this can be disabled by using the hideIcon parameter
- if no theme is provided, the component defaults to the "info" theme
- light / dark themes are no longer available, as it can get counterintuitive which will be rendered as the UI can adapt to the system theme
- titles by default use the <strong> tag; this can be overridden via the titleTag parameter (for instance, if an <h1>-<h6> is deemed more appropriate)
- use the dismissable parameter (no value required) to include a close button so that the user can dismiss the alert
- use the className parameter to provide additional optional class names on the alert component
- custom icon support is currently unsupported
- size options (xs, sm, md, lg, xl) are currently unsupported

# Usage Examples

## Default

```tsx
<Alert variant="success" title={`My alert`} dismissable className="my-custom-class">
	<p>
		Lorem ipsum dolor sit amet, consectetur adipisicing elit. Consequatur quis nihil delectus obcaecati unde ad quas itaque, 
		quaerat sint tempore corporis in voluptatum consequuntur earum cum aliquid facere. Et, accusantium!
	</p>
</Alert>
```
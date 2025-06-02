using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Api.Contexts;
using Api.Database;
using Api.Eventing;
using Api.Startup;
using Api.Users;


namespace Api.Contexts;

/// <summary>
/// Listens out for services starting to mount them to the context system.
/// </summary>
[EventListener]
public class EventListener
{
	
	/// <summary>
	/// Instanced automatically.
	/// </summary>
	public EventListener()
	{
		// Load all the props now.
		var properties = typeof(Context).GetProperties(BindingFlags.Public | BindingFlags.Instance);
		var fields = typeof(Context).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

		var fieldMapping = new Dictionary<string, FieldInfo>();

		foreach (var field in fields)
		{
			fieldMapping[field.Name.ToLower()] = field;
		}
		
		var propertyMapping = new Dictionary<string, PropertyInfo>();

		foreach (var property in properties)
		{
			propertyMapping[property.Name.ToLower()] = property;
		}

		var defaultValueChecker = new Context();
		
		Events.Service.AfterCreate.AddEventListener((Context ctx, AutoService svc) => {
			if (svc == null || svc.ServicedType == null)
			{
				return new ValueTask<AutoService>(svc);
			}

			// Does this service manage a context field?
			// * 'fields' will contain a field of this type whose name starts with an underscore.
			//   The name of that field plus Id must also exist and be of the service's ID type.
			FieldInfo privateValueField = null;

			foreach (var field in fields)
			{
				if (field.FieldType == svc.ServicedType)
				{
					if (field.Name.StartsWith('_'))
					{
						privateValueField = field;
						break;
					}
				}
			}

			if (privateValueField == null)
			{
				return new ValueTask<AutoService>(svc);
			}

			// Must be a ctx field of the types ID type (assuming uint for now).
			var privateIdFieldName = privateValueField.Name.ToLower() + "id"; // _userid

			if (!fieldMapping.TryGetValue(privateIdFieldName, out FieldInfo privateIdField))
			{
				throw new Exception(
					"A field of type '" + svc.ServicedType.Name + "' was present as a Context extension however a " +
					"field called '" + privateValueField.Name + "Id' was expected but does not exist. " +
					"This field is required to hold the ID value from the context token itself and must be of type '" + svc.IdType.Name + "'");	
			}

			if (privateIdField.FieldType != svc.IdType)
			{
				throw new Exception(
					"A field of type '" + svc.ServicedType.Name + "' was present as a Context extension and its ID field " +
					"called '" + privateValueField.Name + "Id' was found but it was an incorrect ID type. The type '" + svc.ServicedType.Name + "' expects " +
					"'" + svc.IdType.Name + "' to be the type used for its IDs.");
			}

			// Get the public ID property:
			if (!propertyMapping.TryGetValue(privateIdFieldName.Substring(1), out PropertyInfo publicIdField))
			{
				var niceName = privateValueField.Name.Substring(1) + "Id";

				throw new Exception(
					"A property of type '" + svc.ServicedType.Name + "' was present as a Context extension " +
					"but it is missing a public property called '" + niceName + "'. This property needs to be of type " + svc.IdType.Name + ".");
			}

			if (publicIdField.PropertyType != svc.IdType)
			{
				var niceName = privateValueField.Name.Substring(1) + "Id";

				throw new Exception(
					"A property of type '" + svc.ServicedType.Name + "' was present as a Context extension " +
					"and it has a public property called '" + niceName + "' however this property needs to be of type " + svc.IdType.Name + ".");
			}

			if (svc.IdType != typeof(uint))
			{
				throw new Exception("The context system doesn't support non-uint content types just yet.");
			}

			var lcName = publicIdField.Name.ToLower();

			var getMethod = publicIdField.GetGetMethod();
			var defaultValue = (uint)getMethod.Invoke(defaultValueChecker, System.Array.Empty<object>());

			// E.g. UserId, LocaleId to User
			var contentName = publicIdField.Name[0..^2];

			var fld = new ContextFieldInfo()
			{
				ContentName = contentName,
				PrivateFieldInfo = privateIdField,
				Property = publicIdField,
				Name = publicIdField.Name,
				DefaultValue = defaultValue,
				SkipOutput = lcName == "roleid",
				Service = svc,
				ContentType = svc.ServicedType,
				ViewCapability = svc.GetEventGroup().GetLoadCapability()
			};

			var shortCodeAttrib = publicIdField.GetCustomAttribute<ContextShortcodeAttribute>();
			var shortcode = shortCodeAttrib == null ? lcName[0] : shortCodeAttrib.Shortcode;

			var shortIndex = shortcode - 'A';

			if (shortIndex < 0 || shortIndex >= 64)
			{
				throw new Exception("Can't use " + shortcode + " as a context field shortcode - it must be A-Z or a-z.");
			}

			fld.Shortcode = shortcode;

			if (ContextFields.FieldsByShortcode[shortIndex] != null)
			{
				throw new Exception(
					"Context property '" + publicIdField.Name + "' can't use context field short name '" + shortcode +
					"' because it's in use. Specify one to use with [ContextShortcode('...')] on the public property '" + publicIdField.Name + "' to avoid this collision.");
			}

			ContextFields.FieldsByShortcode[shortIndex] = fld;

			var jsonHeader = "\"" + fld.JsonFieldName + "\":";

			fld.JsonFieldHeader = Encoding.UTF8.GetBytes(jsonHeader);

			ContextFields.Fields[lcName] = fld;
			ContextFields.FieldList.Add(fld);

			return new ValueTask<AutoService>(svc);
		});

	}
	
}
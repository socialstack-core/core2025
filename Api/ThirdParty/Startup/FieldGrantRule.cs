using Api.Contexts;
using Api.Database;
using Api.Startup;
using System;

namespace Api.Permissions;


/// <summary>
/// A field grant rule (either read or write, not both) for a particular field.
/// </summary>
public class FieldGrantRule<T, ID>
	where T : Content<ID>, new()
	where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
{	
	/// <summary>
	/// The field this rule is for.
	/// </summary>
	public JsonField<T, ID> Field;
	
	/// <summary>
	/// True if it is to be inherited from the code. Check this first.
	/// </summary>
	public bool Inherit;
	
	/// <summary>
	/// A full filter to check.  Check if this is not null.
	/// </summary>
	public Filter<T, ID> Filter;
	
	/// <summary>
	/// If inherit is false and filter is null, use this.
	/// </summary>
	public bool ConstGrant;
	

	/// <summary>
	/// Creates a new field grant rule for the given grant text.
	/// </summary>
	/// <param name="field"></param>
	/// <param name="text"></param>
	public FieldGrantRule(JsonField<T, ID> field, string text)
	{
		Field = field;

		if (string.IsNullOrEmpty(text))
		{
			// Inherit.
			Inherit = true;
		}
		else if (text == "true")
		{
			ConstGrant = true;
		}
		else if (text == "false")
		{
			ConstGrant = false;
		}
		else
		{
			// will be a full filter obj
			Filter = field.Structure.Service.GetFilterFor(text);
		}
	}

	/// <summary>
	/// True if writing is granted.
	/// </summary>
	public bool IsWriteGranted(Context context, T obj)
	{
		if(Inherit)
		{
			return Field.Writeable;
		}
		else if(Filter != null)
		{
			return Filter.Match(context, obj, false);
		}
		
		return ConstGrant;
	}
	
	/// <summary>
	/// True if reading is granted.
	/// </summary>
	public bool IsReadGranted(Context context, T obj, bool isIncluded = false)
	{
		if(Inherit)
		{
			return Field.Readable;
		}
		else if(Filter != null)
		{
			return Filter.Match(context, obj, isIncluded);
		}
		
		return ConstGrant;
	}
	
}
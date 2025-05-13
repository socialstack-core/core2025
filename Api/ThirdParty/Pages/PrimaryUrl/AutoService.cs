using Api.Contexts;
using Api.Pages;
using System;
using System.Threading.Tasks;


public partial class AutoService<T, ID> {

	/// <summary>
	/// The primary URL lookup for this service, which can be null. Use GetPrimaryUrl instead.
	/// </summary>
	public PrimaryUrlLookup<T, ID> UIPrimaryUrlLookup;

	/// <summary>
	/// Gets the primary URL (for the UI group) for the given object from this service.
	/// The primary URL is defined as: 
	/// The latest permalink for a page with a key of either 
	/// "primary:lowercaseContentTypeName:ID" e.g. "primary:user:42"
	/// or, if that does not exist, then
	/// "primary:lowercaseContentTypeName" e.g. "primary:user" 
	/// </summary>
	public virtual string GetPrimaryUrl(Context context, T content)
	{
		if (UIPrimaryUrlLookup == null)
		{
			return null;
		}

		return UIPrimaryUrlLookup.GetUrl(content);
	}

	/// <summary>
	/// Creates a new strongly typed primary URL lookup.
	/// </summary>
	/// <returns></returns>
	public override PrimaryUrlLookup CreatePrimaryUrlLookup()
	{
		return new PrimaryUrlLookup<T, ID>(this);
	}
}


public partial class AutoService {

	/// <summary>
	/// Creates a new strongly typed primary URL lookup.
	/// </summary>
	/// <returns></returns>
	public virtual PrimaryUrlLookup CreatePrimaryUrlLookup()
	{
		throw new NotImplementedException();
	}
}
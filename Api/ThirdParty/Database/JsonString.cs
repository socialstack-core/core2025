namespace Api.Database;

/// <summary>
/// Exists as a type alias for textual JSON fields.
/// </summary>
public struct JsonString
{
	/// <summary>
	/// The raw value.
	/// </summary>
	private string _value;

	/// <summary>
	/// Creates a new json string.
	/// </summary>
	/// <param name="val"></param>
	public JsonString(string val)
	{
		if (string.IsNullOrEmpty(val))
		{
			_value = null;
		}
		else
		{
			_value = val;
		}
	}

	/// <summary>
	/// The JSON string - can be null.
	/// </summary>
	/// <returns></returns>
	public string ValueOf()
	{
		return _value;
	}

	/// <summary>
	/// The JSON string itself.
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return _value;
	}
}
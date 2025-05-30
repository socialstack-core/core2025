using Api.Contexts;
using Api.Database;
using System.Collections.Generic;

namespace Api.Translate;


/// <summary>
/// Use this to declare a field of the specified type as localised.
/// The type is often string but you can localise anything else - IDs etc.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Localized<T>
{
	/// <summary>
	/// The underlying locale code to value lookup.
	/// </summary>
	private readonly Dictionary<string, T> _values = new();

	/// <summary>
	/// Set a specific localised value.
	/// </summary>
	/// <param name="locale"></param>
	/// <param name="value"></param>
	public void Set(Locale locale, T value)
	{
		_values[locale.Code] = value;
	}

	/// <summary>
	/// Gets the value for the current locale.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="fallback"></param>
	/// <returns></returns>
	public T Get(Context context, bool fallback = true)
	{
		var localeId = context.LocaleId;
		var locale = (ContentTypes.Locales != null && localeId <= ContentTypes.Locales.Length ?
			ContentTypes.Locales[localeId - 1] : null);

		T val;

		if (locale != null)
		{
			if (_values.TryGetValue(locale.Code, out val))
			{
				return val;
			}

			if (locale.Id == 1)
			{
				// It's the primary locale. Don't fallback here.
				return default;
			}
		}

		if (!fallback)
		{
			return default;
		}

		if (_values.TryGetValue("en", out val))
		{
			return val;
		}

		return default;
	}

	/// <summary>
	/// Only used by serialisers. Sets the underlying value whilst loading this type.
	/// Always use Set instead.
	/// </summary>
	/// <param name="localeCode"></param>
	/// <param name="value"></param>
	public void SetInternalUseOnly(string localeCode, T value)
	{
		_values[localeCode] = value;
	}

	/// <summary>
	/// Get a specific localised value.
	/// </summary>
	/// <param name="locale"></param>
	/// <param name="fallback"></param>
	/// <returns></returns>
	public T Get(Locale locale, bool fallback = true)
	{
		T val;

		if (locale != null)
		{
			if (_values.TryGetValue(locale.Code, out val))
			{
				return val;
			}

			if (locale.Id == 1)
			{
				// It's the primary locale. Don't fallback here.
				return default;
			}
		}

		if (!fallback)
		{
			return default;
		}

		if (_values.TryGetValue("en", out val))
		{
			return val;
		}

		return default;
	}

	/// <summary>
	/// Iterate through all the localised values in this set.
	/// </summary>
	public IReadOnlyDictionary<string, T> Values => _values;
}
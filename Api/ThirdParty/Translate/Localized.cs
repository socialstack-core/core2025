using Api.Contexts;
using Api.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Api.Translate;


/// <summary>
/// Use this to declare a field of the specified type as localised.
/// The type is often string but you can localise anything else - IDs etc.
/// </summary>
/// <typeparam name="T"></typeparam>
public struct Localized<T>
{
	/// <summary>
	/// Parses the given JSON as a Localized struct, then boxes it. 
	/// The result is never null but the source text can be.
	/// </summary>
	/// <param name="src"></param>
	/// <returns></returns>
	public static object ParseBoxed(string src)
	{
		if (string.IsNullOrEmpty(src))
		{
			return new Localized<T>();
		}

		return Parse(src);
	}

	/// <summary>
	/// Parses the given JSON as a Localized struct. 
	/// The source text can be null.
	/// </summary>
	/// <param name="src"></param>
	/// <returns></returns>
	public static Localized<T> Parse(string src)
	{
		var result = new Localized<T>();
		if (string.IsNullOrWhiteSpace(src))
			return result;

		ReadOnlySpan<char> span = src.AsSpan();
		int i = 0;

		SkipWhitespace(span, ref i);
		if (i >= span.Length || span[i++] != '{')
			return result;

		while (i < span.Length)
		{
			SkipWhitespace(span, ref i);
			if (span[i] == '}') break;

			// Parse key
			if (span[i++] != '"') break;
			int keyStart = i;
			while (i < span.Length && span[i] != '"') i++;
			if (i >= span.Length) break;
			var key = span.Slice(keyStart, i - keyStart).ToString();
			i++; // skip closing quote

			SkipWhitespace(span, ref i);
			if (span[i++] != ':') break;
			SkipWhitespace(span, ref i);

			T value;

			if (typeof(T) == typeof(string))
			{
				if (span[i++] != '"') break;
				int valStart = i;
				while (i < span.Length && span[i] != '"') i++;
				if (i >= span.Length) break;
				value = (T)(object)(span.Slice(valStart, i - valStart).ToString());
				i++; // skip closing quote
			}
			else
			{
				int valStart = i;
				while (i < span.Length && span[i] != ',' && span[i] != '}') i++;
				var valSpan = span.Slice(valStart, i - valStart).Trim();

				if (!TryConvertNumber(valSpan, out value))
					break;
			}

			result.SetInternalUseOnly(key, value);

			SkipWhitespace(span, ref i);
			if (i < span.Length && span[i] == ',')
			{
				i++;
				continue;
			}
			else if (i < span.Length && span[i] == '}')
			{
				break;
			}
		}

		return result;
	}

	private static void SkipWhitespace(ReadOnlySpan<char> s, ref int i)
	{
		while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
	}

	private static bool TryConvertNumber(ReadOnlySpan<char> span, out T value)
	{
		object parsed = null;
		bool success = false;

		if (typeof(T) == typeof(int))
		{
			success = int.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v);
			parsed = v;
		}
		else if (typeof(T) == typeof(uint))
		{
			success = uint.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v);
			parsed = v;
		}
		else if (typeof(T) == typeof(ulong))
		{
			success = ulong.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v);
			parsed = v;
		}
		else if (typeof(T) == typeof(ulong?))
		{
			if (span.SequenceEqual("null".AsSpan()))
			{
				value = (T)(object)null;
				return true;
			}
			success = ulong.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v);
			parsed = (ulong?)v;
		}
		else if (typeof(T) == typeof(float))
		{
			success = float.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out var v);
			parsed = v;
		}
		else if (typeof(T) == typeof(float?))
		{
			if (span.SequenceEqual("null".AsSpan()))
			{
				value = (T)(object)null;
				return true;
			}
			success = float.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out var v);
			parsed = (float?)v;
		}
		else if (typeof(T) == typeof(double))
		{
			success = double.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out var v);
			parsed = v;
		}
		else if (typeof(T) == typeof(double?))
		{
			if (span.SequenceEqual("null".AsSpan()))
			{
				value = (T)(object)null;
				return true;
			}
			success = double.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out var v);
			parsed = (double?)v;
		}
		else if (typeof(T) == typeof(decimal))
		{
			success = decimal.TryParse(span, NumberStyles.Number, CultureInfo.InvariantCulture, out var v);
			parsed = v;
		}
		else if (typeof(T) == typeof(decimal?))
		{
			if (span.SequenceEqual("null".AsSpan()))
			{
				value = (T)(object)null;
				return true;
			}
			success = decimal.TryParse(span, NumberStyles.Number, CultureInfo.InvariantCulture, out var v);
			parsed = (decimal?)v;
		}
		else
		{
			throw new NotSupportedException($"Type {typeof(T)} is not supported in Localized<T>.Parse.");
		}

		value = success ? (T)parsed : default;
		return success;
	}

	/// <summary>
	/// The underlying locale code to value lookup.
	/// </summary>
	private Dictionary<string, T> _values;

	/// <summary>
	/// Set a specific localised value.
	/// </summary>
	/// <param name="locale"></param>
	/// <param name="value"></param>
	public void Set(Locale locale, T value)
	{
		if (_values == null)
		{
			_values = new();
		}

		_values[locale.Code] = value;
	}

	/// <summary>
	/// Set a specific localised value.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="value"></param>
	public void Set(Context context, T value)
	{
		Set(context.LocaleId, value);
	}
	
	/// <summary>
	/// Set the value for "en".
	/// </summary>
	/// <param name="value"></param>
	public void SetFallback(T value)
	{
		if (_values == null)
		{
			_values = new();
		}
		
		_values["en"] = value;
	}

	/// <summary>
	/// Set a specific localised value.
	/// </summary>
	/// <param name="localeId"></param>
	/// <param name="value"></param>
	public void Set(uint localeId, T value)
	{
		var locale = (ContentTypes.Locales != null && localeId <= ContentTypes.Locales.Length ?
			ContentTypes.Locales[localeId - 1] : null);

		if (locale == null)
		{
			throw new System.Exception("Locale #" + localeId + " does not exist.");
		}

		if (_values == null)
		{
			_values = new();
		}

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
		return Get(locale, fallback);
	}

	/// <summary>
	/// Gets the value for the specified locale.
	/// </summary>
	/// <param name="localeId"></param>
	/// <param name="fallback"></param>
	/// <returns></returns>
	public T Get(uint localeId, bool fallback = true)
	{
		var locale = (ContentTypes.Locales != null && localeId <= ContentTypes.Locales.Length ?
			ContentTypes.Locales[localeId - 1] : null);
		return Get(locale, fallback);
	}

	/// <summary>
	/// Only used by serialisers. Sets the underlying value whilst loading this type.
	/// Always use Set instead.
	/// </summary>
	/// <param name="localeCode"></param>
	/// <param name="value"></param>
	public void SetInternalUseOnly(string localeCode, T value)
	{
		if (_values == null)
		{
			_values = new();
		}

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
		if (_values == null)
		{
			return default;
		}
		
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
	/// Gets the fallback locale value, or the default value otherwise.
	/// </summary>
	/// <returns></returns>
	public T GetFallback()
	{
		if (_values == null)
		{
			return default;
		}
		
		if (_values.TryGetValue("en", out T val))
		{
			return val;
		}

		return default;
	}

	/// <summary>
	/// Iterate through all the localised values in this set.
	/// </summary>
	public IReadOnlyDictionary<string, T> Values => _values;

	/// <summary>
	/// Creates a new localized instance.
	/// </summary>
	public Localized()
	{
	}

	/// <summary>
	/// Creates a new localized instance with a specific default value for the default locale.
	/// </summary>
	public Localized(T en)
	{
		_values = new();
		_values["en"] = en;
	}

	/// <summary>
	/// Creates a new localized instance with a specific default value for the given locale.
	/// </summary>
	public Localized(Locale initialLocale, T val)
	{
		_values = new();
		_values[initialLocale.Code] = val;
	}

	/// <summary>
	/// Builds this as a JSON string.
	/// </summary>
	/// <returns></returns>
	public string ToJson()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append('{');

		if (_values != null)
		{
			bool first = true;
			foreach (var kvp in _values)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					sb.Append(',');
				}

				sb.Append('"');
				sb.Append(kvp.Key);
				sb.Append("\":");
				AppendValue(sb, kvp.Value);
			}
		}

		sb.Append('}');
		return sb.ToString();
	}

	private static void AppendValue(StringBuilder sb, T value)
	{
		if (value == null)
		{
			sb.Append("null");
			return;
		}

		Type type = typeof(T);
		if (type == typeof(string))
		{
			sb.Append('"');
			sb.Append(EscapeString(value.ToString()));
			sb.Append('"');
		}
		else
		{
			// Format using invariant culture to preserve dot-decimal format, etc.
			sb.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
		}
	}

	private static string EscapeString(string s)
	{
		if (string.IsNullOrEmpty(s))
			return s;

		StringBuilder sb = new StringBuilder();
		foreach (char c in s)
		{
			switch (c)
			{
				case '\\': sb.Append("\\\\"); break;
				case '"': sb.Append("\\\""); break;
				case '\b': sb.Append("\\b"); break;
				case '\f': sb.Append("\\f"); break;
				case '\n': sb.Append("\\n"); break;
				case '\r': sb.Append("\\r"); break;
				case '\t': sb.Append("\\t"); break;
				default:
					if (char.IsControl(c))
						sb.Append("\\u").Append(((int)c).ToString("x4"));
					else
						sb.Append(c);
					break;
			}
		}
		return sb.ToString();
	}

	/// <summary>
	/// Builds this as a JSON string.
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return ToJson();
	}
}
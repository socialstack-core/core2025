using System;
using Api.Database;
using Api.Translate;
using Api.Users;


namespace Api.Payments;

/// <summary>
/// An Address
/// </summary>
public partial class Address : VersionedContent<uint>
{
	/// <summary>
	/// Resolved UPRN of this address, if known.
	/// </summary>
	public ulong? Uprn;
	
	/// <summary>
	/// Resolved latitude of this address, if known.
	/// </summary>
	public double? Latitude;
	
	/// <summary>
	/// Resolved longitude of this address, if known.
	/// </summary>
	public double? Longitude;
	
	/// <summary>
	/// Address line 2, if present.
	/// </summary>
	public string Line1;
	
	/// <summary>
	/// Address line 2, if present.
	/// </summary>
	public string Line2;
	
	/// <summary>
	/// Address line 3, if present.
	/// </summary>
	public string Line3;
	
	/// <summary>
	/// Address city.
	/// </summary>
	public string City;
	
	/// <summary>
	/// Address postcode.
	/// </summary>
	public string Postcode;
	
}
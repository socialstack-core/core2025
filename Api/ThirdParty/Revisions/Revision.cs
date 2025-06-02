using Api.AutoForms;
using Api.Database;
using Api.Users;
using System;

namespace Api.Revisions;


/// <summary>
/// A specific version of a piece of content.
/// </summary>
public partial class Revision<T, ID> : UserCreatedContent<ID>
	where T : Content<ID>, new()
	where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
{

	/// <summary>
	/// The underlying content.
	/// </summary>
	public string ContentJson;

	/// <summary>
	/// The ID of the content.
	/// </summary>
	public ID ContentId;

	/// <summary>
	/// There is only 1 draft of a given piece of content at a time.
	/// </summary>
	public bool IsDraft;

	/// <summary>
	/// If populated then auto publish this content on the required date 
	/// </summary>
	public DateTime? PublishDraftDate;

	/// <summary>
	/// 1 = Create, 2 = Update, 3 = Delete.
	/// </summary>
	public uint ActionType;

}
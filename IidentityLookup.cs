namespace Fatigue_Calculator_Desktop
{
	/// <summary>
	/// interface used to handle the various identity matching models available
	/// </summary>
	public interface IidentityLookup
	{
		bool displayPage { get; }

		IIdentityService source { get; set; }

		int getMatchCount(string value);

		string getBestMatch(string value);

		identity validate(string value);
	}
}
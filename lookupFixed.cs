namespace Fatigue_Calculator_Desktop
{
	internal class lookupFixed : IidentityLookup
	{
		private identity _fixed;

		public lookupFixed()
		{
			_fixed = new identity(Config.ConfigSettings.settings.fixedId);
		}

		IIdentityService IidentityLookup.source
		{
			get
			{
				return null;
			}
			set
			{
				// do nothing
				// we don't use a source
			}
		}

		int IidentityLookup.getMatchCount(string value)
		{
			return 1;
		}

		string IidentityLookup.getBestMatch(string value)
		{
			return _fixed.Name;
		}

		identity IidentityLookup.validate(string value)
		{
			return new identity(_fixed.Name);
		}

		public bool displayPage
		{
			get { return false; }
		}
	}
}
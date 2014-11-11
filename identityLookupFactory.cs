namespace Fatigue_Calculator_Desktop
{
	internal class identityLookupFactory
	{
		public IidentityLookup getLookup()
		{
			// depends on the compile-time constant and settings
#if (Multiuser || Unprotected)
            // for the moment we can only deal with identity files. this will change with Fatigue Manager
            IIdentityService idFile = new identityFile();
            string filename = Config.ConfigSettings.settings.IDLookupFile;
            idFile.SetIdentityListSource(filename);

            string setting = Config.ConfigSettings.settings.IDLookupType;
            switch (setting.ToLower())
            {
                case "dynamic":
                case "open":
                    {
                        return new lookupOpen(idFile);
                    }
                case "fixed":
                case "single":
                    {
                        return new lookupFixed();
                    }
                case "closed":
                    {
                        return new lookupClosed(idFile);
                    }
                default:
                    {
                        // assume 'none' for all invalid cases
                        return new lookupNone();
                    }
            }
#endif

#if Singleuser
            return new lookupFixed();
#endif

#if Server
            // for the moment we can only deal with identity files. this will change with Fatigue Manager
            IIdentityService idFile = new identityFile();
            string filename = Config.ConfigSettings.settings.IDLookupFile;
            idFile.SetIdentityListSource(filename);

            string setting = Config.ConfigSettings.settings.IDLookupType;
            switch (setting.ToLower())
            {
                case "dynamic":
                case "open":
                    {
                        return new lookupOpen(idFile);
                    }
                case "fixed":
                case "single":
                    {
                        return new lookupFixed();
                    }
                case "closed":
                    {
                        return new lookupClosed(idFile);
                    }
                default:
                    {
                        // assume 'none' for all invalid cases
                        return new lookupNone();
                    }
            }
#endif

#if Kiosk
            // for the moment we can only deal with identity files. this will change with Fatigue Manager
            IIdentityService idFile = new identityFile();
            string filename = Config.ConfigSettings.settings.IDLookupFile;
            idFile.SetIdentityListSource(filename);

            string setting = Config.ConfigSettings.settings.IDLookupType;
            switch (setting.ToLower())
            {
                case "dynamic":
                case "open":
                    {
                        return new lookupOpen(idFile);
                    }
                case "fixed":
                case "single":
                    {
                        return new lookupFixed();
                    }
                case "closed":
                    {
                        return new lookupClosed(idFile);
                    }
                default:
                    {
                        // assume 'none' for all invalid cases
                        return new lookupNone();
                    }
            }
#endif

#if DEBUG
			// for the moment we can only deal with identity files. this will change with Fatigue Manager
			IIdentityService idFile = new identityFile();
			string filename = Config.ConfigSettings.settings.IDLookupFile;
			idFile.SetIdentityListSource(filename);

			string setting = Config.ConfigSettings.settings.IDLookupType;
			switch (setting.ToLower())
			{
				case "open":
					{
						return new lookupOpen(idFile);
					}
				case "fixed":
					{
						return new lookupFixed();
					}
				case "closed":
					{
						return new lookupClosed(idFile);
					}
				default:
					{
						// assume 'none' for all invalid cases
						return new lookupNone();
					}
			}
#endif
		}
	}
}
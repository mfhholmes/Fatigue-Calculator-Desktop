using System;
namespace Fatigue_Calculator_Desktop
{
    public interface ICalculatorSettings
    {
        bool ChangeSetting(string Key, string Value);
        string GetSetting(string Key);
        System.Collections.Generic.Dictionary<string, string> GetSettings();
        string lastValidationError { get; }
        string settingDescription(string Key);
        bool validateSetting(string Key, string Value);
    }
}

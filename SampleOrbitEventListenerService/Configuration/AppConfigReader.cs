using System;
using System.Configuration;

namespace SampleOrbitEventListenerService.Configuration
{
    class AppConfigReader : IConfigReader
    {
        public string ReadSetting(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}
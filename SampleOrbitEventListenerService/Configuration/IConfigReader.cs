using System;

namespace SampleOrbitEventListenerService.Configuration
{
    interface IConfigReader
    {
        string ReadSetting(string name);
    }
}
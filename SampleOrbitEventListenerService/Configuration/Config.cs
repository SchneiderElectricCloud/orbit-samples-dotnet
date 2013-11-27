using System;
using System.ComponentModel;
using System.Diagnostics;

namespace SampleOrbitEventListenerService.Configuration
{
    class Config
    {
        readonly IConfigReader _reader;

        public static Config Global { get; private set; }

        static Config()
        {
            Global = new Config(new AppConfigReader());
        }

        public Config(IConfigReader reader)
        {
            _reader = reader ?? new AppConfigReader();
        }

        public string ServiceBusConnectionString
        {
            get { return ReadConfigSetting<string>("Microsoft.ServiceBus.ConnectionString"); }
        }

        public string OrbitEventsTopicName
        {
            get { return ReadConfigSetting<string>("Orbit.Events.TopicName"); }
        }

        public string OrbitEventsSubscriptionName
        {
            get { return ReadConfigSetting<string>("Orbit.Events.SubscriptionName"); }
        }

        public Uri TaskServicesUrl
        {
            get
            {
                string setting = ReadConfigSetting<string>("TaskServicesUrl");
                Uri url = new Uri(setting);
                return url;
            }
        }

        public string ApiKey
        {
            get { return ReadConfigSetting<string>("ApiKey"); }
        }

        //public string SourceTaskType
        //{
        //    get { return ReadConfigSetting<string>("Source.TaskType"); }
        //}

        //public string TargetTaskType
        //{
        //    get { return ReadConfigSetting<string>("Target.TaskType"); }
        //}

        public string AssignToUpn
        {
            get { return ReadConfigSetting<string>("Target.AssignToUpn"); }
        }

        T ReadConfigSetting<T>(string name, T defaultValue = default(T))
        {
            T setting;
            string value = _reader.ReadSetting(name);
            if (string.IsNullOrEmpty(value))
            {
                setting = defaultValue;
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(typeof (T));
                Debug.Assert(converter != null, string.Format(
                    "Cannot find converter to read setting '{0}' with type '{1}'",
                    name, typeof(T).Name));
                setting = (T) converter.ConvertFromString(value);
            }
            return setting;
        }
    }
}
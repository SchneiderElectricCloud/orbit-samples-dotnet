using System;
using System.Collections.Generic;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.Extensions
{
    static class TaskResourceExtensions
    {
        public static TaskResource CopyLocationFrom(this TaskResource task, TaskResource source)
        {
            if (source.Location != null)
            {
                task.Location = new LocationResource
                {
                    ObjectID = source.Location.ObjectID,
                    GlobalID = source.Location.GlobalID,
                };

                if (source.Location.Gps != null)
                {
                    task.Location.Gps = new GpsResource
                    {
                        Lat = source.Location.Gps.Lat,
                        Lng = source.Location.Gps.Lng
                    };
                }
                else
                {
                    task.Location.Gps = null;
                }
            }
            else
            {
                task.Location = null;
            }
            return task;
        }

        public static TaskResource CopyPropertiesFrom(this TaskResource task, TaskResource source)
        {
            if (task.Properties == null)
            {
                task.Properties = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
            }

            Dictionary<string, object> targetProperties = task.Properties;
            Dictionary<string, object> sourceProperties = source.Properties;
            if (sourceProperties != null)
            {
                foreach (KeyValuePair<string, object> sourceProperty in sourceProperties)
                {
                    if (targetProperties.ContainsKey(sourceProperty.Key))
                    {
                        targetProperties[sourceProperty.Key] = sourceProperty.Value;
                    }
                }
            }

            return task;
        }

        public static TaskResource CreatePropertyStubs(this TaskResource task, TaskTypeResource taskType)
        {
            task.Properties = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
            if (task.Properties.Count < taskType.Properties.Count)
            {
                foreach (var property in taskType.Properties)
                {
                    //if (!task.Properties.Keys.Any(name => ((Object)name).Equals(property.Name)))
                    if (!task.Properties.ContainsKey(property.Name))
                    {
                        object value = string.Empty;
                        switch (property.Type)
                        {
                            case PropertyType.Double:
                                value = (double)0;
                                break;
                            case PropertyType.Integer:
                                value = 0;
                                break;
                        }
                        task.Properties.Add(property.Name, value);
                    }
                }
            }
            return task;
        }

        public static bool IsCompleted(this TaskResource task)
        {
            return task != null && task.Status == Status.Completed;
        }
    }
}
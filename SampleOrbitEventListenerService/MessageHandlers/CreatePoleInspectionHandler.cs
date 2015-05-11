using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SampleOrbitEventListenerService.Extensions;
using SampleOrbitEventListenerService.GeoServicesSdk;
using SE.Orbit.Services.DomainEvents;
using SE.Orbit.Services.Interfaces;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.MessageHandlers
{
    public class CreatePoleInspectionHandler : IAsyncMessageHandler<TaskCompleted>
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        readonly TaskServicesClient _orbit;
        const string GeodatabaseVersion = "SDE.DEFAULT";

        static readonly HashSet<string> ExcludedFields =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "OBJECTID",
                "GLOBALID"
            };

        /// <summary>
        /// This dictionary provides a mapping between the Feature Layer fields and 
        /// the Task Type properties. This is just an example... you must provide your
        /// own mapping!
        /// </summary>
        static readonly Dictionary<string, string> LayerFieldToTaskTypePropertyMapping =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                /* { FeatureLayerFieldName, TaskTypePropertyName }  */
                {"POLEOBJECTID", "OBJECTID"}
            };

        const string SourceTaskTypeName = "LinkPoleInspectionDevTest2";
        const string RelationshipName = "PoleInspection";

        public CreatePoleInspectionHandler(TaskServicesClient orbit)
        {
            _orbit = orbit;
        }

        public async Task Handle(TaskCompleted message)
        {
            TaskTypeResource sourceTaskType = await _orbit.GetTaskTypeAsync(SourceTaskTypeName);
            if (sourceTaskType != null)
            {
                TaskResource task = await _orbit.GetTaskAsync(message.TaskId);
                if (task.HasTaskType(sourceTaskType))
                {
                    EnsureTaskTypeHasAssociatedFeatureLayer(sourceTaskType);

                    Uri featureServerUrl = await GetFeatureServerUrlAsync(sourceTaskType);
                    FeatureServer featureServer = new FeatureServer(featureServerUrl);

                    // ReSharper disable once PossibleInvalidOperationException
                    Layer featureLayer = await featureServer.GetLayerAsync(sourceTaskType.LocationLayerID.Value);
                    Relationship relationship = FindRelationship(featureLayer);

                    Layer relatedLayer = await featureServer.GetLayerAsync(relationship.RelatedTableId);
                    var relatedObject = CreateRecord(relatedLayer, task);

                    Dictionary<string, string> parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "gdbVersion", GeodatabaseVersion },
                        { "rollbackOnFailure", "true" },
                        { "features", JsonConvert.SerializeObject(new [] { relatedObject }) }
                    };

                    await featureServer.AddFeature(relationship.RelatedTableId, parameters);
                }
            }
        }

        // This method will create a record based upon the feature layer and copy attributes
        // from the task into the record. The class, as written, will copy all matching properties
        // (respecting the mapping defined in the LayerFieldToTaskTypePropertyMapping member field).
        // If this doesn't meet your needs you should replace this behavior with any logic you'd like...
        static Feature CreateRecord(Layer layer, TaskResource task)
        {
            Feature relatedObject = new Feature();
            foreach (Field field in layer.Fields.Where(field => !ExcludedFields.Contains(field.Name)))
            {
                // Lookup the task type property for the given feature layer field
                string propertyName;
                if (!LayerFieldToTaskTypePropertyMapping.TryGetValue(field.Name, out propertyName))
                {
                    propertyName = field.Name;
                }

                object propertyValue;
                if (task.Properties.TryGetValue(propertyName, out propertyValue))
                {
                    propertyValue = field.ToCompatibleType(propertyValue);
                }
                else
                {
                    propertyValue = field.GetDefaultValue();
                }
                relatedObject.Attributes[field.Name] = propertyValue;
            }
            return relatedObject;
        }

        static Relationship FindRelationship(Layer featureLayer)
        {
            Relationship relationship = featureLayer.Relationships.FirstOrDefault(
                r => string.Equals(r.Name, RelationshipName, StringComparison.OrdinalIgnoreCase)
                     && r.Role == RelationshipRole.Origin);
            if (relationship == null)
            {
                throw new Exception(string.Format("Feature layer {0} does not contain expected relationship {1}", 
                    featureLayer.Name, RelationshipName));
            }

            return relationship;
        }

        static void EnsureTaskTypeHasAssociatedFeatureLayer(TaskTypeResource sourceTaskType)
        {
            if (!sourceTaskType.LocationLayerID.HasValue)
            {
                throw new Exception(string.Format("TaskType {0} is not associated with a ArcGIS Feature Layer",
                    sourceTaskType.Name));
            }
        }

        async Task<Uri> GetFeatureServerUrlAsync(TaskTypeResource taskType)
        {
            // Using the tasktype from orbit and the API key, find the feature service endpoint
            string mapServiceName = taskType.MapService.ToString("D");
            MapServiceResource mapService = await _orbit.MapServices.Get(mapServiceName).GetContentAsync();

            var options = RegexOptions.IgnoreCase;
            string featureServerUrl = Regex.Replace(mapService.Url, "MapServer", "FeatureServer", options);

            return new Uri(featureServerUrl);
        }
    }
}

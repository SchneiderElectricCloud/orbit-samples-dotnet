using System;

namespace SampleOrbitEventListenerService.GeoServicesSdk
{
    /// <summary>
    /// Caution: ArcGIS Server returns a comma-separated list of enum values for the layer
    /// capabilities. This enum represents the valid list of capabilities, but the int values
    /// may not match Esri's values...
    /// </summary>
    [Flags]
    public enum LayerCapabilities
    {
        Query = 1,
        Create = 2,
        Delete = 4,
        Update = 8,

        /// <summary>
        /// The Editing capability will be included if Create, Delete, or Update is enabled for a feature service.
        /// </summary>
        Editing = 16,
        Sync = 32,
        Uploads = 64,
    }
}
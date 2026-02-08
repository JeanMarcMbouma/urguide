namespace UrGuide.Model.Users
{
    /// <summary>
    /// Status of a data export request
    /// </summary>
    public enum DataExportStatus
    {
        /// <summary>
        /// Export request has been created and is waiting to be processed
        /// </summary>
        Pending = 0,
        
        /// <summary>
        /// Export is currently being generated
        /// </summary>
        Processing = 1,
        
        /// <summary>
        /// Export has been successfully generated and is ready for download
        /// </summary>
        Completed = 2,
        
        /// <summary>
        /// Export generation failed
        /// </summary>
        Failed = 3,
        
        /// <summary>
        /// Export has expired and is no longer available for download
        /// </summary>
        Expired = 4
    }

    /// <summary>
    /// Format of the data export
    /// </summary>
    public enum DataExportFormat
    {
        /// <summary>
        /// Single JSON file containing all data
        /// </summary>
        Json = 0,
        
        /// <summary>
        /// ZIP archive containing multiple CSV files
        /// </summary>
        Csv = 1
    }
}

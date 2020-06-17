using Newtonsoft.Json;

namespace VoltDB.Data.Client.Connections
{
    /// <summary>
    /// Describes the possible options when restoring a snapshot
    /// </summary>
    public struct SnapshotRestoreOptions
    {
        /// <summary>
        /// Path of the directory containing the snapshot.
        /// </summary>
        [JsonProperty("path")]
        public string DirectoryPath { get; set; }
       
        /// <summary>
        /// Specifies the unique identifier for the snapshot.
        /// </summary>
        [JsonProperty("nonce")]
        public string UniqueId { get; set; }

        /// <summary>
        /// Specifies tables to include in the snapshot.
        /// </summary>
        [JsonProperty("tables")]
        public string[] Tables { get; set; }

        /// <summary>
        /// Specifies tables to leave out of the snapshot. 
        /// </summary>
        [JsonProperty("skiptables")]
        public string[] Skiptables { get; set; }
    }
}

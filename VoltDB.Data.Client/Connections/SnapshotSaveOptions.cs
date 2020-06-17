using Newtonsoft.Json;

namespace VoltDB.Data.Client.Connections
{
    /// <summary>
    /// Describes all possible options when creating a snapshot 
    /// </summary>
    public struct SnapshotSaveOptions
    {
        private string _directoryPath;
        /// <summary>
        /// Path of the directory containing the snapshot.
        /// </summary>
        [JsonProperty("uripath")]
        public string DirectoryPath
        {
            get => _directoryPath;
            set => _directoryPath = $"file://{value}";
        }

        /// <summary>
        /// Specifies the unique identifier for the snapshot.
        /// </summary>
        [JsonProperty("nonce")]
        public string UniqueId { get; set; }

        /// <summary>
        /// Specifies whether the snapshot should be synchronous (true) and block other transactions or asynchronous (false).
        /// </summary>
        [JsonProperty("block")]
        public bool Block { get; set; }

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

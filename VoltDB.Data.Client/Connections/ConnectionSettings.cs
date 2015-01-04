/* This file is part of VoltDB.
 * Copyright (C) 2008-2015 VoltDB Inc.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides a conectionstring-compatible settings provider for VoltDB connections.
    /// </summary>
    public sealed class ConnectionSettings : DbConnectionStringBuilder
    {
        /// <summary>
        /// Define internal "keys" for localized attribute values
        /// </summary>
        private static class Localized
        {
            /// <summary>
            /// Prefix for all ConnectionSettings resource strings
            /// </summary>
            public const string ResourcePrefix = "CS_";

            /// <summary>
            /// Categories
            /// </summary>
            public static class Categories
            {
                public const string Connection = "Connection";
                public const string Security = "Security";
                public const string Advanced = "Advanced";
                public const string Pooling = "Pooling";
            }

            /// <summary>
            /// Properties
            /// </summary>
            public static class Properties
            {
                public const string ServiceType = "ServiceType";
                public const string HostList = "HostList";
                public const string Port = "Port";
                public const string ConnectionTimeout = "ConnectionTimeout";
                public const string DefaultCommandTimeout = "DefaultCommandTimeout";
                public const string UserId = "UserId";
                public const string Password = "Password";
                public const string PersistSecurityInfo = "PersistSecurityInfo";
                public const string AllowSystemCalls = "AllowSystemCalls";
                public const string MaxOutstandingTxns = "MaxOutstandingTxns";
                public const string Logging = "Logging";
                public const string Statistics = "Statistics";
                public const string AllowMultipleHostConnections = "AllowMultipleHostConnections";
                public const string ConnectToAllOrNone = "ConnectToAllOrNone";
                public const string LoadBalancingBatchSize = "LoadBalancingBatchSize";
                public const string AllowAdhocQueries = "AllowAdhocQueries";
            }
        }

        /// <summary>
        /// Redefines a localized version of the Category attribute for assembly localization
        /// </summary>
        private class LocalizedCategoryAttribute : CategoryAttribute
        {
            private const string ResourcePrefix = "Category_";
            public LocalizedCategoryAttribute(string resourceName)
                : base(Resources.ResourceManager.GetString(Localized.ResourcePrefix + ResourcePrefix + resourceName))
            { }
        }

        /// <summary>
        /// Redefines a localized version of the DisplayName attribute for assembly localization
        /// </summary>
        private class LocalizedDisplayNameAttribute : DisplayNameAttribute
        {
            private const string ResourcePrefix = "DisplayName_";
            public LocalizedDisplayNameAttribute(string resourceName)
                : base(Resources.ResourceManager.GetString(Localized.ResourcePrefix + ResourcePrefix + resourceName))
            { }
        }

        /// <summary>
        /// Redefines a localized version of the Description attribute for assembly localization
        /// </summary>
        private class LocalizedDescriptionAttribute : DescriptionAttribute
        {
            private const string ResourcePrefix = "Description_";
            public LocalizedDescriptionAttribute(string resourceName)
                : base(Resources.ResourceManager.GetString(Localized.ResourcePrefix + ResourcePrefix + resourceName))
            { }
        }

        /// <summary>
        /// Redefines a localized version of the ValidKeywords attribute for assembly localization
        /// </summary>
        private class LocalizedValidKeywordsAttribute : ValidKeywordsAttribute
        {
            private const string ResourcePrefix = "ValidKeywords_";
            public LocalizedValidKeywordsAttribute(string resourceName)
                : base(Resources.ResourceManager.GetString(Localized.ResourcePrefix + ResourcePrefix + resourceName))
            { }
        }

        /// <summary>
        /// Defines a property default value handler.
        /// </summary>
        private struct PropertyDefaultValue
        {
            /// <summary>
            /// Type of the property default
            /// </summary>
            public readonly Type Type;

            /// <summary>
            /// Actual default value for the property
            /// </summary>
            public readonly object DefaultValue;

            /// <summary>
            /// Defines a default value handler for a given property, making a property explicit definition optional.
            /// </summary>
            /// <param name="t">Data type of the property.</param>
            /// <param name="v">Default value if not specified by the user.</param>
            public PropertyDefaultValue(Type t, object v)
            {
                Type = t;
                DefaultValue = v;
            }
        }

        /// <summary>
        /// Defines a metadata attribute to allows the definition of "synonym" connection string keywords (for instance
        /// 'cluster', 'servers' and 'hosts' all referring to the same 'HostList' property).
        /// </summary>
        private class ValidKeywordsAttribute : Attribute
        {
            /// <summary>
            /// List of provided keywords for the attribute.
            /// </summary>
            private string keywords;

            /// <summary>
            /// Creates a new attribute with a list of permitted keywords for a given property.
            /// </summary>
            /// <param name="keywords">List of permitted keywords/synonyms for a property</param>
            public ValidKeywordsAttribute(string keywords)
            {
                this.keywords = keywords.ToLower(CultureInfo.InvariantCulture);
            }
            /// <summary>
            /// Returns the list of valid keywords assigned through the attribute.
            /// </summary>
            public string[] Keywords
            {
                get { return keywords.Split(','); }
            }
        }

        /// <summary>
        /// Dictionary of valid keywords for the ConnectionSettings class (initialized statically).
        /// </summary>
        private static readonly Dictionary<string, string> ValidKeywords;

        /// <summary>
        /// Dictionary of default property values for the ConnectionSettings class (initialized statically).
        /// </summary>
        private static Dictionary<string, PropertyDefaultValue> DefaultValues;

        /// <summary>
        /// Dictionary of actual values for this ConnectionSettings instance.
        /// </summary>
        private Dictionary<string, object> values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initialize metadata for the ConnectionSettings type using reflection to read attribute based control and
        /// validation details.
        /// </summary>
        static ConnectionSettings()
        {
            ValidKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            DefaultValues = new Dictionary<string, PropertyDefaultValue>(StringComparer.OrdinalIgnoreCase);
            PropertyInfo[] properties = typeof(ConnectionSettings).GetProperties();
            foreach (PropertyInfo pi in properties)
                AddKeywordFromProperty(pi);
        }

        /// <summary>
        /// Use reflection to analyze the ConnectionSettings class and retrieve attribute-based property metadata used
        /// for parsing and validation of connection strings.
        /// </summary>
        /// <param name="pi">PropertyInfo object for the property to analyze.</param>
        private static void AddKeywordFromProperty(PropertyInfo pi)
        {
            // By default the name/displayName of the property are set from the reflected name of the property itself.
            string name = pi.Name.ToLower(CultureInfo.InvariantCulture);
            string displayName = name;

            // Now see if we have defined a display name for this property.
            object[] attr = pi.GetCustomAttributes(false);
            foreach (Attribute a in attr)
                if (a is DisplayNameAttribute)
                {
                    displayName = (a as DisplayNameAttribute).DisplayName;
                    break;
                }

            // Add the display name & displayName to the list of valid keywords.
            ValidKeywords[name] = displayName;
            ValidKeywords[displayName] = displayName;

            // Add any specifically listed valid keyword
            foreach (Attribute a in attr)
            {
                if (a is ValidKeywordsAttribute)
                {
                    foreach (string keyword in (a as ValidKeywordsAttribute).Keywords)
                        ValidKeywords[keyword.ToLower(CultureInfo.InvariantCulture).Trim()] = displayName;
                }
                else if (a is DefaultValueAttribute)
                {
                    DefaultValues[displayName]
                        = new PropertyDefaultValue(
                                                    pi.PropertyType
                                                  , Convert.ChangeType(
                                                                        (a as DefaultValueAttribute).Value
                                                                      , pi.PropertyType
                                                                      , CultureInfo.CurrentCulture
                                                                      )
                                                  );
                }
            }
        }

        /// <summary>
        /// Create a new, empty ConnectionSettings object, using default property values.
        /// </summary>
        public ConnectionSettings()
        {
            Clear();
        }

        /// <summary>
        /// Create a new ConnectionSettings object from the given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to use to initialize the object.</param>
        public ConnectionSettings(string connectionString)
            : this()
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets or sets an enumeration value that indicates the type of connection service requested (Database or
        /// Export - note that Export is not supported by this version of the .NET library).
        /// </summary>
        [LocalizedCategory(Localized.Categories.Connection)]
        [LocalizedDisplayName(Localized.Properties.ServiceType)]
        [LocalizedDescription(Localized.Properties.ServiceType)]
        [LocalizedValidKeywords(Localized.Properties.ServiceType)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(ServiceType.Database)]
        public ServiceType ServiceType
        {
            get { return (ServiceType)values[CS_DisplayName_ServiceType]; }
            set { SetValue(CS_DisplayName_ServiceType, value); }
        }
        static readonly string CS_DisplayName_ServiceType = Resources.CS_DisplayName_ServiceType;

        /// <summary>
        /// Gets or sets the list (one or several) of VoltDB server(s) to connect to.  The list may be provided as a
        /// comma or space-separated list of host names or IP addresses.  If host names are provided, understand the
        /// connection may attempt to create multiple connections to the same host (one per IP address - for more
        /// details <see cref="ConnectionSettings.AllowMultipleHostConnections"/>).
        /// </summary>
        [LocalizedCategory(Localized.Categories.Connection)]
        [LocalizedDisplayName(Localized.Properties.HostList)]
        [LocalizedDescription(Localized.Properties.HostList)]
        [LocalizedValidKeywords(Localized.Properties.HostList)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue("localhost")]
        public string HostList
        {
            get { return values[Resources.CS_DisplayName_HostList] as string; }
            set { SetValue(Resources.CS_DisplayName_HostList, value); }
        }
        static readonly string CS_DisplayName_HostList = Resources.CS_DisplayName_HostList;

        /// <summary>
        /// Default endpoint a connection holding these settings will attach to (for multi-host connectionstring, this
        /// will return the first available endpoint).
        /// </summary>
        public IPEndPoint DefaultIPEndPoint
        {
            get
            {
                return new IPEndPoint(this.HostAddresses[0], this.Port);
            }
        }

        /// <summary>
        /// Parse out the HostList to provide a fully deployed list of IP addresses for the provided host list.  This
        /// method is used by the VoltClient to generate 1 connection per IP for load balancing.
        /// </summary>
        public IPAddress[] HostAddresses
        {
            get
            {
                //  - Split the provided host/IP list into individual elements.
                //  - Trim.
                //  - Remove empty elements.
                //  - Remove any duplicate.
                //  - Pull the entire list of IP addresses for the host (will be a single IP address if an IP was
                //    given, but will possibly be more for a host name (multiple assigned IPs, or multiple adapters).
                //  - Only pick IPv4 addresses since we'll be working over TCP sockets.
                //  - Based on AllowMultipleHostConnections setting, decide whether to pick one address only or all.
                //  - Build and return the array.
                return this.HostList.Split(',', ' ')
                           .Select(i => i.Trim())
                           .Where(i => i != "")
                           .Distinct()
                           .SelectMany(i => Dns.GetHostAddresses(i)
                                               .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                                               .Take(this.AllowMultipleHostConnections ? int.MaxValue : 1)
                                      ).ToArray();
            }
        }

        /// <summary>
        /// Deploy the connection settings object into a list of single-endpoint connection settings, allowing a
        /// cluster connection to multiple servers to be deployed into multiple connections to a single IP address
        /// each.
        /// </summary>
        public ConnectionSettings[] ClusterConnectionSettings
        {
            get
            {
                // Grab the current connection string
                string connectionString = this.GetConnectionString(true);

                //  - Create a new ConnectionSettings instance based on the current connection string and overriding
                //    the endpoint for each IP address found in the original connection string.
                //  - Build and return the array
                return this.HostAddresses.Select(a => new ConnectionSettings(connectionString)
                                                      {
                                                          HostList = a.ToString()
                                                      }
                                                ).ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the port number to connect to on the cluster servers.
        /// </summary>
        [LocalizedCategory(Localized.Categories.Connection)]
        [LocalizedDisplayName(Localized.Properties.Port)]
        [LocalizedDescription(Localized.Properties.Port)]
        [LocalizedValidKeywords(Localized.Properties.Port)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(21212)]
        public int Port
        {
            get { return (int)values[CS_DisplayName_Port]; }
            set
            {
                SetValue(
                          CS_DisplayName_Port
                        , (value > ushort.MaxValue || value < 1)
                          ? (int)DefaultValues[CS_DisplayName_Port].DefaultValue
                          : value
                        );
            }
        }
        static readonly string CS_DisplayName_Port = Resources.CS_DisplayName_Port;

        /// <summary>
        /// Gets or sets the connection timeout (in milliseconds).
        /// </summary>
        [LocalizedCategory(Localized.Categories.Connection)]
        [LocalizedDisplayName(Localized.Properties.ConnectionTimeout)]
        [LocalizedDescription(Localized.Properties.ConnectionTimeout)]
        [LocalizedValidKeywords(Localized.Properties.ConnectionTimeout)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(5000)]
        public int ConnectionTimeout
        {
            get { return (int)values[CS_DisplayName_ConnectionTimeout]; }
            set { SetValue(CS_DisplayName_ConnectionTimeout, value < 0 ? -1 : value); }
        }
        static readonly string CS_DisplayName_ConnectionTimeout = Resources.CS_DisplayName_ConnectionTimeout;

        /// <summary>
        /// Gets or sets the default command timeout (in milliseconds).  Request executions that do not return before
        /// the timeout will be aborted on the client-side, and their callback receive a VoltTimeoutException.
        /// </summary>
        [LocalizedCategory(Localized.Categories.Connection)]
        [LocalizedDisplayName(Localized.Properties.DefaultCommandTimeout)]
        [LocalizedDescription(Localized.Properties.DefaultCommandTimeout)]
        [LocalizedValidKeywords(Localized.Properties.DefaultCommandTimeout)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(5000)]
        public int DefaultCommandTimeout
        {
            get { return (int)values[CS_DisplayName_DefaultCommandTimeout]; }
            set { SetValue(CS_DisplayName_DefaultCommandTimeout, value < 0 ? -1 : value); }
        }
        static readonly string CS_DisplayName_DefaultCommandTimeout = Resources.CS_DisplayName_DefaultCommandTimeout;

        /// <summary>
        /// Gets or sets the user id that should be used for authentication.
        /// </summary>
        [LocalizedCategory(Localized.Categories.Security)]
        [LocalizedDisplayName(Localized.Properties.UserId)]
        [LocalizedDescription(Localized.Properties.UserId)]
        [LocalizedValidKeywords(Localized.Properties.UserId)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue("")]
        public string UserID
        {
            get { return (string)values[CS_DisplayName_UserId]; }
            set { SetValue(CS_DisplayName_UserId, value); }
        }
        static readonly string CS_DisplayName_UserId = Resources.CS_DisplayName_UserId;

        /// <summary>
        /// Gets or sets the password that should be used for authentication.
        /// </summary>
        [LocalizedCategory(Localized.Categories.Security)]
        [LocalizedDisplayName(Localized.Properties.Password)]
        [LocalizedDescription(Localized.Properties.Password)]
        [LocalizedValidKeywords(Localized.Properties.Password)]
        [PasswordPropertyText(true)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue("")]
        public string Password
        {
            get { return (string)values[CS_DisplayName_Password]; }
            set { SetValue(CS_DisplayName_Password, value); }
        }
        static readonly string CS_DisplayName_Password = Resources.CS_DisplayName_Password;

        /// <summary>
        /// Gets or sets a boolean value that indicates if the password should be persisted in the connection string.
        /// </summary>
        [LocalizedCategory(Localized.Categories.Security)]
        [LocalizedDisplayName(Localized.Properties.PersistSecurityInfo)]
        [LocalizedDescription(Localized.Properties.PersistSecurityInfo)]
        [LocalizedValidKeywords(Localized.Properties.PersistSecurityInfo)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool PersistSecurityInfo
        {
            get { return (bool)values[CS_DisplayName_PersistSecurityInfo]; }
            set { SetValue(CS_DisplayName_PersistSecurityInfo, value); }
        }
        static readonly string CS_DisplayName_PersistSecurityInfo = Resources.CS_DisplayName_PersistSecurityInfo;

        /// <summary>
        /// Gets or sets a boolean value that indicates whether access to system stored procedures and ad-hoc queries
        /// will be granted.
        /// </summary>
        [LocalizedCategory(Localized.Categories.Security)]
        [LocalizedDisplayName(Localized.Properties.AllowSystemCalls)]
        [LocalizedDescription(Localized.Properties.AllowSystemCalls)]
        [LocalizedValidKeywords(Localized.Properties.AllowSystemCalls)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool AllowSystemCalls
        {
            get { return (bool)values[CS_DisplayName_AllowSystemCalls]; }
            set { SetValue(CS_DisplayName_AllowSystemCalls, value); }
        }
        static readonly string CS_DisplayName_AllowSystemCalls = Resources.CS_DisplayName_AllowSystemCalls;

        /// <summary>
        /// Gets or sets a boolean value that indicates whether access to system stored procedures and ad-hoc queries
        /// will be granted.
        /// </summary>
        [LocalizedCategory(Localized.Categories.Security)]
        [LocalizedDisplayName(Localized.Properties.AllowAdhocQueries)]
        [LocalizedDescription(Localized.Properties.AllowAdhocQueries)]
        [LocalizedValidKeywords(Localized.Properties.AllowAdhocQueries)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool AllowAdhocQueries
        {
            get { return (bool)values[CS_DisplayName_AllowAdhocQueries]; }
            set { SetValue(CS_DisplayName_AllowAdhocQueries, value); }
        }
        static readonly string CS_DisplayName_AllowAdhocQueries = Resources.CS_DisplayName_AllowAdhocQueries;

        /// <summary>
        /// Gets or sets a value indicating how many requests may be queued up in "pending" state before blocking
        /// execution on all calls (to prevent degradation of performance through server 'fire-hosing').
        /// The Execute/ExecuteAsyn method will sleep the calling thread until the queue as shrunk below this maximum.
        /// Depending on your payload this number can be quite large, or small.  You can avoid performance degradation
        /// due to server 'fire-hosing' by decreasing this value.
        /// </summary>
        [LocalizedCategory(Localized.Categories.Advanced)]
        [LocalizedDisplayName(Localized.Properties.MaxOutstandingTxns)]
        [LocalizedDescription(Localized.Properties.MaxOutstandingTxns)]
        [LocalizedValidKeywords(Localized.Properties.MaxOutstandingTxns)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(3000)]
        public int MaxOutstandingTxns
        {
            get { return (int)values[CS_DisplayName_MaxOutstandingTxns]; }
            set
            {
                SetValue(
                          CS_DisplayName_MaxOutstandingTxns
                        , value < 0
                          ? (int)DefaultValues[CS_DisplayName_MaxOutstandingTxns].DefaultValue
                          : value
                        );
            }
        }
        static readonly string CS_DisplayName_MaxOutstandingTxns = Resources.CS_DisplayName_MaxOutstandingTxns;

        /// <summary>
        /// Gets or sets a boolean value that indicates whether tracing events should be logged by the connection (use
        /// this for development and debugging only as it has negative impact on performance).
        /// </summary>
        [LocalizedCategory(Localized.Categories.Advanced)]
        [LocalizedDisplayName(Localized.Properties.Logging)]
        [LocalizedDescription(Localized.Properties.Logging)]
        [LocalizedValidKeywords(Localized.Properties.Logging)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool TraceEnabled
        {
            get { return (bool)values[CS_DisplayName_Logging]; }
            set { SetValue(CS_DisplayName_Logging, value); }
        }
        static readonly string CS_DisplayName_Logging = Resources.CS_DisplayName_Logging;

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the connection will keep track of performance
        /// statistics (use this for benchmarking, or production monitoring - performance impact is negligible)
        /// </summary>
        [LocalizedCategory(Localized.Categories.Advanced)]
        [LocalizedDisplayName(Localized.Properties.Statistics)]
        [LocalizedDescription(Localized.Properties.Statistics)]
        [LocalizedValidKeywords(Localized.Properties.Statistics)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool StatisticsEnabled
        {
            get { return (bool)values[CS_DisplayName_Statistics]; }
            set { SetValue(CS_DisplayName_Statistics, value); }
        }
        static readonly string CS_DisplayName_Statistics = Resources.CS_DisplayName_Statistics;

        /// <summary>
        /// Gets or sets a boolean value that indicates if pooled connections should be allowed to connect multiple
        /// times to a given host if it exposes multiple IP addresses.
        /// </summary>
        [LocalizedCategory(Localized.Categories.Pooling)]
        [LocalizedDisplayName(Localized.Properties.AllowMultipleHostConnections)]
        [LocalizedDescription(Localized.Properties.AllowMultipleHostConnections)]
        [LocalizedValidKeywords(Localized.Properties.AllowMultipleHostConnections)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool AllowMultipleHostConnections
        {
            get { return (bool)values[CS_DisplayName_AllowMultipleHostConnections]; }
            set { SetValue(CS_DisplayName_AllowMultipleHostConnections, value); }
        }
        static readonly string CS_DisplayName_AllowMultipleHostConnections = Resources.CS_DisplayName_AllowMultipleHostConnections;

        /// <summary>
        /// Gets or sets a boolean value that indicates if a cluster connection should demand that all nodes listed in
        /// the host list be reachable before accepting work.
        /// </summary>
        [LocalizedCategory(Localized.Categories.Pooling)]
        [LocalizedDisplayName(Localized.Properties.ConnectToAllOrNone)]
        [LocalizedDescription(Localized.Properties.ConnectToAllOrNone)]
        [LocalizedValidKeywords(Localized.Properties.ConnectToAllOrNone)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool ConnectToAllOrNone
        {
            get { return (bool)values[CS_DisplayName_ConnectToAllOrNone]; }
            set { SetValue(CS_DisplayName_ConnectToAllOrNone, value); }
        }
        static readonly string CS_DisplayName_ConnectToAllOrNone = Resources.CS_DisplayName_ConnectToAllOrNone;

        /// <summary>
        /// Gets or sets a value indicating the size of load balancing batches: instead of alternating connection with
        /// each call, the first [batchsize] calls are sent to the first connection, the second batch to the second
        /// connection, etc. This is still a round-robin, but it optimizes network usage while preventing fire-hosing.
        /// </summary>
        [LocalizedCategory(Localized.Categories.Pooling)]
        [LocalizedDisplayName(Localized.Properties.LoadBalancingBatchSize)]
        [LocalizedDescription(Localized.Properties.LoadBalancingBatchSize)]
        [LocalizedValidKeywords(Localized.Properties.LoadBalancingBatchSize)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(100)]
        public int LoadBalancingBatchSize
        {
            get { return (int)values[CS_DisplayName_LoadBalancingBatchSize]; }
            set
            {
                SetValue(
                          CS_DisplayName_LoadBalancingBatchSize
                        , value < 0
                          ? (int)DefaultValues[CS_DisplayName_LoadBalancingBatchSize].DefaultValue
                          : value
                        );
            }
        }
        static readonly string CS_DisplayName_LoadBalancingBatchSize = Resources.CS_DisplayName_LoadBalancingBatchSize;

        /// <summary>
        /// Retrieves a connection setting by keyword.
        /// </summary>
        /// <param name="keyword">Keyword of the setting to retrieve.</param>
        /// <returns>Value of the setting</returns>
        public override object this[string keyword]
        {
            get { return values[ValidKeywords[keyword]]; }
            set
            {
                ValidateKeyword(keyword);
                if (value == null)
                    Remove(keyword);
                else
                    SetValue(keyword, value);
            }
        }

        /// <summary>
        /// Empties the connection settings of all custom values and reset to defaults.
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            // Make a copy of our default values array.
            foreach (string key in DefaultValues.Keys)
                values[key] = DefaultValues[key].DefaultValue;
        }

        /// <summary>
        /// Removes a custom setting and reset to default.
        /// </summary>
        /// <param name="keyword">Keyword of the setting to remove.</param>
        /// <returns>true if the key existed within the connection string and was removed; false if the key did not
        /// exist.</returns>
        public override bool Remove(string keyword)
        {
            ValidateKeyword(keyword);
            string primaryKey = ValidKeywords[keyword];

            values.Remove(primaryKey);
            base.Remove(primaryKey);

            values[primaryKey] = DefaultValues[primaryKey].DefaultValue;
            return true;
        }

        /// <summary>
        /// Retrieves a value corresponding to the supplied key from this ConnectionSettings.
        /// </summary>
        /// <param name="keyword">The key of the item to retrieve.</param>
        /// <param name="value">The value corresponding to the key.</param>
        /// <returns>true if keyword was found within the connection string, false otherwise.</returns>
        public override bool TryGetValue(string keyword, out object value)
        {
            ValidateKeyword(keyword);
            return values.TryGetValue(ValidKeywords[keyword], out value);
        }

        /// <summary>
        /// Returns the connection string corresponding to this ConnectionSettings object, optionally building a
        /// complete connectionstring, including the password (needed for string derivations and equality comparisons).
        /// To avoid security issues, this method is only available internally.
        /// </summary>
        /// <param name="includePass">Whether to include the password in the returned connection string.</param>
        /// <returns></returns>
        internal string GetConnectionString(bool includePass)
        {
            if (includePass) return ConnectionString;

            StringBuilder conn = new StringBuilder();
            string delimiter = "";
            foreach (string key in this.Keys)
            {
                if (String.Compare(key, "password", true) == 0 || String.Compare(key, "pwd", true) == 0) continue;
                conn.AppendFormat(CultureInfo.CurrentCulture, "{0}{1}={2}", delimiter, key, this[key]);
                delimiter = ";";
            }
            return conn.ToString();
        }

        /// <summary>
        /// Sets the value of a setting, validating it along the way.
        /// </summary>
        /// <param name="keyword">Key of the setting to set.</param>
        /// <param name="value">Value to assign.</param>
        private void SetValue(string keyword, object value)
        {
            ValidateKeyword(keyword);
            keyword = ValidKeywords[keyword];

            Remove(keyword);

            object val = null;
            if (value is string && DefaultValues[keyword].DefaultValue is Enum)
                val = ParseEnum(DefaultValues[keyword].Type, (string)value, keyword);
            else
                val = ChangeType(value, DefaultValues[keyword].Type);
            values[keyword] = val;
            base[keyword] = val;
        }

        /// <summary>
        /// Parse a value into an enumeration type, validating it belongs to the expected enumeration.
        /// </summary>
        /// <param name="t">Enumeration type.</param>
        /// <param name="requestedValue">Requested connection string value.</param>
        /// <param name="key">Key of the setting to update.</param>
        /// <returns>Enumeration value (boxed) - if successfully converted, otherwise an exception is thrown.</returns>
        private object ParseEnum(Type t, string requestedValue, string key)
        {
            try
            {
                return Enum.Parse(t, requestedValue, true);
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException(
                                                     String.Format(
                                                                    Resources.InvalidConnectionStringValue
                                                                  , requestedValue
                                                                  , key
                                                                  )
                                                   );
            }
        }

        /// <summary>
        /// Perform basic string conversions to various types, to change connection string values into valid,
        /// strongly-typed, internal settings.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="t">Expected type.</param>
        /// <returns>Converted value (boxed) - if successfully converted, oherwise an exception is thrown.</returns>
        private object ChangeType(object value, Type t)
        {
            if (t == typeof(bool) && value is string)
            {
                string s = value.ToString().ToLower(CultureInfo.InvariantCulture);
                if (s == "yes" || s == "true") return true;
                if (s == "no" || s == "false") return false;
                throw new FormatException(String.Format(Resources.InvalidValueForBoolean, value));
            }
            else
                return Convert.ChangeType(value, t, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Validates that a connectionstring keyword corresponds to an existing setting - throws an exception
        /// otherwise.
        /// </summary>
        /// <param name="keyword">Keyword to validate.</param>
        private void ValidateKeyword(string keyword)
        {
            string key = keyword.ToLowerInvariant();
            if (!ValidKeywords.ContainsKey(key))
                throw new ArgumentException(Resources.KeywordNotSupported, keyword);
        }

        /// <summary>
        /// Provide implicit conversion between a ConnectionSettings object and a string.
        /// </summary>
        /// <param name="settings">The settings object to convert to a string.</param>
        /// <returns>string representation of the ConnectionSettings.</returns>
        public static implicit operator string(ConnectionSettings settings)
        {
            return settings.ConnectionString;
        }
        /// <summary>
        /// Provide implicit conversion between a connection string and a ConnectionSettings object.
        /// </summary>
        /// <param name="connectionString">The connection string to convert to a settings object.</param>
        /// <returns>The ConnectionSettings object corresponding to the provided connectionstring.</returns>
        public static implicit operator ConnectionSettings(string connectionString)
        {
            return new ConnectionSettings(connectionString);
        }

        /// <summary>
        /// Override Equals to provide comparison support.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns>True if values are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            ConnectionSettings settings = obj as ConnectionSettings;
            if (settings == null)
                return false;

            if (settings.GetConnectionString(true) == this.GetConnectionString(true))
                return true;

            return false;
        }

        /// <summary>
        /// Override Equals to provide comparison support.
        /// </summary>
        /// <param name="connectionString">ConnectionString to compare with this object.</param>
        /// <returns>True if values are equal.</returns>
        public bool Equals(string connectionString)
        {
            if (connectionString == null)
                return false;

            if (new ConnectionSettings(connectionString).GetConnectionString(true) == this.GetConnectionString(true))
                return true;

            return false;
        }

        /// <summary>
        /// Override Equals to provide comparison support.
        /// </summary>
        /// <param name="settings">Other connection settings object to compare with this object.</param>
        /// <returns>True if values are equal.</returns>
        public bool Equals(ConnectionSettings settings)
        {
            if (settings == null)
                return false;

            if (settings.GetConnectionString(true) == this.GetConnectionString(true))
                return true;

            return false;
        }

        /// <summary>
        /// Override hashcode definition to use the string's hashcode definition.
        /// </summary>
        /// <returns>Hashcode for the object, calculated as the hashcode of the connection string.</returns>
        public override int GetHashCode()
        {
            return this.GetConnectionString(true).GetHashCode();
        }
    }
}

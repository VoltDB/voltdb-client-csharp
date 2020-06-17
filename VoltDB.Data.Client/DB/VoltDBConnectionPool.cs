using System.Collections.Concurrent;
using System.ComponentModel;

namespace VoltDB.Data.Client
{
    /// <summary>Volt database connection pool</summary>
    public class VoltDBConnectionPool
    {
        private bool _usePooling = true;
        private const string MAX_COUNT_CONFIG_KEY = "ConnectionPool_MaxConnectionsInPoolCount";
        private const string ENABLE_POOLING_TOGGLE_CONFIG_KEY = "ConnectionPool_EnablePooling";
        private ConcurrentBag<IVoltConnection> _connections;
        private ConnectionSettings _connectionSettings;
        private int _maxConnectionsInPool;
        private static VoltDBConnectionPool instance;

        private VoltDBConnectionPool(ConnectionSettings connectionSettings, int maxConnectionsInPool)
          : this(connectionSettings)
        {
            this._maxConnectionsInPool = maxConnectionsInPool;
        }

        private VoltDBConnectionPool(ConnectionSettings connectionSettings)
        {
            this._connections = new ConcurrentBag<IVoltConnection>();
            this._connectionSettings = connectionSettings;
            if (!this.TryGetMaxConnectionsCountFromSettings())
                this._maxConnectionsInPool = 100;
            bool? usePooling = connectionSettings.UsePooling;
            this._usePooling = !usePooling.HasValue || usePooling.GetValueOrDefault();
        }

        /// <summary>Gets the connection.</summary>
        public IVoltConnection GetConnection()
        {
            IVoltConnection result = (IVoltConnection)null;
            while (this._connections.Count > 0)
            {
                this._connections.TryTake(out result);
                if (result == null || result.IsHealthy)
                    break;
            }
            if (result == null)
                result = new VoltConnectionProxy(this, this._connectionSettings, false);
            return result;
        }

        internal void ReleaseConnection(VoltConnectionProxy connectionProxy, CancelEventArgs args)
        {
            if (!this._usePooling || this._connections.Count >= this._maxConnectionsInPool)
                return;
            this._connections.Add(connectionProxy);
            if (args == null)
                return;
            args.Cancel = true;
        }

        private bool TryGetMaxConnectionsCountFromSettings()
        {
            int connectionsInPool = this._connectionSettings.MaxConnectionsInPool;
            if (connectionsInPool <= 0)
                return false;
            this._maxConnectionsInPool = connectionsInPool;
            return true;
        }

        /// <summary>Gets the instance.</summary>
        /// <param name="settings">The settings.</param>
        public static VoltDBConnectionPool GetInstance(ConnectionSettings settings)
        {
            if (instance == null)
                instance = new VoltDBConnectionPool(settings);
            return instance;
        }
    }
}

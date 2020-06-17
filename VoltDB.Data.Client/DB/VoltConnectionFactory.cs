using System;

namespace VoltDB.Data.Client
{
    /// <summary>Volt connection factory</summary>
    public static class VoltConnectionFactory
    {
        private static readonly object syncRoot = new object();
        private static VoltDBConnectionPool pool;
        private static VoltConnectionProxy singleConnectionInstance;

        /// <summary>Gets the open connection.</summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="useSingleConnection">if set to <c>true</c> [use single connection].</param>
        /// <returns></returns>
        public static IVoltConnection GetOpenConnection(
          string connectionString,
          bool useSingleConnection = false)
        {
            if (!useSingleConnection)
                return GetPool(new ConnectionSettings(connectionString)).GetConnection().Open();
            if (singleConnectionInstance == null)
            {
                lock (syncRoot)
                {
                    if (singleConnectionInstance == null)
                        singleConnectionInstance = new VoltConnectionProxy(new ConnectionSettings(connectionString), true);
                }
            }
            try
            {
                singleConnectionInstance.Open();
            }
            catch (Exception)
            {
                try
                {
                    if (singleConnectionInstance != null)
                        singleConnectionInstance.Open();
                }
                catch (Exception)
                {
                    singleConnectionInstance = (VoltConnectionProxy)null;
                    throw;
                }
            }
            return (IVoltConnection)singleConnectionInstance;
        }

        private static VoltDBConnectionPool GetPool(
          ConnectionSettings connectionSettings)
        {
            if (pool == null)
                pool = VoltDBConnectionPool.GetInstance(connectionSettings);
            return pool;
        }
    }
}

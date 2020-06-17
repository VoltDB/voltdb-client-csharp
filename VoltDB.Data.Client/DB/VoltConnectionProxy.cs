using System;
using System.ComponentModel;

namespace VoltDB.Data.Client
{
    /// <summary>Volt connection proxy</summary>
    /// <seealso cref="VoltDB.Data.Client.IVoltConnection" />
    /// <seealso cref="System.IDisposable" />
    public class VoltConnectionProxy : IVoltConnection, IDisposable
    {
        private VoltConnection connection;
        private VoltDBConnectionPool pool;
        private bool isPersistent;

        /// <summary>Initializes a new instance of the <see cref="VoltConnectionProxy"/> class.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        public VoltConnectionProxy(ConnectionSettings settings, bool isPersistent)
          : this((VoltDBConnectionPool)null, settings, isPersistent)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="VoltConnectionProxy"/> class.</summary>
        /// <param name="pool">The pool.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        public VoltConnectionProxy(
          VoltDBConnectionPool pool,
          ConnectionSettings settings,
          bool isPersistent = false)
        {
            this.pool = pool;
            this.connection = VoltConnection.Create(settings);
            this.isPersistent = isPersistent;
        }

        /// <summary>Gets the adhoc.</summary>
        public AdhocAccess Adhoc
        {
            get
            {
                return this.connection.Adhoc;
            }
        }

        /// <summary>Gets the procedures.</summary>
        public ProcedureAccess Procedures
        {
            get
            {
                return this.connection.Procedures;
            }
        }

        /// <summary>Gets the system access.</summary>
        public SystemAccess SystemAccess => connection.System;

        /// <summary>Gets a value indicating whether this instance is healthy.</summary>
        public bool IsHealthy
        {
            get
            {
                return this.connection.Status == ConnectionStatus.Connecting || this.connection.Status == ConnectionStatus.Connected;
            }
        }

        /// <summary>Opens this instance.</summary>
        public IVoltConnection Open()
        {
            if (this.connection.Status == ConnectionStatus.Connected || this.connection.Status == ConnectionStatus.Connecting)
                return (IVoltConnection)this;
            this.connection.Open();
            return (IVoltConnection)this;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            CancelEventArgs args = new CancelEventArgs();
            if (this.pool != null && this.IsHealthy)
                this.pool.ReleaseConnection(this, args);
            if (this.isPersistent || args.Cancel)
                return;
            this.connection.Dispose();
        }
    }
}

using System;

namespace VoltDB.Data.Client
{
    ///<summary>
    ///The volt connection
    ///</summary>
    public interface IVoltConnection : IDisposable
    {
        /// <summary>Gets the adhoc.</summary>
        AdhocAccess Adhoc { get; }

        /// <summary>Gets a value indicating whether this instance is healthy.</summary>
        bool IsHealthy { get; }

        /// <summary>Gets the procedures.</summary>
        ProcedureAccess Procedures { get; }

        /// <summary>Gets the system access.</summary>
        SystemAccess SystemAccess { get; }

        /// <summary>Opens this instance.</summary>
        IVoltConnection Open();
    }
}

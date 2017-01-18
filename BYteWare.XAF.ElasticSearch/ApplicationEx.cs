namespace BYteWare.XAF
{
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.SystemModule;
    using DevExpress.ExpressApp.Xpo;
    using DevExpress.Xpo;
    using DevExpress.Xpo.DB;
    using DevExpress.Xpo.DB.Helpers;
    using DevExpress.Xpo.Helpers;
    using Fasterflect;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Database Engine Types
    /// </summary>
    public enum DBType
    {
        /// <summary>
        /// SQL Server
        /// </summary>
        MSSql,

        /// <summary>
        /// SQL Anywhere
        /// </summary>
        SA,

        /// <summary>
        /// XPO InMemory DataSet
        /// </summary>
        InMemory,

        /// <summary>
        /// Unknown Engine
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Extension Methods for a XAF Application
    /// </summary>
    public static class ApplicationEx
    {
        /// <summary>
        /// Returns the connection string which is used for the application
        /// </summary>
        /// <param name="application">A XafApplication instance</param>
        /// <returns>Database Connection string</returns>
        [CLSCompliant(false)]
        public static string GetConnectionString(this XafApplication application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            if (application.ObjectSpaceProvider != null)
            {
                var xposp = application.ObjectSpaceProvider as XPObjectSpaceProvider;
                if (xposp != null)
                {
                    var dsp = (IXpoDataStoreProvider)xposp.GetPropertyValue("DataStoreProvider", Flags.InstanceAnyVisibility);
                    if (dsp != null)
                    {
                        return dsp.ConnectionString;
                    }
                    else
                    {
                        return xposp.ConnectionString;
                    }
                }
                else
                {
                    return application.ObjectSpaceProvider.ConnectionString;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the provider name from the Connection String
        /// </summary>
        /// <param name="connectionString">A DataBase Connection String</param>
        /// <returns>The provider name used in the Connection String</returns>
        public static string XpoProviderName(string connectionString)
        {
            var helper = new ConnectionStringParser(connectionString);
            var providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
            if (string.IsNullOrWhiteSpace(providerType))
            {
                if (helper.GetPartByName("Initial Catalog").Length != 0 && helper.GetPartByName("Provider").Length == 0)
                {
                    providerType = MSSqlConnectionProvider.XpoProviderTypeString;
                }
                else
                {
                    var provider = helper.GetPartByName("Provider");
                    if (provider.IndexOf("microsoft.ace.oledb", StringComparison.OrdinalIgnoreCase) >= 0 || provider.IndexOf("microsoft.jet.oledb", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        providerType = AccessConnectionProvider.XpoProviderTypeString;
                    }
                }
            }
            return providerType;
        }

        /// <summary>
        /// Returns the DataBase Engine Type used in the connection of the ObjectSpace
        /// </summary>
        /// <param name="objectSpace">An IObjectSpace instance</param>
        /// <returns>DataBase Engine Type</returns>
        [CLSCompliant(false)]
        public static DBType GetDBType(this IObjectSpace objectSpace)
        {
            var type = DBType.Unknown;
            var os = objectSpace as XPObjectSpace;
            if (os != null)
            {
                type = GetDBType(os.Session);
            }
            return type;
        }

        /// <summary>
        /// Returns the DataBase Engine Type used in the connection of the Session
        /// </summary>
        /// <param name="session">A XPO session</param>
        /// <returns>DataBase Engine Type</returns>
        public static DBType GetDBType(this Session session)
        {
            var type = DBType.Unknown;
            if (session != null)
            {
                var conn = session.DataLayer != null && session.DataLayer.Connection != null ? session.DataLayer.Connection : session.Connection;
                if (conn == null)
                {
                    var dataLayer = session.DataLayer as BaseDataLayer;
                    if (dataLayer != null && dataLayer.ConnectionProvider != null)
                    {
                        var connProvider = dataLayer.ConnectionProvider as ConnectionProviderSql;
                        if (connProvider != null)
                        {
                            conn = connProvider.Connection;
                        }
                        else if (dataLayer.ConnectionProvider is DataSetDataStore)
                        {
                            type = DBType.InMemory;
                        }
                    }
                }
                if (conn != null)
                {
                    var connType = conn.GetType().Name;
                    if (connType.Equals("SQLConnection", StringComparison.OrdinalIgnoreCase))
                    {
                        type = DBType.MSSql;
                    }
                    else if (connType.Equals("SAConnection", StringComparison.OrdinalIgnoreCase))
                    {
                        type = DBType.SA;
                    }
                }
            }
            return type;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
#if DB2
using IBM.Data.Db2;
#endif

using System.Drawing;

namespace IA_ConverterCommons;

public sealed class DatabaseConnection : IDisposable
{
    private static DatabaseConnection instance;
    private static readonly object lockObject = new object();
    private DbConnection connection;
    private DbTransaction transaction;

    public static DatabaseConnection Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new DatabaseConnection();
                    }
                }
            }
            return instance;
        }
    }
    public DbConnection? Connection => GetConnection();

    private DbConnection? MakeConn()
    {
        DbConnection? con = null;
        try
        {
            var isSqlServerCon = AppSettings.Settings.IsSqlServer;
            var connectionString = isSqlServerCon ? AppSettings.Settings.SqlConnectionString : AppSettings.Settings.DB2ConnectionString;
#if !DB2
            if (AppSettings.Settings.IsSqlServer)
                con = new SqlConnection(connectionString);
#endif

#if DB2
            if (!AppSettings.Settings.IsSqlServer)
                con = new DB2Connection(connectionString);
#endif
        }
        catch
        {
            if (!AppSettings.TestSet.IsTest)
                throw;
        }

        return con;
    }

    private DatabaseConnection()
    {
        connection = MakeConn();
    }

    //public DbCommand CreateCommand(string procName)
    //{
    //    DbCommand command = null;
    //    if (AppSettings.Settings.IsSqlServer)
    //        command = new SqlCommand(procName, (SqlConnection)Connection);
    //    else
    //        command = new DB2Command(procName, (DB2Connection)Connection);

    //    return command;
    //}

    private DbConnection? GetConnection()
    {
        if (connection == null)
        {
            connection = MakeConn();
        }

        if (connection != null && connection.State != System.Data.ConnectionState.Open)
        {
            connection?.Open();
        }

        return connection;
    }

    public DbTransaction GetTransaction()
    {
        if (transaction == null)
        {
            transaction = connection.BeginTransaction();
        }

        return transaction;
    }

    public void CommitTransaction()
    {
        if (transaction != null)
        {
            transaction.Commit();
            transaction = null;
        }
    }

    public void EndTransaction()
    {
        if (transaction != null)
        {
            CommitTransaction();
        }

        if (connection != null && connection.State != System.Data.ConnectionState.Closed)
        {
            connection.Close();
            connection.Dispose();
            connection = null;
        }
    }

    public void RollbackTransaction()
    {
        if (transaction != null)
        {
            transaction.Rollback();
            transaction = null;
        }
    }

    public void SavePoint(string point)
    {
        if (transaction != null)
        {
            transaction.Release($"{point}");
            transaction.Save($"{point}");
        }
    }

    public void Dispose()
    {
        //if (transaction != null)
        //{
        //    transaction.Dispose();
        //    transaction = null;
        //}
        //if (connection != null && connection.State != System.Data.ConnectionState.Closed)
        //{
        //    connection.Close();
        //    connection.Dispose();
        //}
    }
}

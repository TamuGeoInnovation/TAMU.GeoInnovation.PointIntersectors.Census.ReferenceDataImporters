using System;
using System.Collections.Generic;
using System.Text;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Databases.SchemaManagers;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using USC.GISResearchLab.Common.Diagnostics.TraceEvents;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.ApplicationStates.Managers
{
    public class TransactionManager
    {

        #region Properties

        public TraceSource TraceSource { get; set; }
        public string ApplicationConnectionString { get; set; }
        public DataProviderType ApplicationDataProviderType { get; set; }
        public DatabaseType ApplicationDatabaseType { get; set; }
        public string ApplicationPathToDatabaseDlls { get; set; }

        private IQueryManager _QueryManager;
        public IQueryManager QueryManager
        {
            get
            {
                if (_QueryManager == null)
                {
                    _QueryManager = new QueryManager(ApplicationPathToDatabaseDlls, ApplicationDataProviderType, ApplicationDatabaseType, ApplicationConnectionString);
                }
                return _QueryManager;
            }
        }

        public bool TransactionRunning { get; set; }


        #endregion

        public TransactionManager(TraceSource traceSource)
        {
            TraceSource = traceSource;
        }

        public bool RollBackTransaction(string name)
        {
            bool ret = false;

            try
            {

                if (TransactionRunning)
                {
                    TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "rolling back last transaction: " + name);
                    string sql = "ROLLBACK TRANSACTION " + name;
                    SqlCommand cmd = new SqlCommand(sql);

                    QueryManager.AddParameters(cmd.Parameters);
                    QueryManager.ExecuteNonQuery(CommandType.Text, cmd.CommandText, true);

                    TransactionRunning = false;
                }

                ret = true;

            }
            catch (Exception exc)
            {
                string msg = "Error rolling back transaction: " + name + " - " + exc.Message; ;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public bool CommitTransaction(string name)
        {
            bool ret = false;

            try
            {
                if (TransactionRunning)
                {
                    TraceSource.TraceEvent(TraceEventType.Verbose, (int)ProcessEvents.Completing, "committing transaction: " + name);
                    string sql = "COMMIT TRANSACTION " + name;
                    SqlCommand cmd = new SqlCommand(sql);

                    QueryManager.AddParameters(cmd.Parameters);
                    QueryManager.ExecuteNonQuery(CommandType.Text, cmd.CommandText, true);

                    TransactionRunning = false;
                }
                ret = true;

            }
            catch (Exception exc)
            {
                string msg = "Error committing transaction: " + name + " - " + exc.Message; ;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public bool BeginTransaction(string name)
        {
            bool ret = false;

            try
            {
                TraceSource.TraceEvent(TraceEventType.Verbose, (int)ProcessEvents.Completing, "beginning transaction: " + name);
                string sql = "BEGIN TRANSACTION " + name;
                SqlCommand cmd = new SqlCommand(sql);

                QueryManager.AddParameters(cmd.Parameters);
                QueryManager.ExecuteNonQuery(CommandType.Text, cmd.CommandText, true);

                TransactionRunning = true;
                ret = true;

            }
            catch (Exception exc)
            {
                string msg = "Error beginning transaction: " + name + " - " + exc.Message; ;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }
    }
}

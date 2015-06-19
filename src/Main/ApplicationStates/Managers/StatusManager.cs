using System;
using System.Collections.Generic;
using System.Text;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Databases.SchemaManagers;
using System.Diagnostics;
using System.Data.SqlClient;
using USC.GISResearchLab.Common.Utils.Databases;
using System.Data;
using USC.GISResearchLab.Common.Diagnostics.TraceEvents;
using USC.GISResearchLab.Common.Databases.StoredProcedures;
using USC.GISResearchLab.Common.Databases.ImportStatusManagers;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.ApplicationStates.Managers
{
    //public enum Statuses { unknown, start, end, cancelled, success, exception };

    public class StatusManager
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

        private ISchemaManager _SchemaManager;
        public ISchemaManager SchemaManager
        {
            get
            {
                if (_SchemaManager == null)
                {
                    _SchemaManager = new SchemaManager(ApplicationPathToDatabaseDlls, ApplicationDataProviderType, ApplicationDatabaseType, ApplicationConnectionString);
                }
                return _SchemaManager;
            }
        }

        #endregion

        public StatusManager(TraceSource traceSource)
        {
            TraceSource = traceSource;
        }

        public string GetStatusString(Statuses status)
        {
            string ret = "Unknown";
            switch (status)
            {
                case Statuses.cancelled:
                    ret = "Cancelled";
                    break;
                case Statuses.end:
                    ret = "Finished";
                    break;
                case Statuses.exception:
                    ret = "Exception";
                    break;
                case Statuses.start:
                    ret = "Started";
                    break;
                case Statuses.success:
                    ret = "Success";
                    break;
                case Statuses.unknown:
                    ret = "Unknown";
                    break;
                default:
                    ret = "Unknown";
                    break;
            }
            return ret;
        }

        public void InitializeConnections()
        {

        }

        public void CreateStoredProcedures()
        {
            try
            {
                SchemaManager.QueryManager.Connection.Open();

                ForiegnKeyRemover foriegnKeyRemover = new ForiegnKeyRemover(SchemaManager.QueryManager.Connection.Database);
                string dropForeignKeysDropSql = foriegnKeyRemover.GetDropSQL();
                string dropForeignKeysCreateSql = foriegnKeyRemover.GetCreateSQL();

                SchemaManager.AddStoredProcedureToDatabase(dropForeignKeysDropSql);
                SchemaManager.AddStoredProcedureToDatabase(dropForeignKeysCreateSql);
            }
            catch (Exception e)
            {
                string msg = "Error CreateStoredProcedures: " + e.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);

                if (SchemaManager.QueryManager.Connection != null)
                {
                    if (SchemaManager.QueryManager.Connection.State != ConnectionState.Closed)
                    {
                        SchemaManager.QueryManager.Close();
                    }
                }

                throw new Exception(msg, e);
            }
            finally
            {
                if (SchemaManager.QueryManager.Connection != null)
                {
                    if (SchemaManager.QueryManager.Connection.State != ConnectionState.Closed)
                    {
                        SchemaManager.QueryManager.Close();
                    }
                }
            }
        }

        public void CreateImportStatusStateTable(string tableName, bool restart)
        {
            try
            {
                SchemaManager.QueryManager.Connection.Open();
                
                if (restart)
                {
                    SchemaManager.RemoveTableFromDatabase(tableName);
                }

                string sql = "use " + QueryManager.Connection.Database + "; ";
                sql += "IF NOT EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + tableName + "')";
                sql += "CREATE TABLE [" + tableName + "] (";
                sql += "id bigint IDENTITY (1,1) NOT NULL,";
                sql += "state varchar(255) DEFAULT NULL,";
                sql += "status varchar(255) DEFAULT NULL,";
                sql += "startDate datetime DEFAULT NULL,";
                sql += "endDate datetime DEFAULT NULL,";
                sql += "message varchar(1000) DEFAULT NULL,";
                sql += "PRIMARY KEY  (id)";
                sql += ");";


                SchemaManager.AddTableToDatabase(tableName, sql);
            }
            catch (Exception e)
            {
                string msg = "Error CreateImportStatusStateTable: " + e.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);

                if (SchemaManager.QueryManager.Connection != null)
                {
                    if (SchemaManager.QueryManager.Connection.State != ConnectionState.Closed)
                    {
                        SchemaManager.QueryManager.Close();
                    }
                }

                throw new Exception(msg, e);
            }
            finally
            {
                if (SchemaManager.QueryManager.Connection != null)
                {
                    if (SchemaManager.QueryManager.Connection.State != ConnectionState.Closed)
                    {
                        SchemaManager.QueryManager.Close();
                    }
                }
            }
        }

        public void CreateImportStatusCountyTable(string tableName, bool restart)
        {
            try
            {
                SchemaManager.QueryManager.Connection.Open();

                if (restart)
                {
                    SchemaManager.RemoveTableFromDatabase(tableName);
                }

                string sql = "use " + QueryManager.Connection.Database + "; ";
                sql += "IF NOT EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + tableName + "')";
                sql += "CREATE TABLE [" + tableName + "] (";
                sql += "id bigint IDENTITY (1,1) NOT NULL,";
                sql += "state varchar(255) DEFAULT NULL,";
                sql += "county varchar(255) DEFAULT NULL,";
                sql += "status varchar(255) DEFAULT NULL,";
                sql += "startDate datetime DEFAULT NULL,";
                sql += "endDate datetime DEFAULT NULL,";
                sql += "message varchar(1000) DEFAULT NULL,";
                sql += "PRIMARY KEY  (id)";
                sql += ");";

                SchemaManager.AddTableToDatabase(tableName, sql);
            }
            catch (Exception e)
            {
                string msg = "Error CreateImportStatusStateTable: " + e.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);

                if (SchemaManager.QueryManager.Connection != null)
                {
                    if (SchemaManager.QueryManager.Connection.State != ConnectionState.Closed)
                    {
                        SchemaManager.QueryManager.Close();
                    }
                }

                throw new Exception(msg, e);
            }
            finally
            {
                if (SchemaManager.QueryManager.Connection != null)
                {
                    if (SchemaManager.QueryManager.Connection.State != ConnectionState.Closed)
                    {
                        SchemaManager.QueryManager.Close();
                    }
                }
            }
        }

        public void CreateImportStatusFileTable(string tableName, bool restart)
        {
            try
            {
                SchemaManager.QueryManager.Connection.Open();

                if (restart)
                {
                    SchemaManager.RemoveTableFromDatabase(tableName);
                }

                string sql = "use " + QueryManager.Connection.Database + "; ";
                sql += "IF NOT EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + tableName + "')";
                sql += "CREATE TABLE [" + tableName + "] (";
                sql += "id bigint IDENTITY (1,1) NOT NULL,";
                sql += "state varchar(255) DEFAULT NULL,";
                sql += "county varchar(255) DEFAULT NULL,";
                sql += "filename varchar(255) DEFAULT NULL,";
                sql += "status varchar(255) DEFAULT NULL,";
                sql += "startDate datetime DEFAULT NULL,";
                sql += "endDate datetime DEFAULT NULL,";
                sql += "message varchar(1000) DEFAULT NULL,";
                sql += "PRIMARY KEY  (id)";
                sql += ");";

                SchemaManager.AddTableToDatabase(tableName, sql);
            }
            catch (Exception e)
            {
                string msg = "Error CreateImportStatusStateTable: " + e.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);

                if (SchemaManager.QueryManager.Connection != null)
                {
                    if (SchemaManager.QueryManager.Connection.State != ConnectionState.Closed)
                    {
                        SchemaManager.QueryManager.Close();
                    }
                }

                throw new Exception(msg, e);
            }
            finally
            {
                if (SchemaManager.QueryManager.Connection != null)
                {
                    if (SchemaManager.QueryManager.Connection.State != ConnectionState.Closed)
                    {
                        SchemaManager.QueryManager.Close();
                    }
                }
            }
        }

        public bool CheckStatusStateAlreadyDone(string tableName, string state)
        {
            bool ret = false;

            try
            {

                TraceSource.TraceEvent(TraceEventType.Verbose, (int)ProcessEvents.Completing, "checking state status: " + state);

                string sql = "select id FROM [" + tableName + "]";
                sql += " where ";
                sql += " state=@state";
                sql += " and ";
                sql += " status='Finished'";

                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("state", SqlDbType.VarChar, state));

                QueryManager.AddParameters(cmd.Parameters);
                int id = QueryManager.ExecuteScalarInt(CommandType.Text, cmd.CommandText, true);

                if (id > 0)
                {
                    ret = true;
                }

            }
            catch (Exception exc)
            {
                string msg = "Error checking state status: " + state;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public bool CheckStatusCountyAlreadyDone(string tableName, string county)
        {
            bool ret = false;

            try
            {

                TraceSource.TraceEvent(TraceEventType.Verbose, (int)ProcessEvents.Completing, "checking county status: " + county);

                string sql = "select id FROM [" + tableName + "]";
                sql += " where ";
                sql += " county=@county";
                sql += " and ";
                sql += " status='Finished'";

                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("county", SqlDbType.VarChar, county));

                QueryManager.AddParameters(cmd.Parameters);
                int id = QueryManager.ExecuteScalarInt(CommandType.Text, cmd.CommandText, true);

                if (id > 0)
                {
                    ret = true;
                }

            }
            catch (Exception exc)
            {
                string msg = "Error checking county status: " + county;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public bool CheckStatusFileAlreadyDone(string tableName, string state, string county, string file)
        {
            bool ret = false;

            try
            {

                TraceSource.TraceEvent(TraceEventType.Verbose, (int)ProcessEvents.Completing, "checking file status: " + file);

                string sql = "select id FROM [" + tableName + "]";
                sql += " where ";
                sql += " filename=@filename";
                sql += " and ";
                sql += " state=@state";
                sql += " and ";
                sql += " county=@county";
                sql += " and ";
                sql += " status='Finished'";

                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("filename", SqlDbType.VarChar, file));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("state", SqlDbType.VarChar, state));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("county", SqlDbType.VarChar, county));

                QueryManager.AddParameters(cmd.Parameters);
                int id = QueryManager.ExecuteScalarInt(CommandType.Text, cmd.CommandText, true);

                if (id > 0)
                {
                    ret = true;
                }

            }
            catch (Exception exc)
            {
                string msg = "Error checking file status: " + file;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public bool UpdateStatusFile(string tableName, string state, string county, string file, Statuses status)
        {
            return UpdateStatusFile(tableName, state, county, file, status, null);
        }

        public bool UpdateStatusFile(string tableName, string state, string county, string file, Statuses status, string message)
        {
            bool ret = false;

            try
            {

                TraceSource.TraceEvent(TraceEventType.Verbose, (int)ProcessEvents.Completing, "updating file status: " + file + " status: " + status);

                string sql = "select id FROM [" + tableName + "]";
                sql += " where ";
                sql += " filename=@filename";
                sql += " and ";
                sql += " county=@county";
                sql += " and ";
                sql += " state=@state";

                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("filename", SqlDbType.VarChar, file));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("county", SqlDbType.VarChar, county));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("state", SqlDbType.VarChar, state));

                QueryManager.AddParameters(cmd.Parameters);
                int id = QueryManager.ExecuteScalarInt(CommandType.Text, cmd.CommandText, true);

                if (id <= 0 || status == Statuses.start)
                {
                    InsertStatusFile(tableName, state, county,  file);
                }
                else
                {
                    sql = "update " + tableName;
                    sql += " set ";
                    sql += " status=@status,";
                    sql += " endDate=@endDate,";
                    sql += " message=@message";
                    sql += " where ";
                    sql += " id=@id ";

                    cmd = new SqlCommand(sql);
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("status", SqlDbType.VarChar, GetStatusString(status)));
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("endDate", SqlDbType.DateTime, DateTime.Now));
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("message", SqlDbType.VarChar, message));
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("id", SqlDbType.BigInt, id));

                    QueryManager.AddParameters(cmd.Parameters);
                    QueryManager.ExecuteNonQuery(CommandType.Text, cmd.CommandText, true);
                }


                ret = true;

            }
            catch (Exception exc)
            {
                string msg = "Error updating file status: " + file + " status: " + status;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public bool UpdateStatusState(string tableName, string state, Statuses status)
        {
            return UpdateStatusState(tableName, state, status, null);
        }

        public bool UpdateStatusState(string tableName, string state, Statuses status, string message)
        {
            bool ret = false;

            try
            {

                TraceSource.TraceEvent(TraceEventType.Verbose, (int)ProcessEvents.Completing, "updating state status: " + state + " status: " + status);

                string sql = "select id FROM [" + tableName + "]";
                sql += " where ";
                sql += " state=@state";

                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("state", SqlDbType.VarChar, state));

                QueryManager.AddParameters(cmd.Parameters);
                int id = QueryManager.ExecuteScalarInt(CommandType.Text, cmd.CommandText, true);

                if (id <= 0 || status == Statuses.start)
                {
                    InsertStatusState(tableName, state);
                }
                else
                {
                    sql = "update [" + tableName + "]";
                    sql += " set ";
                    sql += " status=@status,";
                    sql += " endDate=@endDate,";
                    sql += " message=@message";
                    sql += " where ";
                    sql += " id=@id ";

                    cmd = new SqlCommand(sql);
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("status", SqlDbType.VarChar, GetStatusString(status)));
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("endDate", SqlDbType.DateTime, DateTime.Now));
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("message", SqlDbType.VarChar, message));
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("id", SqlDbType.BigInt, id));

                    QueryManager.AddParameters(cmd.Parameters);
                    QueryManager.ExecuteNonQuery(CommandType.Text, cmd.CommandText, true);
                }

                ret = true;

            }
            catch (Exception exc)
            {
                string msg = "Error updating state status: " + state + " status: " + status + ":" + exc.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public bool UpdateStatusCounty(string tableName, string state, string county, Statuses status)
        {
            return UpdateStatusCounty(tableName, state, county, status, null);
        }

        public bool UpdateStatusCounty(string tableName, string state, string county, Statuses status, string message)
        {
            bool ret = false;

            try
            {

                TraceSource.TraceEvent(TraceEventType.Verbose, (int)ProcessEvents.Completing, "updating county status: " + county + " status: " + status);

                string sql = "select id FROM [" + tableName + "]";
                sql += " where ";
                sql += " state=@state";
                sql += " and ";
                sql += " county=@county";

                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("state", SqlDbType.VarChar, state));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("county", SqlDbType.VarChar, county));

                QueryManager.AddParameters(cmd.Parameters);
                int id = QueryManager.ExecuteScalarInt(CommandType.Text, cmd.CommandText, true);

                

                if (id <= 0 || status == Statuses.start)
                {
                    InsertStatusCounty(tableName, state, county);
                }
                else
                {
                    sql = "update [" + tableName + "]";
                    sql += " set ";
                    sql += " status=@status,";
                    sql += " endDate=@endDate,";
                    sql += " message=@message";
                    sql += " where ";
                    sql += " id=@id ";

                    cmd = new SqlCommand(sql);
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("status", SqlDbType.VarChar, GetStatusString(status)));
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("endDate", SqlDbType.DateTime, DateTime.Now));
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("message", SqlDbType.VarChar, message));
                    cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("id", SqlDbType.BigInt, id));

                    QueryManager.AddParameters(cmd.Parameters);
                    QueryManager.ExecuteNonQuery(CommandType.Text, cmd.CommandText, true);
                }

                ret = true;

            }
            catch (Exception exc)
            {
                string msg = "Error updating county status: " + county + " status: " + status + ":" + exc.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public bool InsertStatusFile(string tableName, string state, string county, string file)
        {
            bool ret = false;

            try
            {

                TraceSource.TraceEvent(TraceEventType.Verbose, (int)ProcessEvents.Completing, "inserting file status: " + file + " status: ");

                string sql = "INSERT into [" + tableName + "]";
                sql += " (";
                sql += " state,";
                sql += " county,";
                sql += " filename,";
                sql += " status,";
                sql += " startDate";
                sql += " )";
                sql += " VALUES ";
                sql += " (";
                sql += " @state,";
                sql += " @county,";
                sql += " @filename,";
                sql += " @status,";
                sql += " @startDate";
                sql += " )";


                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("state", SqlDbType.VarChar, state));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("county", SqlDbType.VarChar, county));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("filename", SqlDbType.VarChar, file));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("status", SqlDbType.VarChar, "Started"));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("startDate", SqlDbType.DateTime, DateTime.Now));


                QueryManager.AddParameters(cmd.Parameters);
                QueryManager.ExecuteNonQuery(CommandType.Text, cmd.CommandText, true);


                ret = true;

            }
            catch (Exception exc)
            {
                string msg = "Error inserting file status: " + file + " status: " + ":" + exc.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public bool InsertStatusState(string tableName, string state)
        {
            bool ret = false;

            try
            {

                TraceSource.TraceEvent(TraceEventType.Verbose, (int)ProcessEvents.Completing, "inserting state status: " + state + " status: ");

                string sql = "INSERT into [" + tableName + "]";
                sql += " (";
                sql += " state,";
                sql += " status,";
                sql += " startDate";
                sql += " )";
                sql += " VALUES ";
                sql += " (";
                sql += " @state,";
                sql += " @status,";
                sql += " @startDate";
                sql += " )";


                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("state", SqlDbType.VarChar, state));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("status", SqlDbType.VarChar, "Started"));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("startDate", SqlDbType.DateTime, DateTime.Now));

                QueryManager.AddParameters(cmd.Parameters);
                QueryManager.ExecuteNonQuery(CommandType.Text, cmd.CommandText, true);


                ret = true;

            }
            catch (Exception exc)
            {
                string msg = "Error inserting state status: " + state + " status: " + ":" + exc.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public bool InsertStatusCounty(string tableName, string state, string county)
        {
            bool ret = false;

            try
            {

                TraceSource.TraceEvent(TraceEventType.Verbose, (int)ProcessEvents.Completing, "inserting county status: " + county + " status: ");

                string sql = "INSERT into [" + tableName + "]";
                sql += " (";
                sql += " state,";
                sql += " county,";
                sql += " status,";
                sql += " startDate";
                sql += " )";
                sql += " VALUES ";
                sql += " (";
                sql += " @state,";
                sql += " @county,";
                sql += " @status,";
                sql += " @startDate";
                sql += " )";


                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("state", SqlDbType.VarChar, state));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("county", SqlDbType.VarChar, county));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("status", SqlDbType.VarChar, "Started"));
                cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter("startDate", SqlDbType.DateTime, DateTime.Now));

                QueryManager.AddParameters(cmd.Parameters);
                QueryManager.ExecuteNonQuery(CommandType.Text, cmd.CommandText, true);


                ret = true;

            }
            catch (Exception exc)
            {
                string msg = "Error inserting county status: " + county + " status: " + ":" + exc.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }
    }
}

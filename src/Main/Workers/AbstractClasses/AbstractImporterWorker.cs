/*
 * Copyright ?2008 Daniel W. Goldberg
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;

using USC.GISResearchLab.Common.Utils.Directories;
using USC.GISResearchLab.Common.Utils.Strings;
using USC.GISResearchLab.Common.Utils.Files;
using USC.GISResearchLab.Common.Diagnostics.TraceEvents;
using USC.GISResearchLab.Common.Databases;
using USC.GISResearchLab.Common.Census;
using USC.GISResearchLab.Common.Utils.Databases;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Databases.SchemaManagers;


using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.ApplicationStates.Managers;
using USC.GISResearchLab.Common.Core.Databases.BulkCopys;
//using USC.GISResearchLab.AddressProcessing.Core.Standardizing.StandardizedAddresses.Lines.LastLines;

using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;
using USC.GISResearchLab.Common.Databases.ImportStatusManagers;





namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public abstract class AbstractImporterWorker : IImporterWorker
    {
        #region Properties

        public IImportStatusManager StatusManager { get; set; }
        public TransactionManager TransactionManager{ get; set; }
        public bool Restart { get; set; }
        public bool TransactionRunning { get; set; }
        public string ApplicationConnectionString { get; set; }
        public string ApplicationDatabaseName { get; set; }
        public string ApplicationDatabaseUserName { get; set; }
        public string ApplicationDatabasePassword { get; set; }
        public DataProviderType ApplicationDataProviderType { get; set; }
        public DatabaseType ApplicationDatabaseType { get; set; }
        public TraceSource TraceSource { get; set; }
        public ArrayList Directories { get; set; }
        public BackgroundWorker BackgroundWorker { get; set; }
        public DoWorkEventArgs DoWorkEventArgs { get; set; }
        public ITigerFileLayout CurrentTigerFileLayout { get; set; }
        public string ApplicationPathToDatabaseDlls { get; set; }
        public ProgressState ProgressState { get; set; }

        public int BulkCopyBatchSize { get; set; }
        public int BulkCopyReportAfter { get; set; }

        private IQueryManager _QueryManager;
        public IQueryManager QueryManager
        {
            get
            {
                //if (_QueryManager == null)
                //{
                //    _QueryManager = new QueryManager(ApplicationPathToDatabaseDlls, ApplicationDataProviderType, ApplicationDatabaseType, ApplicationConnectionString);
                //}
                return _QueryManager.Clone();
            }
        }

        private ISchemaManager _SchemaManager;
        public ISchemaManager SchemaManager
        {
            get
            {
                //if (_SchemaManager == null)
                //{
                //    _SchemaManager = new SchemaManager(ApplicationPathToDatabaseDlls, ApplicationDataProviderType, ApplicationDatabaseType, ApplicationConnectionString);
                //}
                return (ISchemaManager)_SchemaManager.Clone();
            }
        }

        #endregion

        public AbstractImporterWorker()
        {
            ProgressState = new ProgressState();
            BulkCopyBatchSize = 1;
            BulkCopyReportAfter = 1;
        }

        public AbstractImporterWorker(TraceSource traceSource, BackgroundWorker backgroundWorker, IQueryManager queryManager, ISchemaManager schemaManager)
        {
            TraceSource = traceSource;
            BackgroundWorker = backgroundWorker;
            ProgressState = new ProgressState();
            _QueryManager = queryManager;
            _SchemaManager = schemaManager;

            BulkCopyBatchSize = 1;
            BulkCopyReportAfter = 1;
        }

        public abstract bool RunImports(string topDirectory);
        //public abstract void UpdateRecordsCompletedCount(int completed);
        //public abstract void UpdateRecordsCompletedCount(int completed, int total);
        //public abstract void RecordsRead(int completed, int total);
    
        public bool Run(DoWorkEventArgs e, string topDirectory, bool restart)
        {

            StatusManager = ImportStatusManagerFactory.GetImportStatusManager(ApplicationPathToDatabaseDlls, ApplicationDataProviderType, ApplicationConnectionString);
            //StatusManager = new StatusManager(TraceSource);
            //StatusManager.ApplicationPathToDatabaseDlls = ApplicationPathToDatabaseDlls;
            //StatusManager.ApplicationConnectionString = ApplicationConnectionString;
            //StatusManager.ApplicationDatabaseType = ApplicationDatabaseType;
            //StatusManager.ApplicationDataProviderType = ApplicationDataProviderType;
            //StatusManager.InitializeConnections();

            StatusManager.DefaultDatabase = ApplicationDatabaseName;

            TransactionManager = new TransactionManager(TraceSource);
            TransactionManager.ApplicationPathToDatabaseDlls = ApplicationPathToDatabaseDlls;
            TransactionManager.ApplicationConnectionString = ApplicationConnectionString;
            TransactionManager.ApplicationDatabaseType = ApplicationDatabaseType;
            TransactionManager.ApplicationDataProviderType = ApplicationDataProviderType;

            bool ret = false;
            DoWorkEventArgs = e;
            Restart = restart;

            try
            {

                RunImports(topDirectory);
                ret = true;

            }
            catch (Exception exc)
            {
                string msg = "Error processing direcories";
                msg += exc.ToString();
                throw new Exception(msg);
            }
            return ret;
        }

        public void sqlBulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            UpdateRecordsCompletedCount(Convert.ToInt32(e.RowsCopied));

            if (BackgroundWorker.CancellationPending)
            {
                e.Abort = true;
            }
        }

        public bool CreateIndex(string tableName, string sql)
        {
            bool ret = false;

            try
            {
                if (!String.IsNullOrEmpty(sql))
                {
                    TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "creating index: " + tableName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql);
                    SchemaManager.AddIndexToDatabase(tableName, sql);
                }
                ret = true;
            }
            catch (Exception exc)
            {
                string msg = "Error CreateIndex: " + tableName + " - " + exc.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public void CreateTable(string tableName, string createTableSql, bool dropFirst)
        {
            string sql = "";

            try
            {
                if (dropFirst)
                {

                    //sql += "exec utility$removeRelationships @parent_table_name = '" + tableName + "'";
                    //            qm.ExecuteNonQuery(CommandType.Text, sql, true);

                    SchemaManager.RemoveTableFromDatabase(tableName);
                }

                //sql += " IF NOT EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + tableName + "') ";
                //sql += createTableSql;

                SchemaManager.AddTableToDatabase(tableName, sql);
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in CreateStateTigerTable: " + e.Message, e);
            }
        }

        public void CreateStateTigerTable(ITigerFileLayout tigerFile, bool dropFirst)
        {
            string sql = "";

            try
            {
                if (dropFirst)
                { 
                    
                    //sql += "exec utility$removeRelationships @parent_table_name = '" + tigerFile.OutputTableName + "'";
                    //qm.ExecuteNonQuery(CommandType.Text, sql, true);

                    // then remove the table
                    //SchemaManager.RemoveTableFromDatabase(tigerFile.OutputTableName);

                    SchemaManager.RemoveTableFromDatabase(tigerFile.OutputTableName);
                }

                //sql = "";
                //switch (SchemaManager.DatabaseType)
                //{
                //    case DatabaseType.Npgsql:
                //        break;
                //    default:
                //        sql = " use " + QueryManager.Connection.Database + "; ";
                //        break;
                //}
                //sql += " IF NOT EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + tigerFile.OutputTableName + "') ";
                //sql += tigerFile.SQLCreateTable;

                SchemaManager.AddTableToDatabase(tigerFile.OutputTableName, tigerFile.SQLCreateTable);
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in CreateStateTigerTable: " + e.Message, e);
            }
        }

        public virtual void UpdateRecordsCompletedCount(int completed)
        {
            //ProgressState.ProgressStateRecords.Completed = completed;
            BackgroundWorker.ReportProgress(0, ProgressState);
        }

        public virtual void UpdateRecordsCompletedCount(int completed, int total)
        {
            //ProgressState.ProgressStateRecords.Completed = completed;
            //ProgressState.ProgressStateRecords.Total = total;
            BackgroundWorker.ReportProgress(0, ProgressState);
        }

        public virtual void RecordsRead(int completed, int total)
        {
            //ProgressState.ProgressStateRecords.Completed = completed;
            //ProgressState.ProgressStateRecords.Total = total;
            BackgroundWorker.ReportProgress(0, ProgressState);
        }
    }
}

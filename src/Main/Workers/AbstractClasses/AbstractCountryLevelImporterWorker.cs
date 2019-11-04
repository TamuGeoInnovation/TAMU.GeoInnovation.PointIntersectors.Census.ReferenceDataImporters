/*
 * Copyright © 2008 Daniel W. Goldberg
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
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Factories.Tiger2000.CountryFiles;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Factories.Tiger2010.CountryFiles;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Factories.Tiger2015.CountryFiles;
//using USC.GISResearchLab.AddressProcessing.Core.Standardizing.StandardizedAddresses.Lines.LastLines;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Core.Databases.BulkCopys;
using USC.GISResearchLab.Common.Databases.ImportStatusManagers;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Databases.SchemaManagers;
using USC.GISResearchLab.Common.Diagnostics.TraceEvents;
using USC.GISResearchLab.Common.Shapefiles.ShapefileReaders;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public abstract class AbstractCountryLevelImporterWorker : AbstractImporterWorker, ICountryLevelImporterWorker
    {
        #region Properties

        public int FileCountTotal { get; set; }
        public int FileCountCompleted { get; set; }
        public new CountryLevelImporterProgressState ProgressState { get; set; }

        public bool ShouldDoStates2000 { get; set; }
        public bool ShouldDoCounties2000 { get; set; }
        public bool ShouldDoZcta32000 { get; set; }
        public bool ShouldDoZcta52000 { get; set; }
        public bool ShouldDoStates2008 { get; set; }
        public bool ShouldDoCounties2008 { get; set; }
        public bool ShouldDoZcta32008 { get; set; }
        public bool ShouldDoZcta52008 { get; set; }
        public bool ShouldDoStates2010 { get; set; }
        public bool ShouldDoStates2015 { get; set; }
        public bool ShouldDoCounties2010 { get; set; }
        public bool ShouldDoCounties2015 { get; set; }
        public bool ShouldDoZcta52010 { get; set; }
        public bool ShouldDoZcta52015 { get; set; }
        public bool ShouldDoMetDiv2010 { get; set; }
        public bool ShouldDoMetDiv2015 { get; set; }
        public bool ShouldDoCbsa2010 { get; set; }
        public bool ShouldDoCbsa2015 { get; set; }


        #endregion

        public AbstractCountryLevelImporterWorker() : base()
        {
            ProgressState = new CountryLevelImporterProgressState();
        }

        public AbstractCountryLevelImporterWorker(TraceSource traceSource, BackgroundWorker backgroundWorker, IQueryManager outputDataQueryManager, ISchemaManager schemaManager)
            : base(traceSource, backgroundWorker, outputDataQueryManager, schemaManager)
        {
            ProgressState = new CountryLevelImporterProgressState();
        }


        public override void UpdateRecordsCompletedCount(int completed)
        {
            ProgressState.ProgressStateRecords.Completed = completed;
            BackgroundWorker.ReportProgress(0, ProgressState);
        }

        public override void UpdateRecordsCompletedCount(int completed, int total)
        {
            ProgressState.ProgressStateRecords.Completed = completed;
            ProgressState.ProgressStateRecords.Total = total;
            BackgroundWorker.ReportProgress(0, ProgressState);
        }

        public override void RecordsRead(int completed, int total)
        {
            ProgressState.ProgressStateRecords.Completed = completed;
            ProgressState.ProgressStateRecords.Total = total;
            BackgroundWorker.ReportProgress(0, ProgressState);
        }

        public override bool RunImports(string topDirectory)
        {
            bool ret = false;

            try
            {
                TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "importing files");

                StatusManager.CreateImportStatusStateTable("import_status_states", Restart, ShouldRemoveStatusTablesFirst);
                StatusManager.CreateImportStatusCountyTable("import_status_counties", Restart, ShouldRemoveStatusTablesFirst);
                StatusManager.CreateImportStatusFileTable("import_status_files", Restart, ShouldRemoveStatusTablesFirst);

                StatusManager.CreateStoredProcedures(false);
                CreateNationTigerTables(Restart && ShouldRemoveOutputRecordsTableFirst);

                if (ShouldDoStates2000)
                {
                    FileCountTotal++;
                }

                if (ShouldDoCounties2000)
                {
                    FileCountTotal++;
                }

                if (ShouldDoZcta32000)
                {
                    FileCountTotal++;
                }

                if (ShouldDoZcta52000)
                {
                    FileCountTotal++;
                }

                if (ShouldDoStates2008)
                {
                    FileCountTotal++;
                }

                if (ShouldDoCounties2008)
                {
                    FileCountTotal++;
                }

                if (ShouldDoZcta32008)
                {
                    FileCountTotal++;
                }

                if (ShouldDoZcta52008)
                {
                    FileCountTotal++;
                }

                if (ShouldDoStates2010)
                {
                    FileCountTotal++;
                }

                if (ShouldDoStates2015)
                {
                    FileCountTotal++;
                }

                if (ShouldDoCounties2010)
                {
                    FileCountTotal++;
                }

                if (ShouldDoCounties2015)
                {
                    FileCountTotal++;
                }

                if (ShouldDoZcta52010)
                {
                    FileCountTotal++;
                }

                if (ShouldDoZcta52015)
                {
                    FileCountTotal++;
                }

                if (ShouldDoMetDiv2010)
                {
                    FileCountTotal++;
                }

                if (ShouldDoMetDiv2015)
                {
                    FileCountTotal++;
                }

                if (ShouldDoCbsa2010)
                {
                    FileCountTotal++;
                }
                if (ShouldDoCbsa2015)
                {
                    FileCountTotal++;
                }

                ProgressState.ProgressStateFiles.Total = FileCountTotal;
                BackgroundWorker.ReportProgress(0, ProgressState);

                if (ShouldDoStates2000)
                {
                    ITigerFileLayout tigerFile = States2000FileFactory.GetStates2000File(QueryManager, "us");
                    ImportTigerNationFile(topDirectory, tigerFile);
                    SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                }

                if (ShouldDoCounties2000)
                {
                    ITigerFileLayout tigerFile = County2000FileFactory.GetFile(QueryManager, "us");
                    ImportTigerNationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoZcta32000)
                {
                    ITigerFileLayout tigerFile = Zcta32000FileFactory.GetFile(QueryManager, "us");
                    ImportTigerNationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoZcta52000)
                {
                    ITigerFileLayout tigerFile = Zcta52000FileFactory.GetFile(QueryManager, "us");
                    ImportTigerNationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoStates2008)
                {
                    ITigerFileLayout tigerFile = States2000FileFactory.GetStates2000File(QueryManager, "us");
                    ImportTigerNationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoCounties2008)
                {
                    ITigerFileLayout tigerFile = County2000FileFactory.GetFile(QueryManager, "us");
                    ImportTigerNationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoZcta32008)
                {
                    ITigerFileLayout tigerFile = Zcta32000FileFactory.GetFile(QueryManager, "us");
                    ImportTigerNationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoZcta52008)
                {
                    ITigerFileLayout tigerFile = Zcta52000FileFactory.GetFile(QueryManager, "us");
                    ImportTigerNationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoStates2010)
                {
                    ITigerFileLayout tigerFile = States2010FileFactory.GetFile(QueryManager, "us");

                    if (ShouldUseUnzippedFolder)
                    {
                        ImportTiger2010NationFile(UnzippedFolder, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }
                    else
                    {
                        ImportTiger2010NationFile(topDirectory, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }

                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoStates2015)
                {
                    ITigerFileLayout tigerFile = States2015FileFactory.GetFile(QueryManager, "us");

                    if (ShouldUseUnzippedFolder)
                    {
                        //note we did not update this to 2015 since the function would be the same
                        ImportTiger2010NationFile(UnzippedFolder, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }
                    else
                    {
                        //note we did not update this to 2015 since the function would be the same
                        ImportTiger2010NationFile(topDirectory, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }

                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoCounties2010)
                {
                    ITigerFileLayout tigerFile = County2010FileFactory.GetFile(QueryManager, "us");

                    if (ShouldUseUnzippedFolder)
                    {
                        ImportTiger2010NationFile(UnzippedFolder, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }
                    else
                    {
                        ImportTiger2010NationFile(topDirectory, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }

                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoZcta52010)
                {
                    ITigerFileLayout tigerFile = Zcta52010FileFactory.GetFile(QueryManager, "us");

                    if (ShouldUseUnzippedFolder)
                    {
                        ImportTiger2010NationFile(UnzippedFolder, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }
                    else
                    {
                        ImportTiger2010NationFile(topDirectory, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }

                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoMetDiv2010)
                {
                    ITigerFileLayout tigerFile = MetDiv2010FileFactory.GetFile(QueryManager, "us");

                    if (ShouldUseUnzippedFolder)
                    {
                        ImportTiger2010NationFile(UnzippedFolder, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }
                    else
                    {
                        ImportTiger2010NationFile(topDirectory, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }

                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoCbsa2010)
                {
                    ITigerFileLayout tigerFile = Cbsa2010FileFactory.GetFile(QueryManager, "us");

                    if (ShouldUseUnzippedFolder)
                    {
                        ImportTiger2010NationFile(UnzippedFolder, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }
                    else
                    {
                        ImportTiger2010NationFile(topDirectory, tigerFile, ShouldUseUnzippedFolder, ShouldSkipExistingRecords);
                    }

                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }


                if (!BackgroundWorker.CancellationPending)
                {
                    ret = true;
                }

            }
            catch (Exception exc)
            {
                string msg = "Error importing direcories: " + exc.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
            }
            return ret;
        }

        public bool ImportTigerNationFile(string nationDirectory, ITigerFileLayout tigerFile)
        {
            bool ret = false;
            IDataReader dataReader = null;
            CurrentTigerFileLayout = tigerFile;
            try
            {
                if (!BackgroundWorker.CancellationPending)
                {


                    bool alreadyDone = StatusManager.CheckStatusFileAlreadyDone("import_status_files", "", "", tigerFile.FileName);
                    if (!alreadyDone)
                    {
                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "import file: " + tigerFile.FileName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");
                        StatusManager.UpdateStatusFile("import_status_files", "", "", tigerFile.FileName, Statuses.start);

                        ProgressState.ProgressStateFiles.Current = tigerFile.FileName;
                        ProgressState.ProgressStateFiles.Message = "Unzipping file";

                        ProgressState.ProgressStateRecords.ResetCounts();
                        ProgressState.ProgressStateRecords.ResetTimer();
                        BackgroundWorker.ReportProgress(0, ProgressState);

                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, " -- Reading File: " + tigerFile.FileName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                        ((ITigerShapefileFileLayout)tigerFile).RecordsRead += new FileLayouts.Delegates.RecordsReadHandler(RecordsRead);

                        dataReader = tigerFile.GetDataReaderFromZipFile(nationDirectory);

                        if (QueryManager.DatabaseType == DatabaseType.MongoDB)
                        {
                            ((ExtendedCatfoodShapefileDataReader)dataReader).IncludeSqlGeographyAsGeoJSON = true;
                            ((ExtendedCatfoodShapefileDataReader)dataReader).IncludeSqlGeometryAsGeoJSON = true;
                        }
                        else
                        {
                            ((ExtendedCatfoodShapefileDataReader)dataReader).IncludeSqlGeographyAsGeoJSON = false;
                            ((ExtendedCatfoodShapefileDataReader)dataReader).IncludeSqlGeometryAsGeoJSON = false;
                        }

                        if (dataReader != null)
                        {
                            try
                            {

                                TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, " -- Copy to Server: " + tigerFile.FileName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                                ProgressState.ProgressStateFiles.Current = tigerFile.FileName;
                                ProgressState.ProgressStateFiles.Message = "Copying to server";
                                BackgroundWorker.ReportProgress(0, ProgressState);

                                IBulkCopy bulkCopy = BulkCopyFactory.GetBulkCopy(QueryManager, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.KeepNulls, null);
                                bulkCopy.DestinationTableName = tigerFile.OutputTableName;
                                bulkCopy.DatabaseName = ApplicationDatabaseName;
                                bulkCopy.NotifyAfter = BulkCopyReportAfter;
                                bulkCopy.BatchSize = BulkCopyBatchSize;
                                bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(sqlBulkCopy_SqlRowsCopied);
                                bulkCopy.GenerateColumnMappings(tigerFile.ExcludeColumns);

                                if (((ExtendedCatfoodShapefileDataReader)dataReader).FileDataConnection == null)
                                {
                                    bulkCopy.SchemaDataTable = ((ExtendedCatfoodShapefileDataReader)dataReader).GetSchemaTable();
                                }

                                bulkCopy.WriteToServer(dataReader);

                                ProgressState.ProgressStateFiles.Completed++;
                                UpdateRecordsCompletedCount(ProgressState.ProgressStateRecords.Total);

                                TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, " -- Copy to Server Finsihed: " + tigerFile.FileName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");
                            }
                            catch (Exception e)
                            {
                                if (!BackgroundWorker.CancellationPending)
                                {
                                    throw new Exception("Exception during IBulkCopy: " + e.Message, e);
                                }
                            }
                        }

                        if (dataReader != null)
                        {
                            dataReader.Close();

                            if (tigerFile.DataFileQueryManager != null)
                            {
                                if (tigerFile.DataFileQueryManager.Connection != null)
                                {
                                    if (tigerFile.DataFileQueryManager.Connection.State != ConnectionState.Closed)
                                    {
                                        tigerFile.DataFileQueryManager.Close();
                                    }
                                }
                            }
                        }

                        tigerFile.DeleteTempDirectories();

                        if (!BackgroundWorker.CancellationPending)
                        {
                            StatusManager.UpdateStatusFile("import_status_files", "", "", tigerFile.FileName, Statuses.end);
                        }
                    }
                }
                else
                {
                    TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Cancelling);
                    return false;
                }
            }
            catch (Exception e)
            {
                string msg = "Error importing state nation file: " + tigerFile.FileName + e.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, e);
            }
            return ret;
        }

        public bool ImportTiger2010NationFile(string nationDirectory, ITigerFileLayout tigerFile, bool shouldUseUnzippedFolder, bool shouldSkipExistingRecords)
        {
            bool ret = false;
            IDataReader dataReader = null;
            CurrentTigerFileLayout = tigerFile;
            try
            {
                if (!BackgroundWorker.CancellationPending)
                {


                    bool alreadyDone = StatusManager.CheckStatusFileAlreadyDone("import_status_files", "", "", tigerFile.FileName);
                    if (!alreadyDone)
                    {
                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "import file: " + tigerFile.FileName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");
                        StatusManager.UpdateStatusFile("import_status_files", "", "", tigerFile.FileName, Statuses.start);

                        ProgressState.ProgressStateFiles.Current = tigerFile.FileName;
                        ProgressState.ProgressStateFiles.Message = "Unzipping file";

                        ProgressState.ProgressStateRecords.ResetCounts();
                        ProgressState.ProgressStateRecords.ResetTimer();
                        BackgroundWorker.ReportProgress(0, ProgressState);

                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, " -- Reading File: " + tigerFile.FileName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                        ((ITigerShapefileFileLayout)tigerFile).RecordsRead += new FileLayouts.Delegates.RecordsReadHandler(RecordsRead);


                        if (!shouldUseUnzippedFolder)
                        {
                            dataReader = tigerFile.GetDataReaderFromZipFile(nationDirectory);
                        }
                        else
                        {
                            dataReader = tigerFile.GetDataReaderFromUnZippedFile(nationDirectory);
                        }

                        if (QueryManager.DatabaseType == DatabaseType.MongoDB)
                        {
                            ((ExtendedCatfoodShapefileDataReader)dataReader).IncludeSqlGeographyAsGeoJSON = true;
                            ((ExtendedCatfoodShapefileDataReader)dataReader).IncludeSqlGeometryAsGeoJSON = true;
                        }
                        else
                        {
                            ((ExtendedCatfoodShapefileDataReader)dataReader).IncludeSqlGeographyAsGeoJSON = false;
                            ((ExtendedCatfoodShapefileDataReader)dataReader).IncludeSqlGeometryAsGeoJSON = false;
                        }

                        if (dataReader != null)
                        {
                            try
                            {


                                TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, " -- Copy to Server: " + tigerFile.FileName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                                ProgressState.ProgressStateFiles.Current = tigerFile.FileName;
                                ProgressState.ProgressStateFiles.Message = "Copying to server";
                                BackgroundWorker.ReportProgress(0, ProgressState);

                                IBulkCopy bulkCopy = BulkCopyFactory.GetBulkCopy(QueryManager, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.KeepNulls, null);
                                bulkCopy.DestinationTableName = tigerFile.OutputTableName;
                                bulkCopy.DatabaseName = ApplicationDatabaseName;
                                bulkCopy.NotifyAfter = BulkCopyReportAfter;
                                bulkCopy.BatchSize = BulkCopyBatchSize;
                                bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(sqlBulkCopy_SqlRowsCopied);
                                bulkCopy.GenerateColumnMappings(tigerFile.ExcludeColumns);

                                if (((ExtendedCatfoodShapefileDataReader)dataReader).FileDataConnection == null)
                                {
                                    bulkCopy.SchemaDataTable = ((ExtendedCatfoodShapefileDataReader)dataReader).GetSchemaTable();
                                }

                                bulkCopy.ShouldSkipExistingRecords = ShouldSkipExistingRecords;
                                bulkCopy.ExistingRecordIdField = (string)bulkCopy.SchemaDataTable.Rows[0].ItemArray[0]; // assume that the first field is the one that should be checked to existence in the database

                                bulkCopy.WriteToServer(dataReader);

                                ProgressState.ProgressStateFiles.Completed++;
                                UpdateRecordsCompletedCount(ProgressState.ProgressStateRecords.Total);

                                TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, " -- Copy to Server Finsihed: " + tigerFile.FileName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");
                            }
                            catch (Exception e)
                            {
                                if (!BackgroundWorker.CancellationPending)
                                {
                                    throw new Exception("Exception during IBulkCopy: " + e.Message, e);
                                }
                            }
                        }

                        if (dataReader != null)
                        {
                            dataReader.Close();

                            if (tigerFile.DataFileQueryManager != null)
                            {
                                if (tigerFile.DataFileQueryManager.Connection != null)
                                {
                                    if (tigerFile.DataFileQueryManager.Connection.State != ConnectionState.Closed)
                                    {
                                        tigerFile.DataFileQueryManager.Close();
                                    }
                                }
                            }
                        }

                        tigerFile.DeleteTempDirectories();

                        if (!BackgroundWorker.CancellationPending)
                        {
                            StatusManager.UpdateStatusFile("import_status_files", "", "", tigerFile.FileName, Statuses.end);
                        }
                    }
                }
                else
                {
                    TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Cancelling);
                    return false;
                }
            }
            catch (Exception e)
            {
                string msg = "Error importing state nation file: " + tigerFile.FileName + e.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, e);
            }
            return ret;
        }


        public void CreateNationTigerTables(bool dropFirst)
        {
            if (ShouldDoCounties2000)
            {
                ITigerFileLayout tigerFile = County2000FileFactory.GetFile(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoZcta32000)
            {
                ITigerFileLayout tigerFile = Zcta32000FileFactory.GetFile(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoZcta52000)
            {
                ITigerFileLayout tigerFile = Zcta52000FileFactory.GetFile(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoStates2000)
            {
                ITigerFileLayout tigerFile = States2000FileFactory.GetStates2000File(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoCounties2008)
            {
                ITigerFileLayout tigerFile = County2000FileFactory.GetFile(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoZcta32008)
            {
                ITigerFileLayout tigerFile = Zcta32000FileFactory.GetFile(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoZcta52008)
            {
                ITigerFileLayout tigerFile = Zcta52000FileFactory.GetFile(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoStates2008)
            {
                ITigerFileLayout tigerFile = States2000FileFactory.GetStates2000File(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoCounties2010)
            {
                ITigerFileLayout tigerFile = County2010FileFactory.GetFile(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoZcta52010)
            {
                ITigerFileLayout tigerFile = Zcta52010FileFactory.GetFile(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoMetDiv2010)
            {
                ITigerFileLayout tigerFile = MetDiv2010FileFactory.GetFile(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoCbsa2010)
            {
                ITigerFileLayout tigerFile = Cbsa2010FileFactory.GetFile(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoStates2010)
            {
                ITigerFileLayout tigerFile = States2010FileFactory.GetFile(QueryManager, "us");
                CreateStateTigerTable(tigerFile, dropFirst);
            }

        }
    }
}

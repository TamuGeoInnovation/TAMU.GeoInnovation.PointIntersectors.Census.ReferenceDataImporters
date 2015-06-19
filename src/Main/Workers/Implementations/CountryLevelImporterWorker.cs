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
using USC.GISResearchLab.AddressProcessing.Core.Standardizing.StandardizedAddresses.Lines.LastLines;

using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;




using USC.GISResearchLab.Common.Threading.ProgressStates;
using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.AbstractClasses;
using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.CountryFiles.Implementations;
using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.CountryFiles.AbstractClasses;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.AbstractClasses.Tiger2000;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Factories.Tiger2000.CountryFiles;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Factories.Tiger2010.CountryFiles;
using USC.GISResearchLab.Common.Databases.ImportStatusManagers;
using USC.GISResearchLab.Common.Shapefiles.ShapefileReaders;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public class CountryLevelImporterWorker_Old : AbstractCountryLevelImporterWorker
    {
        #region Properties

        #endregion

        public CountryLevelImporterWorker_Old() : base() { }

        public CountryLevelImporterWorker_Old(TraceSource traceSource, BackgroundWorker backgroundWorker, IQueryManager outputDataQueryManager, ISchemaManager schemaManager) : base(traceSource, backgroundWorker, outputDataQueryManager, schemaManager) { }

        public override bool RunImports(string topDirectory)
        {
            bool ret = false;

            try
            {
                TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "importing files");

                StatusManager.CreateImportStatusStateTable("import_status_states", Restart);
                StatusManager.CreateImportStatusCountyTable("import_status_counties", Restart);
                StatusManager.CreateImportStatusFileTable("import_status_files", Restart);

                StatusManager.CreateStoredProcedures();
                CreateNationTigerTables(Restart);

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

                if (ShouldDoCounties2010)
                {
                    FileCountTotal++;
                }

                if (ShouldDoZcta52010)
                {
                    FileCountTotal++;
                }

                if (ShouldDoMetDiv2010)
                {
                    FileCountTotal++;
                }

                if (ShouldDoCbsa2010)
                {
                    FileCountTotal++;
                }

                ProgressState.ProgressStateFiles.Total = FileCountTotal;
                BackgroundWorker.ReportProgress(0, ProgressState);

                if (ShouldDoStates2000)
                {
                    ITigerFileLayout tigerFile = States2000FileFactory.GetStates2000File(QueryManager, "us");
                    ImportTigerNationFile(topDirectory, tigerFile);
                    SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
                    CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                }

                if (ShouldDoCounties2000)
                {
                    ITigerFileLayout tigerFile = County2000FileFactory.GetFile(QueryManager, "us");
                    ImportTigerNationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
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
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
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
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
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
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
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
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
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
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
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
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoStates2010)
                {
                    ITigerFileLayout tigerFile = States2010FileFactory.GetFile(QueryManager, "us");
                    ImportTiger2010NationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoCounties2010)
                {
                    ITigerFileLayout tigerFile = County2010FileFactory.GetFile(QueryManager, "us");
                    ImportTiger2010NationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }


                if (ShouldDoZcta52010)
                {
                    ITigerFileLayout tigerFile = Zcta52010FileFactory.GetFile(QueryManager, "us");
                    ImportTiger2010NationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoMetDiv2010)
                {
                    ITigerFileLayout tigerFile = MetDiv2010FileFactory.GetFile(QueryManager, "us");
                    ImportTiger2010NationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
                    }
                    if (!BackgroundWorker.CancellationPending)
                    {
                        CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                    }
                }

                if (ShouldDoCbsa2010)
                {
                    ITigerFileLayout tigerFile = Cbsa2010FileFactory.GetFile(QueryManager, "us");
                    ImportTiger2010NationFile(topDirectory, tigerFile);
                    if (!BackgroundWorker.CancellationPending)
                    {
                        SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
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

                        ((AbstractTiger2010ShapefileCountryFileLayout)tigerFile).RecordsRead += new AbstractTiger2010ShapefileCountryFileLayout.RecordsReadHandler(RecordsRead);

                        dataReader = tigerFile.GetDataReaderFromZipFile(nationDirectory);

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

    }
}

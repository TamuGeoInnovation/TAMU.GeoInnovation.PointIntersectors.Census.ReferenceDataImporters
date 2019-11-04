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

using Microsoft.SqlServer.Types;
using SQLSpatialTools;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Factories.Tiger2000.StateFiles;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Factories.Tiger2010.StateFiles;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;
using USC.GISResearchLab.AddressProcessing.Core.Standardizing.StandardizedAddresses.Lines.LastLines;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Core.Databases.BulkCopys;
using USC.GISResearchLab.Common.Databases.ImportStatusManagers;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Databases.SchemaManagers;
using USC.GISResearchLab.Common.Diagnostics.TraceEvents;
using USC.GISResearchLab.Common.Shapefiles.ShapefileReaders;
using USC.GISResearchLab.Common.Utils.Databases;
using USC.GISResearchLab.Common.Utils.Directories;





namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public abstract class AbstractStateLevelImporterWorker : AbstractImporterWorker, IStateLevelImporterWorker
    {
        #region Properties

        public new StateLevelImporterProgressState ProgressState { get; set; }

        public bool ShouldDoPlaces2000 { get; set; }
        public bool ShouldDoCountySubs2000 { get; set; }
        public bool ShouldDoConsolodatedCities2000 { get; set; }

        public bool ShouldDoPlaces2008 { get; set; }
        public bool ShouldDoCountySubs2008 { get; set; }
        public bool ShouldDoConsolodatedCities2008 { get; set; }

        public bool ShouldDoPlaces2010 { get; set; }
        public bool ShouldDoCountySubs2010 { get; set; }
        public bool ShouldDoConsolodatedCities2010 { get; set; }

        #endregion

        public AbstractStateLevelImporterWorker()
            : base()
        {
            ProgressState = new StateLevelImporterProgressState();
        }

        public AbstractStateLevelImporterWorker(TraceSource traceSource, BackgroundWorker backgroundWorker, IQueryManager outputDataQueryManager, ISchemaManager schemaManager)
            : base(traceSource, backgroundWorker, outputDataQueryManager, schemaManager)
        {
            ProgressState = new StateLevelImporterProgressState();
        }

        public virtual bool ImportStateDirectory(string stateName, string stateDirectoryName)
        {
            bool ret = false;

            try
            {
                if (!BackgroundWorker.CancellationPending)
                {

                    TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "import state directory: " + stateDirectoryName);


                    StatusManager.CreateStoredProcedures(false);
                    CreateStateTigerTables(stateName, Restart);


                    if (ShouldDoPlaces2000)
                    {
                        ITigerFileLayout tigerFile = Place2000FileFactory.GetFile(QueryManager, stateName);
                        ImportTigerStateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                        }
                        if (!BackgroundWorker.CancellationPending)
                        {
                            CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                        }
                    }

                    if (ShouldDoCountySubs2000)
                    {
                        ITigerFileLayout tigerFile = CountySub2000FileFactory.GetFile(QueryManager, stateName);
                        ImportTigerStateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                        }
                        if (!BackgroundWorker.CancellationPending)
                        {
                            CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                        }
                    }

                    if (ShouldDoConsolodatedCities2000)
                    {
                        ITigerFileLayout tigerFile = ConsolidatedCity2000FileFactory.GetFile(QueryManager, stateName);
                        ImportTigerStateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                        }
                        if (!BackgroundWorker.CancellationPending)
                        {
                            CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                        }
                    }


                    if (ShouldDoPlaces2008)
                    {
                        ITigerFileLayout tigerFile = Place2000FileFactory.GetFile(QueryManager, stateName);
                        ImportTigerStateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                        }
                        if (!BackgroundWorker.CancellationPending)
                        {
                            CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                        }
                    }

                    if (ShouldDoCountySubs2008)
                    {
                        ITigerFileLayout tigerFile = CountySub2000FileFactory.GetFile(QueryManager, stateName);
                        ImportTigerStateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                        }
                        if (!BackgroundWorker.CancellationPending)
                        {
                            CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                        }
                    }

                    if (ShouldDoConsolodatedCities2008)
                    {
                        ITigerFileLayout tigerFile = ConsolidatedCity2000FileFactory.GetFile(QueryManager, stateName);
                        ImportTigerStateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                        }
                        if (!BackgroundWorker.CancellationPending)
                        {
                            CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                        }
                    }

                    if (ShouldDoPlaces2010)
                    {
                        ITigerFileLayout tigerFile = Place2010FileFactory.GetFile(QueryManager, stateName);
                        ImportTiger2010StateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                        }
                        if (!BackgroundWorker.CancellationPending)
                        {
                            CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                        }
                    }

                    if (ShouldDoCountySubs2010)
                    {
                        ITigerFileLayout tigerFile = CountySub2010FileFactory.GetFile(QueryManager, stateName);
                        ImportTiger2010StateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                        }
                        if (!BackgroundWorker.CancellationPending)
                        {
                            CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                        }
                    }

                    if (ShouldDoConsolodatedCities2010)
                    {
                        ITigerFileLayout tigerFile = ConsolidatedCity2010FileFactory.GetFile(QueryManager, stateName);
                        ImportTiger2010StateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
                        }
                        if (!BackgroundWorker.CancellationPending)
                        {
                            CreateIndex(tigerFile.OutputTableName, tigerFile.SQLCreateTableIndexes);
                        }
                    }

                }

                ret = true;

            }
            catch (Exception exc)
            {
                string msg = "Error importing state directory: " + stateDirectoryName + exc.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, exc);
            }
            return ret;
        }

        public virtual void CreateStateTigerTables(string state, bool dropFirst)
        {

            if (ShouldDoPlaces2000)
            {
                ITigerFileLayout tigerFileLayout = Place2000FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFileLayout, dropFirst);
            }

            if (ShouldDoCountySubs2000)
            {
                ITigerFileLayout tigerFileLayout = CountySub2000FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFileLayout, dropFirst);
            }

            if (ShouldDoConsolodatedCities2000)
            {
                ITigerFileLayout tigerFileLayout = ConsolidatedCity2000FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFileLayout, dropFirst);
            }


            if (ShouldDoPlaces2008)
            {
                ITigerFileLayout tigerFileLayout = Place2000FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFileLayout, dropFirst);
            }

            if (ShouldDoCountySubs2008)
            {
                ITigerFileLayout tigerFileLayout = CountySub2000FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFileLayout, dropFirst);
            }

            if (ShouldDoConsolodatedCities2008)
            {
                ITigerFileLayout tigerFileLayout = ConsolidatedCity2000FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFileLayout, dropFirst);
            }

            if (ShouldDoPlaces2010)
            {
                ITigerFileLayout tigerFileLayout = Place2010FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFileLayout, dropFirst);
            }

            if (ShouldDoCountySubs2010)
            {
                ITigerFileLayout tigerFileLayout = CountySub2010FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFileLayout, dropFirst);
            }

            if (ShouldDoConsolodatedCities2010)
            {
                ITigerFileLayout tigerFileLayout = ConsolidatedCity2010FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFileLayout, dropFirst);
            }
        }

        //public override bool RunImports(string topDirectory)
        //{
        //    bool ret = false;

        //    try
        //    {

        //        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "getting directories");
        //        ArrayList subDirectories = DirectoryUtils.GetSubDirectories(topDirectory);


        //        int countyDirectoryCount = 0;

        //        if (subDirectories != null)
        //        {

        //            for (int i = 0; i < subDirectories.Count; i++)
        //            {
        //                string directory = (string)subDirectories[i];
        //                string[] subSubDirectories = Directory.GetDirectories(directory);
        //                if (subSubDirectories != null)
        //                {
        //                    countyDirectoryCount += subSubDirectories.Length;
        //                }
        //            }

        //            ProgressState.ProgressStateStates.ResetTimer();
        //            ProgressState.ProgressStateStates.Total = subDirectories.Count;
        //            ProgressState.ProgressStateStates.Completed = 0;

        //            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "got directories: " + subDirectories.Count);

        //            StatusManager.CreateStoredProcedures(false);

        //            StatusManager.CreateImportStatusStateTable("import_status_states", Restart, ShouldRemoveStatusTablesFirst);
        //            StatusManager.CreateImportStatusCountyTable("import_status_counties", Restart, ShouldRemoveStatusTablesFirst);
        //            StatusManager.CreateImportStatusFileTable("import_status_files", Restart, ShouldRemoveStatusTablesFirst);

        //            for (int i = 0; i < subDirectories.Count; i++)
        //            {
        //                string directory = (string)subDirectories[i];
        //                string[] filenames = Directory.GetFiles(directory);

        //                for (int files = 0; files < filenames.Length; files++)
        //                {
        //                    if (!BackgroundWorker.CancellationPending)
        //                    {


        //                        string directoryName = DirectoryUtils.GetDirectoryName(directory);


        //                        //testing getting fips from first zip file
        //                        string[] filenameParts = filenames[files].Split('_');
        //                        string stateFips = filenameParts[2].Substring(filenameParts[2].Length - 2);
        //                        //string stateFips = filenameParts[2].Substring(0, 2);


        //                        string[] directoryNameParts = directoryName.Split('_');
        //                        //string stateFips = directoryNameParts[0];


        //                        string stateNameFull = "";
        //                        State state = StateUtils.GetStateFromFIPS(stateFips);
        //                        stateNameFull = state.PrimaryName;

        //                        for (int j = 1; j < directoryNameParts.Length; j++)
        //                        {
        //                            if (j > 1)
        //                            {
        //                                stateNameFull += " ";
        //                            }
        //                            stateNameFull += directoryNameParts[j];
        //                        }

        //                        if (StateUtils.isState(stateNameFull))
        //                        {
        //                            //string stateName = StateUtils.getStateOfficialAbbreviation(stateNameFull);
        //                            string stateName = state.OfficialAbbreviation;
        //                            ProgressState.ProgressStateStates.Current = stateName;
        //                            ProgressState.ProgressStateStates.Completed = files;
        //                            BackgroundWorker.ReportProgress(0, ProgressState);


        //                            if (!String.IsNullOrEmpty(stateName))
        //                            {

        //                                bool alreadyDone = StatusManager.CheckStatusStateAlreadyDone("import_status_states", stateName);
        //                                if (!alreadyDone)
        //                                {
        //                                    StatusManager.UpdateStatusState("import_status_states", stateName, Statuses.start);

        //                                    ImportStateDirectory(stateName, directory);

        //                                    if (!BackgroundWorker.CancellationPending)
        //                                    {
        //                                        StatusManager.UpdateStatusState("import_status_states", stateName, Statuses.end);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "import skipped state: " + stateName);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                throw new Exception("Could not find abbreviation for stateName: " + stateNameFull);
        //                            }

        //                            ProgressState.ProgressStateStates.Completed = files + 1;
        //                            BackgroundWorker.ReportProgress(0, ProgressState);
        //                        }

        //                    }
        //                    else
        //                    {
        //                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Cancelling);
        //                        return false;
        //                    }
        //                }

        //                if (!BackgroundWorker.CancellationPending)
        //                {
        //                    ret = true;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        string msg = "Error importing direcories: " + exc.Message;
        //        TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
        //    }
        //    return ret;
        //}

        public override bool RunImports(string topDirectory)
        {
            bool ret = false;

            try
            {

                TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "getting directories");
                ArrayList subDirectories = DirectoryUtils.GetSubDirectories(topDirectory);


                int countyDirectoryCount = 0;

                if (subDirectories != null)
                {

                    for (int i = 0; i < subDirectories.Count; i++)
                    {
                        string directory = (string)subDirectories[i];
                        string[] subSubDirectories = Directory.GetDirectories(directory);
                        if (subSubDirectories != null)
                        {
                            countyDirectoryCount += subSubDirectories.Length;
                        }
                    }

                    ProgressState.ProgressStateStates.ResetTimer();
                    ProgressState.ProgressStateStates.Total = subDirectories.Count;
                    ProgressState.ProgressStateStates.Completed = 0;

                    TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "got directories: " + subDirectories.Count);

                    StatusManager.CreateStoredProcedures(false);

                    StatusManager.CreateImportStatusStateTable("import_status_states", Restart, ShouldRemoveStatusTablesFirst);
                    StatusManager.CreateImportStatusCountyTable("import_status_counties", Restart, ShouldRemoveStatusTablesFirst);
                    StatusManager.CreateImportStatusFileTable("import_status_files", Restart, ShouldRemoveStatusTablesFirst);

                    for (int i = 0; i < subDirectories.Count; i++)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {

                            string directory = (string)subDirectories[i];
                            string directoryName = DirectoryUtils.GetDirectoryName(directory);

                            string[] directoryNameParts = directoryName.Split('_');
                            string stateFips = directoryNameParts[0];

                            string stateNameFull = "";
                            for (int j = 1; j < directoryNameParts.Length; j++)
                            {
                                if (j > 1)
                                {
                                    stateNameFull += " ";
                                }
                                stateNameFull += directoryNameParts[j];
                            }

                            if (StateUtils.isState(stateNameFull))
                            {
                                string stateName = StateUtils.getStateOfficialAbbreviation(stateNameFull);

                                ProgressState.ProgressStateStates.Current = stateName;
                                ProgressState.ProgressStateStates.Completed = i;
                                BackgroundWorker.ReportProgress(0, ProgressState);


                                if (!String.IsNullOrEmpty(stateName))
                                {

                                    bool alreadyDone = StatusManager.CheckStatusStateAlreadyDone("import_status_states", stateName);
                                    if (!alreadyDone)
                                    {
                                        StatusManager.UpdateStatusState("import_status_states", stateName, Statuses.start);

                                        ImportStateDirectory(stateName, directory);

                                        if (!BackgroundWorker.CancellationPending)
                                        {
                                            StatusManager.UpdateStatusState("import_status_states", stateName, Statuses.end);
                                        }
                                    }
                                    else
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "import skipped state: " + stateName);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Could not find abbreviation for stateName: " + stateNameFull);
                                }

                                ProgressState.ProgressStateStates.Completed = i + 1;
                                BackgroundWorker.ReportProgress(0, ProgressState);
                            }

                        }
                        else
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Cancelling);
                            return false;
                        }
                    }

                    if (!BackgroundWorker.CancellationPending)
                    {
                        ret = true;
                    }
                }
            }
            catch (Exception exc)
            {
                string msg = "Error importing direcories: " + exc.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
            }
            return ret;
        }

        public virtual bool ImportTigerStateFile(string state, string stateDirectory, ITigerFileLayout tigerFile)
        {
            bool ret = false;
            IDataReader dataReader = null;
            CurrentTigerFileLayout = tigerFile;
            try
            {
                if (!BackgroundWorker.CancellationPending)
                {


                    bool alreadyDone = StatusManager.CheckStatusFileAlreadyDone("import_status_files", state, state, tigerFile.FileName);
                    if (!alreadyDone)
                    {


                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "import file: " + tigerFile.FileName);
                        StatusManager.UpdateStatusFile("import_status_files", state, state, tigerFile.FileName, Statuses.start);

                        ProgressState.ProgressStateRecords.ResetCounts();
                        ProgressState.ProgressStateRecords.ResetTimer();
                        ProgressState.ProgressStateRecords.Current = state + " - " + tigerFile.FileName;
                        ProgressState.ProgressStateRecords.Message = "Unzipping file";
                        BackgroundWorker.ReportProgress(0, ProgressState);

                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, " -- Reading File: " + tigerFile.FileName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");


                        ((ITigerShapefileFileLayout)tigerFile).RecordsRead += new FileLayouts.Delegates.RecordsReadHandler(RecordsRead);
                        dataReader = tigerFile.GetDataReaderFromZipFile(stateDirectory);

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

                                ProgressState.ProgressStateRecords.Current = state + " - " + tigerFile.FileName;
                                ProgressState.ProgressStateRecords.Message = "Copying to server";
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
                            StatusManager.UpdateStatusFile("import_status_files", state, state, tigerFile.FileName, Statuses.end);
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
                string msg = "Error importing state tiger file: " + tigerFile.FileName + e.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, e);
            }
            return ret;
        }

        public virtual bool ImportTiger2010StateFile(string state, string stateDirectory, ITigerFileLayout tigerFile)
        {
            bool ret = false;
            IDataReader dataReader = null;
            CurrentTigerFileLayout = tigerFile;
            try
            {
                if (!BackgroundWorker.CancellationPending)
                {


                    bool alreadyDone = StatusManager.CheckStatusFileAlreadyDone("import_status_files", state, state, tigerFile.FileName);
                    if (!alreadyDone)
                    {


                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "import file: " + tigerFile.FileName);
                        StatusManager.UpdateStatusFile("import_status_files", state, state, tigerFile.FileName, Statuses.start);

                        ProgressState.ProgressStateRecords.ResetCounts();
                        ProgressState.ProgressStateRecords.ResetTimer();
                        ProgressState.ProgressStateRecords.Current = state + " - " + tigerFile.FileName;
                        ProgressState.ProgressStateRecords.Message = "Unzipping file";
                        BackgroundWorker.ReportProgress(0, ProgressState);

                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, " -- Reading File: " + tigerFile.FileName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");


                        ((ITigerShapefileFileLayout)tigerFile).RecordsRead += new FileLayouts.Delegates.RecordsReadHandler(RecordsRead);
                        dataReader = tigerFile.GetDataReaderFromZipFile(stateDirectory);


                        if (dataReader != null)
                        {
                            {

                                try
                                {
                                    TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, " -- Copy to Server: " + tigerFile.FileName + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                                    ProgressState.ProgressStateRecords.Current = state + " - " + tigerFile.FileName;
                                    ProgressState.ProgressStateRecords.Message = "Copying to server";
                                    BackgroundWorker.ReportProgress(0, ProgressState);

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

                                    if (QueryManager.DatabaseType == DatabaseType.MongoDB || QueryManager.DatabaseType == DatabaseType.MySql)
                                    {

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
                                    }
                                    else if (QueryManager.DatabaseType == DatabaseType.Npgsql)
                                    {
                                        DataTable schemaDataTable = null;

                                        if (((ExtendedCatfoodShapefileDataReader)dataReader).FileDataConnection == null)
                                        {
                                            schemaDataTable = ((ExtendedCatfoodShapefileDataReader)dataReader).GetSchemaTable();
                                        }
                                        while (dataReader.Read())
                                        {
                                            string sql = "";
                                            sql += "INSERT INTO " + tigerFile.OutputTableName.ToLower();
                                            sql += "(";

                                            int i = 0;
                                            foreach (DataRow dataRow in schemaDataTable.Rows)
                                            {
                                                if (i > 0)
                                                {
                                                    sql += ",";
                                                }

                                                string columnName = (string)dataRow[0];
                                                columnName = columnName.ToLower();
                                                sql += "\"" + columnName + "\"";
                                                i++;
                                            }

                                            sql += ") ";
                                            sql += "VALUES ";
                                            sql += "(";

                                            int j = 0;
                                            foreach (DataRow dataRow in schemaDataTable.Rows)
                                            {
                                                if (j > 0)
                                                {
                                                    sql += ",";
                                                }

                                                Type type = (Type)dataRow["DataType"];
                                                if (type == typeof(SqlGeometry) || type == typeof(SqlGeography))
                                                {
                                                    //sql += "GeomFromText(";
                                                    sql += "st_geomfromwkb(";
                                                }


                                                string columnName = (string)dataRow[0];

                                                sql += "@" + columnName;

                                                if (type == typeof(SqlGeometry) || type == typeof(SqlGeography))
                                                {
                                                    sql += ")";
                                                }
                                                j++;
                                            }
                                            sql += ") ";

                                            //IDbCommand cmd = new MySqlCommand(sql);
                                            IDbCommand cmd = QueryManagerFactory.GetCommand(SchemaManager.PathToDatabaseDLLs, SchemaManager.DataProviderType);
                                            cmd.CommandText = sql;

                                            foreach (DataRow dataRow in schemaDataTable.Rows)
                                            {
                                                string columnName = (string)dataRow[0];
                                                try
                                                {
                                                    object value = dataReader.GetValue(dataReader.GetOrdinal(columnName));

                                                    Type type = (Type)dataRow["DataType"];
                                                    if (type == typeof(SqlGeometry))
                                                    {
                                                        SqlGeometry g = (SqlGeometry)value;

                                                        if (g != null)
                                                        {
                                                            //string geomType = g.STGeometryType().Value;

                                                            //char[] chars = g.STAsText().Value;
                                                            //value = "'" + new string(chars) + "'";
                                                            value = g.STAsBinary().Value;
                                                        }
                                                        else
                                                        {
                                                            value = null;
                                                        }
                                                        cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter(columnName, SqlDbType.Binary, value, false));
                                                    }
                                                    else if (type == typeof(SqlGeography))
                                                    {
                                                        SqlGeography geog = (SqlGeography)value;

                                                        if (geog != null)
                                                        {
                                                            SqlGeometry geom = SQLSpatialToolsFunctions.VacuousGeographyToGeometry(geog, geog.STSrid.Value);

                                                            //string geomType = geom.STGeometryType().Value;

                                                            value = geom.STAsBinary().Value;
                                                        }
                                                        else
                                                        {
                                                            value = null;
                                                        }
                                                        cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter(columnName, SqlDbType.Binary, value, false));
                                                    }
                                                    else
                                                    {
                                                        cmd.Parameters.Add(SqlParameterUtils.BuildSqlParameter(columnName, SqlDbType.VarChar, value));
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    throw new Exception("error in ImportFile NpgSql: " + e.Message);
                                                }

                                            }

                                            IQueryManager qm = SchemaManager.QueryManager;

                                            qm.AddParameters(cmd.Parameters);
                                            // IDbDataParameter []_Parameters = SchemaManager.QueryManager.Parameters;
                                            qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, true);
                                        }

                                    }

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
                                StatusManager.UpdateStatusFile("import_status_files", state, state, tigerFile.FileName, Statuses.end);
                            }
                        }
                    }
                    else
                    {
                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Cancelling);
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                string msg = "Error importing state tiger file: " + tigerFile.FileName + e.Message;
                TraceSource.TraceEvent(TraceEventType.Error, (int)ExceptionEvents.ExceptionOccurred, msg);
                throw new Exception(msg, e);
            }
            return ret;
        }
    }
}

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
using System.ComponentModel;
using System.Diagnostics;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Factories.Tiger2000.StateFiles;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Factories.Tiger2010.StateFiles;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Databases.SchemaManagers;
using USC.GISResearchLab.Common.Diagnostics.TraceEvents;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public abstract class AbstractCensusTractLevelImporterWorker : AbstractStateLevelImporterWorker, ICensusTractLevelImporterWorker
    {
        #region Properties

        public bool ShouldDoCensusTracts2000 { get; set; }
        public bool ShouldDoCensusBlocks2000 { get; set; }
        public bool ShouldDoCensusBlocks2008 { get; set; }
        public bool ShouldDoCensusBlocks2010 { get; set; }
        public bool ShouldDoCensusTracts2010 { get; set; }
        public bool ShouldDoCensusBlockGroups2000 { get; set; }

        public List<string> Zips { get; set; }
        public List<string> States { get; set; }

        #endregion

        public AbstractCensusTractLevelImporterWorker() : base() { }

        public AbstractCensusTractLevelImporterWorker(TraceSource traceSource, BackgroundWorker backgroundWorker, IQueryManager outputDataQueryManager, ISchemaManager schemaManager) : base(traceSource, backgroundWorker, outputDataQueryManager, schemaManager) { }


        public override bool ImportStateDirectory(string stateName, string stateDirectoryName)
        {
            bool ret = false;

            try
            {
                if (!BackgroundWorker.CancellationPending)
                {

                    TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "import state directory: " + stateDirectoryName);

                    StatusManager.CreateStoredProcedures(false);
                    CreateStateTigerTables(stateName, Restart);

                    if (ShouldDoCensusTracts2000)
                    {
                        ITigerFileLayout tigerFile = CensusTract2000FileFactory.GetFile(QueryManager, stateName);
                        ImportTigerStateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
                        }
                    }

                    if (ShouldDoCensusBlockGroups2000)
                    {
                        ITigerFileLayout tigerFile = CensusBlockGroup2000FileFactory.GetFile(QueryManager, stateName);
                        ImportTigerStateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
                        }
                    }

                    if (ShouldDoCensusBlocks2000)
                    {
                        ITigerFileLayout tigerFile = CensusBlock2000FileFactory.GetFile(QueryManager, stateName);
                        ImportTigerStateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
                        }
                    }

                    if (ShouldDoCensusBlocks2008)
                    {
                        ITigerFileLayout tigerFile = CensusBlock2000FileFactory.GetFile(QueryManager, stateName);
                        ImportTigerStateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
                        }
                    }

                    if (ShouldDoCensusBlocks2010)
                    {
                        ITigerFileLayout tigerFile = CensusBlock2010FileFactory.GetFile(QueryManager, stateName);
                        ImportTiger2010StateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, false);
                        }
                    }

                    if (ShouldDoCensusTracts2010)
                    {
                        ITigerFileLayout tigerFile = CensusTract2010FileFactory.GetFile(QueryManager, stateName);
                        ImportTiger2010StateFile(stateName, stateDirectoryName, tigerFile);
                        if (!BackgroundWorker.CancellationPending)
                        {
                            SchemaManager.AddGeogIndexToDatabase(tigerFile.OutputTableName, true);
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
            if (ShouldDoCensusBlockGroups2000)
            {
                ITigerFileLayout tigerFile = CensusBlockGroup2000FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoCensusBlocks2000)
            {
                ITigerFileLayout tigerFile = CensusBlock2000FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoCensusTracts2000)
            {
                ITigerFileLayout tigerFile = CensusTract2000FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoCensusBlocks2008)
            {
                ITigerFileLayout tigerFile = CensusBlock2000FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoCensusBlocks2010)
            {
                ITigerFileLayout tigerFile = CensusBlock2010FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFile, dropFirst);
            }

            if (ShouldDoCensusTracts2010)
            {
                ITigerFileLayout tigerFile = CensusTract2010FileFactory.GetFile(QueryManager, state);
                CreateStateTigerTable(tigerFile, dropFirst);
            }
        }
    }
}

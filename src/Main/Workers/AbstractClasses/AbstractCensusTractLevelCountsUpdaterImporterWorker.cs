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
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.ApplicationStates.Managers;
using USC.GISResearchLab.Common.Databases.ImportStatusManagers;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Databases.SchemaManagers;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public abstract class AbstractCensusTractLevelCountsUpdaterImporterWorker : AbstractCensusTractLevelImporterWorker, ICensusTractLevelCountsUpdaterImporterWorker
    {
        #region Properties


        public bool shouldDoUpdateZIPCT2000Counts { get; set; }
        public bool shouldDoUpdateZIPCT2010Counts { get; set; }
        public bool shouldDoUpdatePlaceCT2000Counts { get; set; }
        public bool shouldDoUpdatePlaceCT2010Counts { get; set; }



        public List<string> Zips { get; set; }
        public List<string> States { get; set; }

        #endregion

        public AbstractCensusTractLevelCountsUpdaterImporterWorker() : base() { }

        public AbstractCensusTractLevelCountsUpdaterImporterWorker(TraceSource traceSource, BackgroundWorker backgroundWorker, IQueryManager outputDataQueryManager, ISchemaManager schemaManager) : base(traceSource, backgroundWorker, outputDataQueryManager, schemaManager) { }

        public virtual bool RunCensusUpdates(DoWorkEventArgs e, string topDirectory, bool restart)
        {

            StatusManager = ImportStatusManagerFactory.GetImportStatusManager(ApplicationPathToDatabaseDlls, ApplicationDataProviderType, ApplicationConnectionString);
            //StatusManager = new StatusManager(TraceSource);
            //StatusManager.ApplicationPathToDatabaseDlls = ApplicationPathToDatabaseDlls;
            //StatusManager.ApplicationConnectionString = ApplicationConnectionString;
            //StatusManager.ApplicationDatabaseType = ApplicationDatabaseType;
            //StatusManager.ApplicationDataProviderType = ApplicationDataProviderType;
            StatusManager.DefaultDatabase = ApplicationDatabaseName;
            StatusManager.InitializeConnections();

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

                if (shouldDoUpdateZIPCT2000Counts)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        UpdateZIPCT2000Counts();
                    }
                }

                if (shouldDoUpdateZIPCT2010Counts)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        UpdateZIPCT2010Counts();
                    }
                }

                if (shouldDoUpdatePlaceCT2000Counts)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        UpdatePlaceCT2000Counts();
                    }
                }

                if (shouldDoUpdatePlaceCT2010Counts)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        UpdatePlaceCT2010Counts();
                    }
                }




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


        public abstract void UpdateZIPCT2000Counts();

        public abstract void UpdateZIPCT2010Counts();


        public abstract void UpdatePlaceCT2000Counts();

        public abstract void UpdatePlaceCT2010Counts();


    }
}

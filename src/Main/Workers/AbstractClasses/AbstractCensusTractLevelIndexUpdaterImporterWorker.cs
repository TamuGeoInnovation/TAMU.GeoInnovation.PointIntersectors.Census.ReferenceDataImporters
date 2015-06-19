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


using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;




using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.AbstractClasses;
using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.StateFiles.Implementations;
using Microsoft.SqlServer.Types;
using USC.GISResearchLab.Common.Databases.ImportStatusManagers;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public abstract class AbstractCensusTractLevelIndexUpdaterImporterWorker: AbstractCensusTractLevelImporterWorker, ICensusTractLevelIndexUpdaterImporterWorker
    {
        #region Properties

        
        public bool shouldDoUpdateCT1990Indexes { get; set; }
        public bool shouldDoUpdateCT2000Indexes { get; set; }
        public bool shouldDoUpdateCT2010Indexes { get; set; }
        public bool shouldDoUpdateCB2000Indexes { get; set; }
        public bool shouldDoUpdateCB2010Indexes { get; set; }

        public bool shouldDoUpdate2000PlaceIndexes { get; set; }
        public bool shouldDoUpdate2000ConCityIndexes { get; set; }
        public bool shouldDoUpdate2000CouSubIndexes { get; set; }

        public bool shouldDoUpdate2010PlaceIndexes { get; set; }
        public bool shouldDoUpdate2010ConCityIndexes { get; set; }
        public bool shouldDoUpdate2010CouSubIndexes { get; set; }

        public List<string> Zips { get; set; }
        public List<string> States { get; set; }

        #endregion

        public AbstractCensusTractLevelIndexUpdaterImporterWorker() : base() { }

        public AbstractCensusTractLevelIndexUpdaterImporterWorker(TraceSource traceSource, BackgroundWorker backgroundWorker, IQueryManager outputDataQueryManager, ISchemaManager schemaManager) : base(traceSource, backgroundWorker, outputDataQueryManager, schemaManager) { }

        public virtual bool RunCensusUpdates(DoWorkEventArgs e, string topDirectory, bool restart)
        {

            StatusManager = ImportStatusManagerFactory.GetImportStatusManager(ApplicationPathToDatabaseDlls, ApplicationDataProviderType, ApplicationConnectionString);
            //StatusManager = new StatusManager(TraceSource);
            //StatusManager.ApplicationPathToDatabaseDlls = ApplicationPathToDatabaseDlls;
            //StatusManager.ApplicationConnectionString = ApplicationConnectionString;
            //StatusManager.ApplicationDatabaseType = ApplicationDatabaseType;
            //StatusManager.ApplicationDataProviderType = ApplicationDataProviderType;
            //StatusManager.InitializeConnections();
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

                

                if (shouldDoUpdateCT1990Indexes)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        UpdateCT1990Indexes();
                    }
                }

                if (shouldDoUpdateCT2000Indexes)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        UpdateCT2000Indexes();
                    }
                }

                if (shouldDoUpdateCT2010Indexes)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        UpdateCT2010Indexes();
                    }
                }

                if (shouldDoUpdateCB2000Indexes)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        UpdateCB2000Indexes();
                    }
                }

                if (shouldDoUpdateCB2010Indexes)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        UpdateCB2010Indexes();
                    }
                }

                if (shouldDoUpdate2000PlaceIndexes)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        Update2000PlaceIndexes();
                    }
                }

                if (shouldDoUpdate2000ConCityIndexes)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        Update2000ConCityIndexes();
                    }
                }

                if (shouldDoUpdate2000CouSubIndexes)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        Update2000CouSubIndexes();
                    }
                }


                if (shouldDoUpdate2010PlaceIndexes)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        Update2010PlaceIndexes();
                    }
                }

                if (shouldDoUpdate2010ConCityIndexes)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        Update2010ConCityIndexes();
                    }
                }

                if (shouldDoUpdate2010CouSubIndexes)
                {
                    if (!BackgroundWorker.CancellationPending)
                    {
                        Update2010CouSubIndexes();
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



        public abstract void UpdateCT1990Indexes();
        public abstract void UpdateCT2000Indexes();
        public abstract void UpdateCT2010Indexes();


        public abstract void UpdateCB2000Indexes();
        public abstract void UpdateCB2010Indexes();


        public abstract void Update2000PlaceIndexes();
        public abstract void Update2000ConCityIndexes();
        public abstract void Update2000CouSubIndexes();


        public abstract void Update2010PlaceIndexes();
        public abstract void Update2010ConCityIndexes();
        public abstract void Update2010CouSubIndexes();

    }
}

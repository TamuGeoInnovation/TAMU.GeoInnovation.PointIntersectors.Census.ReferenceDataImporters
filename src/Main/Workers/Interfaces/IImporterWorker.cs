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

using System.Collections;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.ApplicationStates.Managers;
//using USC.GISResearchLab.AddressProcessing.Core.Standardizing.StandardizedAddresses.Lines.LastLines;

using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Databases.ImportStatusManagers;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Databases.SchemaManagers;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public interface IImporterWorker
    {
        #region Properties

        IImportStatusManager StatusManager { get; set; }
        TransactionManager TransactionManager { get; set; }

        bool Restart { get; set; }
        bool ShouldRemoveOutputRecordsTableFirst { get; set; }
        bool ShouldRemoveStatusTablesFirst { get; set; }
        bool ShouldSkipExistingRecords { get; set; }
        bool ShouldUseUnzippedFolder { get; set; }
        string UnzippedFolder { get; set; }

        bool TransactionRunning { get; set; }
        string ApplicationConnectionString { get; set; }
        DataProviderType ApplicationDataProviderType { get; set; }
        DatabaseType ApplicationDatabaseType { get; set; }
        TraceSource TraceSource { get; set; }
        ArrayList Directories { get; set; }
        BackgroundWorker BackgroundWorker { get; set; }
        DoWorkEventArgs DoWorkEventArgs { get; set; }
        ITigerFileLayout CurrentTigerFileLayout { get; set; }
        string ApplicationPathToDatabaseDlls { get; set; }
        string ApplicationDatabaseName { get; set; }
        ProgressState ProgressState { get; set; }

        IQueryManager QueryManager { get; }
        ISchemaManager SchemaManager { get; }

        int BulkCopyBatchSize { get; set; }
        int BulkCopyReportAfter { get; set; }

        #endregion



        bool RunImports(string topDirectory);
        void UpdateRecordsCompletedCount(int completed);
        void UpdateRecordsCompletedCount(int completed, int total);
        void RecordsRead(int completed, int total);

        bool Run(DoWorkEventArgs e, string topDirectory, bool restart);

        void CreateStateTigerTable(ITigerFileLayout tigerFile, bool dropFirst);

        //void CreateTable(string tableName, string createTableSql, bool dropFirst);

        void sqlBulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e);

        //bool AddGeogIndex(string tableName, bool shouldDrop);

        //bool CreateIndex(string tableName, string sql);
    }
}
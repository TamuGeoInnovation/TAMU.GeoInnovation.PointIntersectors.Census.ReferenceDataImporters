using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Databases.ConnectionStringManagers;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Utils.Directories;
using USC.GISResearchLab.Common.Utils.Files;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.AbstractClasses.Tiger1990.StateFiles
{
    public abstract class AbstractTiger1990FileLayout : ITigerFileLayout
    {

        #region Events

        public event DbfRecordReadHandler DbfRecordRead;
        public event DbfNumberOfRecordsReadHandler DbfNumberOfRecordsRead;

        #endregion

        #region Properties

        public int NumberOfRecords { get; set; }

        public IQueryManager DataFileQueryManager { get; set; }
        public string SQLCreateTable { get; set; }
        public string SQLPostProcessOutputToSingleTable { get; set; }
        public string SQLPostInsertTableUpdate { get; set; }
        public string SQLPostInsertTableDelete { get; set; }
        public string SQLPostInsertTableDeleteStreetsOnly { get; set; }
        public string SQLPostInsertTableDeleteNamedStreetsOnly { get; set; }
        public string SQLPostInsertTableDeleteAddressableStreetsOnly { get; set; }
        public string SQLPostInsertSingleTableUpdate { get; set; }
        public string SQLCreateTableIndexes { get; set; }
        public string SQLAlterTableAddForeignKeys { get; set; }
        public string[] SQLCreateViews { get; set; }

        public string[] ExcludeColumns { get; set; }

        public bool HasSoundexColumns { get; set; }
        public string[] SoundexColumns { get; set; }

        public bool HasEndPointsColumns { get; set; }
        public string[] EndPointsColumns { get; set; }

        public bool HasSoundexDMColumns { get; set; }
        public string[] SoundexDMColumns { get; set; }

        public bool ShouldSplitAddressRanges { get; set; }
        public string[] SplitAddressRangesColumns { get; set; }

        public bool ShouldAddEvenOddFlag { get; set; }
        public string[] AddEvenOddFlagColumns { get; set; }

        public bool ShouldIncludeGeometryProjected { get; set; }

        public bool ShouldIncludeArea { get; set; }
        public bool ShouldIncludeCentroid { get; set; }


        public string StateName { get; set; }
        public string FileName { get; set; }

        public string[][] BulkCopyColumnMappings { get; set; }
        public ArrayList TempFiles { get; set; }
        public ArrayList TempDirectories { get; set; }


        public string OutputTableName
        {
            get
            {
                string ret = "";
                if (!String.IsNullOrEmpty(StateName))
                {
                    ret = StateName;
                }
                else
                {
                    ret = FileUtils.GetFileNameWithoutExtension(FileName);
                }
                return ret;
            }
        }

        #endregion

        public AbstractTiger1990FileLayout()
        {
            TempFiles = new ArrayList();
            TempDirectories = new ArrayList();
        }


        public AbstractTiger1990FileLayout(string tableName)
            : this()
        {
            StateName = tableName;
        }

        public virtual IDataReader GetDataReaderFromUnZippedFile(string unzippedFileDirectory)
        {
            IDataReader ret = null;
            string tempDirectory = null;

            try
            {

                bool hasShapefile = false;
                string shapefileName = null;
                string dbffileName = null;

                ArrayList fileList = DirectoryUtils.getFileList(unzippedFileDirectory);
                if (fileList != null)
                {
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        string file = (string)fileList[i];
                        string fileExtension = FileUtils.GetExtension(file);
                        if (String.Compare(fileExtension, ".shp", true) == 0)
                        {
                            hasShapefile = true;
                            shapefileName = file;
                            break;
                        }
                        if (String.Compare(fileExtension, ".dbf", true) == 0)
                        {
                            dbffileName = file;
                        }
                    }
                }

                if (hasShapefile)
                {
                    ret = GetDataReader(shapefileName);
                }
                else
                {
                    string dbfFileDirectory = FileUtils.GetDirectoryPath(dbffileName);
                    string tempFile = Path.Combine(dbfFileDirectory, "temp.dbf");

                    File.Copy(dbffileName, tempFile);
                    ret = GetDataReader(tempFile);
                }
            }
            catch (Exception e)
            {
                if (!String.IsNullOrEmpty(tempDirectory))
                {
                    if (Directory.Exists(tempDirectory))
                    {
                        DirectoryUtils.DeleteDirectory(tempDirectory);
                    }
                }
                throw new Exception("Error getting datatable: " + e.Message, e);
            }
            return ret;
        }

        public abstract DataTable GetDataTableFromZipFile(string zipFileDirectory);

        public abstract IDataReader GetDataReaderFromZipFile(string zipFileDirectory);

        public virtual DataTable GetDataTable(string fileLocation)
        {
            DataTable ret = null;

            try
            {
                string databasePath = fileLocation;
                string databaseDirectory = FileUtils.GetDirectoryPath(databasePath, false);
                string databaseName = FileUtils.GetFileName(databasePath);
                string tableName = FileUtils.GetFileNameWithoutExtension(databasePath);
                string databaseExtension = FileUtils.GetExtension(databaseName);

                DataProviderType dataProviderType = DataProviderTypes.FromDatabaseExtension(databaseExtension);
                DatabaseType databaseType = DatabaseTypes.FromDatabaseExtension(databaseExtension);

                IConnectionStringManager connectionStringManager = ConnectionStringManagerFactory.GetConnectionStringManager(databaseType, databaseDirectory, databaseName, null, null, null);
                string connectionString = connectionStringManager.GetConnectionString(dataProviderType);
                DataFileQueryManager = new QueryManager(dataProviderType, databaseType, connectionString);

                string sql = "select * from [" + databaseName + "]";
                SqlCommand cmd = new SqlCommand(sql);

                DataFileQueryManager.AddParameters(cmd.Parameters);
                ret = DataFileQueryManager.ExecuteDataTable(CommandType.Text, cmd.CommandText, true);

            }
            catch (Exception e)
            {
                throw new Exception("Error getting datatable: " + e.Message, e);
            }

            return ret;
        }

        public virtual IDataReader GetDataReader(string fileLocation)
        {
            IDataReader ret = null;

            try
            {
                string databasePath = fileLocation;
                string databaseDirectory = FileUtils.GetDirectoryPath(databasePath, false);
                string databaseName = FileUtils.GetFileName(databasePath);
                string tableName = FileUtils.GetFileNameWithoutExtension(databasePath);
                string databaseExtension = FileUtils.GetExtension(databaseName);

                DataProviderType dataProviderType = DataProviderTypes.FromDatabaseExtension(databaseExtension);
                DatabaseType databaseType = DatabaseTypes.FromDatabaseExtension(databaseExtension);

                IConnectionStringManager connectionStringManager = ConnectionStringManagerFactory.GetConnectionStringManager(databaseType, databaseDirectory, databaseName, null, null, null);
                string connectionString = connectionStringManager.GetConnectionString(dataProviderType);
                DataFileQueryManager = new QueryManager(dataProviderType, databaseType, connectionString);

                string sql = "select * from [" + databaseName + "]";
                SqlCommand cmd = new SqlCommand(sql);

                DataFileQueryManager.Open();
                DataFileQueryManager.AddParameters(cmd.Parameters);
                ret = DataFileQueryManager.ExecuteReader(CommandType.Text, cmd.CommandText, false);

            }
            catch (Exception e)
            {
                throw new Exception("Error getting GetDataReader: " + e.Message, e);
            }

            return ret;
        }

        public void dbfRecordRead(int numberOfRecordsRead)
        {
            if (DbfRecordRead != null)
            {
                DbfRecordRead(numberOfRecordsRead);
            }
        }

        public void dbfNumberOfRecordsRead(int numberOfRecords)
        {
            if (DbfNumberOfRecordsRead != null)
            {
                DbfNumberOfRecordsRead(numberOfRecords);
            }
        }


        public void DeleteTempFiles()
        {
            if (TempFiles != null)
            {
                for (int i = 0; i < TempFiles.Count; i++)
                {
                    if (File.Exists((string)TempFiles[i]))
                    {
                        File.Delete((string)TempFiles[i]);
                    }
                }
            }
        }

        public void DeleteTempDirectories()
        {
            if (TempDirectories != null)
            {
                for (int i = 0; i < TempDirectories.Count; i++)
                {
                    if (Directory.Exists((string)TempDirectories[i]))
                    {
                        Directory.Delete((string)TempDirectories[i], true);
                    }
                }
            }
        }
    }
}

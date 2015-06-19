using System;
using System.IO;
using System.Reflection;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers;
using System.Diagnostics;
using System.ComponentModel;
using USC.GISResearchLab.Common.Databases.SchemaManagers;

namespace TAMU.GeoInnovation.PointIntersectors.Census.PointIntersecters.Factories
{
    public sealed class CensusTractLevelCountsUpdaterReferenceImporterFactory
    {
        private CensusTractLevelCountsUpdaterReferenceImporterFactory() { }


        public static ICensusTractLevelCountsUpdaterImporterWorker GetCensusPointIntersectorReferenceImporter(TraceSource traceSource, BackgroundWorker backgroundWorker, IQueryManager outputDataQueryManager, ISchemaManager schemaManager)
        {
            ICensusTractLevelCountsUpdaterImporterWorker ret = null;


            string assemblyPath = "";
            Assembly assembly = null;
            switch (outputDataQueryManager.ProviderType)
            {

                case DataProviderType.SqlServer:
                    assemblyPath = Path.Combine(outputDataQueryManager.PathToDatabaseDLLs, "TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.SqlServer.dll");
                    assembly = Assembly.LoadFile(assemblyPath);
                    if (assembly != null)
                    {
                        ret = (ICensusTractLevelCountsUpdaterImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.SqlServer.SqlServerCensusTractLevelCountsUpdaterImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
                    }
                    else
                    {
                        throw new Exception("Unable to load Assembly: " + assemblyPath);
                    }
                    break;

                case DataProviderType.MySql:
                    assemblyPath = Path.Combine(outputDataQueryManager.PathToDatabaseDLLs, "TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.MySql.dll");
                    assembly = Assembly.LoadFile(assemblyPath);
                    if (assembly != null)
                    {
                        ret = (ICensusTractLevelCountsUpdaterImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.MySql.MySqlCensusTractLevelCountsUpdaterImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
                    }
                    else
                    {
                        throw new Exception("Unable to load Assembly: " + assemblyPath);
                    }
                    break;

                case DataProviderType.Npgsql:
                    assemblyPath = Path.Combine(outputDataQueryManager.PathToDatabaseDLLs, "TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.PostgreSql.dll");
                    assembly = Assembly.LoadFile(assemblyPath);
                    if (assembly != null)
                    {
                        ret = (ICensusTractLevelCountsUpdaterImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.PostgreSql.PostgreSqlCensusTractLevelCountsUpdaterImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
                    }
                    else
                    {
                        throw new Exception("Unable to load Assembly: " + assemblyPath);
                    }
                    break;

                case DataProviderType.MongoDB:
                    assemblyPath = Path.Combine(outputDataQueryManager.PathToDatabaseDLLs, "TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.MongoDB.dll");
                    assembly = Assembly.LoadFile(assemblyPath);
                    if (assembly != null)
                    {
                        ret = (ICensusTractLevelCountsUpdaterImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.MongoDB.MongoDBCensusTractLevelCountsUpdaterImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
                    }
                    else
                    {
                        throw new Exception("Unable to load Assembly: " + assemblyPath);
                    }
                    break;

                default:
                    throw new Exception("Unexpected or UnImplemented DataProviderType: " + outputDataQueryManager.ProviderType);
            }

            if (ret == null)
            {
                throw new Exception("Unable to create ICensusPointIntersector instance: " + assemblyPath);
            }

            return ret;
        }
    }
}
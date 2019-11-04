using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Databases.SchemaManagers;

namespace TAMU.GeoInnovation.PointIntersectors.Census.PointIntersecters.Factories
{
    public sealed class CensusTractLevelIndexUpdaterReferenceImporterFactory
    {
        private CensusTractLevelIndexUpdaterReferenceImporterFactory() { }


        public static ICensusTractLevelIndexUpdaterImporterWorker GetCensusPointIntersectorReferenceImporter(TraceSource traceSource, BackgroundWorker backgroundWorker, IQueryManager outputDataQueryManager, ISchemaManager schemaManager)
        {
            ICensusTractLevelIndexUpdaterImporterWorker ret = null;


            string assemblyPath = "";
            Assembly assembly = null;
            switch (outputDataQueryManager.ProviderType)
            {

                case DataProviderType.SqlServer:
                    assemblyPath = Path.Combine(outputDataQueryManager.PathToDatabaseDLLs, "TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.SqlServer.dll");
                    assembly = Assembly.LoadFile(assemblyPath);
                    if (assembly != null)
                    {
                        ret = (ICensusTractLevelIndexUpdaterImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.SqlServer.SqlServerCensusTractLevelIndexUpdaterImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
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
                        ret = (ICensusTractLevelIndexUpdaterImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.MySql.MySqlCensusTractLevelIndexUpdaterImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
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
                        ret = (ICensusTractLevelIndexUpdaterImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.PostgreSql.PostgreSqlCensusTractLevelIndexUpdaterImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
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
                        ret = (ICensusTractLevelIndexUpdaterImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.MongoDB.MongoDBCensusTractLevelIndexUpdaterImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
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
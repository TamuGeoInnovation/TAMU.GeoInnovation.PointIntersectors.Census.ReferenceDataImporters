using System;
using System.IO;
using System.Reflection;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers;
using USC.GISResearchLab.Common.Databases.SchemaManagers;
using System.ComponentModel;
using System.Diagnostics;

namespace TAMU.GeoInnovation.PointIntersectors.Census.PointIntersecters.Factories
{
    public sealed class CensusStateLevelReferenceImporterFactory
    {
        private CensusStateLevelReferenceImporterFactory() { }


        public static IStateLevelImporterWorker GetCensusPointIntersectorReferenceImporter(TraceSource traceSource, BackgroundWorker backgroundWorker, IQueryManager outputDataQueryManager, ISchemaManager schemaManager)
        {
            IStateLevelImporterWorker ret = null;


            string assemblyPath = "";
            Assembly assembly = null;
            switch (outputDataQueryManager.ProviderType)
            {

                case DataProviderType.SqlServer:
                    assemblyPath = Path.Combine(outputDataQueryManager.PathToDatabaseDLLs, "TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.SqlServer.dll");
                    assembly = Assembly.LoadFile(assemblyPath);
                    if (assembly != null)
                    {
                        ret = (IStateLevelImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.SqlServer.SqlServerStateLevelImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
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
                        ret = (IStateLevelImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.MySql.MySqlStateLevelImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
                    }
                    else
                    {
                        throw new Exception("Unable to load Assembly: " + assemblyPath);
                    }
                    break;

                case DataProviderType.Npgsql:
                    assemblyPath = Path.Combine(outputDataQueryManager.PathToDatabaseDLLs, "TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.PostgreSQL.dll");
                                                                                                                    
                    assembly = Assembly.LoadFile(assemblyPath);
                    if (assembly != null)
                    {
                        ret = (IStateLevelImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.PostgreSQL.PostgreSQLStateLevelImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
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
                        ret = (IStateLevelImporterWorker)assembly.CreateInstance("TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.MongoDB.MongoDBStateLevelImporterWorker", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { traceSource, backgroundWorker, outputDataQueryManager, schemaManager }, null, null);
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
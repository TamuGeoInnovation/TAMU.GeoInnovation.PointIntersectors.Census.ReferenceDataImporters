using System;
using System.IO;
using System.Reflection;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers;
using System.Diagnostics;
using System.ComponentModel;
using USC.GISResearchLab.Common.Databases.SchemaManagers;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Factories.Tiger2010.StateFiles
{
    public sealed class CensusTract2010FileFactory
    {
        private CensusTract2010FileFactory() { }


        public static ITigerFileLayout GetFile(IQueryManager queryManager, string stateName)
        {
            ITigerFileLayout ret = null;


            string assemblyPath = "";
            Assembly assembly = null;
            switch (queryManager.ProviderType)
            {

                case DataProviderType.SqlServer:
                    assemblyPath = Path.Combine(queryManager.PathToDatabaseDLLs, "TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.SqlServer.dll");
                    assembly = Assembly.LoadFile(assemblyPath);
                    if (assembly != null)
                    {
                        ret = (ITigerFileLayout)assembly.CreateInstance("TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.SqlServer.FileLayouts.Implementations.Tiger2010.StateFiles.CensusTract2010File", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { stateName }, null, null);
                    }
                    else
                    {
                        throw new Exception("Unable to load Assembly: " + assemblyPath);
                    }
                    break;

                case DataProviderType.MySql:
                    assemblyPath = Path.Combine(queryManager.PathToDatabaseDLLs, "TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.MySql.dll");
                    assembly = Assembly.LoadFile(assemblyPath);
                    if (assembly != null)
                    {
                        ret = (ITigerFileLayout)assembly.CreateInstance("TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.MySql.FileLayouts.Implementations.Tiger2010.StateFiles.CensusTract2010File", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { stateName }, null, null);
                    }
                    else
                    {
                        throw new Exception("Unable to load Assembly: " + assemblyPath);
                    }
                    break;

                case DataProviderType.Npgsql:
                    assemblyPath = Path.Combine(queryManager.PathToDatabaseDLLs, "TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.PostgreSql.dll");
                    assembly = Assembly.LoadFile(assemblyPath);
                    if (assembly != null)
                    {
                        ret = (ITigerFileLayout)assembly.CreateInstance("TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.PostgreSql.FileLayouts.Implementations.Tiger2010.StateFiles.CensusTract2010File", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { stateName }, null, null);
                    }
                    else
                    {
                        throw new Exception("Unable to load Assembly: " + assemblyPath);
                    }
                    break;

                case DataProviderType.MongoDB:
                    assemblyPath = Path.Combine(queryManager.PathToDatabaseDLLs, "TAMU.GeoInnovation.PointIntersectors.Census.ReferenceDataImporters.MongoDB.dll");
                    assembly = Assembly.LoadFile(assemblyPath);
                    if (assembly != null)
                    {
                        ret = (ITigerFileLayout)assembly.CreateInstance("TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.MongoDB.FileLayouts.Implementations.Tiger2010.StateFiles.CensusTract2010File", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { stateName }, null, null);
                    }
                    else
                    {
                        throw new Exception("Unable to load Assembly: " + assemblyPath);
                    }
                    break;

                default:
                    throw new Exception("Unexpected or UnImplemented DataProviderType: " + queryManager.ProviderType);
            }

            if (ret == null)
            {
                throw new Exception("Unable to create ICensusPointIntersector instance: " + assemblyPath);
            }

            return ret;
        }
    }
}
using System;
using System.IO;
using System.Reflection;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Databases.QueryManagers;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Factories.Tiger2015.CountryFiles
{
    public sealed class County2015FileFactory
    {
        private County2015FileFactory() { }


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
                        ret = (ITigerFileLayout)assembly.CreateInstance("TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.SqlServer.FileLayouts.Implementations.Tiger2015.CountryFiles.County2015File", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { stateName }, null, null);
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
                        ret = (ITigerFileLayout)assembly.CreateInstance("TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.MySql.FileLayouts.Implementations.Tiger2015.CountryFiles.County2015File", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { stateName }, null, null);
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
                        ret = (ITigerFileLayout)assembly.CreateInstance("TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.PostgreSql.FileLayouts.Implementations.Tiger2015.CountryFiles.County2015File", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { stateName }, null, null);
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
                        ret = (ITigerFileLayout)assembly.CreateInstance("TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.MongoDB.FileLayouts.Implementations.Tiger2015.CountryFiles.County2015File", true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { stateName }, null, null);
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
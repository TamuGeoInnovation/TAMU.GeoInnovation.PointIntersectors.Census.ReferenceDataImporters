using System;
using System.Data;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Delegates;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;
using USC.GISResearchLab.Common.Shapefiles.ShapefileReaders;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.AbstractClasses.Tiger1990.StateFiles
{
    public abstract class AbstractTiger1990ShapefileFileLayout : AbstractTiger1990FileLayout, ITigerShapefileFileLayout
    {

        #region Events

        public event ShapefileRecordReadHandler ShapefileRecordRead;
        public event ShapefileRecordConvertedHandler ShapefileRecordConverted;
        public event PercentReadHandler PercentRead;
        public event RecordsReadHandler RecordsRead;

        #endregion

        //#region Delegates

        //public delegate void ShapefileRecordReadHandler(int numberOfRecordsRead);
        //public delegate void ShapefileRecordConvertedHandler(int numberOfRecordsComputed);

        //#endregion


        public AbstractTiger1990ShapefileFileLayout(string tableName)
            : base(tableName) { }

        public override DataTable GetDataTable(string fileLocation)
        {
            DataTable ret = null;

            try
            {
                Reimers.Esri.Shapefile shapeFile = new Reimers.Esri.Shapefile(fileLocation);
                shapeFile.NotifyAfter = 10;
                shapeFile.DbfRecordRead += new Reimers.Esri.DbfRecordReadHandler(dbfRecordRead);
                shapeFile.DbfNumberOfRecordsRead += new Reimers.Esri.DbfNumberOfRecordsReadHandler(dbfNumberOfRecordsRead);
                shapeFile.ShapefileRecordRead += new Reimers.Esri.ShapefileRecordReadHandler(shapeFile_ShapefileRecordRead);
                shapeFile.ShapefileRecordConverted += new Reimers.Esri.ShapefileRecordConvertedHandler(shapeFile_ShapeRecordConverted);

                ret = shapeFile.GetShapefileAsDataTable(false, true, false, false, false, true, true, 4269);
            }
            catch (Exception e)
            {
                throw new Exception("Error getting datatable: " + e.Message, e);
            }

            return ret;
        }

        public override IDataReader GetDataReader(string fileLocation)
        {
            ExtendedShapefileDataReader ret = null;

            try
            {
                ret = new ExtendedShapefileDataReader(fileLocation);
                ret.IncludeSqlGeography = true;
                ret.IncludeSqlGeometry = true;
                ret.IncludeSoundex = HasSoundexColumns;
                ret.IncludeSoundexDM = HasSoundexDMColumns;
                ret.SoundexColumns = SoundexColumns;
                ret.SoundexDMColumns = SoundexDMColumns;

            }
            catch (Exception e)
            {
                throw new Exception("Error getting datatable: " + e.Message, e);
            }

            return ret;
        }

        public void shapeFile_ShapefileRecordRead(int numberOfRecordsRead)
        {
            if (ShapefileRecordRead != null)
            {
                ShapefileRecordRead(numberOfRecordsRead);
            }
        }

        public void shapeFile_ShapeRecordConverted(int numberOfRecordsComputed)
        {
            if (ShapefileRecordConverted != null)
            {
                ShapefileRecordConverted(numberOfRecordsComputed);
            }
        }

        public void shapeFile_PercentRead(double percentRead)
        {
            if (PercentRead != null)
            {
                PercentRead(percentRead);
            }
        }

        public void shapeFile_RecordsRead(int recordsRead, int totalRecords)
        {
            if (RecordsRead != null)
            {
                RecordsRead(recordsRead, totalRecords);
            }
        }
    }
}

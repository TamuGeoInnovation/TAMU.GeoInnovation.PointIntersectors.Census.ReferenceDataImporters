using System;
using System.Data;
//using Reimers.Esri;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Delegates;
using USC.GISResearchLab.Common.Shapefiles.ShapefileReaders;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces
{
    public interface ITigerShapefileFileLayout : ITigerFileLayout
    {

        #region Events

        event ShapefileRecordReadHandler ShapefileRecordRead;
        event ShapefileRecordConvertedHandler ShapefileRecordConverted;
        event PercentReadHandler PercentRead;
        event RecordsReadHandler RecordsRead;

        #endregion

        
        void shapeFile_ShapefileRecordRead(int numberOfRecordsRead);
        
        void shapeFile_ShapeRecordConverted(int numberOfRecordsComputed);
        
        void shapeFile_PercentRead(double percentRead);

        void shapeFile_RecordsRead(int recordsRead, int totalRecords);
        
    }
}

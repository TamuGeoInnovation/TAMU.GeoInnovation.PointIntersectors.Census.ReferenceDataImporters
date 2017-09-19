
namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Delegates
{

    #region Delegates

    public delegate void ShapefileRecordReadHandler(int numberOfRecordsRead);
    public delegate void ShapefileRecordConvertedHandler(int numberOfRecordsComputed);
    public delegate void PercentReadHandler(double percentRead);
    public delegate void RecordsReadHandler(int recordsRead, int totalRecords);

    #endregion

    public abstract class ShapefileReaderDelegates
    {



        


        public ShapefileReaderDelegates()
        { }

    }
}
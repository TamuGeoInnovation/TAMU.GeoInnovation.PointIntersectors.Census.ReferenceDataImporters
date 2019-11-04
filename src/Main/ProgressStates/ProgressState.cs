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

using System.Globalization;
using USC.GISResearchLab.Common.Threading.ProgressStates;
using USC.GISResearchLab.Common.Utils.Numbers;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public class ProgressState : TimeRemainableProgressState
    {
        #region Properties




        public string CurrentFile { get; set; }

        public string CurrentCounty { get; set; }


        public string CurrentState { get; set; }

        public int EdgesWroteTotal { get; set; }

        public int EdgesWroteCompleted { get; set; }

        public int EdgesShapefileRecordsReadTotal { get; set; }

        public int EdgesShapefileRecordsReadCompleted { get; set; }

        public int EdgesShapefileRecordsComputedTotal { get; set; }

        public int EdgesShapefileRecordsComputedCompleted { get; set; }

        public int EdgesDbfRecordsReadTotal { get; set; }

        public int EdgesDbfRecordsReadCompleted { get; set; }

        public int FeatureNamesTotal { get; set; }

        public int FeatureNamesCompleted { get; set; }

        public int AddressRangesTotal { get; set; }

        public int AddressRangesCompleted { get; set; }

        public int AddressRangesFeatureNamesTotal { get; set; }

        public int AddressRangesFeatureNamesCompleted { get; set; }

        public int FacesTotal { get; set; }

        public int FaceRecordsCompleted { get; set; }

        public int FilesTotal { get; set; }

        public int FilesCompleted { get; set; }

        public int StatesCompleted { get; set; }

        public int StatesTotal { get; set; }

        public int CountiesCompleted { get; set; }

        public int CountiesTotal { get; set; }


        public void ResetTimer()
        {
            StartTimeSet = false;
        }

        public string StatesString
        {
            get { return StatesCompleted + "/" + StatesTotal + " : " + PercentStatesCompleted.ToString("0.00", CultureInfo.InvariantCulture) + "%"; }
        }

        public string CountiesString
        {
            get { return CountiesCompleted + "/" + CountiesTotal + " : " + PercentCountiesCompleted.ToString("0.00", CultureInfo.InvariantCulture) + "%"; }
        }

        public string FilesString
        {
            get { return FilesCompleted + "/" + FilesTotal + " : " + PercentFilesCompleted.ToString("0.00", CultureInfo.InvariantCulture) + "%"; }
        }

        public string EdgesWroteString
        {
            get { return EdgesWroteCompleted + "/" + EdgesWroteTotal + " : " + PercentEdgesWroteCompleted.ToString("0.00", CultureInfo.InvariantCulture) + "% " + RemainingTimeString; }
        }

        public string EdgesShapefileRecordsReadString
        {
            get { return EdgesShapefileRecordsReadCompleted + "/" + EdgesShapefileRecordsReadTotal + " : " + PercentEdgesShapefileRecordsReadCompleted.ToString("0.00", CultureInfo.InvariantCulture) + "%"; }
        }

        public string EdgesShapefileRecordsComputedString
        {
            get { return EdgesShapefileRecordsComputedCompleted + "/" + EdgesShapefileRecordsComputedTotal + " : " + PercentEdgesShapefileRecordsComputedCompleted.ToString("0.00", CultureInfo.InvariantCulture) + "%"; }
        }

        public string EdgesDbfRecordsReadString
        {
            get { return EdgesDbfRecordsReadCompleted + "/" + EdgesDbfRecordsReadTotal + " : " + PercentEdgesDbfRecordsReadCompleted.ToString("0.00", CultureInfo.InvariantCulture) + "%"; }
        }

        public string FeatureNamesString
        {
            get { return FeatureNamesCompleted + "/" + FeatureNamesTotal + " : " + PercentFeatureNamesCompleted.ToString("0.00", CultureInfo.InvariantCulture) + "%"; }
        }

        public string AddressRangesString
        {
            get { return AddressRangesCompleted + "/" + AddressRangesTotal + " : " + PercentAddressRangesCompleted.ToString("0.00", CultureInfo.InvariantCulture) + "%"; }
        }

        public string AddressRangesFeatureNamesString
        {
            get { return AddressRangesFeatureNamesCompleted + "/" + AddressRangesFeatureNamesTotal + " : " + PercentAddressRangesFeatureNamesCompleted.ToString("0.00", CultureInfo.InvariantCulture) + "%"; }
        }

        public double PercentStatesCompleted
        {
            get { return NumberUtils.GetPercentageCompleted(StatesTotal, StatesCompleted); }
        }

        public double PercentCountiesCompleted
        {
            get { return NumberUtils.GetPercentageCompleted(CountiesTotal, CountiesCompleted); }
        }

        public double PercentFilesCompleted
        {
            get { return NumberUtils.GetPercentageCompleted(FilesTotal, FilesCompleted); }
        }

        public double PercentEdgesWroteCompleted
        {
            get { return NumberUtils.GetPercentageCompleted(EdgesWroteTotal, EdgesWroteCompleted); }
        }

        public double PercentEdgesShapefileRecordsReadCompleted
        {
            get { return NumberUtils.GetPercentageCompleted(EdgesShapefileRecordsReadTotal, EdgesShapefileRecordsReadCompleted); }
        }

        public double PercentEdgesShapefileRecordsComputedCompleted
        {
            get { return NumberUtils.GetPercentageCompleted(EdgesShapefileRecordsComputedTotal, EdgesShapefileRecordsComputedCompleted); }
        }

        public double PercentEdgesDbfRecordsReadCompleted
        {
            get { return NumberUtils.GetPercentageCompleted(EdgesDbfRecordsReadTotal, EdgesDbfRecordsReadCompleted); }
        }

        public double PercentFeatureNamesCompleted
        {
            get { return NumberUtils.GetPercentageCompleted(FeatureNamesTotal, FeatureNamesCompleted); }
        }

        public double PercentAddressRangesCompleted
        {
            get { return NumberUtils.GetPercentageCompleted(AddressRangesTotal, AddressRangesCompleted); }
        }

        public double PercentAddressRangesFeatureNamesCompleted
        {
            get { return NumberUtils.GetPercentageCompleted(AddressRangesFeatureNamesTotal, AddressRangesFeatureNamesCompleted); }
        }

        public double PercentFacesCompleted
        {
            get { return NumberUtils.GetPercentageCompleted(FacesTotal, FaceRecordsCompleted); }
        }

        #endregion

        public ProgressState()
        {
            StatesTotal = 0;
            StatesCompleted = 0;
            CurrentFile = "";
        }

        public override string ToString()
        {
            string ret = Message;
            if (StatesTotal > 0)
            {
                ret += " - " + StatesCompleted + "/" + StatesTotal + " : " + PercentStatesCompleted + "%";
            }
            return ret;
        }
    }
}

/*
 * Copyright © 2008 Daniel W. Goldberg
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

using System;
using USC.GISResearchLab.Common.Utils.Numbers;
using System.Globalization;
using System.Collections;
using USC.GISResearchLab.Common.Threading.ProgressStates;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public class CountryLevelImporterProgressState : TimeRemainableProgressState
    {
        #region Properties

        public string CurrentFile { get; set; }
        public string CurrentState { get; set; }

        public TimeRemainableProgressState ProgressStateFiles { get; set; }
        public TimeRemainableProgressState ProgressStateRecords { get; set; }

        

        #endregion

        public CountryLevelImporterProgressState()
        {
            ProgressStateFiles = new TimeRemainableProgressState();
            ProgressStateRecords = new TimeRemainableProgressState();
        }
    }
}

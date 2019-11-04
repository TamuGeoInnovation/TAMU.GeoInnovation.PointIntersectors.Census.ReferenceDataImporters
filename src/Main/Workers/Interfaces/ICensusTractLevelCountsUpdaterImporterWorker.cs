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

using System.Collections.Generic;
using System.ComponentModel;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public interface ICensusTractLevelCountsUpdaterImporterWorker : ICensusTractLevelImporterWorker
    {
        #region Properties


        bool shouldDoUpdateZIPCT2000Counts { get; set; }
        bool shouldDoUpdateZIPCT2010Counts { get; set; }
        bool shouldDoUpdatePlaceCT2000Counts { get; set; }
        bool shouldDoUpdatePlaceCT2010Counts { get; set; }

        List<string> Zips { get; set; }
        List<string> States { get; set; }

        #endregion

        bool RunCensusUpdates(DoWorkEventArgs e, string topDirectory, bool restart);

        void UpdateZIPCT2000Counts();
        void UpdateZIPCT2010Counts();


        void UpdatePlaceCT2000Counts();
        void UpdatePlaceCT2010Counts();

    }
}

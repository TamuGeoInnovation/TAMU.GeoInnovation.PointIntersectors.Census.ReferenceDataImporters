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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;

using USC.GISResearchLab.Common.Utils.Directories;
using USC.GISResearchLab.Common.Utils.Strings;
using USC.GISResearchLab.Common.Utils.Files;
using USC.GISResearchLab.Common.Diagnostics.TraceEvents;
using USC.GISResearchLab.Common.Databases;
using USC.GISResearchLab.Common.Census;
using USC.GISResearchLab.Common.Utils.Databases;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Core.Databases;
using USC.GISResearchLab.Common.Databases.SchemaManagers;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.ApplicationStates.Managers;
using USC.GISResearchLab.Common.Core.Databases.BulkCopys;
//using USC.GISResearchLab.AddressProcessing.Core.Standardizing.StandardizedAddresses.Lines.LastLines;

using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;

using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.AbstractClasses;
using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.StateFiles.Implementations;
using Microsoft.SqlServer.Types;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public interface ICensusTractLevelIndexUpdaterImporterWorker: ICensusTractLevelImporterWorker 
    {
        #region Properties

       

        bool shouldDoUpdateCT1990Indexes { get; set; }
        bool shouldDoUpdateCT2000Indexes { get; set; }
        bool shouldDoUpdateCT2010Indexes { get; set; }
        bool shouldDoUpdateCB2000Indexes { get; set; }
        bool shouldDoUpdateCB2010Indexes { get; set; }

        bool shouldDoUpdate2000PlaceIndexes { get; set; }
        bool shouldDoUpdate2000ConCityIndexes { get; set; }
        bool shouldDoUpdate2000CouSubIndexes { get; set; }

        bool shouldDoUpdate2010PlaceIndexes { get; set; }
        bool shouldDoUpdate2010ConCityIndexes { get; set; }
        bool shouldDoUpdate2010CouSubIndexes { get; set; }

        List<string> Zips { get; set; }
        List<string> States { get; set; }

        #endregion

        bool RunCensusUpdates(DoWorkEventArgs e, string topDirectory, bool restart);

        void UpdateCT1990Indexes();
        void UpdateCT2000Indexes();
        void UpdateCT2010Indexes();

        void UpdateCB2000Indexes();
        void UpdateCB2010Indexes();

        void Update2000PlaceIndexes();
        void Update2000ConCityIndexes();
        void Update2000CouSubIndexes();

        void Update2010PlaceIndexes();
        void Update2010ConCityIndexes();
        void Update2010CouSubIndexes();

    }
}

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
using USC.GISResearchLab.Common.Core.Databases.BulkCopys;
//using USC.GISResearchLab.AddressProcessing.Core.Standardizing.StandardizedAddresses.Lines.LastLines;
using USC.GISResearchLab.Common.Threading.ProgressStates;
using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.CountryFiles.AbstractClasses;
using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;
using USC.GISResearchLab.Common.Databases.ImportStatusManagers;
using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.AbstractClasses;
using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.CountryFiles.Implementations;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public interface ICountryLevelImporterWorker: IImporterWorker
    {
        #region Properties

        int FileCountTotal { get; set; }
        int FileCountCompleted { get; set; }

        bool ShouldDoStates2000 { get; set; }
        bool ShouldDoCounties2000 { get; set; }
        bool ShouldDoZcta32000 { get; set; }
        bool ShouldDoZcta52000 { get; set; }
        bool ShouldDoStates2008 { get; set; }
        bool ShouldDoCounties2008 { get; set; }
        bool ShouldDoZcta32008 { get; set; }
        bool ShouldDoZcta52008 { get; set; }
        bool ShouldDoStates2010 { get; set; }
        bool ShouldDoCounties2010 { get; set; }
        bool ShouldDoZcta52010 { get; set; }
        bool ShouldDoMetDiv2010 { get; set; }
        bool ShouldDoCbsa2010 { get; set; }
        bool ShouldDoStates2015 { get; set; }
        bool ShouldDoCounties2015 { get; set; }
        bool ShouldDoZcta52015 { get; set; }
        bool ShouldDoMetDiv2015 { get; set; }
        bool ShouldDoCbsa2015 { get; set; }
        
       
        #endregion



        bool ImportTigerNationFile(string nationDirectory, ITigerFileLayout tigerFile);

        bool ImportTiger2010NationFile(string nationDirectory, ITigerFileLayout tigerFile, bool shouldUseUnzippedFolder, bool shouldSkipExistingRecords);

        void CreateNationTigerTables(bool dropFirst);
    }
}

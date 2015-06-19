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


using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;




using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.AbstractClasses;
using USC.GISResearchLab.Common.Census.Tiger2010.FileLayouts.StateFiles.Implementations;
using Microsoft.SqlServer.Types;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public interface ICensusTractLevelImporterWorker : IStateLevelImporterWorker
    {
        #region Properties

        bool ShouldDoCensusTracts2000 { get; set; }
        bool ShouldDoCensusBlocks2000 { get; set; }
        bool ShouldDoCensusBlocks2008 { get; set; }
        bool ShouldDoCensusBlocks2010 { get; set; }
        bool ShouldDoCensusTracts2010 { get; set; }
        bool ShouldDoCensusBlockGroups2000 { get; set; }
        
        List<string> Zips { get; set; }
        List<string> States { get; set; }

        #endregion


    }
}

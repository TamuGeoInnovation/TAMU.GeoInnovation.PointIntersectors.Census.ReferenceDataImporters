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

using TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.FileLayouts.Interfaces;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public interface IStateLevelImporterWorker : IImporterWorker
    {
        #region Properties

        //new StateLevelImporterProgressState ProgressState { get; set; }

        bool ShouldDoPlaces2000 { get; set; }
        bool ShouldDoCountySubs2000 { get; set; }
        bool ShouldDoConsolodatedCities2000 { get; set; }

        bool ShouldDoPlaces2008 { get; set; }
        bool ShouldDoCountySubs2008 { get; set; }
        bool ShouldDoConsolodatedCities2008 { get; set; }

        bool ShouldDoPlaces2010 { get; set; }
        bool ShouldDoCountySubs2010 { get; set; }
        bool ShouldDoConsolodatedCities2010 { get; set; }

        #endregion

        void CreateStateTigerTables(string state, bool dropFirst);

        bool ImportStateDirectory(string stateName, string stateDirectoryName);

        bool ImportTigerStateFile(string state, string stateDirectory, ITigerFileLayout tigerFile);

        bool ImportTiger2010StateFile(string state, string stateDirectory, ITigerFileLayout tigerFile);

    }
}

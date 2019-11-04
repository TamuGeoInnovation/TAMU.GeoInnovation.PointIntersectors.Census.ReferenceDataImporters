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

using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using USC.GISResearchLab.AddressProcessing.Core.Standardizing.StandardizedAddresses.Lines.LastLines;
using USC.GISResearchLab.Common.Databases.QueryManagers;
using USC.GISResearchLab.Common.Databases.SchemaManagers;
using USC.GISResearchLab.Common.Diagnostics.TraceEvents;

namespace TAMU.GeoInnovation.Applications.Census.ReferenceDataImporter.Workers
{
    public class CensusTractLevelImporterWorker_Old : AbstractCensusTractLevelImporterWorker
    {
        #region Properties

        #endregion

        public CensusTractLevelImporterWorker_Old() : base() { }

        public CensusTractLevelImporterWorker_Old(TraceSource traceSource, BackgroundWorker backgroundWorker, IQueryManager outputDataQueryManager, ISchemaManager schemaManager) : base(traceSource, backgroundWorker, outputDataQueryManager, schemaManager) { }


        public void UpdateZIPCT2000Counts()
        {
            string sql = "";
            try
            {

                if (Zips != null)
                {

                    foreach (string zip in Zips)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update Tiger2000 ZIP Census Tract Counts: " + zip + "% - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql += " use Tiger2010CountryFiles ";
                            sql += " Select zcta5ce10, shapeGeog from us_zcta510 ";
                            sql += " where zcta5ce10 like '" + zip + "%' ";

                            SqlCommand cmd = new SqlCommand(sql);
                            IQueryManager qm = QueryManager;
                            qm.AddParameters(cmd.Parameters);
                            DataTable dataTable = qm.ExecuteDataTable(CommandType.Text, cmd.CommandText, 6000, true);

                            if (dataTable != null && dataTable.Rows.Count > 0)
                            {
                                ProgressState.ProgressStateRecords.Total = dataTable.Rows.Count;
                                ProgressState.ProgressStateRecords.Completed = 0;
                                TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "got records : " + dataTable.Rows.Count);

                                int i = 1;

                                foreach (DataRow dataRow in dataTable.Rows)
                                {
                                    if (!BackgroundWorker.CancellationPending)
                                    {
                                        List<string> intersectingStates = new List<string>();
                                        int totalIntersections = 0;

                                        string id = (String)dataRow["zcta5ce10"];

                                        if (dataRow["shapeGeog"] != null && dataRow["shapeGeog"] != DBNull.Value)
                                        {
                                            SqlGeography sqlGeography = (SqlGeography)dataRow["shapeGeog"];

                                            if (sqlGeography != null)
                                            {

                                                ProgressState.ProgressStateRecords.Completed = i;
                                                BackgroundWorker.ReportProgress(Convert.ToInt32(ProgressState.ProgressStateRecords.PercentCompleted), ProgressState);

                                                string stateName1 = StateUtils.GetStateByZIP3Digit(id.Substring(0, 3));

                                                if (StateUtils.isState(stateName1))
                                                {
                                                    sql = "";
                                                    sql += " use Tiger2000CensusTracts ";
                                                    sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + stateName1 + "_tract00') ";
                                                    sql += " SELECT COUNT(CtidFp00) ";
                                                    sql += " FROM  " + stateName1 + "_tract00 ";
                                                    sql += " WITH (INDEX (idx_geog))";
                                                    sql += " WHERE shapeGeog.STIntersects(@shapeGeog) = 1";

                                                    cmd = new SqlCommand(sql);

                                                    SqlParameter parameter = new SqlParameter("shapeGeog", sqlGeography);

                                                    parameter.Direction = ParameterDirection.Input;
                                                    parameter.ParameterName = "shapeGeog";
                                                    parameter.SourceColumn = "shapeGeog";
                                                    parameter.SqlDbType = SqlDbType.Udt;
                                                    parameter.UdtTypeName = "GEOGRAPHY";
                                                    parameter.SourceVersion = DataRowVersion.Current;

                                                    cmd.Parameters.Add(parameter);


                                                    qm = QueryManager;
                                                    qm.AddParameters(cmd.Parameters);
                                                    int intersections = qm.ExecuteScalarInt(CommandType.Text, cmd.CommandText, 6000, true);

                                                    if (intersections >= 0)
                                                    {
                                                        totalIntersections += intersections;

                                                        if (intersections > 0)
                                                        {
                                                            intersectingStates.Add(stateName1);
                                                        }

                                                    }
                                                    else
                                                    {
                                                        string here = "";
                                                    }
                                                }



                                                i++;
                                            }

                                            if (!BackgroundWorker.CancellationPending)
                                            {
                                                if (totalIntersections > 0)
                                                {
                                                    sql = " use Tiger2010CountryFiles ";
                                                    sql += " update us_zcta510";
                                                    sql += " set num2000TractsIntersect = " + totalIntersections;
                                                    sql += " WHERE zcta5ce10 = '" + id + "'";

                                                    cmd = new SqlCommand(sql);
                                                    qm = QueryManager;
                                                    qm.AddParameters(cmd.Parameters);
                                                    qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in UpdateZIPCT2010Counts: " + e.Message, e);
            }
        }

        public void UpdateZIPCT2010Counts()
        {
            string sql = "";
            try
            {

                if (Zips != null)
                {

                    foreach (string zip in Zips)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update Tiger2010 ZIP Census Tract Counts: " + zip + "% - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql += " use Tiger2010CountryFiles ";
                            sql += " Select zcta5ce10, shapeGeog from us_zcta510 ";
                            sql += " where zcta5ce10 like '" + zip + "%' ";

                            SqlCommand cmd = new SqlCommand(sql);
                            IQueryManager qm = QueryManager;
                            qm.AddParameters(cmd.Parameters);
                            DataTable dataTable = qm.ExecuteDataTable(CommandType.Text, cmd.CommandText, 6000, true);

                            if (dataTable != null && dataTable.Rows.Count > 0)
                            {
                                ProgressState.ProgressStateRecords.Total = dataTable.Rows.Count;
                                ProgressState.ProgressStateRecords.Completed = 0;
                                TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "got records : " + dataTable.Rows.Count);

                                int i = 1;

                                foreach (DataRow dataRow in dataTable.Rows)
                                {
                                    if (!BackgroundWorker.CancellationPending)
                                    {
                                        List<string> intersectingStates = new List<string>();
                                        int totalIntersections = 0;

                                        string id = (String)dataRow["zcta5ce10"];

                                        if (dataRow["shapeGeog"] != null && dataRow["shapeGeog"] != DBNull.Value)
                                        {
                                            SqlGeography sqlGeography = (SqlGeography)dataRow["shapeGeog"];

                                            if (sqlGeography != null)
                                            {

                                                ProgressState.ProgressStateRecords.Completed = i;
                                                BackgroundWorker.ReportProgress(Convert.ToInt32(ProgressState.ProgressStateRecords.PercentCompleted), ProgressState);

                                                string stateName1 = StateUtils.GetStateByZIP3Digit(id.Substring(0, 3));

                                                if (StateUtils.isState(stateName1))
                                                {
                                                    sql = "";
                                                    sql += " use Tiger2010CensusTracts ";
                                                    sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + stateName1 + "_tract10') ";
                                                    sql += " SELECT COUNT(uniqueId) ";
                                                    sql += " FROM  " + stateName1 + "_tract10 ";
                                                    sql += " WITH (INDEX (idx_geog))";
                                                    sql += " WHERE shapeGeog.STIntersects(@shapeGeog) = 1";

                                                    cmd = new SqlCommand(sql);

                                                    SqlParameter parameter = new SqlParameter("shapeGeog", sqlGeography);

                                                    parameter.Direction = ParameterDirection.Input;
                                                    parameter.ParameterName = "shapeGeog";
                                                    parameter.SourceColumn = "shapeGeog";
                                                    parameter.SqlDbType = SqlDbType.Udt;
                                                    parameter.UdtTypeName = "GEOGRAPHY";
                                                    parameter.SourceVersion = DataRowVersion.Current;

                                                    cmd.Parameters.Add(parameter);


                                                    qm = QueryManager;
                                                    qm.AddParameters(cmd.Parameters);
                                                    int intersections = qm.ExecuteScalarInt(CommandType.Text, cmd.CommandText, 6000, true);

                                                    if (intersections >= 0)
                                                    {
                                                        totalIntersections += intersections;

                                                        if (intersections > 0)
                                                        {
                                                            intersectingStates.Add(stateName1);
                                                        }

                                                    }
                                                    else
                                                    {
                                                        string here = "";
                                                    }
                                                }



                                                i++;
                                            }

                                            if (!BackgroundWorker.CancellationPending)
                                            {
                                                if (totalIntersections > 0)
                                                {
                                                    sql = " use Tiger2010CountryFiles ";
                                                    sql += " update us_zcta510";
                                                    sql += " set num2010TractsIntersect = " + totalIntersections;
                                                    sql += " WHERE zcta5ce10 = '" + id + "'";

                                                    cmd = new SqlCommand(sql);
                                                    qm = QueryManager;
                                                    qm.AddParameters(cmd.Parameters);
                                                    qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in UpdateZIPCT2010Counts: " + e.Message, e);
            }
        }



        public void UpdatePlaceCT2000Counts()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update Tiger2000 Place Census Tract Counts: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place10') ";
                            sql += " ALTER TABLE  " + state + "_place10 ";
                            sql += " ADD num2000TractsIntersect int null default 0; ";

                            SqlCommand cmd = new SqlCommand(sql);
                            IQueryManager qm = QueryManager;
                            qm.AddParameters(cmd.Parameters);
                            qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);

                            sql = "";
                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place10') ";
                            sql += " Select placeFp10, shapeGeog from " + state + "_place10; ";

                            cmd = new SqlCommand(sql);
                            qm = QueryManager;
                            qm.AddParameters(cmd.Parameters);
                            DataTable dataTable = qm.ExecuteDataTable(CommandType.Text, cmd.CommandText, 6000, true);

                            if (dataTable != null && dataTable.Rows.Count > 0)
                            {
                                ProgressState.ProgressStateRecords.Total = dataTable.Rows.Count;
                                ProgressState.ProgressStateRecords.Completed = 0;
                                TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "got records : " + dataTable.Rows.Count);

                                int i = 1;

                                foreach (DataRow dataRow in dataTable.Rows)
                                {
                                    if (!BackgroundWorker.CancellationPending)
                                    {
                                        List<string> intersectingStates = new List<string>();
                                        int totalIntersections = 0;

                                        string id = (String)dataRow["placeFp10"];

                                        if (dataRow["shapeGeog"] != null && dataRow["shapeGeog"] != DBNull.Value)
                                        {
                                            SqlGeography sqlGeography = (SqlGeography)dataRow["shapeGeog"];

                                            if (sqlGeography != null)
                                            {

                                                ProgressState.ProgressStateRecords.Completed = i;
                                                BackgroundWorker.ReportProgress(Convert.ToInt32(ProgressState.ProgressStateRecords.PercentCompleted), ProgressState);

                                                sql = "";
                                                sql += " use Tiger2000CensusTracts ";
                                                sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_tract00') ";
                                                sql += " SELECT COUNT(CtidFp00) ";
                                                sql += " FROM  " + state + "_tract00 ";
                                                sql += " WITH (INDEX (idx_geog))";
                                                sql += " WHERE shapeGeog.STIntersects(@shapeGeog) = 1 ;";

                                                cmd = new SqlCommand(sql);

                                                SqlParameter parameter = new SqlParameter("shapeGeog", sqlGeography);

                                                parameter.Direction = ParameterDirection.Input;
                                                parameter.ParameterName = "shapeGeog";
                                                parameter.SourceColumn = "shapeGeog";
                                                parameter.SqlDbType = SqlDbType.Udt;
                                                parameter.UdtTypeName = "GEOGRAPHY";
                                                parameter.SourceVersion = DataRowVersion.Current;

                                                cmd.Parameters.Add(parameter);


                                                qm = QueryManager;
                                                qm.AddParameters(cmd.Parameters);
                                                int intersections = qm.ExecuteScalarInt(CommandType.Text, cmd.CommandText, 6000, true);

                                                if (intersections >= 0)
                                                {
                                                    totalIntersections += intersections;

                                                    if (intersections > 0)
                                                    {
                                                        intersectingStates.Add(state);
                                                    }

                                                }
                                                else
                                                {
                                                    string here = "";
                                                }
                                            }



                                            i++;
                                        }

                                        if (!BackgroundWorker.CancellationPending)
                                        {
                                            if (totalIntersections > 0)
                                            {
                                                sql = " use Tiger2010StateFiles ";
                                                sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place10') ";
                                                sql += " update  " + state + "_place10 ";
                                                sql += " set num2000TractsIntersect = " + totalIntersections;
                                                sql += " WHERE placeFp10 = '" + id + "' ;";

                                                cmd = new SqlCommand(sql);
                                                qm = QueryManager;
                                                qm.AddParameters(cmd.Parameters);
                                                qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in UpdatePlaceCT2000Counts: " + e.Message, e);
            }
        }

        public void UpdatePlaceCT2010Counts()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update Tiger2010 Place Census Tract Counts: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place10') ";
                            sql += " ALTER TABLE  " + state + "_place10 ";
                            sql += " ADD num2010TractsIntersect int null default 0; ";

                            SqlCommand cmd = new SqlCommand(sql);
                            IQueryManager qm = QueryManager;
                            qm.AddParameters(cmd.Parameters);
                            qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);

                            sql = "";
                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place10') ";
                            sql += " Select placeFp10, shapeGeog from " + state + "_place10; ";

                            cmd = new SqlCommand(sql);
                            qm = QueryManager;
                            qm.AddParameters(cmd.Parameters);
                            DataTable dataTable = qm.ExecuteDataTable(CommandType.Text, cmd.CommandText, 6000, true);

                            if (dataTable != null && dataTable.Rows.Count > 0)
                            {
                                ProgressState.ProgressStateRecords.Total = dataTable.Rows.Count;
                                ProgressState.ProgressStateRecords.Completed = 0;
                                TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "got records : " + dataTable.Rows.Count);

                                int i = 1;

                                foreach (DataRow dataRow in dataTable.Rows)
                                {
                                    if (!BackgroundWorker.CancellationPending)
                                    {
                                        List<string> intersectingStates = new List<string>();
                                        int totalIntersections = 0;

                                        string id = (String)dataRow["placeFp10"];

                                        if (dataRow["shapeGeog"] != null && dataRow["shapeGeog"] != DBNull.Value)
                                        {
                                            SqlGeography sqlGeography = (SqlGeography)dataRow["shapeGeog"];

                                            if (sqlGeography != null)
                                            {

                                                ProgressState.ProgressStateRecords.Completed = i;
                                                BackgroundWorker.ReportProgress(Convert.ToInt32(ProgressState.ProgressStateRecords.PercentCompleted), ProgressState);

                                                sql = "";
                                                sql += " use Tiger2010CensusTracts ";
                                                sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_tract10') ";
                                                sql += " SELECT COUNT(uniqueId) ";
                                                sql += " FROM  " + state + "_tract10 ";
                                                sql += " WITH (INDEX (idx_geog))";
                                                sql += " WHERE shapeGeog.STIntersects(@shapeGeog) = 1 ;";

                                                cmd = new SqlCommand(sql);

                                                SqlParameter parameter = new SqlParameter("shapeGeog", sqlGeography);

                                                parameter.Direction = ParameterDirection.Input;
                                                parameter.ParameterName = "shapeGeog";
                                                parameter.SourceColumn = "shapeGeog";
                                                parameter.SqlDbType = SqlDbType.Udt;
                                                parameter.UdtTypeName = "GEOGRAPHY";
                                                parameter.SourceVersion = DataRowVersion.Current;

                                                cmd.Parameters.Add(parameter);


                                                qm = QueryManager;
                                                qm.AddParameters(cmd.Parameters);
                                                int intersections = qm.ExecuteScalarInt(CommandType.Text, cmd.CommandText, 6000, true);

                                                if (intersections >= 0)
                                                {
                                                    totalIntersections += intersections;

                                                    if (intersections > 0)
                                                    {
                                                        intersectingStates.Add(state);
                                                    }

                                                }
                                                else
                                                {
                                                    string here = "";
                                                }
                                            }



                                            i++;
                                        }

                                        if (!BackgroundWorker.CancellationPending)
                                        {
                                            if (totalIntersections > 0)
                                            {
                                                sql = " use Tiger2010StateFiles ";
                                                sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place10') ";
                                                sql += " update  " + state + "_place10 ";
                                                sql += " set num2010TractsIntersect = " + totalIntersections;
                                                sql += " WHERE placeFp10 = '" + id + "' ;";

                                                cmd = new SqlCommand(sql);
                                                qm = QueryManager;
                                                qm.AddParameters(cmd.Parameters);
                                                qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in UpdatePlaceCT2000Counts: " + e.Message, e);
            }
        }


        public void UpdateCT1990Indexes()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update CT 1990 Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql = "";

                            sql += " use Tiger1990CensusTracts ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "') ";
                            sql += " ALTER TABLE [" + state + "] ADD  CONSTRAINT [PK_" + state + "] PRIMARY KEY CLUSTERED ([UniqueId] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger1990CensusTracts ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "') ";
                            sql += " CREATE SPATIAL INDEX [idx_geog] ON [" + state + "] ";
                            sql += " ( ";
                            sql += " [shapeGeog] ";
                            sql += " )USING  GEOGRAPHY_GRID  ";
                            sql += " WITH ( ";
                            sql += " GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM),  ";
                            sql += " CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ; ";




                            if (!String.IsNullOrEmpty(sql))
                            {
                                string[] sqls = sql.Split(';');

                                for (int i = 0; i < sqls.Length; i++)
                                {
                                    string sql1 = sqls[i].Trim();

                                    if (!String.IsNullOrEmpty(sql1))
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "running Update CT 1990 Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql1);


                                        SqlCommand cmd = new SqlCommand(sql1);
                                        IQueryManager qm = QueryManager;
                                        qm.AddParameters(cmd.Parameters);
                                        qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                    }
                                }
                            }

                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in UpdateCT1990Indexes: " + e.Message, e);
            }
        }

        public void UpdateCT2000Indexes()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update CT 2000 Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql = "";

                            sql += " use Tiger2000CensusTracts ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_tract00') ";
                            sql += " ALTER TABLE [" + state + "_tract00] ADD  CONSTRAINT [PK_" + state + "_tract00] PRIMARY KEY CLUSTERED ([CtidFp00] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000CensusTracts ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_tract00') ";
                            sql += " CREATE SPATIAL INDEX [idx_geog] ON [" + state + "_tract00] ";
                            sql += " ( ";
                            sql += " [shapeGeog] ";
                            sql += " )USING  GEOGRAPHY_GRID  ";
                            sql += " WITH ( ";
                            sql += " GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM),  ";
                            sql += " CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ; ";




                            if (!String.IsNullOrEmpty(sql))
                            {
                                string[] sqls = sql.Split(';');

                                for (int i = 0; i < sqls.Length; i++)
                                {
                                    string sql1 = sqls[i].Trim();

                                    if (!String.IsNullOrEmpty(sql1))
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "running Update CT 2000 Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql1);


                                        SqlCommand cmd = new SqlCommand(sql1);
                                        IQueryManager qm = QueryManager;
                                        qm.AddParameters(cmd.Parameters);
                                        qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                    }
                                }
                            }

                        }
                        else
                        {
                            break;
                        }

                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update CT 2000 Indexes - Complete: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in UpdateCT2000Indexes: " + e.Message, e);
            }
        }

        public void UpdateCT2010Indexes()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update CT 2010 Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql = "";

                            sql += " use Tiger2010CensusTracts ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_tract10') ";
                            sql += " ALTER TABLE [" + state + "_tract10] ADD  CONSTRAINT [PK_" + state + "_tract10] PRIMARY KEY CLUSTERED ([UniqueId] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010CensusTracts ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_tract10') ";
                            sql += " CREATE SPATIAL INDEX [idx_geog] ON [" + state + "_tract10] ";
                            sql += " ( ";
                            sql += " [shapeGeog] ";
                            sql += " )USING  GEOGRAPHY_GRID  ";
                            sql += " WITH ( ";
                            sql += " GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM),  ";
                            sql += " CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ; ";




                            if (!String.IsNullOrEmpty(sql))
                            {
                                string[] sqls = sql.Split(';');

                                for (int i = 0; i < sqls.Length; i++)
                                {
                                    string sql1 = sqls[i].Trim();

                                    if (!String.IsNullOrEmpty(sql1))
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "running Update CT 2010 Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql1);


                                        SqlCommand cmd = new SqlCommand(sql1);
                                        IQueryManager qm = QueryManager;
                                        qm.AddParameters(cmd.Parameters);
                                        qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                    }
                                }
                            }

                        }
                        else
                        {
                            break;
                        }

                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update CT 2010 Indexes - Complete: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in UpdateCT2010Indexes: " + e.Message, e);
            }
        }




        public void UpdateCB2000Indexes()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update CB 2000 Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql = "";

                            sql += " use Tiger2000CensusBlocks ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_tabblock00') ";
                            sql += " ALTER TABLE [" + state + "_tabblock00] ADD  CONSTRAINT [PK_" + state + "_tabblock00] PRIMARY KEY CLUSTERED ([blkIdFp00] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000CensusBlocks ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_tabblock00') ";
                            sql += " CREATE SPATIAL INDEX [idx_geog] ON [" + state + "_tabblock00] ";
                            sql += " ( ";
                            sql += " [shapeGeog] ";
                            sql += " )USING  GEOGRAPHY_GRID  ";
                            sql += " WITH ( ";
                            sql += " GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM),  ";
                            sql += " CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ; ";




                            if (!String.IsNullOrEmpty(sql))
                            {
                                string[] sqls = sql.Split(';');

                                for (int i = 0; i < sqls.Length; i++)
                                {
                                    string sql1 = sqls[i].Trim();

                                    if (!String.IsNullOrEmpty(sql1))
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "running Update CB 2000 Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql1);


                                        SqlCommand cmd = new SqlCommand(sql1);
                                        IQueryManager qm = QueryManager;
                                        qm.AddParameters(cmd.Parameters);
                                        qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                    }
                                }
                            }

                        }
                        else
                        {
                            break;
                        }

                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update CB 2000 Indexes - Complete: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in UpdateCB2000Indexes: " + e.Message, e);
            }
        }

        public void UpdateCB2010Indexes()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update CB 2010 Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql = "";

                            sql += " use Tiger2010CensusBlocks ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_tabblock10') ";
                            sql += " ALTER TABLE [" + state + "_tabblock10] ADD  CONSTRAINT [PK_" + state + "_tabblock10] PRIMARY KEY CLUSTERED ([uniqueId] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010CensusBlocks ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_tabblock10') ";
                            sql += " CREATE SPATIAL INDEX [idx_geog] ON [" + state + "_tabblock10] ";
                            sql += " ( ";
                            sql += " [shapeGeog] ";
                            sql += " )USING  GEOGRAPHY_GRID  ";
                            sql += " WITH ( ";
                            sql += " GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM),  ";
                            sql += " CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ; ";




                            if (!String.IsNullOrEmpty(sql))
                            {
                                string[] sqls = sql.Split(';');

                                for (int i = 0; i < sqls.Length; i++)
                                {
                                    string sql1 = sqls[i].Trim();

                                    if (!String.IsNullOrEmpty(sql1))
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "running Update CB 2010 Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql1);


                                        SqlCommand cmd = new SqlCommand(sql1);
                                        IQueryManager qm = QueryManager;
                                        qm.AddParameters(cmd.Parameters);
                                        qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                    }
                                }
                            }

                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update CB 2010 Indexes - Complete: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");


                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in UpdateCB2010Indexes: " + e.Message, e);
            }
        }




        public void Update2000PlaceIndexes()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2000 Place Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql = "";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place00') ";
                            sql += " ALTER TABLE [" + state + "_place00] ADD  CONSTRAINT [PK_" + state + "_place00] PRIMARY KEY CLUSTERED ([placeFp00] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place00') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name00] ON [dbo].[" + state + "_place00] (Name00) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place00') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name00_Soundex] ON [dbo].[" + state + "_place00] (Name00_Soundex) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place00') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name00_SoundexDM] ON [dbo].[" + state + "_place00] (Name00_SoundexDM) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place00') ";
                            sql += " CREATE SPATIAL INDEX [idx_geog] ON [" + state + "_place00] ";
                            sql += " ( ";
                            sql += " [shapeGeog] ";
                            sql += " )USING  GEOGRAPHY_GRID  ";
                            sql += " WITH ( ";
                            sql += " GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM),  ";
                            sql += " CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ; ";




                            if (!String.IsNullOrEmpty(sql))
                            {
                                string[] sqls = sql.Split(';');

                                for (int i = 0; i < sqls.Length; i++)
                                {
                                    string sql1 = sqls[i].Trim();

                                    if (!String.IsNullOrEmpty(sql1))
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "running Update 2000 Place Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql1);


                                        SqlCommand cmd = new SqlCommand(sql1);
                                        IQueryManager qm = QueryManager;
                                        qm.AddParameters(cmd.Parameters);
                                        qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                    }
                                }
                            }

                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2000 Place Indexes - Complete: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");


                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in Update2000PlaceIndexes: " + e.Message, e);
            }
        }

        public void Update2000ConCityIndexes()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2000 ConCity Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql = "";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_conCity00') ";
                            sql += " ALTER TABLE [" + state + "_conCity00] ADD  CONSTRAINT [PK_" + state + "_conCity00] PRIMARY KEY CLUSTERED ([conctyFp00] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_conCity00') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name00] ON [dbo].[" + state + "_conCity00] (Name00) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_conCity00') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name00_Soundex] ON [dbo].[" + state + "_conCity00] (Name00_Soundex) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_conCity00') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name00_SoundexDM] ON [dbo].[" + state + "_conCity00] (Name00_SoundexDM) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_conCity00') ";
                            sql += " CREATE SPATIAL INDEX [idx_geog] ON [" + state + "_conCity00] ";
                            sql += " ( ";
                            sql += " [shapeGeog] ";
                            sql += " )USING  GEOGRAPHY_GRID  ";
                            sql += " WITH ( ";
                            sql += " GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM),  ";
                            sql += " CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ; ";




                            if (!String.IsNullOrEmpty(sql))
                            {
                                string[] sqls = sql.Split(';');

                                for (int i = 0; i < sqls.Length; i++)
                                {
                                    string sql1 = sqls[i].Trim();

                                    if (!String.IsNullOrEmpty(sql1))
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "running Update 2000 ConCity Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql1);


                                        SqlCommand cmd = new SqlCommand(sql1);
                                        IQueryManager qm = QueryManager;
                                        qm.AddParameters(cmd.Parameters);
                                        qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                    }
                                }
                            }

                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2000 ConCity Indexes - Complete: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");


                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in Update2000ConCityIndexes: " + e.Message, e);
            }
        }

        public void Update2000CouSubIndexes()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2000 CouSub Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql = "";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_cousub00') ";
                            sql += " ALTER TABLE [" + state + "_cousub00] ADD  CONSTRAINT [PK_" + state + "_cousub00] PRIMARY KEY CLUSTERED ([cousubFp00] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_cousub00') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name00] ON [dbo].[" + state + "_cousub00] (Name00) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_cousub00') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name00_Soundex] ON [dbo].[" + state + "_cousub00] (Name00_Soundex) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_cousub00') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name00_SoundexDM] ON [dbo].[" + state + "_cousub00] (Name00_SoundexDM) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2000StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_cousub00') ";
                            sql += " CREATE SPATIAL INDEX [idx_geog] ON [" + state + "_cousub00] ";
                            sql += " ( ";
                            sql += " [shapeGeog] ";
                            sql += " )USING  GEOGRAPHY_GRID  ";
                            sql += " WITH ( ";
                            sql += " GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM),  ";
                            sql += " CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ; ";





                            if (!String.IsNullOrEmpty(sql))
                            {
                                string[] sqls = sql.Split(';');

                                for (int i = 0; i < sqls.Length; i++)
                                {
                                    string sql1 = sqls[i].Trim();

                                    if (!String.IsNullOrEmpty(sql1))
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "running Update 2000 CouSub Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql1);


                                        SqlCommand cmd = new SqlCommand(sql1);
                                        IQueryManager qm = QueryManager;
                                        qm.AddParameters(cmd.Parameters);
                                        qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                    }
                                }
                            }

                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2000 CouSub Indexes - Complete: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");


                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in Update2000CouSubIndexes: " + e.Message, e);
            }
        }





        public void Update2010PlaceIndexes()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2010 Place Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql = "";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place10') ";
                            sql += " ALTER TABLE [" + state + "_place10] ADD  CONSTRAINT [PK_" + state + "_place10] PRIMARY KEY CLUSTERED ([placeFp10] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place10') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name10] ON [dbo].[" + state + "_place10] (Name10) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place10') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name10_Soundex] ON [dbo].[" + state + "_place10] (Name10_Soundex) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place10') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name10_SoundexDM] ON [dbo].[" + state + "_place10] (Name10_SoundexDM) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_place10') ";
                            sql += " CREATE SPATIAL INDEX [idx_geog] ON [" + state + "_place10] ";
                            sql += " ( ";
                            sql += " [shapeGeog] ";
                            sql += " )USING  GEOGRAPHY_GRID  ";
                            sql += " WITH ( ";
                            sql += " GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM),  ";
                            sql += " CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ; ";




                            if (!String.IsNullOrEmpty(sql))
                            {
                                string[] sqls = sql.Split(';');

                                for (int i = 0; i < sqls.Length; i++)
                                {
                                    string sql1 = sqls[i].Trim();

                                    if (!String.IsNullOrEmpty(sql1))
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "running Update 2010 Place Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql1);


                                        SqlCommand cmd = new SqlCommand(sql1);
                                        IQueryManager qm = QueryManager;
                                        qm.AddParameters(cmd.Parameters);
                                        qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                    }
                                }
                            }

                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2010 Place Indexes - Complete: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");


                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in Update2010PlaceIndexes: " + e.Message, e);
            }
        }

        public void Update2010ConCityIndexes()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2010 ConCity Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql = "";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_conCity10') ";
                            sql += " ALTER TABLE [" + state + "_conCity10] ADD  CONSTRAINT [PK_" + state + "_conCity10] PRIMARY KEY CLUSTERED ([conctyFp10] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_conCity10') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name10] ON [dbo].[" + state + "_conCity10] (Name10) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_conCity10') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name10_Soundex] ON [dbo].[" + state + "_conCity10] (Name10_Soundex) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_conCity10') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name10_SoundexDM] ON [dbo].[" + state + "_conCity10] (Name10_SoundexDM) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_conCity10') ";
                            sql += " CREATE SPATIAL INDEX [idx_geog] ON [" + state + "_conCity10] ";
                            sql += " ( ";
                            sql += " [shapeGeog] ";
                            sql += " )USING  GEOGRAPHY_GRID  ";
                            sql += " WITH ( ";
                            sql += " GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM),  ";
                            sql += " CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ; ";




                            if (!String.IsNullOrEmpty(sql))
                            {
                                string[] sqls = sql.Split(';');

                                for (int i = 0; i < sqls.Length; i++)
                                {
                                    string sql1 = sqls[i].Trim();

                                    if (!String.IsNullOrEmpty(sql1))
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "running Update 2010 ConCity Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql1);


                                        SqlCommand cmd = new SqlCommand(sql1);
                                        IQueryManager qm = QueryManager;
                                        qm.AddParameters(cmd.Parameters);
                                        qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                    }
                                }
                            }

                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2010 ConCity Indexes - Complete: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");


                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in Update2010ConCityIndexes: " + e.Message, e);
            }
        }

        public void Update2010CouSubIndexes()
        {
            string sql = "";
            try
            {

                if (States != null)
                {

                    foreach (string state in States)
                    {
                        if (!BackgroundWorker.CancellationPending)
                        {
                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2010 CouSub Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");

                            sql = "";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_cousub10') ";
                            sql += " ALTER TABLE [" + state + "_cousub10] ADD  CONSTRAINT [PK_" + state + "_cousub10] PRIMARY KEY CLUSTERED ([cousubFp10] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_cousub10') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name10] ON [dbo].[" + state + "_cousub10] (Name10) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_cousub10') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name10_Soundex] ON [dbo].[" + state + "_cousub10] (Name10_Soundex) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_cousub10') ";
                            sql += " CREATE NONCLUSTERED INDEX [IDX_" + state + "Name10_SoundexDM] ON [dbo].[" + state + "_cousub10] (Name10_SoundexDM) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]; ";

                            sql += " use Tiger2010StateFiles ";
                            sql += " IF  EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND name = '" + state + "_cousub10') ";
                            sql += " CREATE SPATIAL INDEX [idx_geog] ON [" + state + "_cousub10] ";
                            sql += " ( ";
                            sql += " [shapeGeog] ";
                            sql += " )USING  GEOGRAPHY_GRID  ";
                            sql += " WITH ( ";
                            sql += " GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM),  ";
                            sql += " CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ; ";





                            if (!String.IsNullOrEmpty(sql))
                            {
                                string[] sqls = sql.Split(';');

                                for (int i = 0; i < sqls.Length; i++)
                                {
                                    string sql1 = sqls[i].Trim();

                                    if (!String.IsNullOrEmpty(sql1))
                                    {
                                        TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "running Update 2010 CouSub Indexes: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ") - " + sql1);


                                        SqlCommand cmd = new SqlCommand(sql1);
                                        IQueryManager qm = QueryManager;
                                        qm.AddParameters(cmd.Parameters);
                                        qm.ExecuteNonQuery(CommandType.Text, cmd.CommandText, 6000, true);
                                    }
                                }
                            }

                            TraceSource.TraceEvent(TraceEventType.Information, (int)ProcessEvents.Completing, "Update 2010 CouSub Indexes - Complete: " + state + " - (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")");


                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error occured in Update2010CouSubIndexes: " + e.Message, e);
            }
        }

    }


}

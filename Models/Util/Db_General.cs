﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

public class Db_General
{
    protected static DataTable getDataTable(string qry)
    {

        var dt = new DataTable();

        using (var con = new MySql.Data.MySqlClient.MySqlConnection(ConfigurationManager.ConnectionStrings["ezesb"].ConnectionString))
        {
            var da = new MySql.Data.MySqlClient.MySqlDataAdapter();

            da.SelectCommand = con.CreateCommand();
            da.SelectCommand.CommandText = qry;

            QueryTrace(qry);
            try
            {
                da.Fill(dt);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + ": " + qry);
            }
        }

        return dt;
    }

    protected static Dictionary<string, string> getRow(string qry)
    {
        var v = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        var dt = getDataTable(qry);
        foreach (DataColumn c in dt.Columns)
        {
            if (dt.Rows.Count == 0)
                v[c.ColumnName] = "";
            else
                v[c.ColumnName] = Convert.ToString(dt.Rows[0][c]);

        }
        return v;
    }

    protected static int Execute(string qry)
    {
        using (var con = new MySql.Data.MySqlClient.MySqlConnection(ConfigurationManager.ConnectionStrings["ezesb"].ConnectionString))
        {
            con.Open();
            var cmd = new MySql.Data.MySqlClient.MySqlCommand(qry, con);
            QueryTrace(qry);
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + ": " + qry);
            }
        }
    }

    protected static string DbDate(object Date)
    {
        if(Date is DateTime)
        {
            return ((DateTime)Date).ToString("s").Replace("T"," ");
        }
        var s = Convert.ToString(Date);
        if (s == "") return "";
        DateTime d;
        if (!DateTime.TryParse(s, out d))
            return "";
        return d.ToString("s").Replace("T", " ");


    }

    protected static void QueryTrace(string Message)
    {
        System.Diagnostics.Trace.Write(Message, "Query");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class database : MonoBehaviour {

    //public GameObject Aircraft;
    public string conn;
    public IDbConnection dbconn;
    public string query;
    public List<string> callsigns;
    public Dictionary<string, string> phonetic_callsigns;
    public Dictionary<string, string> heading_matches;
    public List<object> characteristics;

    public void Build_Lexicon()
    {
        string built_text = "";
        string conn2 = "URI=file:" + Application.dataPath + "/atcDB.sqlite";
        IDbConnection dbconn2 = (IDbConnection)new SqliteConnection(conn2);
        dbconn2.Open();

        IDbCommand dbcmd = dbconn2.CreateCommand();

        Dictionary<string, string> cs = get_phonetic_call_signs();
        List<string> cs_keys = new List<string>(cs.Keys);
        string[] alt = get_possible_altitudes();
        string[] comm = get_commands();
        string[] headings = get_spoken_headings();

        for(int x = 0; x < comm.Length; x++)
         {
                for(int y = 0; y < alt.Length; y++)
                {
                    //simple altitude change
                    if (comm[x].Equals("descend and maintain", StringComparison.Ordinal) || 
                        comm[x].Equals("climb and maintain", StringComparison.Ordinal))
                    {
           
                        built_text = comm[x] + " " + alt[y];                       
                        //print(built_text);
                        query = "INSERT INTO phraseology VALUES ("+"'"+ built_text+"'"+")";
                        dbcmd.CommandText = query;
                        dbcmd.ExecuteNonQuery();
                    }
                    if (comm[x].Equals("turn left heading", StringComparison.Ordinal) ||
                        comm[x].Equals("turn right heading", StringComparison.Ordinal))
                    {

                        built_text = comm[x] + " " + headings[y];
                        //print(built_text);
                        query = "INSERT INTO phraseology VALUES (" + "'" + built_text + "'" + ")";
                        dbcmd.CommandText = query;
                        dbcmd.ExecuteNonQuery();
                    }
              }
        }

        for(int i = 0; i < alt.Length; i++)
        {
            for(int x = 0; x < headings.Length; x++)
            {
                built_text = "descend and maintain " + alt[i] + " turn left heading " + headings[x];
                query = "INSERT INTO phraseology VALUES (" + "'" + built_text + "'" + ")";
                dbcmd.CommandText = query;
                dbcmd.ExecuteNonQuery();

                built_text = "climb and maintain " + alt[i] + " turn left heading " + headings[x];
                query = "INSERT INTO phraseology VALUES (" + "'" + built_text + "'" + ")";
                dbcmd.CommandText = query;
                dbcmd.ExecuteNonQuery();

                built_text = "descend and maintain " + alt[i] + " turn right heading " + headings[x];
                query = "INSERT INTO phraseology VALUES (" + "'" + built_text + "'" + ")";
                dbcmd.CommandText = query;
                dbcmd.ExecuteNonQuery();

                built_text = "climb and maintain " + alt[i] + " turn right heading " + headings[x];
                query = "INSERT INTO phraseology VALUES (" + "'" + built_text + "'" + ")";
                dbcmd.CommandText = query;
                dbcmd.ExecuteNonQuery();
            }
        }
        

        dbcmd.Dispose();
        dbcmd = null;
        dbconn2.Close();
        dbconn2 = null;

    }

    //Actual phrases used in ATC
    public List<string> get_lexicon()
    {
        List<string> lexicon = new List<string>();
        conn = "URI=file:" + Application.dataPath + "/atcDB.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT spoken FROM phraseology";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string words = reader.GetString(0);
            lexicon.Add(words);
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return lexicon;
    }

    public string[] get_spoken_headings()
    {
        List<string> headings = new List<string>();
        conn = "URI=file:" + Application.dataPath + "/atcDB.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT headings FROM phrases";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string h = reader.GetString(0);
            headings.Add(h);
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return headings.ToArray();
    }

    public Dictionary<string, string> get_heading_dictionary()
    {
        Dictionary<string, string> heading_dict = new Dictionary<string, string>();
        conn = "URI=file:" + Application.dataPath + "/atcDB.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT headings, heading_value FROM phrases";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string heading_string = reader.GetString(0);
            string heading_value = reader.GetString(1);
            heading_dict[heading_string] = heading_value;
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return heading_dict;
    }

    public Dictionary<string, int> get_alt_dictionary()
    {
        Dictionary<string, int> alt_dict = new Dictionary<string, int>();
        conn = "URI=file:" + Application.dataPath + "/atcDB.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT altitudes, alt_int FROM phrases";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string alt_string = reader.GetString(0);
            int alt_int = reader.GetInt32(1);
            alt_dict[alt_string] = alt_int;
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return alt_dict;
    }

    public string[] get_possible_altitudes()
    {
        List<string> altitudes = new List<string>();
        conn = "URI=file:" + Application.dataPath + "/atcDB.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT altitudes FROM phrases";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string alt = reader.GetString(0);
            altitudes.Add(alt);
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return altitudes.ToArray();

    }

    public string[] get_commands()
    {
        List<string> commands = new List<string>();
        conn = "URI=file:" + Application.dataPath + "/atcDB.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT commands FROM phrases WHERE commands != '' ";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string comm = reader.GetString(0);
            if(comm.Length > 1)
                commands.Add(comm);
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return commands.ToArray();
    }

    public string[] get_call_signs()
    {
        callsigns = new List<string>();
        conn = "URI=file:" + Application.dataPath + "/atcDB.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT call_sign FROM aircraft WHERE in_use = 0";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string cs = reader.GetString(0);
            callsigns.Add(cs);
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
       
        return callsigns.ToArray();
    }

    public Dictionary<string, string> get_phonetic_call_signs()
    {
        phonetic_callsigns = new Dictionary<string, string>();
        conn = "URI=file:" + Application.dataPath + "/atcDB.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT phonetic_call_sign, call_sign FROM aircraft";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string phonetic_cs = reader.GetString(0);
            string cs = reader.GetString(1);           
            phonetic_callsigns[phonetic_cs] = cs;
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return phonetic_callsigns;
    }
    public List<object> get_ac_chars(string callsign)
    {
        characteristics = new List<object>();
        conn = "URI=file:" + Application.dataPath + "/atcDB.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT * FROM aircraft WHERE call_sign = " + "'" + callsign + "'";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            
            string c1 = reader.GetString(0); //call sign
            string c2 = reader.GetString(1); //ac type
            int c3 = reader.GetInt32(2); //initial heading, not currently used 29 Dec 17
            float c4 = reader.GetFloat(3); //xVelocity
            float c5 = reader.GetFloat(4); //yVeocity
            int c6 = reader.GetInt32(5); //initial altitude
            int c7 = reader.GetInt32(6); //new altitude
            int c8 = reader.GetInt32(7); //aircraft in_use, 0 or 1
            float c9 = reader.GetFloat(8); //turn rate
            int c10 = reader.GetInt32(9); //ground_speed
            int c11 = reader.GetInt32(10); //climb rate
            int c12 = reader.GetInt32(11); //decent rate
            float c13 = reader.GetFloat(12); //aircraft rotation, 0 = 180 heading, 180 = 360 heading

            
            characteristics.Add(c1);
            characteristics.Add(c2);
            characteristics.Add(c3);
            characteristics.Add(c4);
            characteristics.Add(c5);
            characteristics.Add(c6);
            characteristics.Add(c7);
            characteristics.Add(c8);
            characteristics.Add(c9);
            characteristics.Add(c10);
            characteristics.Add(c11);
            characteristics.Add(c12);
            characteristics.Add(c13);
            
        }
        
        
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return characteristics;

    }

    public Dictionary<string, List<float>> get_headings()
    {
        Dictionary<string, List<float>> map = new Dictionary<string, List<float>>();

        conn = "URI=file:" + Application.dataPath + "/atcDB.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT * FROM aircraft_headings";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            List<float> xy_values = new List<float>();
            string heading = reader.GetString(0);
            float x = reader.GetFloat(1);
            float y = reader.GetFloat(2);
            xy_values.Add(x);
            xy_values.Add(y);
            map.Add(heading, xy_values);
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        
        return map;
    }
}

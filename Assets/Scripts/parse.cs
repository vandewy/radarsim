using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parse : MonoBehaviour {

    public Dictionary<string, int> altitude;
    public Dictionary<string, string> heading;
    public string cs_key = "";
    public database db = new database();
    public int turn_direction = 0;

    public void create_alt_dictionary()
    {
        altitude = db.get_alt_dictionary();
        heading = db.get_heading_dictionary();
    }

    public string parse_altitude(List<string> phrase)
    {
        string alt_key = "";
        if (phrase.Contains("thousand"))
        {
            int loc = phrase.IndexOf("thousand");
            if(phrase[loc-2] == "one")
                alt_key = phrase[loc-2] + " " + phrase[loc - 1] + " " + phrase[loc];
            else
                alt_key = phrase[loc - 1] + " " + phrase[loc];
            return alt_key;            

        }else if(phrase.Contains("flight") && phrase.Contains("level"))
        {
            int loc = phrase.IndexOf("flight");
            alt_key = phrase[loc] + " " + phrase[loc + 1] + " " + phrase[loc + 2] + " " + phrase[loc+3] + " " + phrase[loc+4];
            return alt_key;
        }else
            return "empty";
    }

    public string parse_heading(List<string> phrase)
    {
        
        string heading_key = "";
        if(phrase.Contains("turn") && phrase.Contains("right"))
        {
            
            int loc = phrase.IndexOf("heading");
            heading_key = phrase[loc + 1] + " " + phrase[loc + 2] + " " + phrase[loc + 3];
            turn_direction = 2;
            return heading_key;
        }else if(phrase.Contains("turn") && phrase.Contains("left"))
        {
            
            int loc = phrase.IndexOf("heading");
            heading_key = phrase[loc + 1] + " " + phrase[loc + 2] + " " + phrase[loc + 3];
            turn_direction = 1;
            return heading_key;
        }else
            return "empty";
    }

    public int elevation_action(List<string> phrase)
    {
        if (phrase.Contains("climb"))
        {
            return 2;
        }
        else if (phrase.Contains("descend"))
        {
            return 1;
        }
        else
            return 0;
    }


    public string parse_callsign(List<string> phrase)
    {
        cs_key = phrase[0] + " " + phrase[1] + " " + phrase[2];
        return cs_key;
    }
    private void Start()
    {
        //create_alt_dictionary();
    }

}

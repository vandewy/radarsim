using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using System;

public class Recognition : MonoBehaviour {

    public database db;
    //key,value example ["bigfoot two five"] = "Bigfoot25  
    public Dictionary<string,string> phonetic_ac_callsigns;

    //public int current_altitude = 9000;
    public GameObject update_aircraft;
    public pilot_speech pilot;
    public flight_control control;
    //public string[] keywords = new String[] { "up", "down", "left", "right" };
    //public string[] keys = new string[] { "descend", "four thousand", "Pace her two three" };
    public string words;
    public ConfidenceLevel confidence = ConfidenceLevel.High;
    public string total;
    string n = "bully one two ";
    string o = "descend and maintain ";
    string p = "five thousand ";
    string q = "traffic";
    string p2 = "six thousand";

    public parse parser;
    public List<string> lexicon;
    public string current_phrase;  //phrase that was recognized used for parsing
    

    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords;// = new Dictionary<string, System.Action>();
    // Use this for initialization
    void Start() {
        keywords = new Dictionary<string, System.Action>();

        pilot = new pilot_speech();
        //get call signs to add in front of phraseology
        db = new database();
        // DO not call again db.Build_Lexicon();
        phonetic_ac_callsigns = db.get_phonetic_call_signs();

        parser = new parse();  //used for parsing recognized phrases
        parser.create_alt_dictionary();

        lexicon = db.get_lexicon();
        Build_Phrases(lexicon);

        total = n + o + p + q;
        string total2 = n + o + p2 + q;
        
        //keywords.Add(total, () => { TrafficCalled(); });
        //keywords.Add("bigfoot two five descend and maintain five thousand turn right heading two seven zero vector to i l s runway two one right", () => { test_phrase(); });
        //keywords.Add(total2, () => { test_phrase(); });
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPhraseRecognized;
        keywordRecognizer.Start();
        print(keywords.Count + " phrases");
	}

    public void Build_Phrases(List<string> lex)
    {
        //List of phonetic call signs, example - bully one two
        List<string> keys = new List<string>(phonetic_ac_callsigns.Keys);
        foreach(string key in keys)
        {
            for (int i = 0; i < lex.Count; i++)
                keywords.Add(key + " " + lex[i], () => { test_phrase(); });
        }
    }

    void KeywordRecognizerOnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        words = args.text;
        current_phrase = words;
        
        if(keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    void test_phrase()
    {
        //convert string to List for easier processing
        string[] words = current_phrase.Split(' ');
        List<string> phrase_words = new List<string>(words);
        
        string alt_key = parser.parse_altitude(phrase_words);
        string cs_key = parser.parse_callsign(phrase_words);
        string heading_key = parser.parse_heading(phrase_words);

        
        
        if(alt_key != "empty" && phonetic_ac_callsigns[cs_key] != null && parser.altitude[alt_key] > 0)
        {
            if (parser.elevation_action(phrase_words) == 1)
            {
                GameObject.Find(phonetic_ac_callsigns[cs_key]).GetComponent<flight_control>().descend(parser.altitude[alt_key]);
            }
            else if (parser.elevation_action(phrase_words) == 2)
            {
                GameObject.Find(phonetic_ac_callsigns[cs_key]).GetComponent<flight_control>().climb(parser.altitude[alt_key]);
            }
        }

        
        if (heading_key != "empty" && phonetic_ac_callsigns[cs_key] != null){
            print("here");
            print(parser.heading[heading_key]);
            int turn = Int32.Parse(parser.heading[heading_key]);
            print(turn);
            if (turn > 0)
            {
                if (parser.turn_direction == 1)
                {
                    GameObject.Find(phonetic_ac_callsigns[cs_key]).GetComponent<flight_control>().left_turn_controller(turn);
                }
                else if (parser.turn_direction == 2)
                {
                    GameObject.Find(phonetic_ac_callsigns[cs_key]).GetComponent<flight_control>().right_turn_controller(turn);
                }
            }
        }


        //Pilot readback 
        pilot.pilot_action = pilot.pilot_readback(current_phrase, cs_key);
        pilot.Speak();
        //GameObject.Find(ac).GetComponent<flight_control>().right_turn_controller(270);
    }

    //Descend_And_Turn
    //Climb_And_Turn
    //Maintain_Altitude
    //

    void TrafficCalled()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        audio.PlayDelayed(1);
    }

    // Update is called once per frame
    void Update () {

       
	}

    private void OnApplicationQuit()
    {
        if(keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.OnPhraseRecognized -= KeywordRecognizerOnPhraseRecognized;
            keywordRecognizer.Stop();
        }
    }
}

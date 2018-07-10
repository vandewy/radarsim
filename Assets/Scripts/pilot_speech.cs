using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Crosstales.RTVoice;

public class pilot_speech : MonoBehaviour {

    public string call_sign = "bigfoot two five";
    public string VoiceName = "Microsoft David Desktop";
    public string pilot_action = "";

    public void Speak()
    {
        //Speaker.Speak(pilot_action, null, Speaker.VoiceForName(VoiceName));
    }

    public string pilot_readback(string pilot_words, string call_sign)
    {
        pilot_words = pilot_words.Replace(call_sign, "");
        pilot_words += ", " + call_sign;

        return pilot_words;
    }
	
}

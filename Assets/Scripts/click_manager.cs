using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class click_manager : MonoBehaviour {

    public GameObject acft;
    public aircraft acft_chars;
    public Sprite approach_tag;

    private void Start()
    {
        approach_tag = Resources.Load<Sprite>("approach");
    }

    public void stop_center_flash()
    {
        acft.GetComponent<flight_control>().blip.radar_contact = true;
        if(approach_tag == null)
        {
            print("tag null");
        }
        GameObject.Find(acft.name + "/Sector_Canvas").GetComponent<Image>().sprite = approach_tag;
        GameObject.Find(acft.name + "/Sector_Canvas").GetComponent<Image>().enabled = true;
    }

    public void click_on_off()
    {
        print("C was gone");
    }
}

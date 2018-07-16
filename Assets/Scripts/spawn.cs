using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn : MonoBehaviour {

    public GameObject Aircraft;
    public string[] ac_callsigns;
    public List<object> characteristics;
    public database db;

    public void Start()
    {
        db = new database();
        ac_callsigns = db.get_call_signs();
        spawn_aircraft(ac_callsigns[0]);
    }

	public void spawn_aircraft(string ac_call_sign)
    {
        //-5,3  -6.3,-.75   -4f,-4.10   6.2,-4.10   6.5,.35  3.12,3.86  
        //departure .07f,-.83f
         GameObject clone = GameObject.Instantiate(Aircraft, new Vector2(3.12f, 3.86f), Quaternion.identity);
         clone.name = ac_call_sign;
    }
}

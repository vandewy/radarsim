using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class flight_control : MonoBehaviour
{
    private GameObject blip_fade1;
    private GameObject blip_fade2;
    private GameObject blip_fade3;
    public float xVal = 0;
    public float yVal = 0;
    //public float rotate;
    public float xTurnValue;
    public float yTurnValue;
    public float degree_turn;
    public float degree_count;  //counter for turn degrees
    public bool IsDone;

    //enables control of gameobject (blip)
    private Rigidbody rb;
    public aircraft blip;
    public database ac_database;
    public GameObject acft;
    public GameObject c; //Canvas of aircraft
    public GameObject sector_tag;
    public List<object> ac_chars;
    public Dictionary<string, List<float>> headings;
    public Renderer sector_tag_rend;

    public bool turning = false;
    public bool left_turn = false;
    public bool right_turn = false;
    public int initial_heading; //remove after testing

    public Thread cThread;
    public Thread dThread;

    // Use this for initialization
    void Start()
    {
        //blip contains info on A/C characteristics
        blip = new aircraft();
        blip.call_sign = this.name.ToString(); //give call sign for db look up during initialization

        rb = GetComponent<Rigidbody>();
        Vector2 start = transform.position;
        //TESTING
        ac_database = new database();
        headings = ac_database.get_headings();
        
		//xy used for determining force applied when spawned
        xVal = start.x;
        yVal = start.y;
        degree_turn = 20;
        degree_count = .0f;
        
        acft = GameObject.Find(blip.call_sign + "/Canvas/datablock_text");
        c = GameObject.Find(blip.call_sign + "/Canvas");
        sector_tag = GameObject.Find(blip.call_sign + "/Sector_Canvas");
        //initialize_aircraft(blip, headings);

        

        //blip_fade1 = GameObject.Find(blip.call_sign + "blip/blip_fade1");
        //blip_fade2 = GameObject.Find(blip.call_sign + "blip/blip_fade1/blip_fade2");
        //blip_fade3 = GameObject.Find(blip.call_sign + "blip/blip_fade1/blip_fade2/blip_fade3");

        initial_heading = -90; //remove after testing
        blip.heading = initial_heading;
        blip.rx = 0;
        blip.ry = 0;
        blip.rz = blip.heading;
        turn_tester();
    }

	public void update_datablock(aircraft ac){
        string alt;
        //100 is fine, 90 will be made 090
        if (ac.current_altitude.ToString().Length < 3)
            alt = "0" + ac.current_altitude.ToString();
        else
            alt = ac.current_altitude.ToString();

		string txt = ac.call_sign + "\n";
		txt += ac.type + " " + alt + "\n";
		txt += ac.ground_speed.ToString ();
        
		if (acft != null) {
			acft.GetComponent<Text> ().text = txt;
		} else {
			print ("error: updating datablock");
		}

        
        GameObject c = GameObject.Find(ac.call_sign + "/Canvas");
		if (c != null) {
            //keeps data block vertical
            c.transform.eulerAngles = new Vector3(0, 0, 0);
        } else {
			print ("Canvas was null");
		}
	}

    public void Update_Sector_Tag()
    {
        if(sector_tag != null)
        {
            sector_tag.transform.eulerAngles = new Vector3(0, 0, 0);
        }
    } 

    public void initialize_aircraft(aircraft blip, Dictionary<string, List<float>> heading)
    {
        ac_database = new database();
        ac_chars = new List<object>();
        ac_chars = ac_database.get_ac_chars(blip.call_sign);
      
        if (xVal < -4.5 && yVal > 2.5)
        { //NW quadrant
            
            blip.current_altitude = (int)ac_chars[5] / 100;
            blip.descent_rate = (int)ac_chars[11];
            blip.climb_rate = (int)ac_chars[10];
            blip.ground_speed = (int)ac_chars[9];
            blip.type = ac_chars[1].ToString();
            blip.new_altitude = (int)ac_chars[6];
            blip.rotation = 60f; // (float)ac_chars[12];
            blip.xVelocity = (decimal)heading["120"][0] / 1000; // (float)ac_chars[3];
            blip.yVelocity = (decimal)heading["120"][1] / 1000; // (float)ac_chars[4];
            blip.turn_rate = 1;
            blip.quadrant = 3;
            blip.heading = 120;
            
        }
        else if (xVal < -6.0 && yVal < 0 && yVal > -1)
        { //direct from west
            print("Direct West");
            blip.current_altitude = (int)ac_chars[5] / 100;
            blip.descent_rate = (int)ac_chars[11];
            blip.climb_rate = (int)ac_chars[10];
            blip.ground_speed = (int)ac_chars[9];
            blip.type = ac_chars[1].ToString();
            blip.new_altitude = (int)ac_chars[6];
            blip.rotation = 90f; // (float)ac_chars[12];
            blip.xVelocity = (decimal)heading["90"][0] / 1000; // (float)ac_chars[3];
            blip.yVelocity = (decimal)heading["90"][1] / 1000; // (float)ac_chars[4];
            blip.heading = 90;
            blip.turn_rate = 1;

        }
        else if (xVal < -3.5 && yVal < -4 && yVal > -4.5)
        { //from SW quadrant
            print("SW");
            blip.current_altitude = (int)ac_chars[5] / 100;
            blip.descent_rate = (int)ac_chars[11];
            blip.climb_rate = (int)ac_chars[10];
            blip.ground_speed = (int)ac_chars[9];
            blip.type = ac_chars[1].ToString();
            blip.new_altitude = (int)ac_chars[6];
            blip.rotation = 130f; // (float)ac_chars[12];
            blip.xVelocity = (decimal)heading["50"][0] / 1000; // (float)ac_chars[3];
            blip.yVelocity = (decimal)heading["50"][1] / 1000; // (float)ac_chars[4];
            blip.heading = 50;
            blip.turn_rate = 1;
            blip.quadrant = 4;

            blip.blip1_rotation = blip.rotation;
            blip.blip2_rotation = blip.rotation;
            blip.blip3_rotation = blip.rotation;

        }
        /*else if (xVal > 6 && yVal < -4 && yVal > -4.5)
        { //from SE quadrant
            print("SE");
            blip.current_altitude = (int)ac_chars[5] / 100;
            blip.descent_rate = (int)ac_chars[11];
            blip.climb_rate = (int)ac_chars[10];
            blip.ground_speed = (int)ac_chars[9];
            blip.type = ac_chars[1].ToString();
            blip.new_altitude = (int)ac_chars[6];
            blip.rotation = -120f; // (float)ac_chars[12];
            blip.xVelocity = heading["300"][0]; // (float)ac_chars[3];
            blip.yVelocity = heading["300"][1]; // (float)ac_chars[4];
            blip.heading = (int)ac_chars[2];
            blip.turn_rate = .017f;
            GameObject text = GameObject.Find(blip.call_sign + "Canvas/datablock_text");
            float xt = text.transform.position.x;
            float yt = text.transform.position.y;
            text.transform.position = new Vector2(xt+.5f, yt-.5f);

        }
        else if (xVal > 6 && yVal < .45 && yVal > 0)
        { //from East
            print("East");
            blip.current_altitude = (int)ac_chars[5] / 100;
            blip.descent_rate = (int)ac_chars[11];
            blip.climb_rate = (int)ac_chars[10];
            blip.ground_speed = (int)ac_chars[9];
            blip.type = ac_chars[1].ToString();
            blip.new_altitude = (int)ac_chars[6];
            blip.rotation = -80f; // (float)ac_chars[12];
            blip.xVelocity = heading["260"][0]; // (float)ac_chars[3];
            blip.yVelocity = heading["260"][1]; // (float)ac_chars[4];
            blip.heading = (int)ac_chars[2];
            blip.turn_rate = .017f;
            GameObject text = GameObject.Find(blip.call_sign + "Canvas/datablock_text");
            float xt = text.transform.position.x;
            float yt = text.transform.position.y;
            //datablock
            text.transform.position = new Vector2(xt + .5f, yt);

        }
        else if (xVal > 3 && yVal < 3.95 && yVal > 3.75)
        { //High Approach
            print("High Approach");
            blip.current_altitude = (int)ac_chars[5] / 100;
            blip.descent_rate = (int)ac_chars[11];
            blip.climb_rate = (int)ac_chars[10];
            blip.ground_speed = (int)ac_chars[9];
            blip.type = ac_chars[1].ToString();
            blip.new_altitude = (int)ac_chars[6];
            blip.rotation = -33f; // (float)ac_chars[12];
            blip.xVelocity = heading["210"][0]; // (float)ac_chars[3];
            blip.yVelocity = heading["210"][1]; // (float)ac_chars[4];
            blip.heading = (int)ac_chars[2];
            blip.turn_rate = .017f;
            GameObject text = GameObject.Find(blip.call_sign + "Canvas/datablock_text");
            float xt = text.transform.position.x;
            float yt = text.transform.position.y;
            text.transform.position = new Vector2(xt + .5f, yt);

        }
        else if (xVal > 0 && xVal < 1 && yVal < 0 && yVal > -1)
        { //departure
            print("Departure");
            blip.departure = true;
            blip.current_altitude = 1000 / 100;
            blip.descent_rate = (int)ac_chars[11];
            blip.climb_rate = (int)ac_chars[10];
            blip.ground_speed = (int)ac_chars[9];
            blip.type = ac_chars[1].ToString();
            blip.new_altitude = 9000;
            blip.rotation = -33f; // (float)ac_chars[12];
            blip.xVelocity = heading["210"][0]; // (float)ac_chars[3];
            blip.yVelocity = heading["210"][1]; // (float)ac_chars[4];
            blip.heading = (int)ac_chars[2];
            blip.turn_rate = .017f;
            GameObject text = GameObject.Find(blip.call_sign + "Canvas/datablock_text");
            float xt = text.transform.position.x;
            float yt = text.transform.position.y;
            text.transform.position = new Vector2(xt + .5f, yt);
            climb(blip.new_altitude);

        }
        else  //Used for testing movement
        {
            print("Test Movement");
            blip.current_altitude = (int)ac_chars[5] / 100;
            blip.descent_rate = (int)ac_chars[11];
            blip.climb_rate = (int)ac_chars[10];
            blip.ground_speed = (int)ac_chars[9];
            blip.type = ac_chars[1].ToString();
            blip.new_altitude = (int)ac_chars[6];
            blip.rotation = (float)ac_chars[12];
            blip.xVelocity = 10f; // (float)ac_chars[3];
            blip.yVelocity = 0f; // (float)ac_chars[4];
            
        }*/

    }
		
    private void stop_thread(Thread stopThread)
    {
        stopThread.Abort();
    }

	public void climb_aircraft(aircraft ac){
		float feet_per_second = ac.descent_rate / 60;
		float altitude = ac.current_altitude * 100;  //90 changes to 9000 for math
		float target_altitude = ac.new_altitude;

		while ((altitude) != target_altitude) {
			print (altitude);
			System.Threading.Thread.Sleep (500);
			//altitude = ac.current_altitude * 100;
			if (altitude >= target_altitude) {
				altitude = target_altitude;
				ac.current_altitude = (int)altitude / 100;
				break;
			} else {
				altitude = (int)System.Math.Ceiling (altitude + feet_per_second);
				ac.current_altitude = (int)System.Math.Ceiling (altitude / 100);
			}
		}
	}

	public void climb(int alt){
        blip.new_altitude = alt;
        cThread = new System.Threading.Thread(() => climb_aircraft(blip));
        cThread.Start();
	}
		
	public void descend_aircraft(aircraft ac){
		float feet_per_second = ac.descent_rate / 60;
		float altitude = ac.current_altitude * 100;  //90 changes to 9000 for math
		float target_altitude = ac.new_altitude;

		while ((altitude) != target_altitude) {
			//print (altitude);
			System.Threading.Thread.Sleep (1000);
			//altitude = ac.current_altitude * 100;
			if (altitude <= target_altitude) {
				altitude = target_altitude;
				ac.current_altitude = (int)altitude / 100;
				break;
			} else {
				altitude = (int)System.Math.Ceiling (altitude - feet_per_second);
				ac.current_altitude = (int)System.Math.Ceiling (altitude / 100);
			}
		}
	}
		
	public void descend(int alt){
        blip.new_altitude = alt;
        dThread = new System.Threading.Thread(() => descend_aircraft(blip));
        dThread.Start ();
	}

    // Update is called once per frame
    void Update()
    {
        Move();
		/**update_datablock (blip);
        Update_Sector_Tag();
        if(blip.radar_contact == false)
        {
            bool oddeven = Mathf.FloorToInt(Time.time) % 2 == 0;
            flash_from_center(oddeven);
        }**/
    }

    public void turn_tester()
    {
        //right_turn = true;
        blip.new_heading = 360;
        left_turn = true;
        Turn_Controller(blip, get_degree_turn(blip, blip.new_heading, 1));
    }
    
    public void Move()
    {
        rb.AddRelativeForce(Vector3.up * .025f);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, .025f);
        transform.localEulerAngles = new Vector3(blip.rx, blip.ry, blip.rz);
    }

    public int normalize_heading(aircraft blip)
    {
        int h = blip.heading;

        if (h > 360)
        {
            h = h % 360;
        }
        else if (h < 0)
        {
            h *= -1;
            if (h > 360)
            {
                h = h % 360;
            }
        }
        else if (h == 0 || h == 360)
        {
            h = 360;
        }

        return h;
    }

    public int get_degree_turn(aircraft blip, int heading, int turn_direction)
    {
        int current_heading = normalize_heading(blip);
        int degree_turn = 0;

        //1 is left turn, 2 is right turn
        if (turn_direction == 1 && heading < current_heading)
        {
            degree_turn = current_heading - heading;
        }
        else if (turn_direction == 1)
        {
            degree_turn = (360 - heading) + current_heading;
        }

        if (turn_direction == 2 && heading > current_heading)
        {
            degree_turn = heading - current_heading;
        }
        else if (turn_direction == 2)
        {
            degree_turn = (360 - current_heading) + heading;
        }

        return degree_turn;
    }

    public void Turn_Controller(aircraft ac, float degree_turn)
    {
        System.Threading.Thread mThread = new System.Threading.Thread(() => Turn(ac, degree_turn));
        mThread.Start();
    }

    public void Turn(aircraft ac, float degree_turn)
    {
        float target = 0;

        if (left_turn == true)
        {
            turning = true;
            while (target < degree_turn)
            {
                System.Threading.Thread.Sleep(80);
                ac.rz += ac.turn_rate;
                target += ac.turn_rate;
            }
            left_turn = false;
            turning = false;
        }
        else if (right_turn == true)
        {
            turning = true;
            print(target);
            print(degree_turn);
            while (target < degree_turn)
            {
                System.Threading.Thread.Sleep(80);
                ac.rz -= ac.turn_rate;
                target += ac.turn_rate;
            }
            right_turn = false;
            turning = false;
        }
    }
    void FixedUpdate()
    {
        //blip_fade1.transform.rotation = Quaternion.Euler(new Vector3(0, 0, blip.blip1_rotation));
        //blip_fade2.transform.rotation = Quaternion.Euler(new Vector3(0, 0, blip.blip2_rotation));
        //blip_fade3.transform.rotation = Quaternion.Euler(new Vector3(0, 0, blip.blip3_rotation));
    }

    public void flash_from_center(bool oddeven)
    {
        Image sector_sprite = sector_tag.GetComponent<Image>();
        sector_sprite.enabled = oddeven;
        //sector_tag_rend.enabled = oddeven;
    }

}

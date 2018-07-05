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
    private Rigidbody2D rb2D;
    public aircraft blip;
    public database ac_database;
    public GameObject acft;
    public GameObject c; //Canvas of aircraft
    public GameObject sector_tag;
    public List<object> ac_chars;
    public Dictionary<string, List<float>> headings;
    public Renderer sector_tag_rend;

    //turn left, turn right, descend, climb threads
    private Thread lThread;
    private Thread rThread;
    private Thread dThread;
    private Thread cThread;

    // Use this for initialization
    void Start()
    {
        //blip contains info on A/C characteristics
        blip = new aircraft();
        blip.call_sign = this.name.ToString(); //give call sign for db look up during initialization

        rb2D = GetComponent<Rigidbody2D>();
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
        initialize_aircraft(blip, headings);

        blip_fade1 = GameObject.Find(blip.call_sign + "blip/blip_fade1");
        blip_fade2 = GameObject.Find(blip.call_sign + "blip/blip_fade1/blip_fade2");
        blip_fade3 = GameObject.Find(blip.call_sign + "blip/blip_fade1/blip_fade2/blip_fade3");

		//System.Threading.Thread mThread = new System.Threading.Thread( () => turn_right (blip, headings, 360));
		//mThread.Start ();
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

    public void Update_Sector_Tage()
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
		
    public void Move(aircraft ac)
    {
        rb2D.AddForce(new Vector2((float)ac.xVelocity, (float)ac.yVelocity));
        rb2D.velocity = Vector3.ClampMagnitude(rb2D.velocity, .05f);
		rb2D.MoveRotation (blip.rotation);
		//coroutine = descend_aircraft(ac);
		//StartCoroutine (coroutine);
        //rb2D.MoveRotation(52);
        
    }

    public void left_turn_controller(int h)
    {
        lThread = new Thread(() => turn_left(blip, headings, h));
        lThread.Start();
    }

    public void right_turn_controller(int h)
    {
        rThread = new Thread(() => turn_right(blip, headings, h));
        rThread.Start();
    }

    private void stop_thread(Thread stopThread)
    {
        stopThread.Abort();
    }

    public void turn_right(aircraft ac, Dictionary<string, List<float>> headings, int heading)
    {
        direction_check_right(ac);
        int count = 0;
        print("turning");
        //z is blip.rotation of the blip
        print("heading " + ac.heading);
        float degree_turn = Mathf.Abs(ac.heading - (float)heading);
        decimal final_x = (decimal)headings[heading.ToString()][0];
        decimal final_y = (decimal)headings[heading.ToString()][1];

        decimal current_x = ac.xVelocity * 1000;
        decimal current_y = ac.yVelocity * 1000;


        bool x_flag = false;
        bool y_flag = false;

        while (current_x != final_x || current_y != final_y)
        {
            count++;
            Thread.Sleep(50);
            if (ac.quadrant == 3)
            {
                print("quad 3");
                if (current_x != final_x || x_flag == false)
                {
                    current_x -= ac.turn_rate;
                    ac.xVelocity = current_x / 1000;
                    if (current_x == final_x || current_x == 0)
                        x_flag = true;
                }

                if (y_flag == false || current_y != final_y)

                {
                    current_y -= ac.turn_rate;
                    ac.yVelocity = current_y / 1000;
                    if (current_y == final_y || current_y == -1000)
                        y_flag = true;
                }

                ac.rotation -= .085f;
                degree_turn -= .085f;

                if (x_flag == true && y_flag == true)
                {
                    if (current_x == 0 && current_y == -1000)
                    {
                        ac.quadrant = 2;
                        x_flag = false;
                        y_flag = false;
                    }
                }
            }
            else if (ac.quadrant == 4)
            {
                print("quad 4");
                if (current_x != final_x || x_flag == false)
                {
                    current_x -= ac.turn_rate;
                    ac.xVelocity = current_x / 1000;
                    if (current_x == final_x || current_x == 1000)
                        x_flag = true;
                }

                if (current_y != final_y || y_flag == false)
                {
                    current_y += ac.turn_rate;
                    ac.yVelocity = current_y / 1000;
                    if (current_y == final_y || current_y == 0)
                        y_flag = true;
                }
                ac.rotation -= .085f;
                degree_turn -= .085f;


                if (x_flag == true && y_flag == true)
                {
                    if (current_x == 1000 && current_y == 0)
                    {
                        ac.quadrant = 3;
                        x_flag = false;
                        y_flag = false;
                    }
                }
            }
            else if (ac.quadrant == 1)
            {
                print("quad 1");
                if (current_x != final_x || x_flag == false)
                {
                    current_x -= ac.turn_rate;
                    ac.xVelocity = current_x / 1000;
                    if (current_x == final_x || current_x == 0)
                        x_flag = true;
                }

                if (current_y != final_y || y_flag == false)
                {
                    current_y -= ac.turn_rate;
                    ac.yVelocity = current_y / 1000;
                    if (current_y == final_y || current_y == 1000)
                        y_flag = true;
                }
                ac.rotation -= .085f;
                degree_turn -= .085f;

                if (x_flag == true && y_flag == true)
                {
                    if (current_x == 0 && current_y == 1000)
                    {
                        ac.quadrant = 4;
                        x_flag = false;
                        y_flag = false;
                    }
                }
            }
            else if (ac.quadrant == 2)
            {
                print("quad 2");
                if (current_x != final_x || x_flag == false)
                {
                    current_x += ac.turn_rate;
                    ac.xVelocity = current_x / 1000;
                    if (current_x == final_x || current_x == -1000)
                        x_flag = true;
                }

                if (y_flag == false || current_y != final_y)
                {
                    current_y -= ac.turn_rate;
                    ac.yVelocity = current_y / 1000;
                    if (current_y == final_y || current_y == 0)
                        y_flag = true;
                }
                ac.rotation -= .085f;
                degree_turn -= .085f;


                if (x_flag == true && y_flag == true)
                {
                    if (current_x == -1000 && current_y == 0)
                    {
                        ac.quadrant = 1;
                        x_flag = false;
                        y_flag = false;
                    }
                }
            }

            if (count > 3000)
                break;

            //print(ac.rotation);

        }
        print("x: " + current_x);
        print("y: " + current_y);
        ac.xVelocity = final_x / 1000;
        ac.yVelocity = final_y / 1000;
        ac.rotation -= degree_turn;
        ac.heading = heading;

    }

    public void direction_check_right(aircraft ac)
    {
        if (ac.xVelocity == 0 && ac.yVelocity == -1)
            ac.quadrant = 2;
        else if (ac.xVelocity == 1 && ac.yVelocity == 0)
            ac.quadrant = 3;
        else if (ac.xVelocity == 0 && ac.yVelocity == 1)
            ac.quadrant = 4;
        else if (ac.xVelocity == -1 && ac.yVelocity == 0)
            ac.quadrant = 1;
    }

    public void direction_check_left(aircraft ac)
    {
        if (ac.xVelocity == 0 && ac.yVelocity == -1)
            ac.quadrant = 3;
        else if (ac.xVelocity == 1 && ac.yVelocity == 0)
            ac.quadrant = 4;
        else if (ac.xVelocity == 0 && ac.yVelocity == 1)
            ac.quadrant = 1;
        else if (ac.xVelocity == -1 && ac.yVelocity == 0)
            ac.quadrant = 2;
    }

    public void turn_left(aircraft ac, Dictionary<string, List<float>> headings, int heading)
    {
        direction_check_left(ac);

        float fade_rotation_one = ac.rotation;
        float fade_rotation_two = ac.rotation;
        float fade_rotation_three = ac.rotation;
        int fade_count = 0;

        int count = 0;
        print("turnin rate" + ac.turn_rate);
        //z is blip.rotation of the blip
        print("initial heading " + ac.heading);
        float degree_turn = Mathf.Abs(ac.heading - (float)heading);
        decimal final_x = (decimal)headings[heading.ToString()][0];
        decimal final_y = (decimal)headings[heading.ToString()][1];
        
        decimal current_x = ac.xVelocity * 1000;
        decimal current_y = ac.yVelocity * 1000;

        bool x_flag = false;
        bool y_flag = false;
        
        while(current_x != final_x || current_y != final_y)
        {
            count++;

            Thread.Sleep(50);
            if (ac.quadrant == 3)
            {
                print("quad 3");
                if (current_x != final_x && x_flag == false)
                {
                    current_x += ac.turn_rate;
                    //ac.xVelocity = current_x / 1000;
                    if (current_x == final_x && current_y != final_y)
                        current_x += 1;
                    else if (current_x == 1000 && current_x != final_x)
                        x_flag = true;
                    else if (current_x == 1000 && current_x == final_x)
                        x_flag = true;
                    else if (current_x == final_x)
                        x_flag = true;
                }

                if (y_flag == false && current_y != final_y)
                {
                    current_y += ac.turn_rate;
                    //ac.yVelocity = current_y / 1000;
                    if (current_y == final_y || current_y == 0)
                        y_flag = true;
                }

                ac.xVelocity = current_x / 1000;
                ac.yVelocity = current_y / 1000;
                ac.rotation += .085f;
                degree_turn -= .085f;
                
                
                if (x_flag == true && y_flag == true)
                {
                    if(current_x == 1000 && current_y == 0)
                    {
                        ac.quadrant = 4;
                        x_flag = false;
                        y_flag = false;
                    }                       
                }

                print("x: " + current_x);
                print("y: " + current_y);
            }else if(ac.quadrant == 4)
            {
                print("quad 4");
                if (current_x != final_x && x_flag == false)
                {
                    current_x -= ac.turn_rate;
                    //ac.xVelocity = current_x / 1000;
                    if (current_x == final_x && current_y + 1 != final_y)
                        x_flag = false;
                    else if (current_x == 0 && current_x == final_x)
                        x_flag = true;
                    else if (current_x == 0 && current_x != final_x)
                        x_flag = true;
                    else if (current_x == final_x && current_y + 1 == final_y)
                        x_flag = true;
                }

                if (current_y != final_y && y_flag == false)
                {
                    current_y += ac.turn_rate;
                    //ac.yVelocity = current_y / 1000;
                    if (current_y == 1000 && current_y != final_y)
                        y_flag = true;
                    else if (current_y == 1000 && current_y == final_y)
                        y_flag = true;
                    else if (current_y == final_y)
                        y_flag = true;
                }

                ac.xVelocity = current_x / 1000;
                ac.yVelocity = current_y / 1000;
                ac.rotation += .085f;
                degree_turn -= .085f;
                fade_count += 1;
                print("fade " + fade_count);
                if (fade_count >= 20 && fade_count < 50)
                {
                    blip.blip1_rotation = ac.rotation - 3f;
                }else if(fade_count >= 50 && fade_count < 85)
                {
                    blip.blip1_rotation = ac.rotation - 3f;
                    blip.blip2_rotation = ac.rotation - 7f;
                }else if(fade_count >= 85)
                {
                    blip.blip1_rotation = ac.rotation - 3f;
                    blip.blip2_rotation = ac.rotation - 7f;
                    blip.blip3_rotation = ac.rotation - 11f;
                }

                if (x_flag == true && y_flag == true)
                {
                    if (current_x == 0 && current_y == 1000)
                    {
                        ac.quadrant = 1;
                        x_flag = false;
                        y_flag = false;
                    }
                    else if (current_x == final_x && current_y == final_y)
                    {
                        print("rot " + ac.rotation);
                        print("final degree turn " + degree_turn);
                        ac.rotation += degree_turn;
                    }
                }
            }
            else if(ac.quadrant == 1)
            {
                print("quad 1");
                if (current_x != final_x || x_flag == false)
                {
                    current_x -= ac.turn_rate;
                    ac.xVelocity = current_x / 1000;
                    if (current_x == final_x && current_y != final_y)
                        current_x -= 1;
                    else if (current_x == -1000 && current_x != final_x)
                        x_flag = true;
                    else if (current_x == -1000 && current_x == final_x)
                        x_flag = true;
                    else if (current_x == final_x)
                        x_flag = true;
                }

                if (current_y != final_y || y_flag == false)
                {
                    current_y -= ac.turn_rate;
                    ac.yVelocity = current_y / 1000;
                    if (current_y == final_y && current_x != final_x)
                        current_y -= 1;
                    else if (current_y == 0 && current_y != final_y)
                        y_flag = true;
                    else if (current_y == 0 && current_y == final_y)
                        y_flag = true;
                    else if (current_y == final_y)
                        y_flag = true;
                }
                ac.rotation += .085f;
                degree_turn -= .085f;

                if (x_flag == true && y_flag == true)
                {
                    if (current_x == -1000 && current_y == 0)
                    {
                        ac.quadrant = 2;
                        x_flag = false;
                        y_flag = false;
                    }
                }
            }
            else if(ac.quadrant == 2)
            {
                print("quad 2");
                if (current_x != final_x || x_flag == false)
                {
                    current_x += ac.turn_rate;
                    ac.xVelocity = current_x / 1000;
                    if (current_x == final_x && current_y != final_y)
                        current_x += 1;
                    else if (current_x == 0 && current_x != final_x)
                        x_flag = true;
                    else if (current_x == 0 && current_x == final_x)
                        x_flag = true;
                    else if (current_x == final_x)
                        x_flag = true;
                }

                if (y_flag == false || current_y != final_y)
                {
                    current_y -= ac.turn_rate;
                    ac.yVelocity = current_y / 1000;
                    if (current_y == final_y && current_x != final_x)
                        current_y -= 1;
                    else if (current_y == -1000 && current_y != final_y)
                        y_flag = true;
                    else if (current_y == -1000 && current_y == final_y)
                        y_flag = true;
                    else if (current_y == final_y)
                        y_flag = true;
                }
                ac.rotation += .085f;
                degree_turn -= .085f;


                if (x_flag == true && y_flag == true)
                {
                    if (current_x == 0 && current_y == -1000)
                    {
                        ac.quadrant = 3;
                        x_flag = false;
                        y_flag = false;
                    }
                }
            }

            if (count > 3000)
                break;
            
            //print(ac.rotation);
                         
        }
        
        ac.xVelocity = final_x / 1000;
        ac.yVelocity = final_y / 1000;
        //ac.rotation += degree_turn;
        blip.blip1_rotation = ac.rotation;
        blip.blip2_rotation = ac.rotation;
        blip.blip3_rotation = ac.rotation;
        ac.heading = heading;            

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
        print("x: " + blip.xVelocity);
        print("y: " + blip.yVelocity);
        //Move(blip);
		update_datablock (blip);
        Update_Sector_Tage();
        if(blip.radar_contact == false)
        {
            bool oddeven = Mathf.FloorToInt(Time.time) % 2 == 0;
            flash_from_center(oddeven);
        }
    }

    void FixedUpdate()
    {
        rb2D.AddForce(new Vector3((float)blip.xVelocity, (float)blip.yVelocity, 0));
        rb2D.velocity = Vector3.ClampMagnitude(rb2D.velocity, .03f);
        rb2D.MoveRotation(blip.rotation);
        blip_fade1.transform.rotation = Quaternion.Euler(new Vector3(0, 0, blip.blip1_rotation));
        blip_fade2.transform.rotation = Quaternion.Euler(new Vector3(0, 0, blip.blip2_rotation));
        blip_fade3.transform.rotation = Quaternion.Euler(new Vector3(0, 0, blip.blip3_rotation));
    }

    public void flash_from_center(bool oddeven)
    {
        Image sector_sprite = sector_tag.GetComponent<Image>();
        sector_sprite.enabled = oddeven;
        //sector_tag_rend.enabled = oddeven;
    }

}

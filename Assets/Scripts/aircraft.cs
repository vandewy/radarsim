﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aircraft : MonoBehaviour {

    public int current_altitude;
    public int new_altitude;
    public string call_sign;
    public int heading;
    public int new_heading;
    public string type;
    public decimal xVelocity;
    public decimal yVelocity;
    public float rotation;
	public int climb_rate;
	public int descent_rate;
	public int ground_speed;
    public float turn_rate = .30f;
    public bool departure = false;
    public int quadrant;
    public bool radar_contact = false;

    public float blip1_rotation;
    public float blip2_rotation;
    public float blip3_rotation;

    public float rx;
    public float ry;
    public float rz;
}

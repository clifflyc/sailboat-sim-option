using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimControl : MonoBehaviour {

	//-------- Messages ---------//

	[Serializable]
	public class BoatState {
		public enum Major {
			Disabled = 0,
			Rc = 1,
			Autonomous = 2,
		}

		public enum Minor {
			Complete = 0,
			Planning = 1,
			Tracking = 2,
		}

		public int major;
		public int minor;

		public BoatState (int major, int minor) {
			this.major = major;
			this.minor = minor;
		}
	}

	[Serializable]
	public class Float32 {
		public float data;

		public Float32 (float data) {
			this.data = data;
		}
	}

	[Serializable]
	public class Point {
		public float x;
		public float y;

		public Point (float x, float y) {
			this.x = x;
			this.y = y;
		}

		public Point (Vector2 point) : this (point.x, point.y) {

		}
	}

	[Serializable]
	public class PointArray {
		public Point[] points;

		public PointArray (Vector2[] points) {
			this.points = new Point[points.Length];
			for (int i = 0; i < points.Length; i++) {
				this.points [i] = new Point (points [i]);
			}
		}
	}

	//------------ Variables ----------//

	[NonSerializedAttribute]
	public BoatState boatState;
	[NonSerializedAttribute]
	public float rudderValue = 90;
	[NonSerializedAttribute]
	public float winchValue;
	[NonSerializedAttribute]
	public float windStrength = 1;
	[NonSerializedAttribute]
	public float windDirection;
	[NonSerializedAttribute]
	public float currentStrength = 5;
	[NonSerializedAttribute]
	public float currentDirection;

	[NonSerializedAttribute]
	public Vector2 boatPos;
	[NonSerializedAttribute]
	public Vector2 boatVel;
	[NonSerializedAttribute]
	public Vector2 boatAccel;
	[NonSerializedAttribute]
	public float boatHeading;

	[NonSerializedAttribute]
	public Vector2[] waypoints;

	float timeSinceLastSend = 0;
	bool ready = false;

	//entry point
	public void InitSimulation (string ip) {
		//expect ip to be in format "192.168.0.29:9090"
		string fullUrl = "ws://" + ip;
		RosInterface.Connect (fullUrl);
		RosInterface.Subscribe<BoatState> ("/boat_state", "boat_msgs/BoatState").MessageReceived += OnBoatStateMsg;
		RosInterface.Subscribe<Float32> ("/rudder", "std_msgs/Float32").MessageReceived += OnRudderMsg;
		RosInterface.Subscribe<Float32> ("/winch", "std_msgs/Int32").MessageReceived += OnWinchMsg;
		RosInterface.Advertise ("/lps", "boat_msgs/Point");
		RosInterface.Advertise ("/waypoints", "boat_msgs/PointArray");
		RosInterface.Advertise ("/anemometer", "std_msgs/Float32");
		ready = true;
	}

	void OnBoatStateMsg (BoatState state) {
		this.boatState = state;
	}

	void OnRudderMsg (Float32 rudderMsg) {
		this.rudderValue = rudderMsg.data;
	}

	public void SetRudder (float rudder) {
		this.rudderValue = rudder;
	}

	void OnWinchMsg (Float32 winchMsg) {
		this.winchValue = winchMsg.data;
	}

	public void SetWindStrength (float windStrength) {
		this.windStrength = windStrength;
	}

	public void SetWindDirection (float windDirection) {
		this.windDirection = windDirection;
	}

	public void SetCurrentStrength (float currentStrength) {
		this.currentStrength = currentStrength;
	}

	public void SetCurrentDirection (float currentDirection) {
		this.currentDirection = currentDirection;
	}

	public void SetWaypoints (Vector2[] waypoints) {
		this.waypoints = waypoints;
		PublishWaypoints (waypoints);
	}

	void PublishCurrentPos (Vector2 currentPos) {
		RosInterface.Publish<Point> ("/lps", new Point (currentPos));
	}

	void PublishWaypoints (Vector2[] waypoints) {
		RosInterface.Publish<PointArray> ("/waypoints", new PointArray (waypoints));
	}

	void PublishWindDirection (float windDirection) {
		RosInterface.Publish<Float32> ("/anemometer", new Float32 (windDirection));
	}

	//--------------------------//

	void Update () {
		if (ready) {	
			UpdateBoatPosVelAccel (rudderValue, winchValue, windStrength, windDirection, currentStrength, currentDirection, ref boatHeading, ref boatPos, ref boatVel, ref boatAccel);
			if (timeSinceLastSend > 0.2f) {
				PublishCurrentPos (boatPos);
				PublishWindDirection (windDirection);
				timeSinceLastSend = 0;
			}
			timeSinceLastSend += Time.deltaTime;
		}
	}

	void UpdateBoatPosVelAccel (float rudder, float winch, float windStrength, float windDirection, float currentStrength, float currentDirection, ref float heading, ref Vector2 position, ref Vector2 velocity, ref Vector2 acceleration) {
		//winch and wind direction unused
		//acceleration unused
		//current direction unused

		float rudCentered = rudder - 90; //map 0 to 180 into -90 to 90
		float torque = (currentStrength + velocity.magnitude) * Mathf.Sqrt (1 - 1 * Mathf.Cos (rudCentered * Mathf.Deg2Rad)); // use cosing cos law
		heading -= Mathf.Sign (rudCentered) * torque * Time.deltaTime;
		heading %= 360;

		float deltaAngle = Mathf.Abs (Mathf.DeltaAngle (heading, windDirection));
		float boatSpeed = -deltaAngle * (deltaAngle - 180) * 0.00012345678f * windStrength; //quadratically map 0 to 180 into 0 to 1, * current strength
		velocity = Quaternion.Euler (new Vector3 (0, 0, heading)) * Vector3.up * boatSpeed;
		position += velocity * Time.deltaTime;
	}



}

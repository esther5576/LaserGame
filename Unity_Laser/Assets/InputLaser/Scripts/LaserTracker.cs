using UnityEngine;
using System.Collections;
using OSC.NET;

/// <summary>
/// Lien entre le tracker et unity
/// </summary>

public class LaserTracker : MonoBehaviour {
	
  static public Vector2 UNUSED_POSITION = new Vector3(-1000f,-1000f);

  static public int LASER_COUNT_MAX = 100;
	public const float TRACKER_WIDTH 		= 640; 
	public const float TRACKER_HEIGHT 		= 480;
	public const float TRACKER_FULL_AREA	= TRACKER_WIDTH * TRACKER_HEIGHT;
	public static float APP_WIDTH;
	public static float APP_HEIGHT;
	
  int msgReceivedNoArgs = 0;
  int msgReceived = 0;
  protected Vector2[] laserPoints;
	public int port = 12345;

  private OSCPacket lastPacket;
  private OSCMessage lastMessage;
	private OSCReceiverAsync oscReceiver;
	
  int lastIndex = -1;
  int screenPosX = -1;
  int screenPosY = -1;
  bool outOfBounds = false;

	void Awake(){
    laserPoints = new Vector2[LASER_COUNT_MAX];
    clearLasers ();
	}
	
	void Start(){
		APP_WIDTH 	= Camera.main.pixelWidth;
		APP_HEIGHT 	= Camera.main.pixelHeight;
		
		//Debug.Log("Sarting OSC receiver");
		oscReceiver = new OSCReceiverAsync(port);
		oscReceiver.Start(processOSCPacket);
	}
	
	public void kill(){
		if(oscReceiver != null)	oscReceiver.Close();
	}

	void OnApplicationQuit(){
		kill();
	}
	
	public Vector2[] getLasers(){ return laserPoints; }

  public void clearLasers() {
    for (int i = 0; i < laserPoints.Length; i++) {
      laserPoints[i].x = UNUSED_POSITION.x;
      laserPoints[i].y = UNUSED_POSITION.y;
    } 
  }
	
	private void processOSCPacket(OSCPacket packet){
		if(packet != null){

      lastPacket = packet;

			if (packet.IsBundle()) {
				ArrayList messages = packet.Values;
				for (int i = 0; i < messages.Count; i++) {
					processMessage((OSCMessage)messages[i]);
				}
			} else {
				processMessage((OSCMessage)packet);
			}
		}
	}
	
  /* on reçoit un message par point de laser */
	private string processMessage(OSCMessage message) {
		string address = message.Address;
		ArrayList args = message.Values;
		
    lastMessage = message;

		if (address == "/newFrame" || address =="/noLasers")
    {
      clearLasers();
			//lock(laserScreenFrames){ laserScreenFrames[currentFrame].Clear(); }
    }

		if(args.Count > 0){

      msgReceived++;
      //Debug.Log("\t\treceived message ("+msgReceived+"):"+address+" args ? "+args.Count);

			if(address == "/laser"){
				/*
        Debug.Log("Pos:"+(int)args[1]+","+(int)args[2]);
				Debug.Log("Size:"+(int)args[3]+", Ratio:"+(int)args[3]/TRACKER_FULL_AREA);
				Debug.Log("width:"+(int)args[4]+", height"+(int)args[5]);
				Debug.Log("Bounding rect center:"+(int)args[6]+","+(int)args[7]);
        */
				
        //raw coords
				screenPosX = (int)args[1];
				screenPosY = (int)args[2];

        //transform into screen coord
				screenPosX = (int)(((float)screenPosX / TRACKER_WIDTH) * APP_WIDTH);
				screenPosY = (int)((TRACKER_HEIGHT - (float)screenPosY) / TRACKER_HEIGHT * APP_HEIGHT);
				
				//Debug.Log("Screen:"+APP_WIDTH+","+APP_HEIGHT);
        outOfBounds = false;

				lock(laserPoints){
          lastIndex = 0;

          //search for next available point in array

          //le x de laserPoints[] est exprimé en screenspace transformé, donc -1000f <> -45f
          //while(laserPoints[lastIndex].x <= UNUSED_POSITION.x && lastIndex < laserPoints.Length)

          //UNUSED_POSITION.x à -1000f <> -45f in screen space
          while(laserPoints[lastIndex].x >= -40f && lastIndex < laserPoints.Length) {
            //Debug.Log("" + laserPoints[lastIndex]);
            lastIndex++;
          }

          if(lastIndex < laserPoints.Length)
          {
            laserPoints[lastIndex].x = screenPosX;
            laserPoints[lastIndex].y = screenPosY;

            //Debug.Log("updated "+i);
          }else{
            outOfBounds = true;
            //Debug.LogWarning("<LaserTracker> trying to add a point at index "+lastIndex+" for a array of "+laserPoints.Length+" possible lasers");
          }

				}
			}
		}else{

      msgReceivedNoArgs++;

    }

		return message.Address;
	}

  public string toString(){
    string ct = "";
    ct += "\n[OSC] msgReceived : "+msgReceived + " || "+msgReceivedNoArgs;

    if(lastMessage != null){
      ct += "\n"+lastMessage.Address;
      ct += "\nvalue : "+lastMessage.Values;
    }

    ct += "\n oob ? "+outOfBounds;
    ct += "\n last used index = "+lastIndex;
    ct += "\nscreenPosX = "+screenPosX;
    ct += "\nscreenPosY = "+screenPosY;

    if(lastPacket != null){
      ct += "\nlastPacket = "+lastPacket;
    }

    return ct;
  }

  static protected LaserTracker tracker;
  static public LaserTracker get(){
    if(tracker == null) tracker = GameObject.FindObjectOfType<LaserTracker>();
    return tracker;
  }
}

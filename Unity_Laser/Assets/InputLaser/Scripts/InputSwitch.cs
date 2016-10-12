using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Surcouche pour gérer des infos liés au laser OU l'utilisation de la souris
/// </summary>

/*
 * Si on est dans la build !Application.isEditor make debug_noLaser à false
 * La layer qui gère les lasers n'existent pas dans l'éditeur tant qu'on les toggle pas
 * */

public class InputSwitch : MonoBehaviour {
  
  Vector3[] laserPositions;

  Vector3 _mousePosition = Vector3.zero;
  Vector3 temp = Vector3.zero;

	Camera cam; // va prendre la cam par defaut
	LaserTracker track; // handle du manager de laser

	public bool debugUsingCursor = false;
  public bool debugShowIncomingLasers = false;
	
	void Awake() {
    
    track = getTracker();
    if(track == null) Debug.LogWarning("{InputSwitch} no tracker found");

		cam = Camera.main;

    laserPositions = new Vector3[LaserTracker.LASER_COUNT_MAX];

    for (int i = 0; i < laserPositions.Length; i++) {
      laserPositions[i] = LaserTracker.UNUSED_POSITION;
    }
		
	}

  void Update(){
    
    update_Lasers(); // osc stuff
    update_debugPointers();
	}

  protected void update_debugPointers(){

    //pas de debug quand on voit pas la souris !
    if (!Cursor.visible) return;

    //update le vector
    getMousePosition();

    //par defaut le curseur de la souris est un laser actif
    laserPositions[laserPositions.Length-1] = _mousePosition;
    
    //add debug points
    if(Input.GetMouseButtonDown(0)){

      //remove point around click position
      if(Input.GetKey(KeyCode.LeftShift)){

        float clearRadius = 2f;

        for (int i = laserPositions.Length-1; i > 0; i--) {

          //found unused pointer
          if (laserPositions[i].x < -20f) continue;

          laserPositions[i].z = _mousePosition.z;

          float dist = Vector3.Distance(laserPositions[i], _mousePosition);
          if(dist < clearRadius){
            laserPositions[i] = LaserTracker.UNUSED_POSITION;
          }
        }

      }else{
        
        //add current mouse position to static pointer list
        int idx = getDebugLastLaserFreePosition();
        if(idx < 0){
          Debug.LogWarning("{InputSwitch} can't add more static, need bigger array ("+laserPositions.Length+" length)");
          return;
        }

        //Debug.Log(_mousePosition + " at " + idx);
        laserPositions[idx] = _mousePosition;
      }

    }

  }

  protected int getDebugLastLaserFreePosition() {
    for (int i = laserPositions.Length-1; i > 0; i--)
    {
      //Debug.Log(laserPositions[i]);
      if (laserPositions[i].x < -20f) return i;
    }
    return -1;
  }

	protected void update_Lasers(){
    LaserTracker tracker = LaserTracker.get();
    if (tracker == null) return;

    Vector2[] points = LaserTracker.get().getLasers();
    
    int laserIdx = 0;

    //on clear les anciens
    while (laserPositions[laserIdx].x > -20f && laserIdx < laserPositions.Length) {
      laserPositions[laserIdx].x = LaserTracker.UNUSED_POSITION.x;
      laserPositions[laserIdx].y = LaserTracker.UNUSED_POSITION.y;
      laserIdx++;
    }

    //on choppe les nouveaux lasers
    laserIdx = 0;

    for(int i = 0; i < points.Length; i++){

      //on prend pas les points 'null'
      if (points[i].x < -20f) continue;

      //transform la coord du laser dans l'écran vers le monde
      temp.x = (float)points[i].x;
      temp.y = (float)points[i].y;
      temp.z = -cam.transform.position.z;
		  	
      laserPositions[laserIdx] = cam.ScreenToWorldPoint(temp);

      laserIdx++;
		}
    
	}
	
	// est-ce qu'il y a au moins 1 pointeur à l'écran ?
	public bool hasPointer(){
    return countActivePointers() > 0;
	}

  public int countPointers(){ return laserPositions.Length; }

  public int countActivePointers(){
    int count = 0;

    //je sais pas pourquoi mais les laserPositions sont transformé de -1000px -> world space (-45~)
    for (int i = 0; i < laserPositions.Length; i++) {
      if(laserPositions[i].x > -20f) count++;
    }
    
    return count;
  }
  
  Vector3 getMousePosition(){
    //process
    _mousePosition.x = Input.mousePosition.x;
    _mousePosition.y = Input.mousePosition.y;
    _mousePosition.z = Input.mousePosition.z;
    _mousePosition.z = -cam.transform.position.z;
    _mousePosition = cam.ScreenToWorldPoint(_mousePosition);
    return _mousePosition;
  }
  
  public void clearPoints(){
    for (int i = 1; i < laserPositions.Length; i++) {
      laserPositions[i] = LaserTracker.UNUSED_POSITION;
    }
  }
  
  public bool hasInputs(Bounds objBounds)
  {
    for (int i = 0; i < laserPositions.Length; i++)
    {
      if (laserPositions[i].x < -20f) continue; // skip out of bounds positions
      if (objBounds.Contains(laserPositions[i])) return true;
    }
    return false;
  }

	// retourne le nombre de pointeurs dans un object bounds donné
	public int getNbInputsInBounds(Camera refCam, Bounds objBounds){
		int count = 0;

    count += evalPointsInBound(laserPositions, objBounds);
    //Debug.Log("eval laser : " + count);

    return count;
	}

  int evalPointsInBound(Vector3[] pts, Bounds bnd){
    int count = 0;
    for (int i = 0; i < pts.Length; i++) {
      if(pts[i].x < -20f) continue; // skip out of bounds positions
      if(bnd.Contains(pts[i])) count++;
    }
    return count;
  }
  
  public LaserTracker getTracker(){ 
    if(track == null) track = GameObject.FindObjectOfType<LaserTracker>();
    return track;
  }

  public Vector3[] getLasersPositions(){
    return laserPositions;
  }
  
  static public int getCount(){ return manager.countActivePointers(); }
	
  
  static protected InputSwitch manager = null;
  static public InputSwitch get(){
    if(manager == null) manager = GameObject.FindObjectOfType<InputSwitch>();
    return manager;
  }

}

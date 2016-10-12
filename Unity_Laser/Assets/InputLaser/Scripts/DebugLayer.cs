using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// D pour afficher le debug des lasers
/// permet aussi d'avoir des infos sur le SON ou les INPUT
/// </summary>

public class DebugLayer : MonoBehaviour {
	
	GameObject carrier;
	List<DebugGizmo> gizmos = new List<DebugGizmo>();
  List<Renderer> gizmosRender = new List<Renderer>();

  InputSwitch input;
  public bool draw = false;

  int panelId = 0;

  public const int DEFAULT = 0;
  public const int SOUND = 1;
  public const int INPUT = 2;

  LaserTracker input_track;

	void Start () {

    input = InputSwitch.get();

    input_track = LaserTracker.get();

    carrier = gameObject;

    //default pool
    //for(int i = 0; i < 30; i++){ add(); }

    kill ();
	}

  public void toggle(){

		if(draw)	kill ();
		else open ();

    Debug.Log("<DebugLayer> toggled("+draw+")");
	}
	
	public void open(){
    draw = true;
	}
	
	public void kill(){
    killGizmos();
    draw = false;
	}

  void killGizmos(){
    for(int i = 0; i < gizmosRender.Count; i++){
      gizmosRender[i].enabled = false;
    }
  }
	
	DebugGizmo add(){
		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("debug/laserGizmo"));
		obj.transform.parent = carrier.transform;
    obj.transform.localPosition = LaserTracker.UNUSED_POSITION;

		DebugGizmo giz = obj.GetComponent<DebugGizmo>();

    Renderer gizRender = giz.GetComponent<Renderer>();
    gizmosRender.Add(gizRender);
    gizRender.enabled = false;
		gizmos.Add(giz);

		return giz;
	}

  void Update(){

    if(Input.GetKeyDown(KeyCode.D)) toggle();
    
    if(!draw) return;
    
    if(Input.GetKeyUp(KeyCode.LeftArrow)){
      panelId--;
      if(panelId < 0) panelId = DEFAULT;
      event_swapPanel();
    }else if(Input.GetKeyUp(KeyCode.RightArrow)){
      panelId++;
      if(panelId > INPUT) panelId = INPUT;
      event_swapPanel();
    }

    //if(!input.hasPointer()) return;

    //draw all debug input sphere
    refreshGizmos();
  }

  void event_swapPanel(){
    if(!draw || panelId != DEFAULT){
      killGizmos ();
    }
  }
	
  void refreshGizmos(){

    if(panelId != DEFAULT) return;

    //[laser,laser,...,nothing,nothing,...,debug,debug]
    Vector3[] all = input.getLasersPositions();
    
    //rajoute les gizmos qui manque pour le rendu
    while(gizmos.Count < all.Length){ add();  }

    int i = 0;

    //laser points
    while(all[i].x > -20f) {
      gizmosRender[i].enabled = true;
      if (gizmosRender[i].enabled)
      {
        gizmos[i].setPosition(all[i]); // need world position
        gizmos[i].killText();
      }
      i++;
    }

    //unused points
    while(i < all.Length) {
      if (all[i].x < -20f) gizmosRender[i].enabled = false;
      else {

        //show debug points
        gizmosRender[i].enabled = true;
        if (gizmosRender[i].enabled)
        {
          gizmos[i].setPosition(all[i]); // need world position
          gizmos[i].killText();
        }

      }
      i++;
    }
    
  }
	
  Rect guiRec = new Rect(10,10,400,300);
  string content = "";
	void OnGUI(){
    if(!draw) return;

    content = "";

    switch(panelId){
    case DEFAULT : 

      content = "[DEFAULT]";

      content = "\ncount:"+input.countPointers();
      content += "\ncount active:"+input.countActivePointers();

      int qty = 0;

      Vector3[] lasers = input.getLasersPositions();

      if(lasers != null && lasers.Length > 0){
        
        //on affiche au max 5 points
        qty = Mathf.Min(5, lasers.Length);

        //on affiche le compte total de lasers
        content += "\n lasers[] length = "+lasers.Length;
        for (int i = 0; i < qty; i++) {
          content += "\n laser #"+i+" : "+lasers[i];
        }

        for (int i = lasers.Length-1; i > lasers.Length-qty; i--)
        {
          content += "\n debug #" + i + " : " + lasers[i];
        }

      }
      
      break;
      
    case INPUT :

      content = "[INPUT]";
      content += "\n"+input_track.toString();

      break;
    }

		
		GUI.TextArea(guiRec, content);
	}

  static protected DebugLayer manager;
  static public DebugLayer get(){
    if(manager == null) manager = GameObject.FindObjectOfType<DebugLayer>();
    return manager;
  }
}

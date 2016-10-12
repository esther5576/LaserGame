using UnityEngine;
using System.Collections;

/*
 * Affiche simplement un cube à l'endroit du GameObject
 * */

public class DebugGizmo : MonoBehaviour {
	TextMesh txt;
	
	bool useText = true;
	
  Renderer txtRender;

	void Start(){
		txt = gameObject.GetComponentInChildren<TextMesh>();
		//txt = transform.position.x+","+transform.position.y;
		if(txt != null){
      txtRender = txt.GetComponent<Renderer>();
			useText = txtRender.enabled;
		}
		
		killText();
	}
	
	public void setPosition(Vector3 worldPosition){
		//Vector3 screen = Camera.main.WorldToScreenPoint(worldPosition);
		transform.position = worldPosition;
		//txt.text = Mathf.FloorToInt(app.x).ToString()+","+Mathf.FloorToInt(app.y).ToString();
	}
	
	public void setDebugText(string content){
		if(txt == null)	return;
    if (!txtRender.enabled) txtRender.enabled = useText;
		txt.text = content;
	}
	
	public void killText(){
		if(txt == null)	return;
    if(!txtRender.enabled) txtRender.enabled = false;
		txt.text = "";
	}
}

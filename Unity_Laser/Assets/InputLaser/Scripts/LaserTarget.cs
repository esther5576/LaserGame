using UnityEngine;
using System;
using System.Collections.Generic;

public class LaserTarget : MonoBehaviour {

  protected InputSwitch input;
  protected Collider[] _colliders;
  protected Camera _camera;
  
  bool _mouseOver = false;
  
  public Action onMouseOut;
  public Action onMouseOutUpdate;
  public Action onMouseOver;
  public Action onMouseOverUpdate;

  virtual protected void Awake(){
    _camera = Camera.main;

    if (_camera == null) Debug.LogError("no camera ?");

    input = InputSwitch.get();

    fetchCollider();
  }

  protected void reset(){
    _mouseOver = false;
  }

  //gather collider on object and children (symbol !)
  public void fetchCollider()
  {
    List<Collider> tmp = new List<Collider>();

    Collider _collider = GetComponent<Collider>();

    if (_collider != null) tmp.Add(_collider);

    _colliders = gameObject.GetComponentsInChildren<Collider>();
    for (int i = 0; i < _colliders.Length; i++)
    {
      tmp.Add(_colliders[i]);
    }

    _colliders = tmp.ToArray();

    //Debug.Log("{LaserTarget} target " + name + " has " + _colliders.Length + " colliders", gameObject);
  }

  virtual protected void Update(){
    
    int countTouch = 0;

    if(_colliders.Length > 0) {
      //countTouch = countInputs();
      countTouch = hasInputs() ? 1 : 0;
    }

    //onMouseOver
    if (countTouch >= 1){
      
      //si il était pas touché et qu'il l'est maintenant
      if (!_mouseOver)
      {
        _mouseOver = true;
        if(onMouseOver != null) onMouseOver();
      }
      else
      {
        if (onMouseOverUpdate != null) onMouseOverUpdate();
      }
      
    }else {

      if (_mouseOver)
      {
        _mouseOver = false;
        if (onMouseOut != null) onMouseOut();
      } 
      else
      {
        if (onMouseOutUpdate != null) onMouseOutUpdate();
      }
      
    }
  }
  
  protected bool hasInputs() {
    return input.hasInputs(_colliders[0].bounds);
    /*
    for (int i = 0; i < _colliders.Length; i++)
    {
      if (input.hasInputs(_colliders[i].bounds)) return true;
    }
    return false;
    */
  }

  protected int countInputs() {
    int countTouch = 0;
    for (int i = 0; i < _colliders.Length; i++)
    {
      countTouch += input.getNbInputsInBounds(_camera, _colliders[i].bounds);
    }
    return countTouch;
  }
}

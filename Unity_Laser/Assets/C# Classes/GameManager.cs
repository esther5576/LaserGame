using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public List<GameObject> m_aEntranceList = new List<GameObject>();
    int m_iEntranceIndex = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void NextLevel()
    {
        if (m_iEntranceIndex <= m_aEntranceList.Count)
            m_iEntranceIndex++;
        else
            m_iEntranceIndex = 0;
    }

    public Vector3 GetNextEntrancePos()
    {
        return m_aEntranceList[m_iEntranceIndex].transform.position;
    }
}

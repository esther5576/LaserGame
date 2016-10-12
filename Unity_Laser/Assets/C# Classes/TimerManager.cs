using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TimerManager:MonoBehaviour {

    public float m_fTimer;
    public bool m_bTick = false;
    string m_sTimerText;

	// Use this for initialization
	void Start () {
        StartTimer();
	}
	
	// Update is called once per frame
	void Update () {

        Tick();
	}

    void Tick()
    {
        if(m_bTick)
        {
            m_fTimer += Time.deltaTime;
            UpdateText();
        }
    }

    void UpdateText()
    {
        GetComponent<Text>().text = ToText();
    }

    public string ToText()
    {
        int minutes = Mathf.FloorToInt(m_fTimer / 60F);
        int seconds = Mathf.FloorToInt(m_fTimer - minutes * 60);
        m_sTimerText = string.Format("{0:0}:{1:00}", minutes, seconds);
        return m_sTimerText;
    }

    public void Reset()
    {
        m_fTimer = 0;
        m_bTick = true;
    }

    public void StopTimer()
    {
        m_bTick = false;
    }

    public void StartTimer()
    {
        m_bTick = true;
    }

    public void Victory()
    {
        m_bTick = false;
        string sWinTimeText = m_sTimerText;
        GetComponent<Text>().text = "GG WP ! You won in: " + sWinTimeText;
    }
}

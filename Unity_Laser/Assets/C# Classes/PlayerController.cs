using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float m_fMoveSpeed = 1;

    Vector3 m_vInitialPos;

    bool m_bHasWon = false;

    TimerManager m_pTimerScript;
    GameManager m_pGameManagerScript;

	// Use this for initialization
	void Start () {

        m_vInitialPos = transform.position;
        m_pTimerScript = GameObject.Find("Timer").GetComponent<TimerManager>();
        m_pGameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();

    }
	
	// Update is called once per frame
	void Update () {

        UpdateMovement();
	}

    void UpdateMovement()
    {

        if(!m_bHasWon)
        {
            if (Input.GetKey(KeyCode.D))
                GetComponent<Rigidbody>().AddForce(Vector3.right * m_fMoveSpeed);
            if (Input.GetKey(KeyCode.A))
                GetComponent<Rigidbody>().AddForce(-Vector3.right * m_fMoveSpeed);
            if (Input.GetKey(KeyCode.W))
                GetComponent<Rigidbody>().AddForce(Vector3.up * m_fMoveSpeed);
            if (Input.GetKey(KeyCode.S))
                GetComponent<Rigidbody>().AddForce(-Vector3.up * m_fMoveSpeed);
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        

    }

    void Reset()
    {
        transform.position = m_vInitialPos;
        m_bHasWon = false;      
    }

    void Victory()
    {
        m_pTimerScript.Victory();
        m_bHasWon = true;
        StartCoroutine("VictoryCo");
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            Debug.Log("hit");
            Reset();
        }
            
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Exit")
        {
            Victory();
        }
    }

    IEnumerator VictoryCo()
    {
        yield return new WaitForSeconds(3f);
        m_pTimerScript.Reset();
        Reset();
        m_pGameManagerScript.NextLevel();
        transform.position = m_pGameManagerScript.GetNextEntrancePos();
        yield return null;
    }
}

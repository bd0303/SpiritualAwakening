using UnityEngine;
using System.Collections;

public class GrabObject : MonoBehaviour
{
    public Transform target;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    
    void OnMouseDown()
    {
        //move object ahead of player
        this.transform.position = target.position;
        this.transform.parent = GameObject.Find("FirstPersonCharacter").transform;
        
    }

    void OnMouseUp()
    {
        //let go of object
        this.transform.parent = null;
    }
    
}

using UnityEngine;
using System.Collections;

public class WeaponSwing : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetButtonDown("Attack"))
            GetComponent<Animation>().Play("AttackAnimation");
	}
}

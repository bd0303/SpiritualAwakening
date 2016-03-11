using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class HealthHandle : MonoBehaviour
{

    public Text texter;
    public bool done = false;
    public static int health = 100;

    void Start()
    {
        texter = GetComponent<Text>(); //
    }


    // Update is called once per frame
    void Update()
    {

        if (done == false)    texter.text = "Health: " + health ;

        if (health == 0)
        {
            done = true;
            texter.text = "I'm Dead";
        }

    }

}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class Scorer : MonoBehaviour {

    public Text texter;
    public static int score = 0;
    public int tickCounter = 0;
	
    void Start()
    {
        texter = GetComponent<Text>(); //
    }


    // Update is called once per frame
    void Update ()
    {

        tickCounter++;

        if (tickCounter % 10 == 0)
        texter.text = "Score: " + score++;
	}
}

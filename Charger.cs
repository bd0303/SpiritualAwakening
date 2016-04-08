using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Charger : MonoBehaviour
{

    public Text texter;
    public static int chargesP = 0;
    public static int chargesE = 0;

    // Use this for initialization
    void Start ()
    {
        texter = GetComponent<Text>(); //
    }
	
	// Update is called once per frame
	void Update ()
    {
        texter.text = "Charges: " + chargesP;
	}
}

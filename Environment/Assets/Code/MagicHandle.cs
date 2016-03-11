using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MagicHandle : MonoBehaviour
{

    public Text texter;
    public int magic = 100;
    public int tickCounter = 0;
    bool hasMagic = true;

    void Start()
    {
        texter = GetComponent<Text>(); //
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetMouseButtonDown(0) && hasMagic)
        {
            if (magic - 10 < 0)
            {

                magic = 0;
                hasMagic = false;

            }

            else
            {

                magic = magic - 10;

                texter.text = "Magic: " + (magic);

            }

        }


        if ((Input.GetKeyDown("space")) && hasMagic)
        {

            if (magic - 5 < 0)
            {

                magic = 0;
                hasMagic = false;

            }

            else
            {

                magic = magic - 5;

                texter.text = "Magic: " + (magic);

            }

        }

        tickCounter++;

        if (tickCounter % 50 == 0 && (magic > 0) && hasMagic)
        {

            magic = magic - 1;

            texter.text = "Magic: " + (magic);
        }

        if (magic == 0)    hasMagic = false;

    }
}

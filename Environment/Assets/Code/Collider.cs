using UnityEngine;
using System.Collections;

public class Collider: MonoBehaviour {




	void OnCollisionEnter(Collision col)
    {

        Debug.Log(col.gameObject.name);


        if (col.gameObject.name == "Coin")
        {          
            Destroy(col.gameObject);
            Scorer.score += 100;
           
        }

        if (col.gameObject.name == "Ball")
        {

            HealthHandle.health -= 10;

        }

        //if (col.gameObject.name == "Enemy")
        //{
        //    Destroy(col.gameObject);
        //    HealthHandler.heakth -= 10;
        //
        //}

    }

}

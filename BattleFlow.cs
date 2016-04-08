using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleFlow : MonoBehaviour
{
    public GameObject l1, l2, h1, h2, h3, r1, r2, r3;
    public GameObject canvas;
    public Vector3 ObjectSpawnPosition;
    public bool notBusy = true;
    public bool emptyTank = true;
    public int aiChoice = 0;
    public int playerHits = 0;
    public int compHits = 0;
    //public GameObject obj;

    void Start()
    {

        canvas = GameObject.Find("Canvas");

        r1 = Instantiate(Resources.Load("r1"), new Vector3(425, 5, 22), Quaternion.Euler(0, 0, 0)) as GameObject;
        r1.transform.SetParent(canvas.transform, false);

        r2 = Instantiate(Resources.Load("r1"), new Vector3(485, 5, 22), Quaternion.Euler(0, 0, 0)) as GameObject;
        r2.transform.SetParent(canvas.transform, false);

        r3 = Instantiate(Resources.Load("r1"), new Vector3(545, 5, 22), Quaternion.Euler(0, 0, 0)) as GameObject;
        r3.transform.SetParent(canvas.transform, false);

        h1 = Instantiate(Resources.Load("h1"), new Vector3(-374, -300, 22), Quaternion.Euler(0, 0, 0)) as GameObject;
        h1.transform.SetParent(canvas.transform, false);

        h2 = Instantiate(Resources.Load("h1"), new Vector3(-315, -300, 22), Quaternion.Euler(0, 0, 0)) as GameObject;
        h2.transform.SetParent(canvas.transform, false);

        h3 = Instantiate(Resources.Load("h1"), new Vector3(-255, -300, 22), Quaternion.Euler(0, 0, 0)) as GameObject;
        h3.transform.SetParent(canvas.transform, false);

    }

    int enemyAI()
    {
        if (Charger.chargesE == 0)
        {
            Charger.chargesE++;
            return 0;
        }

        else
        {
            int val = Random.Range(0, 3);

            if (val == 0) Charger.chargesE++; 
            if (val == 1) Charger.chargesE--;

            return val;

        }

    }

    void processPHits(int hits)
    {
        if (hits == 1)
        {
            h1.SetActive(false);
        }

        if (hits == 2)
        {
            h2.SetActive(false);
        }

        if (hits == 3)
        {
            h3.SetActive(false);
        }
    }

    void processCHits(int hits)
    {

        if (hits == 1)
        {
            r1.SetActive(false);
        }

        if (hits == 2)
        {
            r2.SetActive(false);
        }

        if (hits == 3)
        {
            r3.SetActive(false);
        }

    }

    void Update()
    {
        if (Charger.chargesP == 0) emptyTank = true;
        else emptyTank = false;

        if (Input.GetKeyDown(KeyCode.A) && notBusy)
        {

            aiChoice = enemyAI();

            notBusy = false;

            Debug.Log(l1.gameObject.name);
            //l1.transform.SetParent(canvasObject.transform);
            l1 = Instantiate(Resources.Load("powerGold"), new Vector3(-55, -160, 70), Quaternion.Euler(0, 0, 0)) as GameObject;
            
            l1.transform.SetParent(canvas.transform, false);

            if (aiChoice == 0)
            {
                l2 = Instantiate(Resources.Load("powerBlue"), new Vector3(230, -1, 70), Quaternion.Euler(0, 0, 0)) as GameObject;

                l2.transform.SetParent(canvas.transform, false);
            }

            if (aiChoice == 1)
            {
                l2 = Instantiate(Resources.Load("lightBlue"), new Vector3(50, 80, 70), Quaternion.Euler(0, 0, -60)) as GameObject;

                l2.transform.SetParent(canvas.transform, false);

                playerHits++;

                processPHits(playerHits);

            }

            if (aiChoice == 2)
            {
                l2 = Instantiate(Resources.Load("shieldBlue"), new Vector3(230, -1, 70), Quaternion.Euler(0, 0, 0)) as GameObject;

                l2.transform.SetParent(canvas.transform, false);
            }

            StartCoroutine("pause2");

            Debug.Log("Falsify");

            Charger.chargesP++;

        }

        else if (Input.GetKeyDown(KeyCode.S) && notBusy && !emptyTank)
        {

            aiChoice = enemyAI();
            notBusy = false;

            Debug.Log(l1.gameObject.name);
            //l1.transform.SetParent(canvasObject.transform);
            l1 = Instantiate(Resources.Load("lightGold"), new Vector3(25,70,70),  Quaternion.Euler(0,0,-30)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            StartCoroutine("pause2");

            if (aiChoice == 0)
            {
                l2 = Instantiate(Resources.Load("powerBlue"), new Vector3(230, -1, 70), Quaternion.Euler(0, 0, 0)) as GameObject;

                l2.transform.SetParent(canvas.transform, false);

                compHits++;

                processCHits(compHits);

            }

            if (aiChoice == 1)
            {
                l2 = Instantiate(Resources.Load("lightBlue"), new Vector3(50, 80, 70), Quaternion.Euler(0, 0, -60)) as GameObject;

                l2.transform.SetParent(canvas.transform, false);
            }

            if (aiChoice == 2)
            {
                l2 = Instantiate(Resources.Load("shieldBlue"), new Vector3(230, -1, 70), Quaternion.Euler(0, 0, 0)) as GameObject;

                l2.transform.SetParent(canvas.transform, false);
            }

            Debug.Log("Falsify");

            Charger.chargesP--;

        }


        else if (Input.GetKeyDown(KeyCode.D) && notBusy)
        {

            aiChoice = enemyAI();
            notBusy = false;

            Debug.Log(l1.gameObject.name);
            //l1.transform.SetParent(canvasObject.transform);
            l1 = Instantiate(Resources.Load("shieldGold"), new Vector3(25, 70, 70), Quaternion.Euler(0, 0, -30)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            StartCoroutine("pause2");

            if (aiChoice == 0)
            {
                l2 = Instantiate(Resources.Load("powerBlue"), new Vector3(230, -1, 70), Quaternion.Euler(0, 0, 0)) as GameObject;

                l2.transform.SetParent(canvas.transform, false);
            }

            if (aiChoice == 1)
            {
                l2 = Instantiate(Resources.Load("lightBlue"), new Vector3(50, 80, 70), Quaternion.Euler(0, 0, -60)) as GameObject;

                l2.transform.SetParent(canvas.transform, false);
            }

            if (aiChoice == 2)
            {
                l2 = Instantiate(Resources.Load("shieldBlue"), new Vector3(230, -1, 70), Quaternion.Euler(0, 0, 0)) as GameObject;

                l2.transform.SetParent(canvas.transform, false);
            }

            Debug.Log("Falsify");

            Charger.chargesP--;

        }


    }

    IEnumerator pause2()
    {
        yield return new WaitForSeconds(0.2f);


        

        Debug.Log(l1.gameObject.name);
        if (l1.active && l1.gameObject.name == "powerGold(Clone)")
        {

            Debug.Log("entered");

            l1.SetActive(false);

            l1 = Instantiate(Resources.Load("powerGold"), new Vector3(-75, -160, 70), Quaternion.Euler(0, 0, 0)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            yield return new WaitForSeconds(0.15f);
            l1.SetActive(false);

            l1 = Instantiate(Resources.Load("powerGold"), new Vector3(-55, -160, 70), Quaternion.Euler(0, 0, 0)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            yield return new WaitForSeconds(0.15f);
            l1.SetActive(false);

            l1 = Instantiate(Resources.Load("powerGold"), new Vector3(-75, -160, 70), Quaternion.Euler(0, 0, 0)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            yield return new WaitForSeconds(0.15f);
            l1.SetActive(false);

            l1 = Instantiate(Resources.Load("powerGold"), new Vector3(-55, -160, 70), Quaternion.Euler(0, 0, 0)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            yield return new WaitForSeconds(0.15f);
            l1.SetActive(false);

            l1 = Instantiate(Resources.Load("powerGold"), new Vector3(-75, -160, 70), Quaternion.Euler(0, 0, 0)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            yield return new WaitForSeconds(0.15f);
            l1.SetActive(false);

        }

        if (l1.active && l1.gameObject.name == "lightGold(Clone)")
        {

            Debug.Log("entered");

            l1.SetActive(false);

            l1 = Instantiate(Resources.Load("lightGold"), new Vector3(70, 80, 70), Quaternion.Euler(0, 0, -60)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            yield return new WaitForSeconds(0.15f);
            l1.SetActive(false);

            l1 = Instantiate(Resources.Load("lightGold"), new Vector3(50, 80, 70), Quaternion.Euler(0, 0, -60)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            yield return new WaitForSeconds(0.15f);
            l1.SetActive(false);

            l1 = Instantiate(Resources.Load("lightGold"), new Vector3(70, 80, 70), Quaternion.Euler(0, 0, -60)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            yield return new WaitForSeconds(0.15f);
            l1.SetActive(false);

            l1 = Instantiate(Resources.Load("lightGold"), new Vector3(50, 80, 70), Quaternion.Euler(0, 0, -60)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            yield return new WaitForSeconds(0.15f);
            l1.SetActive(false);

            l1 = Instantiate(Resources.Load("lightGold"), new Vector3(70, 80, 70), Quaternion.Euler(0, 0, -60)) as GameObject;
            l1.transform.SetParent(canvas.transform, false);
            yield return new WaitForSeconds(0.15f);
            l1.SetActive(false);

        }

        if (l2.active && (l2.gameObject.name == "powerBlue(Clone)" || l2.gameObject.name == "lightBlue(Clone)" || l2.gameObject.name == "shieldBlue(Clone)"))
        {
            yield return new WaitForSeconds(0.15f);
            l2.SetActive(false);
        }


        notBusy = true;

    }

}
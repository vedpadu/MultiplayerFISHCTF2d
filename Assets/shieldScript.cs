using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shieldScript : MonoBehaviour
{
    public float team;

    private float time;

    public float deathStartTime;

    public string particlesRed;

    public string particlesBlue;

    private bool instantiated;
    // Start is called before the first frame update
    void Start()
    {
        if (team == 1)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1,0,0,0);
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= deathStartTime)
        {
            if (instantiated == false)
            {
                if (team == 1)
                {
                    PhotonNetwork.Instantiate(particlesRed, this.transform.position, this.transform.rotation, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(particlesBlue, this.transform.position, this.transform.rotation, 0);
                }

                instantiated = true;
            }
            Color temp = this.gameObject.GetComponent<SpriteRenderer>().color;
            float alpha = Mathf.Lerp(1f, 0f, time - deathStartTime);
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(temp.r, temp.g, temp.b, alpha);
            if (time >= deathStartTime + 1f)
            {
                
                
                Destroy(this.gameObject);
            }
        }

        if (time <= 1)
        {
            Color temp = this.gameObject.GetComponent<SpriteRenderer>().color;
            float alpha = Mathf.Lerp(0f, 1f, time);
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(temp.r, temp.g, temp.b, alpha);
        }
        
    }

    [PunRPC]
    void setLayerShield(float localTeam)
    {
        team = localTeam;
        if (localTeam == 1)
        {
            
            this.gameObject.layer = 10;
        }
        else
        {
            this.gameObject.layer = 11;
        }
    }
    
    
}

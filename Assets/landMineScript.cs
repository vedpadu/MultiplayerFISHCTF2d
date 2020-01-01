using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class landMineScript : MonoBehaviour
{
    public string explosionPref;
    private float team;
    private float lerpTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lerpTime += Time.deltaTime;
        if (GameObject.Find("teamScoreManager").GetComponent<teamScoreScript>().team != team)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1, Mathf.Lerp(1,0,lerpTime * 0.4f));
        }
    }

    public void Explode()
    {
        PhotonNetwork.Instantiate(explosionPref, this.transform.position, this.transform.rotation, 0);
        PhotonNetwork.Destroy(this.gameObject);
    }

    [PunRPC]
    void setTeam(float teamToSet)
    {
        team = teamToSet;
    }
}

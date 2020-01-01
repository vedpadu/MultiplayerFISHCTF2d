using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalScript : MonoBehaviour
{
    public float team;

    public string particleEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.inRoom)
        {
            if ((string)PhotonNetwork.room.CustomProperties["gameMode"] == "Assassin Ball")
            {
                this.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((string)PhotonNetwork.room.CustomProperties["gameMode"] == "Assassin Ball")
        {
            if (other.gameObject.layer == 12)
            {
                for (var i = 0; i < GameObject.FindGameObjectsWithTag("ship").Length; i++)
                {
                    StartCoroutine(GameObject.FindGameObjectsWithTag("ship")[i].transform.GetChild(0).gameObject
                        .GetComponent<CamerShake>().Shake(0.15f, 2f));
                }
                GameObject.Find("teamScoreManager").GetComponent<PhotonView>().RPC("addScore", PhotonTargets.All, 1f, team);
                PhotonNetwork.Instantiate(particleEffect, other.gameObject.transform.position, Quaternion.identity, 0);
                other.gameObject.transform.position = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                PhotonNetwork.Instantiate(particleEffect, other.gameObject.transform.position, Quaternion.identity, 0);
            }
        }
    }
}

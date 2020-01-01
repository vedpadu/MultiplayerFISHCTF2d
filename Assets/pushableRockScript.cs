using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushableRockScript : MonoBehaviour
{
    public float team;

    public bool isBall;

    public bool noDoDamage;
    // Start is called before the first frame update
    void Start()
    {
        
        Vector3 origPos = this.transform.position;
        if (PhotonNetwork.isMasterClient)
        {
            this.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.masterClient);
        }
        
        this.transform.position = origPos;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (PhotonNetwork.inRoom)
        {
            if ((string)PhotonNetwork.room.CustomProperties["gameMode"] == "Juggernaut")
            {
                this.gameObject.GetComponent<PhotonView>().RPC("setInactive", PhotonTargets.All, false);
            }

            if ((string) PhotonNetwork.room.CustomProperties["gameMode"] == "Death Match")
            {
                this.gameObject.GetComponent<PhotonView>().RPC("setInactive", PhotonTargets.All, true);
            }
        }*/
        if (PhotonNetwork.inRoom)
        {
            if ((string) PhotonNetwork.room.CustomProperties["gameMode"] == "Death Match")
            {
                if (noDoDamage == false)
                {
                    if (team == 1)
                    {
                        this.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                    else
                    {
                        this.GetComponent<SpriteRenderer>().color = Color.blue;
                    }
                }
                else
                {
                    this.GetComponent<SpriteRenderer>().color = Color.white;
                }

            }
            else
            {
                this.GetComponent<SpriteRenderer>().color = new Color(1, 0, 1, 1);
            }
        }


    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "ship")
        {
            
            if (other.gameObject.GetComponent<teamManager>().team == team)
            {
                this.gameObject.GetComponent<PhotonView>().TransferOwnership(other.gameObject.GetComponent<PhotonView>().owner);
            }
            else
            {
                
                if ((string)PhotonNetwork.room.CustomProperties["gameMode"] == "Death Match")
                {
                    if (other.gameObject.GetComponent<teamManager>().team != team && noDoDamage == false)
                    {
                        other.gameObject.GetComponent<PhotonView>().RPC("punResetLife", PhotonTargets.All, 0f);
                    }
                    
                }
                else
                {
                    this.gameObject.GetComponent<PhotonView>().TransferOwnership(other.gameObject.GetComponent<PhotonView>().owner);
                }
                
            }
            
        }
    }

    [PunRPC]
    void setInactive(bool setValue)
    {
        this.gameObject.GetComponent<Collider2D>().enabled = setValue;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = setValue;
    }
}

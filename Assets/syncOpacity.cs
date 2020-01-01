using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class syncOpacity : Photon.MonoBehaviour
{
    private float realOpacity = 0;
    public float interpolationRate;
    //public bool isPufferfish;
    // Use this for initialization
    void Start () {
		
    }
	
    // Update is called once per frame
    void Update () {
        if (photonView.isMine)
        {
            //Do nothing--it is moving us;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(gameObject.GetComponent<SpriteRenderer>().color.r, gameObject.GetComponent<SpriteRenderer>().color.g, gameObject.GetComponent<SpriteRenderer>().color.b, realOpacity);
            
            //transform.localScale = Vector3.Lerp(transform.localScale, realScale, interpolationRate);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("Syncing");
        if (stream.isWriting)
        {
            //This is Our player. We need to send our position to network.
            stream.SendNext(this.gameObject.GetComponent<SpriteRenderer>().color.a);
        }
        else
        {
            //This is someone else's player. We need to recieve their position and update our position of them.
            realOpacity = (float) stream.ReceiveNext();
            
            float lag = Mathf.Abs((float) (PhotonNetwork.time - info.timestamp));
           // realOpacity += float(this.GetComponent<Rigidbody2D>().velocity * lag);
            //realScale = (Vector3)stream.ReceiveNext();
            

        }
    } 
}
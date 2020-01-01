using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class networkTextSync : Photon.MonoBehaviour
{
    private string realText = "";
    private float realColorR;
    private float realColorG;
    private float realColorB;
    
    

    
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
            this.gameObject.GetComponent<TextMesh>().text = realText;
            this.gameObject.GetComponent<TextMesh>().color = new Color(realColorR,realColorG,realColorB);

            //transform.localScale = Vector3.Lerp(transform.localScale, realScale, interpolationRate);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("Syncing");
        if (stream.isWriting)
        {
            //This is Our player. We need to send our position to network.
            stream.SendNext(this.gameObject.GetComponent<TextMesh>().text);
            stream.SendNext(this.gameObject.GetComponent<TextMesh>().color.r);
            stream.SendNext(this.gameObject.GetComponent<TextMesh>().color.g);
            stream.SendNext(this.gameObject.GetComponent<TextMesh>().color.b);
        }
        else
        {
            //This is someone else's player. We need to recieve their position and update our position of them.
            realText = (string) stream.ReceiveNext();
            realColorR = (float) stream.ReceiveNext();
            realColorG = (float) stream.ReceiveNext();
            realColorB = (float) stream.ReceiveNext();
            //realScale = (Vector3)stream.ReceiveNext();


        }
    } 
}
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class networkChar : Photon.MonoBehaviour {
    Vector3 realPosition = Vector2.zero;
    Quaternion realRotation = Quaternion.identity;
    Vector3 realScale = Vector3.zero;
    private float realTeam = 0f;
    private float colorR = 0f;
    private float colorG = 0f;
    private float colorB = 0f;
    private float colorA = 0f;
    public bool teamSend;
    public float interpolationRate;

    public bool isScaleSend;

    public bool isNotRotationSend;

    public bool isNotPositionSend;
    public bool isColorSend;

    //public bool isTextSend;
    //public bool isPufferfish;
    // Use this for initialization
    void Start ()
    {
        realPosition = this.transform.position;
    }
	
    // Update is called once per frame
    void Update () {
        if (photonView.isMine)
        {
            //Do nothing--it is moving us;
        }
        else
        {
            if (isNotPositionSend == false)
            {
                transform.position = Vector3.Lerp(transform.position, realPosition, interpolationRate);
            }

            if (isNotRotationSend == false)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, interpolationRate);
            }
            
            if (isScaleSend)
            {
                transform.localScale = realScale;
            }

            if (isColorSend)
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color(colorR,colorG,colorB,colorA);
            }

            if (teamSend)
            {
                gameObject.GetComponent<teamManager>().team = realTeam;
            }
            
            //transform.localScale = Vector3.Lerp(transform.localScale, realScale, interpolationRate);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("Syncing");
        if (stream.isWriting)
        {
            //This is Our player. We need to send our position to network.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            if (teamSend)
            {
                stream.SendNext(this.gameObject.GetComponent<teamManager>().team);
            }

            if (isColorSend)
            {
                stream.SendNext(this.gameObject.GetComponent<SpriteRenderer>().color.r);
                stream.SendNext(this.gameObject.GetComponent<SpriteRenderer>().color.g);
                stream.SendNext(this.gameObject.GetComponent<SpriteRenderer>().color.b);
                stream.SendNext(this.gameObject.GetComponent<SpriteRenderer>().color.a);
            }
            
            stream.SendNext(this.transform.localScale);
            
        }
        else
        {
            //This is someone else's player. We need to recieve their position and update our position of them.
            realPosition = (Vector3) stream.ReceiveNext();
            realRotation = (Quaternion) stream.ReceiveNext();
            if (teamSend)
            {
                realTeam = (float)stream.ReceiveNext();
            }

            if (isColorSend)
            {
                colorR = (float) stream.ReceiveNext();
                colorG = (float) stream.ReceiveNext();
                colorB = (float) stream.ReceiveNext();
                colorA = (float) stream.ReceiveNext();
            }
            
            float lag = Mathf.Abs((float) (PhotonNetwork.time - info.timestamp));
            realPosition += (Vector3)(this.GetComponent<Rigidbody2D>().velocity * lag);
            if (isScaleSend)
            {
                realScale = (Vector3) stream.ReceiveNext();
            }
            
                //realScale = (Vector3)stream.ReceiveNext();
            

        }
    }

    [PunRPC]
    void setLayer(int layer)
    {
        this.gameObject.layer = layer;
    }
}
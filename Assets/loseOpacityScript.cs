using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loseOpacityScript : MonoBehaviour
{
    private SpriteRenderer sR;

    public float opacityLoss;
    // Start is called before the first frame update
    void Start()
    {
        sR = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, sR.color.a - opacityLoss * Time.deltaTime);
        if (sR.color.a <= 0 && this.GetComponent<PhotonView>().isMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}

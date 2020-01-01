using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invincibilityFrames : MonoBehaviour
{
    public bool invincible;

    public float invincibilityTime;

    private float invincibilityTimer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
            if (invincible)
            {
                invincibilityTimer += Time.deltaTime;
                if (invincibilityTimer >= invincibilityTime)
                {
                    this.GetComponent<PhotonView>().RPC("setInvincible", PhotonTargets.All, false);
                    invincibilityTimer = 0;
                }
            }
        
        
    }

    [PunRPC]
    void setInvincible(bool setValue)
    {
        invincible = setValue;
    }
}

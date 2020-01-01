using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class teamManager : MonoBehaviour
{
    public float team;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    [PunRPC]
    void setTeam(float value)
    {
        team = value;
    }
}

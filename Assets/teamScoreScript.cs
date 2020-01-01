using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class teamScoreScript : MonoBehaviour
{
    public float redScore = 0;

    public float blueScore = 0;

    public float winAmountDeathMatch;
    public float winAmountAssassinBall;

    public float team;

    public float juggernautMatchTime;
    public float time;

    public bool juggernautDead;
    // Start is called before the first frame update
    void Start()
    {
        
            
            Debug.Log(PhotonNetwork.room.CustomProperties);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (PhotonNetwork.inRoom)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            if ((string) PhotonNetwork.room.CustomProperties["gameMode"] == "Death Match" || (string) PhotonNetwork.room.CustomProperties["gameMode"] == "Assassin Ball")
            {
                this.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                this.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                this.transform.GetChild(0).GetChild(0).gameObject.transform.localPosition = new Vector3(-20, 200,0);
                this.transform.GetChild(0).GetChild(1).gameObject.transform.localPosition = new Vector3(20, 200,0);
                this.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().color = new Color(0,1,1);
                this.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>().color = new Color(1,4/255,4/255);
                this.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = PhotonNetwork.room.CustomProperties["blueScore"].ToString();
                this.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = PhotonNetwork.room.CustomProperties["redScore"].ToString();
                if ((string) PhotonNetwork.room.CustomProperties["gameMode"] == "Death Match")
                {
                    if ((float) PhotonNetwork.room.CustomProperties["redScore"] >= winAmountDeathMatch)
                    {
                        if (team == 1)
                        {
                            this.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                            setGameRunFalse();
                        }
                        else
                        {
                            this.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                            setGameRunFalse();
                        }
                    }

                    if ((float) PhotonNetwork.room.CustomProperties["blueScore"] >= winAmountDeathMatch)
                    {
                        if (team == 1)
                        {
                            this.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                            setGameRunFalse();
                        }
                        else
                        {
                            this.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                            setGameRunFalse();
                        }
                    }
                }
                else
                {
                    if ((float) PhotonNetwork.room.CustomProperties["redScore"] >= winAmountAssassinBall)
                    {
                        if (team == 1)
                        {
                            this.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                            setGameRunFalse();
                        }
                        else
                        {
                            this.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                            setGameRunFalse();
                        }
                    }

                    if ((float) PhotonNetwork.room.CustomProperties["blueScore"] >= winAmountAssassinBall)
                    {
                        if (team == 1)
                        {
                            this.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                            setGameRunFalse();
                        }
                        else
                        {
                            this.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                            setGameRunFalse();
                        }
                    }
                }
                
            }
            if (GameObject.FindGameObjectsWithTag("ship").Length > 1 && PhotonNetwork.isMasterClient)
            {
                if (time < juggernautMatchTime)
                {
                    time += Time.deltaTime;
                }
                
                float value = (float) PhotonNetwork.room.CustomProperties["matchTime"];
                float newValue = time;
                Hashtable setValue = new Hashtable();
                setValue.Add("matchTime", newValue);
                Hashtable expectedValue = new Hashtable();
                expectedValue.Add("matchTime", value);
        
                PhotonNetwork.room.SetCustomProperties(setValue, expectedValue, false);
            }

            if ((string) PhotonNetwork.room.CustomProperties["gameMode"] == "Juggernaut")
            {
                

                float minutesLeft = (juggernautMatchTime - (float) PhotonNetwork.room.CustomProperties["matchTime"]) /
                                    60;
                float decimalMinutes = minutesLeft - Mathf.Floor(minutesLeft);
                int seconds = Mathf.RoundToInt(decimalMinutes * 59);
                string stringSeconds = "";
                if (seconds < 10)
                {
                    stringSeconds = "0" + seconds.ToString();
                }
                else
                {
                    stringSeconds = seconds.ToString();
                }
                this.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text =
                    Mathf.Floor(minutesLeft).ToString() + ":" + stringSeconds;
                this.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().color = new Color(150/255,0,1);
                this.transform.GetChild(0).GetChild(0).gameObject.transform.localPosition = new Vector3(0, 200,0);
                this.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                if ((float)PhotonNetwork.room.CustomProperties["matchTime"] >= juggernautMatchTime)
                {
                    
                    if (team == 1)
                    {
                        this.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                        setGameRunFalse();
                    }
                    else
                    {
                        this.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                        setGameRunFalse();
                    }
                }

                if (juggernautDead)
                {
                    if (team == 1)
                    {
                        this.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                        setGameRunFalse();
                    }
                    else
                    {
                        this.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                        setGameRunFalse();
                       
                    }
                }
            }
            
        }
        
       /*if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("reee");
            this.GetComponent<PhotonView>().RPC("setScore", PhotonTargets.All, blueScore, 1);
            this.GetComponent<PhotonView>().RPC("setScore", PhotonTargets.All, redScore, 2);
        }*/
            
        
        
    }

    public void leaveRoom(float seconds)
    {
        GameObject.Find("_SCRIPTS").GetComponent<NetworkManager>().leaving = true;
        PhotonNetwork.LeaveRoom();
        
        GameObject.Find("_SCRIPTS").GetComponent<NetworkManager>().leaving = false;
        
    }

    [PunRPC]
    void setJuggernautDead(int temp)
    {
        juggernautDead = true;
    }

    void setGameRunFalse()
    {
        bool value = (bool) PhotonNetwork.room.CustomProperties["gameRunning"];
        bool newValue = false;
        Hashtable setValue = new Hashtable();
        setValue.Add("gameRunning", newValue);
        Hashtable expectedValue = new Hashtable();
        expectedValue.Add("gameRunning", value);
        
        PhotonNetwork.room.SetCustomProperties(setValue, expectedValue, false);
    }

    [PunRPC]
    void addScore(float amount, float team)
    {
        if (team == 1)
        {
            float value = (float) PhotonNetwork.room.CustomProperties["blueScore"];
            float newValue  = value  + amount;
            Hashtable setValue = new Hashtable();
            setValue.Add("blueScore", newValue);
            Hashtable expectedValue = new Hashtable();
            expectedValue.Add("blueScore", value);
        
            PhotonNetwork.room.SetCustomProperties(setValue, expectedValue, false);
            blueScore += amount;
        }
        else
        {
            float value = (float) PhotonNetwork.room.CustomProperties["redScore"];
            float newValue  = value  + amount;
            Hashtable setValue = new Hashtable();
            setValue.Add("redScore", newValue);
            Hashtable expectedValue = new Hashtable();
            expectedValue.Add("redScore", value);
            
            PhotonNetwork.room.SetCustomProperties(setValue, expectedValue, false);
            redScore += amount;
        }
    }

    [PunRPC]
    void setScore(float amount, float team)
    {
        if (team == 1)
        {
            blueScore = amount;
        }
        else
        {
            redScore = amount;
        }
    }
}

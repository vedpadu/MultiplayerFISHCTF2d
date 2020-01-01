using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class networkPlayer : MonoBehaviour {
    public string playerPrefName;
    public bool spawned = false;
    public float amountSpawned = 0;
    public GameObject playerChar;
    public bool selectingChar;
    public GameObject selectScreen;
    public GameObject selectScreenJug;
    public GameObject selectScreenAssassin;

    public string name;

    private GameObject textObject;

    private TextMesh tM;

    private Text pingText;

    public float team;

    public Vector3 spawnPosChar;
    
	// Use this for initialization
	void Start ()
    {
        if (team == 1)
        {
            spawnPosChar = GameObject.Find("redSpawn").transform.position;
        }
        else
        {
            spawnPosChar = GameObject.Find("blueSpawn").transform.position;
        }
        /*if ((string)PhotonNetwork.room.CustomProperties["gameMode"] == "Juggernaut" && team == 2)
        {
            PhotonNetwork.Destroy(playerChar);
            playerPrefName = "juggernaut";
            SpawnThisPlayer(this.transform.position);
        }
        if ((string)PhotonNetwork.room.CustomProperties["gameMode"] == "Assassin Ball")
        {
            PhotonNetwork.Destroy(playerChar);
            playerPrefName = "assassinShip";
            SpawnThisPlayer(this.transform.position);
        }*/
        selectScreen = this.transform.GetChild(1).GetChild(1).gameObject;
        selectScreenJug = this.transform.GetChild(1).GetChild(2).gameObject;
        selectScreenAssassin = this.transform.GetChild(1).GetChild(3).gameObject;
        pingText = this.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (playerChar != null)
        {
            if (team == 1)
            {
                playerChar.transform.GetChild(3).gameObject.GetComponent<TextMesh>().color = Color.red;
            }
            else
            {
                playerChar.transform.GetChild(3).gameObject.GetComponent<TextMesh>().color = Color.blue;
            }
        }
        pingText.text = "Ping: " + PhotonNetwork.networkingPeer.RoundTripTime.ToString() + " ms";
        if (this.GetComponent<PhotonView>().isMine)
        {
            
            if (Input.GetKeyDown(KeyCode.X))
            {
                selectingChar = true;
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                selectingChar = false;
            }

            if (selectingChar)
            {
                
                if (team == 1)
                {
                    if ((string) PhotonNetwork.room.CustomProperties["gameMode"] == "Death Match" ||
                        (string) PhotonNetwork.room.CustomProperties["gameMode"] == "Juggernaut")
                    {
                        selectScreen.SetActive(true);
                    
                    
                        if (Input.GetKeyDown(KeyCode.F1))
                        {
                            playerPrefName = "sniperShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }

                        if (Input.GetKeyDown(KeyCode.F2))
                        {
                            playerPrefName = "machineGunTankShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }

                        if (Input.GetKeyDown(KeyCode.F3))
                        {
                            playerPrefName = "healerShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }

                        if (Input.GetKeyDown(KeyCode.F4))
                        {
                            playerPrefName = "assassinShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }
                        if (Input.GetKeyDown(KeyCode.F5))
                        {
                            playerPrefName = "bomberShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }
                        if (Input.GetKeyDown(KeyCode.F6))
                        {
                            playerPrefName = "minigunShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }
                    }
                    if((string)PhotonNetwork.room.CustomProperties["gameMode"] == "Assassin Ball")
                    {
                        selectScreenAssassin.SetActive(true);
                    }
                   
                }
                else
                {
                    if ((string)PhotonNetwork.room.CustomProperties["gameMode"] == "Death Match")
                    {
                        selectScreen.SetActive(true);
                        if (Input.GetKeyDown(KeyCode.F1))
                        {
                            playerPrefName = "sniperShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }

                        if (Input.GetKeyDown(KeyCode.F2))
                        {
                            playerPrefName = "machineGunTankShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }

                        if (Input.GetKeyDown(KeyCode.F3))
                        {
                            playerPrefName = "healerShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }

                        if (Input.GetKeyDown(KeyCode.F4))
                        {
                            playerPrefName = "assassinShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }
                        if (Input.GetKeyDown(KeyCode.F5))
                        {
                            playerPrefName = "bomberShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }
                        if (Input.GetKeyDown(KeyCode.F6))
                        {
                            playerPrefName = "minigunShip";
                            PhotonNetwork.Destroy(playerChar);
                            SpawnThisPlayer(spawnPosChar);
                            selectingChar = false;
                        }
                    } else if ((string)PhotonNetwork.room.CustomProperties["gameMode"] == "Juggernaut")
                    {
                        //selectScreenJug.transform.position = playerChar.transform.position;
                        selectScreenJug.SetActive(true);
                    }else if ((string) PhotonNetwork.room.CustomProperties["gameMode"] == "Assassin Ball")
                    {
                        selectScreenAssassin.SetActive(true);
                    }
                }
                
                
               
            }
            else
            {
                selectScreenJug.SetActive(false);
                selectScreen.SetActive(false);
                selectScreenAssassin.SetActive(false);
            }
        }
        
	}

    public void selectChar(string playerPrefabName)
    {
        if (playerPrefabName != "juggernaut")
        {
            playerPrefName = playerPrefabName;
            PhotonNetwork.Destroy(playerChar);
            SpawnThisPlayer(spawnPosChar);
            selectingChar = false;
        }
        else
        {
            playerChar.transform.position = spawnPosChar;
            selectingChar = false;
        }
        
    }

    public void SpawnThisPlayer(Vector3 position)
    {
        GameObject player = (GameObject)PhotonNetwork.Instantiate(playerPrefName, position, Quaternion.identity, 0);
        //player.GetComponent<PhotonView>().RPC("setTeam", PhotonTargets.All, team);
        player.GetComponent<teamManager>().team = team;
        player.GetComponent<respawnPosSave>().respawnPos = spawnPosChar;
        playerChar = player;
        if (team == 1)
        {
            playerChar.GetComponent<PhotonView>().RPC("setLayer", PhotonTargets.All, 10);
        }
        else
        {
            playerChar.GetComponent<PhotonView>().RPC("setLayer", PhotonTargets.All, 11);
        }
        player.transform.GetChild(3).gameObject.GetComponent<TextMesh>().text = name;

        player.transform.GetChild(0).gameObject.SetActive(true); //turn on Camera
        if (playerPrefName == "sniperShip")
        {
            player.GetComponent<sniperShipScript>().enabled = true;
            player.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (playerPrefName == "machineGunTankShip")
        {
            player.GetComponent<machineGunShipScript>().enabled = true;
            player.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (playerPrefName == "healerShip")
        {
            player.GetComponent<healerShipScript>().enabled = true;
            player.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (playerPrefName == "assassinShip")
        {
            player.GetComponent<assassinScript>().enabled = true;
            player.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (playerPrefName == "bomberShip")
        {
            player.GetComponent<bomberScript>().enabled = true;
            player.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (playerPrefName == "minigunShip")
        {
            player.GetComponent<minigunScript>().enabled = true;
            player.transform.GetChild(1).gameObject.SetActive(true);
        }

        if (playerPrefName == "juggernaut")
        {
            player.GetComponent<juggernautScript>().enabled = true;
            player.transform.GetChild(1).gameObject.SetActive(true);
        }
        

    }
}

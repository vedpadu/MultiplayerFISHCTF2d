using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviour {
    public GameObject standbyCam;

    public GameObject standbyCanvas;

    private InputField userIF;
    private InputField roomCreate;

    private string userName;
    private bool instantiated;
    public GameObject joinButton;

    private GameObject joinSV;

    public float team;

    private bool teamSet;

    private string roomSetGametype;

    private bool setProperties = false;

    private Dropdown gameChooseDropdown;

    public bool leaving = false;
	// Use this for initialization
	void Start ()
    {
        gameChooseDropdown = standbyCanvas.transform.GetChild(4).gameObject.GetComponent<Dropdown>();
        joinSV = standbyCanvas.transform.GetChild(6).GetChild(0).GetChild(0).gameObject;
        userIF = standbyCanvas.transform.GetChild(0).gameObject.GetComponent<InputField>();
        roomCreate = standbyCanvas.transform.GetChild(3).gameObject.GetComponent<InputField>();
        Connect();
	}
	
	void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("1.0.0.1");
    }

    void OnGUI()
    { 
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
    

    void OnJoinedLobby()
    {
        GameObject.Find("teamScoreManager").transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        GameObject.Find("teamScoreManager").transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        GameObject.Find("teamScoreManager").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("teamScoreManager").GetComponent<teamScoreScript>().time = 0;
        GameObject.Find("teamScoreManager").GetComponent<teamScoreScript>().juggernautDead = false;
        standbyCam.SetActive(true);
        standbyCanvas.SetActive(true);
        instantiated = false;
        setProperties = false;
        //inLobby = true;
        //Debug.Log("skrrtskrt");
        //PhotonNetwork.JoinRandomRoom();
    }

    void OnLeftRoom()
    {
        //standbyCam.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (PhotonNetwork.inRoom)
            {
                PhotonNetwork.LeaveRoom();
                
                standbyCam.SetActive(true);
                standbyCanvas.SetActive(true);
                instantiated = false;
            }
            
        }

        if (PhotonNetwork.inRoom)
        {
            if (teamSet == false)
            {
                if (setProperties)
                {
                    Hashtable customProperties = new Hashtable() {{"blueScore", 0f}, {"redScore", 0f}, {"gameMode", gameChooseDropdown.options[gameChooseDropdown.value].text}, {"matchTime", 0f}, {"gameRunning", true}};
                    
                    PhotonNetwork.room.SetCustomProperties(customProperties, PhotonNetwork.room.CustomProperties);
                    Debug.Log(PhotonNetwork.room.CustomProperties);
                    setProperties = false;
                }
                StartCoroutine(waiterMethod());
                
                
                teamSet = true;
            }
        }
        if (PhotonNetwork.insideLobby)
        {
            leaving = false;
            if (instantiated == false)
            {
                for (var i = joinSV.transform.GetChildCount() - 1; i >= 0; i++)
                {
                    Destroy(joinSV.transform.GetChild(i).gameObject);
                }
                InstantiateScrollView();
                
                instantiated = true;
            }
            
        }
    }
    
    void OnConnectionFail()
    {
       // Debug.Log(DisconnectCause.);
        
    }
    
    IEnumerator waiterMethod()
    {


        //Wait for 4 seconds
        Debug.Log("ok");
        yield return new WaitForSeconds(1f);
        Debug.Log("waited");
        //PhotonNetwork.room.CustomProperties.Add("val", 0);
        GameObject[] allShips = GameObject.FindGameObjectsWithTag("ship");
        int redCount = 0;
        int blueCount = 0;
        for (var i = 0; i < allShips.Length; i++)
        {
            if (allShips[i].GetComponent<teamManager>().team == 1)
            {
                redCount += 1;
            }
            else
            {
                blueCount += 1;
            }
        }

        if ((string)PhotonNetwork.room.CustomProperties["gameMode"] == "Death Match" || (string)PhotonNetwork.room.CustomProperties["gameMode"] == "Assassin Ball")
        {
            if ((string) PhotonNetwork.room.CustomProperties["gameMode"] == "Death Match")
            {
                GameObject[] rocks = GameObject.FindGameObjectsWithTag("pushableRock");
                for (var i = 0; i < rocks.Length; i++)
                {
                    if (rocks[i].GetComponent<pushableRockScript>().isBall)
                    {
                        rocks[i].GetComponent<PhotonView>().RPC("setInactive", PhotonTargets.All, false);
                    }
                    else
                    {
                        rocks[i].GetComponent<PhotonView>().RPC("setInactive", PhotonTargets.All, true);
                    }
                }
            }
            else
            {
                GameObject[] rocks = GameObject.FindGameObjectsWithTag("pushableRock");
                for (var i = 0; i < rocks.Length; i++)
                {
                    if (rocks[i].GetComponent<pushableRockScript>().isBall)
                    {
                        rocks[i].GetComponent<PhotonView>().RPC("setInactive", PhotonTargets.All, true);
                    }
                    else
                    {
                        rocks[i].GetComponent<PhotonView>().RPC("setInactive", PhotonTargets.All, false);
                    }
                    
                }
            }
            if (blueCount > redCount)
            {
                team = 1;
            }
            else if(redCount > blueCount)
            {
                team = 2;
            }else if (redCount == blueCount)
            {
                float rand = Random.Range(0, 100);
                if (rand <= 50)
                {
                    team = 1;
                }
                else
                {
                    team = 2;
                }
            } 
        }else if ((string)PhotonNetwork.room.CustomProperties["gameMode"] == "Juggernaut")
        {
            GameObject[] rocks = GameObject.FindGameObjectsWithTag("pushableRock");
            for (var i = 0; i < rocks.Length; i++)
            {
                rocks[i].GetComponent<PhotonView>().RPC("setInactive", PhotonTargets.All, false);
            }
            //blue is the juggernaut - 2
            //red is all others - 1
            if (blueCount == 0)
            {
                team = 2;
            }
            else
            {
                team = 1;
            }
        }

        GameObject.Find("teamScoreManager").GetComponent<teamScoreScript>().team = team;
        
        teamSet = true;
        SpawnMyPlayer(team);
        
        yield return null;

    }

    void OnPhotonRandomJoinFailed()
    {
        setProperties = true;
        PhotonNetwork.CreateRoom("random");
    }

    public void createRoom()
    {
        if (leaving)
        {
            return;
        }
        setProperties = true;
        PhotonNetwork.CreateRoom(roomCreate.text);
        
    }

    void InstantiateScrollView()
    {
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            return;
        }
        for (var i = 0; i < PhotonNetwork.countOfRooms; i++)
        {
            GameObject button = GameObject.Instantiate(joinButton, new Vector3(joinSV.transform.position.x + 91.5f, i * 30 + joinSV.transform.position.y, joinSV.transform.position.z), Quaternion.identity, joinSV.transform);
            button.transform.localPosition = new Vector2(91.5f, -i * 30 - 30);
            button.transform.GetChild(0).gameObject.GetComponent<Text>().text = PhotonNetwork.GetRoomList()[i].Name.ToString();
            button.transform.parent = joinSV.transform;
            UnityEngine.UI.Button buttonButton = button.GetComponent<UnityEngine.UI.Button>();
            buttonButton.onClick.AddListener(() => roomJoin(button.transform.GetChild(0).GetComponent<Text>().text));

        }
    }
    

    void roomJoin(string roomName)
    {
        if (leaving)
        {
            return;
        }
        PhotonNetwork.JoinRoom(roomName);
    }
    

    public void joinRandom()
    {
        if (leaving)
                {
                    return;
                }
        //Debug.Log(PhotonNetwork.GetRoomList()[0]);
        PhotonNetwork.JoinRandomRoom();
    }

    void OnJoinedRoom()
    {
        Debug.Log("joinedRoom");
        
        teamSet = false;

    }

    void SpawnMyPlayer(float teamLocal)
    {
        Debug.Log("ConnectionSpawned");
        GameObject go = (GameObject)PhotonNetwork.Instantiate("playerConnectionObj", Vector3.zero, Quaternion.identity, 0);
        go.GetComponent<networkPlayer>().team = teamLocal;
        go.GetComponent<networkPlayer>().name = userIF.text;
        if ((string) PhotonNetwork.room.CustomProperties["gameMode"] == "Juggernaut")
        {
            go.GetComponent<networkPlayer>().playerPrefName = "juggernaut";
        }
        if ((string) PhotonNetwork.room.CustomProperties["gameMode"] == "Assassin Ball")
        {
            go.GetComponent<networkPlayer>().playerPrefName = "assassinShip";
        }
        go.GetComponent<networkPlayer>().SpawnThisPlayer(go.GetComponent<networkPlayer>().spawnPosChar);
        
        
        standbyCam.SetActive(false);
        standbyCanvas.SetActive(false);
    }
}

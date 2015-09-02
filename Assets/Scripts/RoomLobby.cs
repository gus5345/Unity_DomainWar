using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


// 팀을 선택하는 스크립트

public class RoomLobby : MonoBehaviour {

    private PhotonView pv = null;
    public Button b1, b2; // 팀별 이동하는 버튼
    public Text roomName;
    public Text totalPeople; // 총 인원
    public Text aimKill; // 목표 킬수
    public GameObject koreaName; // 한국팀 정보 출력 변수
    public GameObject koreaScroll; // 한국팀 스크롤
    public GameObject chinaName; // 중국팀 정보 출력 변수
    public GameObject chinaScroll; // 중국팀 스크롤
    public GameObject myEventSystem; // 버튼을 클릭후 Enter 눌렀을시 중복되는것을 방지하기 위한 변수
    public GameObject roomLobby; // 로비를 끄기 위한 변수
    int setRedTeam = 0; // 인원설정 변수
    int setBlueTeam = 0;
    int teamBalance = 1; // 팀밸런스를 맞추기위한 변수

    void Awake()
    {
        PhotonNetwork.isMessageQueueRunning = true;
    }
	void Start () 
    {
        pv = GetComponent<PhotonView>();
        roomName.text = PhotonNetwork.room.name;
        totalPeople.text = PhotonNetwork.room.maxPlayers.ToString();
        b1.onClick.AddListener(delegate { KoreaTeam(); }); // 버튼 연결
        b2.onClick.AddListener(delegate { ChinaTeam(); });
        myEventSystem = GameObject.Find("EventSystem");
        KoreaTeam(); // 맨처음 방을 들어오면 한국팀으로 설정
        try
        {
            aimKill.text = PhotonNetwork.room.customProperties["aimKill"].ToString(); // 방,포톤 재접속시 오류 발생 문제없음
        }
        catch (System.Exception)
        {
        }
	}
    void KoreaTeam()
    {
        PhotonNetwork.player.SetTeam(PunTeams.Team.red);
        myEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null); // Enter 눌렀을시 중복 클릭되는 것을 방지

        if (setRedTeam > 10) // 10명이 넘었다면
        {
            PhotonNetwork.player.SetTeam(PunTeams.Team.blue); // 블루팀으로
        }

        Renew();
    }
    void ChinaTeam()
    {
        PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
        myEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);

        if (setBlueTeam > 10)
        {
            PhotonNetwork.player.SetTeam(PunTeams.Team.red);
        }
        
        Renew();
    }
    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Renew();
    }
    void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Renew();
    }
    void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        Renew();
    }
    void Renew()
    {
        for (int a = 0; a < GameObject.FindGameObjectsWithTag("KoreaTeam").Length; a++) // 다 지웠다가 다시 그림 나간 유저정보를 없애기 위해서
        {
            Destroy(GameObject.FindGameObjectsWithTag("KoreaTeam")[a]);
        }
        for (int b = 0; b < GameObject.FindGameObjectsWithTag("ChinaTeam").Length; b++)
        {
            Destroy(GameObject.FindGameObjectsWithTag("ChinaTeam")[b]);
        }

        setBlueTeam = 0;
        setRedTeam = 0;
        koreaScroll.GetComponent<RectTransform>().sizeDelta = Vector2.zero; //스크롤 영역 초기화
        chinaScroll.GetComponent<RectTransform>().sizeDelta = Vector2.zero; //스크롤 영역 초기화

        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            if (PhotonNetwork.playerList[i].GetTeam() == PunTeams.Team.red)
            {
                GameObject player = (GameObject)Instantiate(koreaName); // 생성하고
                player.transform.SetParent(koreaScroll.transform, false); // 부모설정
                
                TeamPlayerID playerData = player.GetComponent<TeamPlayerID>(); // 팀정보 스크립트 참조
                playerData.playersID = PhotonNetwork.playerList[i].name;
                playerData.userIntID = PhotonNetwork.playerList[i].ID;
                playerData.DispPlayersID();

                koreaScroll.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 20);
            }
            else if (PhotonNetwork.playerList[i].GetTeam() == PunTeams.Team.blue)
            {
                GameObject player = (GameObject)Instantiate(chinaName);
                player.transform.SetParent(chinaScroll.transform, false);

                TeamPlayerID playerData = player.GetComponent<TeamPlayerID>();
                playerData.playersID = PhotonNetwork.playerList[i].name;
                playerData.userIntID = PhotonNetwork.playerList[i].ID;
                playerData.DispPlayersID();

                chinaScroll.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 20); // 20씩 띄워줌 간격
            }
            if(System.Convert.ToInt32(PhotonNetwork.playerList[i].GetTeam()) == 1) // 한국팀이라면
            {
                setRedTeam++;
            }
            else
            {
                setBlueTeam++;
            }
        }
    }
    public void StartButton() // 게임하기 버튼
    {
        int redteamBalance = setRedTeam + teamBalance; //팀 밸런스를 위한 부분
        int blueteamBalance = setBlueTeam + teamBalance;

        //↓↓↓ 상대팀 또는 자신의 팀 인원수가 +1 많다면 강제적으로 팀 이전
        if (blueteamBalance < setRedTeam)
        {
            PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
        }
        else if (redteamBalance < setBlueTeam)
        {
            PhotonNetwork.player.SetTeam(PunTeams.Team.red);
        }
        Renew();
        StartCoroutine("setField");
        myEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null); // 버튼을 누루고 게임을 시작했을시 버튼이 클릭되어있어 엔터 클릭시 무한 생성 막기 위한 부분
    }
    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString()); //화면 좌측 상단에 접속 과정에 대한 로그 출력
    }
    IEnumerator setField() // 밸런스로 인해서 팀이 강제로 바뀌었을때 바뀐것을 보여주기 위한 코루틴함수
    {
        yield return new WaitForSeconds(1.0f);
        roomLobby.SetActive(false);
        GetComponent<GameMgr>().enabled = true;
    }
}

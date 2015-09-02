using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

// 게임 전반적인걸 컨트롤하고 , 채팅 하는 스크립트

public class GameMgr : MonoBehaviour {

    private PhotonView pv; //RPC 호출을 위한 것 
	public Canvas onOffcanvas; //게임 나가기 Canvas
	public Canvas kDcanvas; // 킬데스 확인 Canvas
    public TankPosition tankPosition;

	public Text people; // 현재 룸 접속자수
	public Text logMsg; // 접속 로그를 표시할 변수
    public Text toralnumberKD; // 총 킬수
    public Text koreaKD; // 총킬수 한국
    public Text chinaKD; // 총킬수 중국

    public GameObject chatting; //채팅 입력하는 공간
    public RectTransform scrollContents; // 채팅 스크롤바
    public GameObject ROKscroll; // 한국팀 스크롤변수
    public GameObject ROK; // 한국팀 정보 출력변수
    public GameObject DPRKScroll; // 중국팀 스크롤 변수
    public GameObject DPRK; // 중국팀 정보 출력변수
    public Text outComeWin; // 이겼을시 Win 출력 변수
    public Text outComeLose; // 졌을시 Lose  출력변수
    public Text hpText; // HP 라고 쓰여진 텍스트 변수
    public Text nowHp; // 현재 체력을 보여주는 변수
    public Text esc; // ESC 버튼을 알려주는 변수
    public GameObject outComeButton; // 게임이 끝난 후 나가기 버튼
    public GameObject hpPanel;
    
    public int _addKill = 0; // 킬 누적
    int koreaTeamBack = 0; // 게임 중이던 플레이어가 나갔을시 그 킬이 팀 킬수에서 빠지는것을 방지하기 위한 변수
    int ChinaTeamBack = 0;
    int intKoreaKD = 0; // 팀별 킬
    int intChinaKD = 0;
    public static bool reset = false;

    public int playerKill = 0;
    public int playerDie = 0;
    
	void Awake()
	{
        pv = GetComponent<PhotonView>();
        PhotonNetwork.isMessageQueueRunning = true; //네트워크 메시지 수신을 다시 연결
        outComeLose = GameObject.Find("OutcomeLose").GetComponent<Text>();
        outComeWin = GameObject.Find("OutcomeWin").GetComponent<Text>();
        tankPosition = GetComponent<TankPosition>();
        reset = false;
        SetKD();
	}
	void Start()
	{
        Screen.lockCursor = true; // 마우스 잠그는 부분
        string msg = "\n<color=#00ff00>[" + PhotonNetwork.player.name + "] 게임에 참가하셨습니다</color>";
        pv.RPC("LogMsg", PhotonTargets.All, msg);
        scrollContents.sizeDelta += new Vector2(0, 50);
        toralnumberKD.text = PhotonNetwork.room.customProperties["aimKill"].ToString(); // 목표킬수 텍스트 출력하는 부분
        GetConnectPlayerCount(); // Player 수 세는 함수
        setCreateTank(); // 탱크 생성
	}
   
    void Update()
	{
        if(!reset)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) //ESC 눌렀을때
            {
                onOffcanvas.enabled = !onOffcanvas.enabled;
                if (onOffcanvas.enabled == true)
                {
                    Screen.lockCursor = false; //마우스 롹온
                }
                else
                {
                    Screen.lockCursor = true;
                }
                //Screen.lockCursor = !Screen.lockCursor; 
            }
            if (Input.GetKeyDown(KeyCode.Tab)) // Tab 눌렀을때
            {
                kDcanvas.enabled = !kDcanvas.enabled; //킬데스 상황판 끄고키는 부분
            }
        }
	}

	void GetConnectPlayerCount() // 현재 입장한 Player 수를 세는부분
	{
        Room currRoom = PhotonNetwork.room; //현재 입장한 룸 정보를 받아옴
        people.text = currRoom.playerCount.ToString() + "/" + currRoom.maxPlayers.ToString(); //현재 룸의 접속자 수와 최대 접속 가능한 수 출력
	}
    //void OnPhotonPlayerConnected(PhotonPlayer newPlayer) //네트워크 플레이어가 접속했을 때 호출되는 함수
    //{
    //    UserSetScore();    
    //}
    void OnPhotonPlayerDisconnected(PhotonPlayer outPlayer) //네트워크 플레이어가 룸을 나가거나 접속 끊어졌을대 호출 함수
	{
        //팅겼을때 출력되게 하는것
        string msg = "\n<color=#ff0000>[" + outPlayer.name + "] 게임 퇴장하셨습니다.</color>";
        LogMsg(msg);
        UserSetScore();
        GetConnectPlayerCount();
	}
    void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        UserSetScore();
        GetConnectPlayerCount();
    }
	public void onOFF()
	{
		onOffcanvas.enabled = false; // esc눌렀을시 나갈것인지 아닌지 UI 껏다 키는 부분
	}
	void setCreateTank() //플레이어 접속시 맨처음 탱크 생성 함수
	{
        tankPosition.completion = false;
        tankPosition.TankPositionChoice = 0;
        hpPanel.GetComponent<Image>().enabled = true; // UI 중 HP가 로비에서도 보이기 때문에 생성과 동시에 활성화시키는 부분
        hpText.enabled = true;
        nowHp.enabled = true;
        esc.enabled = true;
	}
	[RPC]void LogMsg(string Msg)
	{
        logMsg.text = logMsg.text + Msg; //로그 메시지 Text에 누적시켜 표시
        var height = logMsg.rectTransform.rect.height;
        scrollContents.sizeDelta = new Vector2(0, 20 + height);
	}
    public void OnClickExitRoom() //룸 나가기 버튼 클릭 이벤트에 연결된 함수
	{
        PhotonNetwork.LeaveRoom();
	}
    void OnLeftRoom() //룸에서 접속 종료되었을때 호출되는 콜백함수
	{
        Application.LoadLevel("Lobby"); //로비 씬 호출
	}
    public void SetKD() //킬데스 적용 함수
    {
        PhotonHashtable table = new PhotonHashtable(); // 변수만드는 부분
        table.Add("playerKill", playerKill);
        table.Add("playerDie", playerDie);
        PhotonNetwork.SetPlayerCustomProperties(table);
    }
    public void UserSetScore() 
    {
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Scroll").Length; i++)
        {
            Destroy(GameObject.FindGameObjectsWithTag("Scroll")[i]);
        }
        koreaTeamBack = intKoreaKD; // 현재 킬을 보관함
        ChinaTeamBack = intChinaKD;
        intKoreaKD = 0; // 현재 킬 초기화
        intChinaKD = 0;
        ROKscroll.GetComponent<RectTransform>().sizeDelta = Vector2.zero; //스크롤 영역 초기화
        DPRKScroll.GetComponent<RectTransform>().sizeDelta = Vector2.zero; //스크롤 영역 초기화

        for (int j = 0; j < PhotonNetwork.playerList.Length; j++)
        {
             if (PhotonNetwork.playerList[j].GetTeam() == PunTeams.Team.red)
             {
                GameObject player = (GameObject)Instantiate(ROK); //player  프리팹을 동적으로 생성
                player.transform.SetParent(ROKscroll.transform, false); //생성한 player 프리팹의 Parent를 지정

                PlayersScoreData playerData = player.GetComponent<PlayersScoreData>(); // 유저 데이터 스크립트를 참조받아서 적용
                playerData.playerID = PhotonNetwork.playerList[j].name;
                playerData.userIntID = PhotonNetwork.playerList[j].ID;
                
                playerData.kill = (int)PhotonNetwork.playerList[j].customProperties["playerKill"];
                playerData.die = (int)PhotonNetwork.playerList[j].customProperties["playerDie"];
                playerData.DispPlayerDate();
                ROKscroll.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 45);
                intKoreaKD += (int)PhotonNetwork.playerList[j].customProperties["playerKill"];
             }
            else if (PhotonNetwork.playerList[j].GetTeam() == PunTeams.Team.blue)
            {
                GameObject player = (GameObject)Instantiate(DPRK); //player 프리팹을 동적으로 생성  
                player.transform.SetParent(DPRKScroll.transform, false); //생성한 player 프리팹의 Parent를 지정

                PlayersScoreData playerData = player.GetComponent<PlayersScoreData>();
                playerData.playerID = PhotonNetwork.playerList[j].name;
                playerData.userIntID = PhotonNetwork.playerList[j].ID;

                playerData.kill = (int)PhotonNetwork.playerList[j].customProperties["playerKill"];
                playerData.die = (int)PhotonNetwork.playerList[j].customProperties["playerDie"];
                playerData.DispPlayerDate();
                DPRKScroll.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 45);
                intChinaKD += (int)PhotonNetwork.playerList[j].customProperties["playerKill"];
            }
        }
      
        if (koreaTeamBack > intKoreaKD) // 게임중이던 플레이어가 나갔을시 그 플레이어의 킬데스를 보관하기 위해서
        {
            int min = koreaTeamBack - intKoreaKD;
            intKoreaKD = intKoreaKD + min;
        }
        if (ChinaTeamBack > intChinaKD)
        {
            int min = ChinaTeamBack - intChinaKD;
            intChinaKD = intChinaKD + min;
        }
        koreaKD.text = intKoreaKD.ToString();
        chinaKD.text = intChinaKD.ToString();
        // 킬수와 목표킬수가 같아지면 게임 끝내는 부분
        if (int.Parse(toralnumberKD.text) <= intKoreaKD || int.Parse(toralnumberKD.text) <= intChinaKD)
        {
            if (intKoreaKD > intChinaKD)
            {
                if(PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
                {
                    outComeWin.enabled = true;
                }
                else
                {
                    outComeLose.enabled = true;
                }
            }
            else if (intKoreaKD < intChinaKD)
            {
                if(PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
                {
                    outComeWin.enabled = true;
                }
                else
                {
                    outComeLose.enabled = true;
                }
            }
            Screen.lockCursor = false;
            outComeButton.SetActive(true);
            GameEnd();
        }
    }

    void GameEnd()
    {
        PhotonNetwork.room.open = false; // 방목록에서 방이름이 안보이게 설정
        PhotonNetwork.room.visible = false; // 방 못들어오게 설정
        kDcanvas.enabled = true;
        reset = true;

        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < tanks.Length; i++)
        {
            tanks[i].GetComponent<TankDamage>().GameEnd();
        }
    }
}
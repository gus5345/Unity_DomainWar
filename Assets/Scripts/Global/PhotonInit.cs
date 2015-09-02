using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//포톤 클라우드에 접속하는 스크립트

public class PhotonInit : MonoBehaviour {

    public string version = "NSH"; // App 버전
	public InputField userId; // 유저가 아이디를 입력하는 변수
	public InputField roomName; // 유저가 방 이름을 입력하는 변수
	public GameObject ScrollContents; // 방생성시 위치잡아주는 변수
	public GameObject roomItem; // 방 생성 정보 출력 변수
    public Text nowID; // 현재 아이디를 보여주는 변수
    public static string userName; // 유저 아이디 저장 변수


    void Awake()
    {
		if (!PhotonNetwork.connected) // 기존에 접속하고 있는지 확인하는 곳
		{
			PhotonNetwork.ConnectUsingSettings(version);  //포톤 클라우드에 접속
        }
        nowID.text = GetUserId(); // 가지고있던 이름 다시 넣어주기
        Renew();
    }
    void OnJoinedLobby()  //포톤 클라우드에 정상적으로 접속한 후 로비에 입장하면 호출되는 콜백함수
    {
        Debug.Log("서버 정상 접속");
        nowID.text = GetUserId(); 
    }
    void Update()
    {
        string nowUserID = userId.text;
        if (!string.IsNullOrEmpty(nowUserID)) // 입력이 되어있지 않다면
        {
            nowID.text = userId.text;
        }
    }
	string GetUserId()
	{
        string playerID = userName;

        if (string.IsNullOrEmpty(playerID)) // 입력이 되어있지 않다면
		{
            playerID = "입력하세요" + Random.Range(0, 999);
		}
        return playerID;
	}
    public void UnityExit() // 끝내기 눌렀을시 종료시키는 함수
    {
        Application.Quit();
    }
    public void OncClickSetRoom() // 플레이어의 이름을 넘겨주면서, 방 설정씬으로 넘겨주는 함수
    {
        PhotonNetwork.player.name = nowID.text; //로컬 플레이어의 이름을 설정
        userName = nowID.text;
        PhotonNetwork.isMessageQueueRunning = false;
        Application.LoadLevel("CreateRoom");	//씬 로딩
    }
    void OnClickRoomItem(string roomName) // 선택한 방으로 입장시키는 함수
    {
        PhotonNetwork.player.name = nowID.text; //로컬 플레이어의 이름을 설정
        userName = nowID.text;
        PhotonNetwork.JoinRoom(roomName); //인자로 전달된 이름에 해당하는 룸으로 입장
    }
    void OnJoinedRoom()
    {
        PhotonNetwork.isMessageQueueRunning = false;
        Application.LoadLevel("Field");	//씬 로딩
    }
  
    void OnReceivedRoomListUpdate() //생성된 룸 목록이 변경됐을때 호출되는 콜백함수
	{
        Renew();
	}
    void Renew()
    {
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("ROOM").Length; i++)
        {
            Destroy(GameObject.FindGameObjectsWithTag("ROOM")[i]);
        }
 
        int rowCount = 0; //Grid Layout Group 컴포넌트의 Constraint Count 값을 증가시킬 변수
        ScrollContents.GetComponent<RectTransform>().sizeDelta = Vector2.zero; //스크롤 영역 초기화

        for (int j = 0; j < PhotonNetwork.GetRoomList().Length; j++)
        {
            GameObject room = (GameObject)Instantiate(roomItem); //RoomItem 프리팹을 동적으로 생성
            room.transform.SetParent(ScrollContents.transform, false); //생성한 RoomItem 프리팹의 Parent를 지정

            //생성한 RoomItem에 표시하기 위한 텍스트 정보 전달
            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = PhotonNetwork.GetRoomList()[j].name;
            roomData.connectPlayer = PhotonNetwork.GetRoomList()[j].playerCount;
            roomData.maxPlayers = PhotonNetwork.GetRoomList()[j].maxPlayers;
            roomData.DispRoomDate(); //텍스트 정보를 표시

            //RoomItem의 Button 컴포넌트에 클릭 이벤트를 동적으로 연결
            roomData.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomName); });
            //위 코드는 델리게이트로 구현한 것이 아니라 c# 2.0에 추가된 무명 메서드 방식이다.

            //Grid Layout Group 컴포넌트의 Constraint Count 값을 증가시킴
            ScrollContents.GetComponent<GridLayoutGroup>().constraintCount = ++rowCount;
            ScrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 45);
        }
    }
    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString()); //화면 좌측 상단에 접속 과정에 대한 로그 출력
    }
}
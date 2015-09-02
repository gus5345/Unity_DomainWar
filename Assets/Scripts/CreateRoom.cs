using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

//방 킬,데스,시간 등 설정하는 스크립트

public class CreateRoom : MonoBehaviour {

    public InputField roomName; // 방 이름 입력받는 변수
    public Text maxPlayers; // 방만들때 버튼 옆에 표시되는 플레이어 텍스트 변수
    public Text maxKill; // 방만들때 버튼 옆에 표시되는 킬 텍스트 변수
    public Button b1, b2, b3, b4, b5; // 인원수 버튼
    public Button bk1, bk2, bk3, bk4, bk5; // 킬수 버튼
    int aimKill = 0; // 총 킬수


    void Awake()
    {
        PhotonNetwork.isMessageQueueRunning = true;
        b1.onClick.AddListener(delegate { ButtonMaxPlayers(b1, 0); }); // 버튼 클릭 이벤트 ↓
        b2.onClick.AddListener(delegate { ButtonMaxPlayers(b2, 0); });
        b3.onClick.AddListener(delegate { ButtonMaxPlayers(b3, 0); });
        b4.onClick.AddListener(delegate { ButtonMaxPlayers(b4, 0); });
        b5.onClick.AddListener(delegate { ButtonMaxPlayers(b5, 0); });

        bk1.onClick.AddListener(delegate { ButtonMaxPlayers(bk1, 1); });
        bk2.onClick.AddListener(delegate { ButtonMaxPlayers(bk2, 1); });
        bk3.onClick.AddListener(delegate { ButtonMaxPlayers(bk3, 1); });
        bk4.onClick.AddListener(delegate { ButtonMaxPlayers(bk4, 1); });
        bk5.onClick.AddListener(delegate { ButtonMaxPlayers(bk5, 1); });
    }
 
    public void ButtonMaxPlayers(Button b,int type) // 타입을 주어서 킬과 인원수를 동시처리하는 함수로 만듬
    {
        if (type == 0) // 0 일때 플레이어 인원 수
        {
            maxPlayers.text = b.GetComponentInChildren<Text>().text;
        }
        else // 아닐때 총 킬수
        {
            maxKill.text = b.GetComponentInChildren<Text>().text;
        }
    }
    public void ClickCreateRoom()
    {
        string _roomName = roomName.text;
        if (string.IsNullOrEmpty(roomName.text)) //룸 이름이 입력되지 않았을 경우
        {
            _roomName = "입력하세요 " + Random.Range(0, 999); // 방 이름 중복을 막기위 해서
        }
        if (maxPlayers.text == "0") maxPlayers.text = "20"; // 버튼 입력없었을 경우 디폴트 값으로 만듬
        if (maxKill.text == "0") maxKill.text = "20"; // 버튼 입력 없었을 경우 디폴트 값으로 만듬

        aimKill = System.Convert.ToInt32(maxKill.text); // string 형을 int 형으로 변환

        PhotonNetwork.player.name = PhotonInit.userName;
        //PlayerPrefs.GetString("USER_ID"); //로컬 플레이어의 이름을 설정
        RoomOptions roomOptions = new RoomOptions() { isOpen = true, isVisible = true, maxPlayers = System.Convert.ToInt32(maxPlayers.text) }; //생성할 룸의 조건
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default); //지정한 조건에 맞는 룸생성
        
    }
    public void ClickExit()
    {
        Application.LoadLevel("Lobby");	//씬 로딩
    }
    void OnJoinedRoom()
    {
        PhotonHashtable table = new PhotonHashtable(); // 변수만드는 부분
        table.Add("aimKill", aimKill);
        PhotonNetwork.room.SetCustomProperties(table);

        PhotonNetwork.isMessageQueueRunning = false;
        PhotonNetwork.LoadLevel("Field");	//씬 로딩
    }
    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString()); //화면 좌측 상단에 접속 과정에 대한 로그 출력
    }
}

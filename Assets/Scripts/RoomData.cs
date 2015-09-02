using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 룸 정보를 출력해주는 스크립트

public class RoomData : MonoBehaviour {

	public string roomName = "";
	public int connectPlayer = 0;
	public int maxPlayers = 0;

	public Text textRoomName;
	public Text textConnectInfo;

	public void DispRoomDate()
	{
		textRoomName.text = roomName;
		textConnectInfo.text = "(" + connectPlayer.ToString () + "/" + maxPlayers.ToString () + ")"; // 방 입장할때 클릭하는 버튼의 출력되는 부분
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 팀 선택화면에서 이름을 노출시켜주는 스크립트

public class TeamPlayerID : MonoBehaviour {

    public Text playerID;
    public Text myID;
    public string playersID;
    public int userIntID;

    public void DispPlayersID()
    {
        if(PhotonNetwork.player.ID != userIntID)
        {
            playerID.text = playersID;
        }
        else
        {
            myID.color = Color.green;
            myID.text = PhotonNetwork.player.name;
        }
    }
}

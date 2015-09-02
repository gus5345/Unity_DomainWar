using UnityEngine;
using System.Collections;

// 미니맵의 삼각형 이미지가 상하로 꺽이는 걸 방지하는 스크립트
// 유저별 미니맵 색깔 지정

public class MinMapPoint : MonoBehaviour
{
    public Material blue; // 같은편
    public Material green; // 나 자신
    public Material red; // 상대편

    void Update()
    {
        transform.rotation = Quaternion.Euler(90, transform.parent.localEulerAngles.y, 0); // 다른 곳은 바뀌지않고 좌우로만 이동하게끔 하기 위한 곳
    }

    public void RoomLobbyCall()
    {
        if (GetComponent<PhotonView>().isMine) // 나 자신이라면
        {
            GetComponent<MeshRenderer>().material = green;
        }
        else
        {
            if (GetComponent<PhotonView>().owner.GetTeam() != PhotonNetwork.player.GetTeam()) // 나와 팀이 같지않다면
            {
                GetComponent<MeshRenderer>().material = red;
                GetComponent<MeshRenderer>().enabled = false;
            }
            else // if가 아니라면 
            {
                GetComponent<MeshRenderer>().material = blue;
            }
        }
    }
}

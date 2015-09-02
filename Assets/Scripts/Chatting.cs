using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Chatting : MonoBehaviour{

    private PhotonView pv; //RPC 호출을 위한 것 
    public GameObject chatting;
    public Text logMsg;
    public RectTransform scrollContents; // 채팅 스크롤바
    public GameObject myEventSystem;

	void Start () {
        pv = GetComponent<PhotonView>();
        string msg = "\n<color=#FF0084>[" + PhotonNetwork.player.name + "] 대기실에 참가하셨습니다</color>";
        pv.RPC("SendMsg", PhotonTargets.All, msg);
        myEventSystem = GameObject.Find("EventSystem");
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatting.GetActive())
            {
                InputField inputField = chatting.GetComponent<InputField>();
                if (string.IsNullOrEmpty(inputField.text))
                {
                    chatting.SetActive(false);
                    return;
                }
                string msg = "\n<color=#ffffff>" + "[" + PhotonNetwork.player.name + "]: " + inputField.text + "</color>";
                pv.RPC("SendMsg", PhotonTargets.All, msg);
                inputField.text = "";
                chatting.SetActive(false);
            }
            else
            {
                StartCoroutine(OpenChatting());
            }
        }
	}
    [RPC]void SendMsg(string Msg)
    {
        logMsg.text = logMsg.text + Msg; //로그 메시지 Text에 누적시켜 표시
        var height = logMsg.rectTransform.rect.height;
        scrollContents.sizeDelta = new Vector2(0, 20 + height);
    }
    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom(); //룸 나가기 버튼 클릭 이벤트에 연결된 함수
        Application.LoadLevel("Lobby"); //로비 씬 호출
        myEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
    IEnumerator OpenChatting() // 유니티 엔터키에 설정이 되어있어, 엔터 입력이 되지않아 처리한 부분
    {
        yield return new WaitForSeconds(0.01f);
        InputField inputField = chatting.GetComponent<InputField>();
        chatting.SetActive(true);
        inputField.ActivateInputField();
        inputField.Select();
    }
}

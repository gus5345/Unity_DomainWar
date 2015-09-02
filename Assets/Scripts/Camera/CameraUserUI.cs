using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraUserUI : MonoBehaviour {

    private PhotonView pv = null;  //Photon View 변수
    public Camera zoomCamera; // 줌카메라

    void Awake()
    {
        pv = GetComponent<PhotonView>(); 
    }

    void FixedUpdate()
    {
        if (pv.isMine)
        {
            //ScreenPointToRay(Input.mousePosition)
            Ray ray = zoomCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 10)); // 줌카메라 위치에서 정면으로
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.yellow); // 테스트를 위한 부분
                if (hit.transform.name == "KoreaTank(Clone)") // 그것이 탱크라면 
                {
                    hit.transform.GetComponentInChildren<Canvas>().enabled = true; // 이름표 보이게 
                    if (hit.transform.GetComponent<PhotonView>().owner.GetTeam() != PhotonNetwork.player.GetTeam()) // 나랑 팀이 다를시
                    {
                        hit.transform.GetComponentInChildren<Text>().color = Color.red; // 상대편 이름 색깔
                    }
                }
                else // 탱크가 아니라면 같은편인지 아닌지 찾아서 끈다.
                {
                    GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player"); // Player 라는 이름을 가진 태그를 찾는다.

                    for (int i = 0; i < tanks.Length; i++)
                    {
                        if (tanks[i].GetComponent<PhotonView>().owner.GetTeam() != PhotonNetwork.player.GetTeam())
                        {
                            tanks[i].GetComponentInChildren<Canvas>().enabled = false; //이름표 다시끄기
                        }
                    }
                }
            }
        }
    }
}

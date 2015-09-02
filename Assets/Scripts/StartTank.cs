using UnityEngine;
using System.Collections;

// 탱크 모델링 바꾸고 , 탱크의 초기생성 위치 중복체크를 하는 스크립트
// material 만 바꾸어 , 모델링을 달리 출력함

public class StartTank : MonoBehaviour {
    private PhotonView pv = null;
    public Material bodyMesh;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        //↓↓↓↓↓ 탱크 모델은 같되, material만 바꿔 사용, 팀별 바꾸는 부분
        MeshRenderer[] mats = GetComponentsInChildren<MeshRenderer>();
        if (pv.owner.GetTeam() == PunTeams.Team.red) // 한국팀일 경우 그대로
        {
        }
        else
        {
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i].name == "BodyMesh" || mats[i].name == "Turret" || mats[i].name == "Cannon") // 탱크의 Material만 바꾸는 부분
                {
                    mats[i].material = bodyMesh;
                }
            }
        }
        //for (int i = 0; i < mats.Length; i++)
        //{
        //    mats[i].enabled = false;
        //}
        //GetComponentInChildren<Canvas>().enabled = false;
    }
    //void Update() //↓↓↓ 탱크 생성후 자신 밑에 또 다른 탱크가있는지 중복체크하는 부분
    //{
    //    Ray ray = new Ray();
    //    Vector3 rayPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 4.5f);
    //    ray.origin = rayPosition;
    //    ray.direction = Vector3.down;

    //    RaycastHit[] hits = Physics.SphereCastAll(ray, 12.0f, 250.0f); // 구형으로 체크한다.

    //    int nearestId = -1;
    //    for(int i = 0; i < hits.Length; i++)
    //    {
    //        RaycastHit hit = hits[i];
    //        if (hit.collider == GetComponent<FireCannon>().turretCollider) continue;
    //        // 맨 처음 hit 찾기
    //        if (nearestId == -1)
    //        {
    //            nearestId = i;
    //        }
    //        else
    //        {
    //            float dist = hit.distance;
    //            if (dist >= hits[nearestId].distance) continue;
    //            nearestId = i;
    //        }
    //        if (nearestId != -1)
    //        {
    //            if (hits[nearestId].transform.name == "KoreaTank(Clone)") // 찾은 것이 탱크라면 재생성
    //            {
    //                if (pv.owner.GetTeam() == PunTeams.Team.red)
    //                {
    //                    float zpos = Random.Range(50.0f, 200.0f);
    //                    float xpos = Random.Range(1000.0f, 1080.0f);
    //                    Vector3 tankPos = new Vector3(xpos, 150, zpos);
    //                    transform.position = tankPos;
    //                }
    //                else
    //                {
    //                    float zpos = Random.Range(750.0f, 850.0f);
    //                    float xpos = Random.Range(100.0f, 180.0f);
    //                    Vector3 pos = new Vector3(xpos, 150, zpos);
    //                    Vector3 ro = new Vector3(0, 180, 0);
    //                    transform.position = pos;
    //                    transform.rotation = Quaternion.Euler(ro);
    //                }
    //            }
    //            else // 탱크가 아니라면 그곳으로 탱크 위치 이동
    //            {
    //                transform.position = hit.point;
    //                MeshRenderer[] mats = GetComponentsInChildren<MeshRenderer>();
    //                for (int j = 0; j < mats.Length; j++)
    //                {
    //                    mats[j].enabled = true;
    //                }
    //                pv.RPC("MinMapPlayer", PhotonTargets.All);
    //                GetComponentInChildren<Canvas>().enabled = true;
    //                GetComponent<AudioSource>().enabled = true;
    //                GetComponent<StartTank>().enabled = false; // 그후 스크립트 끄기
    //            }
    //        }
    //    }
    //}
    //[RPC] void MinMapPlayer()
    //{
    //    GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");

    //    for (int i = 0; i < tanks.Length; i++)
    //    {
    //        tanks[i].GetComponentInChildren<MinMapPoint>().RoomLobbyCall();
    //        //tanks[i].SendMessage("RoomLobbyCall");
    //    }
    //}
}

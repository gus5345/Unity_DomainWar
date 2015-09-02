using UnityEngine;
using System.Collections;

// 탱크 HP,이름,등을 표시하는 Canvas를 메인카메라의 맞게 돌려주는 스크립트

public class Billboard : MonoBehaviour{

    //private Transform mainCameraTr;
    //public GameObject Target;

	void Update () {
        transform.LookAt(Camera.main.transform);
	}
}

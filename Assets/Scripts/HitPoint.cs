using UnityEngine;
using System.Collections;

//탱크를 맞춘 탱크의 초기 위치를 표시하는 스크립트

public class HitPoint : MonoBehaviour {

    private float speed = 0.2f;
    private int drawdepth = -1000;
    private float now = 0.0f; //지금 상태는 밝은 상태의 화면
    private float fadedir = -1;

    public Texture hit;
    public Texture hitPoint;
    public bool start = false; //페이드 시작할 거 
    public float attackRotation;

    // 페이드 인이 되었다가 아웃될 시점을 카운트,  페이드 인/아웃이 자동으로 되는 것
    void Update()
    {
        if (now > 0.99f)
        {
            start = false;
        }
    }
    void OnGUI()
    {
        if (start == true)  //페이드 인 시작
        {
            now -= fadedir * speed * Time.deltaTime;
            now = Mathf.Clamp01(now);
        }
        else //false가 되면 아웃 시작 
        {   
            now += fadedir * speed * Time.deltaTime;
            now = Mathf.Clamp01(now);
        }
        
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, now); //알파값 컨트롤
        GUI.depth = drawdepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hit); //최종으로 화면에 텍스쳐를 보여줌
        GUIUtility.RotateAroundPivot(attackRotation, new Vector2(Screen.width / 2, Screen.height / 2));
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hitPoint); //최종으로 화면에 텍스쳐를 보여줌.
        
    }
}

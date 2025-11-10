using UnityEngine;

public class TeleportStraight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform teleportCircleUI;
    LineRenderer lr;
    //최초 텔레포트 ui의 크기
    Vector3 originScale = Vector3.one * 0.02f; // Vector3.one == new Vector3(1f,1f,1f)를 의미함.

    void Start()
    {
        teleportCircleUI.gameObject.SetActive(false);
        lr=GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update() //HandTrigger > button.one , ltouch > rtouch , // lr.enabled false 주석처리 해제해야 occulus 가능
    {
        if(ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger,ARAVRInput.Controller.RTouch))  //버튼을 누르는 순간, 한 프레임만 실행
        {
                lr.enabled = true;
        }
        else if(ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger,ARAVRInput.Controller.RTouch)) //버튼에서 손을 뗀 순간, 한 프레임만 실행
        {
                //lr.enabled = false;
                if(teleportCircleUI.gameObject.activeSelf)
                {
                    GetComponent<CharacterController>().enabled = false;
                    transform.position = teleportCircleUI.position + Vector3.up;
                    GetComponent<CharacterController>().enabled = true;
                }
                teleportCircleUI.gameObject.SetActive(false);
        }
        else if(ARAVRInput.Get(ARAVRInput.Button.HandTrigger,ARAVRInput.Controller.RTouch)) //버튼을 누르고 있는 동안 반복 실행
        {
            //텔레포트 ui 그리기
            Ray ray = new Ray(ARAVRInput.RHandPosition,ARAVRInput.RHandDirection);
            RaycastHit hitinfo;
            int layer = 1 << LayerMask.NameToLayer("Terrain"); //Terrain 레이어만을 레이캐스트 대상으로 설정합니다.
            if (Physics.Raycast(ray,out hitinfo, 200 , layer)) //거리 200까지 Terrain 레이어의 오브젝트와의 충돌을 감지합니다.
            {
                //부딪힌 지점에 텔레포트 ui 표시
                lr.SetPosition(0,ray.origin);
                lr.SetPosition(1,hitinfo.point);
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = hitinfo.point;
                teleportCircleUI.forward = hitinfo.normal;
                teleportCircleUI.localScale = originScale * Mathf.Max(1,hitinfo.distance);
            }
            else
            {
                lr.SetPosition(0,ray.origin);
                lr.SetPosition(1,ray.origin + ARAVRInput.RHandDirection * 200);
                teleportCircleUI.gameObject.SetActive(false);
            }
        }
        else
        {
            
        }
    }
}

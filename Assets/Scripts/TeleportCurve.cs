using UnityEngine;
using System.Collections.Generic;

public class TeleportCurve : MonoBehaviour
{
    public Transform teleportCircleUI;
    LineRenderer lr;
    Vector3 originScale = Vector3.one * 0.02f;
    //커브 전용 변수
    public int lineSmooth = 40;
    public float curveLength = 50;
    public float gravity = -60;
    public float simulateTime = 0.02f;
    List<Vector3> lines = new List<Vector3>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        teleportCircleUI.gameObject.SetActive(false);
        lr=GetComponent<LineRenderer>();
        lr.startWidth = 0.5f;
        lr.endWidth = 0.05f;
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
            MakeLines();
        }
    }
    void MakeLines()
    {
        lines.RemoveRange(0,lines.Count);
        Vector3 dir = ARAVRInput.RHandDirection * curveLength;
        Vector3 pos = ARAVRInput.RHandPosition;
        lines.Add(pos);

        for(int i=0; i < lineSmooth; i++)
        {
            Vector3 lastPos = pos;
            dir.y += gravity * simulateTime;
            pos += dir * simulateTime;
            if(CheckHitRay(lastPos,ref pos))
            {
                lines.Add(pos);
                break;
            }
            else
            {
                teleportCircleUI.gameObject.SetActive(false);
            }
            lines.Add(pos);
        }
        lr.positionCount = lines.Count;
        lr.SetPositions(lines.ToArray());
    }

    private bool CheckHitRay(Vector3 lastPos, ref Vector3 pos) //점과 점 사이에 ray를 생성하여, 그 사이에서 충돌을 감지하고 이를 통해서 실행을 마칠 지점을 찾는다.
    {
        Vector3 rayDir = pos - lastPos; 
        Ray ray = new Ray(lastPos,rayDir);
        RaycastHit hitinfo;

        if(Physics.Raycast(ray, out hitinfo, rayDir.magnitude))
        {
            pos = hitinfo.point;
            int layer = LayerMask.NameToLayer("Terrain");
            if(hitinfo.transform.gameObject.layer == layer)
            {
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = pos;
                teleportCircleUI.forward = hitinfo.normal;
                float distance = (pos - ARAVRInput.RHandPosition).magnitude;
                teleportCircleUI.localScale=originScale * Mathf.Max(1,distance);
            }
            return true;
        }
        return false;
    }
}  

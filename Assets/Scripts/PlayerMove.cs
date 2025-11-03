using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5;
    public float jumpPower = 5;

    //캐릭터컨트롤러 컴포넌트 타입 변수 선언
    CharacterController cc;

    //중력 가속도
    public float gravity = -2;
    float yVelocity = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //x,z축 입력값을 받아온다.
        float h = ARAVRInput.GetAxis("Horizontal");
        float v = ARAVRInput.GetAxis("Vertical");
        //방향을 설정한다(키 입력에 따라서 방향값이 프레임마다 설정된다)
        Vector3 dir = new Vector3(h,0,v);
        


        yVelocity +=gravity * Time.deltaTime;
        //바닥 아래로 떨어지는 경우를 차단
        if(cc.isGrounded)
        {
            yVelocity=0;
        }
        if(ARAVRInput.GetDown(ARAVRInput.Button.Two,ARAVRInput.Controller.RTouch))
        {
            yVelocity = jumpPower;
        }
        dir.y=yVelocity;
        //이동한다
        cc.Move(dir*speed*Time.deltaTime);
    }
}

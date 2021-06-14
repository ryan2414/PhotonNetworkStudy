using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 3.0f;

    public float rotSpeed = 200.0f;

    public GameObject cameraRig;

    public Transform myCharacter;

    public Animator anim;
    
    //서버에서 받은 데이터를 저장할 변수
    private Vector3 setPos;

    private Quaternion setRot;

    private float dir_speed = 0;

    public Text nameText;
    
    // Start is called before the first frame update
    void Start()
    {
        //사용자의 오브젝트일 때만 카메라 장치를 활성화한다.
        cameraRig.SetActive(photonView.IsMine);
        
        //각 접속자의 닉네임을 출력한다.
        nameText.text = photonView.Owner.NickName;
        
        //자신의 이름은 녹색, 다른 사람의 이름은 빨간색으로 출력한다.
        if (photonView.IsMine)
        {
            nameText.color = Color.green;
        }
        else
        {
            nameText.color = Color.red; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
    }

    //이동 기능
    private void Move()
    {
        if (photonView.IsMine)
        {
            //왼손 썸스틱의 방향 값을 가져와 캐릭터의 이동 방향을 정한다.
            Vector2 stickPos = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
            Vector3 dir = new Vector3(stickPos.x, 0, stickPos.y);
            dir.Normalize();
        
            //캐릭터의 이동 방향 벡터를 카메라가 바라보는 방향을 정면으로 하도록 변경한다.
            dir = cameraRig.transform.TransformDirection(dir);
            transform.position += dir * moveSpeed * Time.deltaTime;
        
            //만일, 왼손 썸스틱을 기울이면 그 방향으로 캐릭터를 회전시킨다.
            float magnitude = dir.magnitude;

            if (magnitude > 0)
            {
                myCharacter.rotation = Quaternion.LookRotation(dir);
            }
        
            //애니메이터 블랜드 트리 변수에 벡터의 크기를 전달한다.
            anim.SetFloat("Speed", magnitude); 
        }
        else
        {
            //전체 오브젝트의 위치 값과 캐릭터의 회전 값을 서버에서 전달받은 값으로 선형 보간해 동기화한다.
            transform.position = Vector3.Lerp(transform.position, setPos,Time.deltaTime * 20.0f);
            myCharacter.rotation = Quaternion.Lerp(myCharacter.rotation, setRot,Time.deltaTime * 20.0f);
            
            //서버에서 전달받은 값으로 애니메이터 파라미터 값을 동기화한다.
            anim.SetFloat("Speed", dir_speed);
        }
        
    }

    //회전 기능
    private void Rotate()
    {
        if (photonView.IsMine)
        {
            //오른손의 방향 값에서 좌우 기울기를 누적시킨다.
            float rotH = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).x;
        
            //CameraRig 오브젝트를 회전시킨다.
            cameraRig.transform.eulerAngles += new Vector3(0, rotH, 0) * rotSpeed * Time.deltaTime; 
        }
       
    }

    //데이터 동기화를 위한 데이터 전송 및 수신 기능
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //만일 데이터를 전송하는 상황이라면...
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(myCharacter.rotation);
            stream.SendNext(anim.GetFloat("Speed"));
        }
        //만일 데이터를 수신하는 상황이라면
        else if(stream.IsReading)
        {
            setPos = (Vector3) stream.ReceiveNext();
            setRot = (Quaternion) stream.ReceiveNext();
            dir_speed = (float) stream.ReceiveNext();
        }
    }
}

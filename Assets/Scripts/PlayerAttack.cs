using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviourPun
{
    public Animator anim;

    public float maxHP = 10;

    public float attackPower = 2;

    public Slider hpSlider;

    public BoxCollider weaponCol;

    private float curHP = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        //현재 체력을 최대 체력으로 채운다.
        curHP = maxHP;
        hpSlider.value = curHP / maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            //사용자 자신의 캐릭터일 때만,
            //자신의 오브젝트의 AttackAnimation 함수를 실행한다.
            if (photonView.IsMine)
            {
                photonView.RPC("AttackAnimation", RpcTarget.AllBuffered);
            }
        }
    }
    
    //공격 애니메이션 호출 함수 + RPC 애트리뷰트
    [PunRPC]
    public void AttackAnimation()
    {
        anim.SetTrigger("Attack");
    }
    
    //데미지 감소 처리 함수 + RPC 애트리뷰트
    [PunRPC]
    public void Damaged(float pow)
    {
        //0을 하한으로 해 현재 체력에서 공격력만큼을 감소시킨다.
        curHP = Mathf.Max(0, curHP - pow);
        
        //hp 슬라이더에 현재 체력 상태를 출력한다.
        hpSlider.value = curHP / maxHP;
    }

    private void OnTriggerEnter(Collider other)
    {
        //만일 자신의 캐릭터이면서 검에 닿은 대상의 이름이 Player라는 글자를 포함하고 있다면...
        if (photonView.IsMine && other.gameObject.name.Contains("Player"))
        {
            //무기에 닿은 대상의 포톤뷰에서 데미지 처리 함수를 RPC로 호출한다.
            PhotonView pv = other.GetComponent<PhotonView>();
            pv.RPC("Damaged",RpcTarget.AllBuffered,attackPower);
            
            //무기의 콜라이더를 비활성화한다.
            weaponCol.enabled = false;
        }
    }
}

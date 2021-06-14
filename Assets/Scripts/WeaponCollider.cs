using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public BoxCollider weaponCol;
    
    // Start is called before the first frame update
    void Start()
    {
        //무기의 충돌 영역을 비활성화한다.
        DeactivateCollider();
    }

    //콜라이더 활성화 함수
    void ActivateCollider()
    {
        weaponCol.enabled = true;
    }
    
    //콜라이더 비활성화 함수
    private void DeactivateCollider()
    {
        weaponCol.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

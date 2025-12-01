using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tower : MonoBehaviour
{
    public Transform damageUI;
    public Image damageImage;
    public int initialHP=10;
    int _hp = 0;
    //
    public float damageTime = 0.1f;

    IEnumerator DamageEvent()
    {
        damageImage.enabled = true;
        yield return new WaitForSeconds(damageTime);
        damageImage.enabled = false;
    }

    public int HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
            StopAllCoroutines();
            StartCoroutine(DamageEvent());
            if(_hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damageImage.enabled = true;
        _hp = initialHP;
        float z = Camera.main.nearClipPlane + 0.01f;
        damageUI.parent = Camera.main.transform;
        damageUI.localPosition = new Vector3(0,0,z);
        damageUI.localRotation = Quaternion.identity;
        damageImage.enabled = false;
    }

    public static Tower Instance;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

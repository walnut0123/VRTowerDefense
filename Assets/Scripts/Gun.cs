using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletImpact;
    ParticleSystem bulletEffect;
    AudioSource bulletAudio;
    public Transform crosshair;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = bulletImpact.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        ARAVRInput.DrawCrosshair(crosshair);
        if(ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            bulletAudio.Stop();
            bulletAudio.Play();

            Ray ray = new Ray(ARAVRInput.RHandPosition,ARAVRInput.RHandDirection);
            RaycastHit hitinfo;
            int playerLayer = 1 << LayerMask.NameToLayer("Player");
            int towerLayer = 1 << LayerMask.NameToLayer("Tower");
            int layerMaker = playerLayer | towerLayer;
            if(Physics.Raycast(ray, out hitinfo, 200, ~layerMaker))
            {
                //총알 파편 효과 처리
                bulletEffect.Stop();
                bulletEffect.Play();
                bulletImpact.position = hitinfo.point;
                bulletImpact.forward = hitinfo.normal;
            }

        }
    }
}

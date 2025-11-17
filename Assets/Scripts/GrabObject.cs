using UnityEngine;

public class GrabObject : MonoBehaviour
{
    bool isGrabbing = false;
    GameObject grabbedObject;
    public LayerMask grabbedLayer;
    public float grabRange = 1.0f;
    Vector3 prevPos;
    float throwPower = 500.0f;
    Quaternion prevRot; // rotation + movement, there has a singular phenomenon(problem) in third dimension when there is rotation/move event
    public float rotPower = 5;

    public bool isRemoteGrab = true;
    public float remoteGrabDistance = 20;

    void Start()
    {
        
    }


    void Update()
    {
        if(isGrabbing == false) //if not grabbing something
        {
            TryGrab(); //try grab
        }
        else
        {
            TryUngrab();
        }
    }

    private void TryGrab()
    {
        if(isRemoteGrab)
        {
            Ray ray = new Ray(ARAVRInput.RHandPosition,ARAVRInput.RHandDirection);
            RaycastHit hitinfo;
            if(Physics.SphereCast(ray, 0.5f, out hitinfo, remoteGrabDistance))
            {
                isGrabbing = true;
                grabbedObject = hitinfo.transform.gameObject;
                //StartCoroutine(GrabbingAnination());
            }
        }
        if(ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger,ARAVRInput.Controller.RTouch)) //if occulus handtrigger button is pressed, right click
        {
            //search and grab the bomb
            Collider[] hitObjects = Physics.OverlapSphere(ARAVRInput.RHandPosition,grabRange,grabbedLayer); //check if there is a object in the sphere surrounded by [grabRange]
            int closest = -1; //define default object index
            float closestDistance = float.MaxValue;
            for(int i =0;i < hitObjects.Length; i++)
            {
                var rigid = hitObjects[i].GetComponent<Rigidbody>();
                if(rigid == null) continue;

                Vector3 nextPos = hitObjects[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos,ARAVRInput.RHandPosition);
                if(nextDistance < closestDistance)
                {
                    closest = i; //change object index
                    closestDistance = nextDistance;
                }
            }

            if(closest>-1) //if there was change in object index
            {
                isGrabbing = true; 
                grabbedObject = hitObjects[closest].gameObject;
                grabbedObject.transform.parent = ARAVRInput.RHand;

                grabbedObject.GetComponent<Rigidbody>().isKinematic = true; // active the collisions, forces

                prevPos = ARAVRInput.RHandPosition; //update the position of grabbed bomb
                prevRot = ARAVRInput.RHand.rotation; //update the rotation of graabed bomb
            }

        }
    }
    private void TryUngrab()
    {
        Vector3 throwDirection = (ARAVRInput.RHandPosition - prevPos);
        prevPos = ARAVRInput.RHandPosition;

        //angle 1 = Q1, angle = Q2
        //angle 1 + angle 2 = Q1 * Q2
        //
        Quaternion deltaRotation = ARAVRInput.RHand.rotation * Quaternion.Inverse(prevRot);
        prevRot = ARAVRInput.RHand.rotation;

        if(ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger,ARAVRInput.Controller.RTouch))
        {
            isGrabbing = false;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false; //inactive the collisions, forces
            grabbedObject.transform.parent = null;
            grabbedObject.GetComponent<Rigidbody>().linearVelocity = throwDirection * throwPower; //set the action of throw with direction & power
            //simulate the rotation of throwed bomb
            float angle;
            Vector3 axis;
            deltaRotation.ToAngleAxis(out angle, out axis);
            Vector3 angularVelocity = (1.0f / Time.deltaTime) * angle * axis ;
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = angularVelocity;

            grabbedObject = null;
        }
    }
}

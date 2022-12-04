using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{

    #region Variables [private]

    [SerializeField]
    private float distanceAway;
    [SerializeField]
    private float distanceUp;
    [SerializeField]
    private Transform follow;
    [SerializeField]
    private float smooth;
    [SerializeField]
    private Vector3 targetPosition;
    [SerializeField]
    private Vector3 offset = new Vector3(0.0f, 1.25f, 0.0f);

    private Vector3 lookDir;

    // smoothing and damping
    private float camSmoothDampTime = 0.1f;
    private Vector3 velocityCamSmooth = Vector3.zero;

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate() {

        Vector3 characterOffset = follow.position + new Vector3(0, distanceUp, 0);

        // calculate the direction from the camera to player
        lookDir = characterOffset - this.transform.position;
        lookDir.y = 0;
        lookDir.Normalize();

        //targetPosition = follow.position + Vector3.up * distanceUp - follow.forward * distanceAway;

        targetPosition = characterOffset + follow.up * distanceUp - lookDir * distanceAway;

        //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);
        smoothPosition(this.transform.position, targetPosition);

        transform.LookAt(follow);

    }

    private void smoothPosition(Vector3 fromPos, Vector3 toPos) {

        this.transform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCamSmooth, camSmoothDampTime);

    }
}

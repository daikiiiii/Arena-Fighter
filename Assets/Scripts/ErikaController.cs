using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErikaController : MonoBehaviour
{

    # region Variables [Private]

    [SerializeField]
    private Animator anim;
    [SerializeField]
    private float directionDampTime = 0.25f;
    [SerializeField]
    private float rotationDegreesPerSecond = 120.0f;

    private float direction = 0.0f;
    private float speed = 0.0f;
    private float horizontal = 0.0f;
    private float vertical = 0.0f;

    private AnimatorStateInfo stateInfo;

    private int m_LocomotionId = 0;

    # endregion

    // Start is called before the first frame update
    void Start()
    {
        
        anim = GetComponent<Animator>();
        m_LocomotionId = Animator.StringToHash("BaseLayer.Locomotion");

    }

    // Update is called once per frame
    void Update()
    {

        if (anim) {

            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (Input.GetMouseButtonDown(0)) {
                anim.SetTrigger("runningAttack");
            }

            // pull values from keyboard
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            direction = 0.0f;
            speed = new Vector2(horizontal, vertical).sqrMagnitude;

            anim.SetFloat("Speed", speed);
            anim.SetFloat("Direction", horizontal, directionDampTime, Time.deltaTime);

        }

    }

    void FixedUpdate() {

        if (IsInLocomotion() && ((direction >= 0 && horizontal >= 0) || (direction < 0 && horizontal < 0))) {

            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0.0f, rotationDegreesPerSecond * (horizontal < 0 ? -1.0f : 1.0f), 0.0f), Mathf.Abs(horizontal));
            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
            this.transform.rotation = this.transform.rotation * deltaRotation;

        }

    }

    public bool IsInLocomotion() {

        return stateInfo.nameHash == m_LocomotionId;

    }
}

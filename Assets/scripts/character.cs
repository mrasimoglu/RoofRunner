using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character : MonoBehaviour
{
    public AudioClip finishmusic;
    public int coin;
    public bool isGameDone;


    public HUD hudscript;
    public Animator animator;
    public Rigidbody rigidbody;
    public CapsuleCollider collider;

    float rotateMultiplier;
    
    public vaultObject aVaultObject;
    public ladder aLadder;
    public bool animPosition;

    public bool isGrounded;
    public bool isClimbing;
    public bool isHolding;

    public bool canHold;
    public bool canVault;
    public bool canSlide;
    public bool canClimb;
    public float distToGround;

    float speed;

    String currentanimationname;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();

        animator.SetBool("sprintMode", true);

        coin = 0;
    }

    void Update()
    {
        if (!isGameDone)
        {
            Movement();

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("falling"))
                if (isGrounded || distToGround < 0.75f)
                    animator.SetTrigger("landing");

            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            {
                distToGround = hit.distance;
            }
        }
    }

    private void LateUpdate()
    {
        if (!isGameDone)
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("idle"))
                GetComponent<AudioSource>().pitch = 0.8f;
            else if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("walk"))
                GetComponent<AudioSource>().pitch = 0.9f;
            else if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("run"))
                GetComponent<AudioSource>().pitch = 1f;

            if (aLadder && isClimbing)
            {
                foreach (var clipinfo in animator.GetCurrentAnimatorClipInfo(0))
                {
                    if (clipinfo.clip.name.Equals("climb_slide"))
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
                        {
                            if (aLadder.MoveDown())
                            {
                                animator.SetTrigger("climb_end");
                                animator.SetBool("climb_slide", false);
                                isClimbing = false;
                                rigidbody.isKinematic = false;
                            }
                        }

                        int index = aLadder.currentStep;

                        if (index > 0)
                        {
                            Vector3 lerped = Vector3.Lerp(aLadder.transform.TransformPoint(aLadder.steps[index]), aLadder.transform.TransformPoint(aLadder.steps[index - 1]), animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                            rigidbody.MovePosition(lerped);
                        }
                    }
                    else if (clipinfo.clip.name.Contains("climb"))
                    {
                        float len = clipinfo.clip.length;
                        float cur = len * Mathf.Min(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0.999f);

                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f)
                        {
                            Vector3 lerped;
                            if (aLadder.dir)
                                lerped = Vector3.Lerp(aLadder.transform.TransformPoint(aLadder.steps[aLadder.currentStep - 1]), aLadder.transform.TransformPoint(aLadder.steps[aLadder.currentStep]), cur / len);
                            else
                                lerped = Vector3.Lerp(aLadder.transform.TransformPoint(aLadder.steps[aLadder.currentStep + 1]), aLadder.transform.TransformPoint(aLadder.steps[aLadder.currentStep]), cur / len);
                            rigidbody.MovePosition(lerped);
                        }
                    }
                }
            }
            if (aVaultObject)
            {
                foreach (var clipinfo in animator.GetCurrentAnimatorClipInfo(0))
                {
                    if (clipinfo.clip.name.Contains("_vault"))
                    {
                        rigidbody.isKinematic = true;
                        canVault = false;

                        float len = clipinfo.clip.length;
                        float cur = len * Mathf.Min(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0.999f);

                        float gap = clipinfo.clip.length / (float)(aVaultObject.curve.Length - 1);
                        int index = (int)(cur / gap);

                        Vector3 lerped;
                        if (animPosition)
                            lerped = Vector3.Lerp(aVaultObject.transform.TransformPoint(aVaultObject.curve[aVaultObject.curve.Length - 1 - index]), aVaultObject.transform.TransformPoint(aVaultObject.curve[(aVaultObject.curve.Length - 1) - index - 1]), (cur % gap) / gap);
                        else
                            lerped = Vector3.Lerp(aVaultObject.transform.TransformPoint(aVaultObject.curve[index]), aVaultObject.transform.TransformPoint(aVaultObject.curve[index + 1]), (cur % gap) / gap);
                        rigidbody.MovePosition(lerped);

                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.05f)
                        {
                            animator.SetTrigger("goslide");
                            rigidbody.isKinematic = false;
                            animPosition = !animPosition;
                        }
                    }
                }

                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("pullup") && aLadder)
                    {
                        float len = animator.GetCurrentAnimatorStateInfo(0).length;
                        float cur = len * Mathf.Min(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0.999f);

                        Vector3 lerped = Vector3.Lerp(aLadder.transform.TransformPoint(aLadder.steps[aLadder.stepCount - 1]), aVaultObject.transform.TransformPoint(aLadder.steps[aLadder.stepCount - 2]), cur / len);
                        rigidbody.MovePosition(lerped);
                    }
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("pullup") && aVaultObject)
                    {
                        float len = animator.GetCurrentAnimatorStateInfo(0).length;
                        float cur = len * Mathf.Min(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0.999f);

                        Vector3 lerped = Vector3.Lerp(aVaultObject.transform.TransformPoint(aVaultObject.curve[0]), aVaultObject.transform.TransformPoint(aVaultObject.curve[1]), cur / len);
                        rigidbody.MovePosition(lerped);
                    }
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("ski"))
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                        {
                            float dist = Vector3.Distance(aVaultObject.transform.TransformPoint(aVaultObject.curve[0]), aVaultObject.transform.TransformPoint(aVaultObject.curve[1]));

                            Vector3 lerped = Vector3.Lerp(aVaultObject.transform.TransformPoint(aVaultObject.curve[1]), aVaultObject.transform.TransformPoint(aVaultObject.curve[0]), animator.GetCurrentAnimatorStateInfo(0).normalizedTime / dist);
                            rigidbody.MovePosition(lerped);
                            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime / dist >= 1)
                            {
                                animator.SetTrigger("goslide");
                                animator.SetBool("ski", false);
                                aVaultObject = null;
                                rigidbody.isKinematic = false;
                                Vector3 r = transform.rotation.eulerAngles;
                                r.x = 0;
                                rigidbody.MoveRotation(Quaternion.Euler(r));
                            }
                        }
                    }
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("slide_1"))
                    {
                        rigidbody.isKinematic = true;
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                        {
                            float dist = Vector3.Distance(aVaultObject.transform.TransformPoint(aVaultObject.curve[0]), aVaultObject.transform.TransformPoint(aVaultObject.curve[1]));

                            Vector3 lerped;
                            if (animPosition)
                                lerped = Vector3.Lerp(aVaultObject.transform.TransformPoint(aVaultObject.curve[1]), aVaultObject.transform.TransformPoint(aVaultObject.curve[0]), animator.GetCurrentAnimatorStateInfo(0).normalizedTime / dist);
                            else
                                lerped = Vector3.Lerp(aVaultObject.transform.TransformPoint(aVaultObject.curve[0]), aVaultObject.transform.TransformPoint(aVaultObject.curve[1]), animator.GetCurrentAnimatorStateInfo(0).normalizedTime / dist);
                            lerped.y = transform.position.y;
                            rigidbody.MovePosition(lerped);
                            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime / dist >= 1)
                            {
                                animator.SetTrigger("goslide");
                                rigidbody.isKinematic = false;
                                animPosition = !animPosition;
                            }
                        }
                        else
                        {
                            Vector3 pos;
                            if (animPosition)
                                pos = aVaultObject.transform.TransformPoint(aVaultObject.curve[1]);
                            else
                                pos = aVaultObject.transform.TransformPoint(aVaultObject.curve[0]);
                            pos.y = transform.position.y;
                            rigidbody.MovePosition(pos);
                        }

                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if(!isGameDone)
            AnimationKeys();        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "VaultObject" && !animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("vault"))
        {
            hudscript.setAction(ActionTypes.Vault);
            aVaultObject = other.GetComponent<vaultObject>();
            canVault = true;
            animPosition = (Vector3.Distance(transform.position, aVaultObject.transform.TransformPoint(aVaultObject.curve[0])) > Vector3.Distance(transform.position, aVaultObject.transform.TransformPoint(aVaultObject.curve[aVaultObject.curve.Length - 1])));
        }
        if (other.tag == "slideObject")
        {
            hudscript.setAction(ActionTypes.Slide);
            aVaultObject = other.GetComponent<vaultObject>();
            canSlide = true;
            animPosition = (Vector3.Distance(transform.position, aVaultObject.transform.TransformPoint(aVaultObject.curve[0])) > Vector3.Distance(transform.position, aVaultObject.transform.TransformPoint(aVaultObject.curve[aVaultObject.curve.Length - 1])));
        }
        if (other.tag == "ladder")
        {
            hudscript.setAction(ActionTypes.HoldLadder);
            aLadder = other.GetComponent<ladder>();
            canClimb = true;
        }
        if (other.tag == "ground")
        {
            
            isGrounded = true;
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("_jump") || animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("falling") || animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("pullup"))
                animator.SetTrigger("landing");
        }
        if(other.tag == "slopeGround")
        {
            
            isGrounded = true;
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("_jump") || animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("falling"))
                animator.SetTrigger("landing");

            if (Quaternion.Angle(other.transform.rotation, Quaternion.Euler(0, other.transform.rotation.eulerAngles.y, other.transform.rotation.eulerAngles.z)) > 15.0f)
            {
                aVaultObject = other.GetComponent<vaultObject>();
                if (Vector3.Distance(aVaultObject.transform.TransformPoint(aVaultObject.curve[1]), transform.position) < 5.0f)
                {
                    Vector3 apos = (aVaultObject.transform.position - transform.position), bpos = transform.forward;
                    apos.y = bpos.y = 0;
                    if (Vector3.Angle(apos, bpos) < 30 && Vector3.Angle(apos, bpos) > -30)
                    {
                        animator.SetBool("ski", true);
                        rigidbody.isKinematic = true;
                        Vector3 r = transform.rotation.eulerAngles;
                        r.x = aVaultObject.transform.rotation.eulerAngles.x;
                        rigidbody.MoveRotation(Quaternion.Euler(r));
                    }
                }
            }
        }
        if (other.tag == "holdable")
        {
            canHold = true;
            aVaultObject = other.GetComponent<vaultObject>();
        }
        if (other.tag == "slip")
        {
            animator.SetTrigger("slip");
        }
        if(other.tag == "coin")
        {
            Destroy(other.gameObject);
            hudscript.setCoinCount(++coin);
        }
        if(other.tag == "Finish")
        {
            finish ff = other.GetComponent<finish>();
            GetComponent<AudioSource>().clip = finishmusic;
            GetComponent<AudioSource>().Play();
            ff.animator.SetTrigger("open");
            isGameDone = true;
            Screen.lockCursor = false;
            hudscript.GameFinish();
        }
        if (other.tag == "deadzone")
        {
            hudscript.Dead();
            Screen.lockCursor = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "ground")
        {
            isGrounded = true;
        }
        if (other.tag == "slopeGround")
        {
            isGrounded = true;
        }
        if (other.tag == "informer")
            hudscript.setAction(ActionTypes.HoldWall);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "VaultObject")
        {
            hudscript.clearAction();
            canVault = false;
        }
        else if (other.tag == "slideObject")
        {
            hudscript.clearAction();
            canSlide = false;
        }
        else if (other.tag == "ladder")
        {
            hudscript.clearAction();
            canClimb = false;
        }
        else if (other.tag == "ground")
            isGrounded = false;
        else if (other.tag == "holdable")
        {
            hudscript.clearAction();
            canHold = false;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    private void OnCollisionExit(Collision collision)
    {
    }

    void AnimationKeys()
    {
        if (distToGround > 0.0f && distToGround < 0.9f && !isGrounded && rigidbody.velocity.y < 0)
            animator.SetTrigger("landing");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle_jump") || animator.GetCurrentAnimatorStateInfo(0).IsName("walk_jump") || animator.GetCurrentAnimatorStateInfo(0).IsName("run_jump"))
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f && isGrounded && rigidbody.velocity.y < 0)
                animator.SetTrigger("landing");

        if (!isHolding && !isGrounded && distToGround > 0.9f && !animator.GetCurrentAnimatorStateInfo(0).IsTag("vault"))
            animator.SetBool("falling", true);
        else
            animator.SetBool("falling", false);
         
        if (Input.GetKeyDown(KeyCode.LeftShift))
            animator.SetBool("sprintMode", !animator.GetBool("sprintMode"));

        if (canHold && !isHolding && Input.GetKey(KeyCode.F))
        {
            Vector3 apos = (aVaultObject.transform.position - transform.position), bpos = transform.forward;
            apos.y = bpos.y = 0;
            if (Vector3.Angle(apos, bpos) < 30)
            {
                animator.SetBool("holding", true);
                isHolding = true;
                rigidbody.isKinematic = true;
                rigidbody.MoveRotation(aVaultObject.transform.rotation);
            }
        }

        if (isHolding && animator.GetCurrentAnimatorStateInfo(0).IsName("holding"))
        {
            rigidbody.MovePosition(aVaultObject.transform.TransformPoint(aVaultObject.curve[0]));
            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("pullup", true);
                animator.SetBool("holding", false);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                animator.SetTrigger("pulldown");
                animator.SetBool("holding", false);
                isHolding = false;
                rigidbody.isKinematic = false;
            }

        }
        else if (isClimbing)
        {
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("climb"))
                rigidbody.MovePosition(aLadder.transform.TransformPoint(aLadder.steps[aLadder.currentStep]));

            if (Input.GetKey(KeyCode.W))
                if (aLadder.currentStep < aLadder.stepCount - 1)
                    animator.SetBool("moveForward", true);
                else
                {
                    animator.SetBool("pullup", true);
                }
            else
                animator.SetBool("moveForward", false);

            if (Input.GetKey(KeyCode.S))
                animator.SetBool("moveBackward", true);
            else
                animator.SetBool("moveBackward", false);

            if (Input.GetKey(KeyCode.Space))
            {
                animator.SetBool("climb_slide", true);
                aLadder.dir = false;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.W) && isGrounded)
                animator.SetBool("moveForward", true);
            else
                animator.SetBool("moveForward", false);

            if (Input.GetKey(KeyCode.A) && isGrounded)
                animator.SetBool("moveLeft", true);
            else
                animator.SetBool("moveLeft", false);

            if (Input.GetKey(KeyCode.S) && isGrounded)
                animator.SetBool("moveBackward", true);
            else
                animator.SetBool("moveBackward", false);

            if (Input.GetKey(KeyCode.D) && isGrounded)
                animator.SetBool("moveRight", true);
            else
                animator.SetBool("moveRight", false);

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                if (!canVault)
                    animator.SetBool("jump", true);
                else
                {
                    Vector3 apos = (aVaultObject.transform.position - transform.position), bpos = transform.forward;
                    apos.y = bpos.y = 0;
                    if (Vector3.Angle(apos, bpos) < 30)
                        animator.SetBool("vault", true);
                }

                if (animator.GetBool("crouch") == true)
                    animator.SetBool("crouch", false);
            }
            else
            {
                animator.SetBool("jump", false);
                animator.SetBool("vault", false);
            }

            rotateMultiplier = Input.GetAxisRaw("Mouse X");
            if (Input.GetAxis("Mouse X") < 0)
                animator.SetBool("turnLeft", true);
            else
                animator.SetBool("turnLeft", false);

            if (Input.GetAxis("Mouse X") > 0)
                animator.SetBool("turnRight", true);
            else
                animator.SetBool("turnRight", false);
            
            
            if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
            {
                if (aVaultObject && canSlide)
                {
                    Vector3 apos = (aVaultObject.transform.position - transform.position), bpos = transform.forward;
                    apos.y = bpos.y = 0;
                    if (Vector3.Angle(apos, bpos) < 30)
                    {
                        /*Vector3 p1 = aVaultObject.transform.TransformPoint(aVaultObject.curve[0]), p2 = aVaultObject.transform.TransformPoint(aVaultObject.curve[1]);
                        float rot;
                        if (Vector3.Distance(transform.position, p1) > Vector3.Distance(transform.position, p2))
                            rigidbody.MoveRotation(Quaternion.Euler(0f, -aVaultObject.transform.rotation.eulerAngles.y, 0f));
                        else
                            rigidbody.MoveRotation(Quaternion.Euler(0f, aVaultObject.transform.rotation.eulerAngles.y, 0f));*/

                        animator.SetBool("slide", true);
                    }
                }
            }
            else
            {
                animator.SetBool("slide", false);
            }
            if(Input.GetKey(KeyCode.C))
                animator.SetBool("crouch", !animator.GetBool("crouch"));

            if (Input.GetKey(KeyCode.F))
            {
                if (aLadder && canClimb)
                {
                    Vector3 apos = (aLadder.transform.position - transform.position), bpos = transform.forward;
                    apos.y = bpos.y = 0;
                    if (Vector3.Angle(apos, bpos) < 30 && Vector3.Angle(apos, bpos) > -30)
                    {
                        animator.SetTrigger("climb");
                        animator.SetBool("climbing", true);
                        isClimbing = true;
                        rigidbody.isKinematic = true;
                        animator.ResetTrigger("climb_end");
                        rigidbody.MoveRotation(aLadder.transform.rotation * Quaternion.Euler(new Vector3(0, 0, 0)));
                        rigidbody.MovePosition(aLadder.transform.TransformPoint(aLadder.steps[aLadder.Init(this.transform.position)]));
                    }
                }
            }
        }
    }
    void Movement()
    {
        rigidbody.MoveRotation(Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));

        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("climb") && isClimbing)
        {
                rigidbody.MoveRotation(aLadder.transform.rotation);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("climb_up_right") || animator.GetCurrentAnimatorStateInfo(0).IsName("climb_down_right"))
                animator.SetBool("climb_rl", true);
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("climb_up_left") || animator.GetCurrentAnimatorStateInfo(0).IsName("climb_down_left"))
                animator.SetBool("climb_rl", false);
        }
        else
        {
            rotateMultiplier = 2.5f;
            if (animator.GetBool("turnRight"))
                rigidbody.MoveRotation(rigidbody.rotation * Quaternion.EulerAngles(0, 0.01f * rotateMultiplier, 0));
            if (animator.GetBool("turnLeft"))
                rigidbody.MoveRotation(rigidbody.rotation * Quaternion.EulerAngles(0, -0.01f * rotateMultiplier, 0));

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("run"))
                rigidbody.MovePosition(transform.position + transform.forward * Time.deltaTime * 3);
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk"))
                rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime);
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk_left"))
            {
                rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk_right"))
            {
                rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("run_left"))
            {
                rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime * 3);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("run_right"))
            {
                rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime * 3);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk-backward"))
            {
                rigidbody.MovePosition(rigidbody.position - transform.forward * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk-left"))
            {
                rigidbody.MovePosition(rigidbody.position - transform.right * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk-right"))
            {
                rigidbody.MovePosition(rigidbody.position + transform.right * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle_jump"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
                    rigidbody.AddForce(new Vector3(0, 30 * Time.deltaTime *40, 0));
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk_jump"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
                    rigidbody.AddForce(new Vector3(0, 28f * Time.deltaTime * 40, 0));
                rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("run_jump"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
                    rigidbody.AddForce(new Vector3(0, 26f * Time.deltaTime * 40, 0));
                rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime * 3);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_walk"))
            {
                rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_walkbackward"))
            {
                rigidbody.MovePosition(rigidbody.position - transform.forward * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_walkright"))
            {
                rigidbody.MovePosition(rigidbody.position + transform.right * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_walkleft"))
            {
                rigidbody.MovePosition(rigidbody.position - transform.right * Time.deltaTime);
            }
        }
    }
}

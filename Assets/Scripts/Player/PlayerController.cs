using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 1;
    private Vector2 moveDirection;
    private Vector2 lastFacingDirection;
    private Animator anim;
    private void Start()
    {
        anim=GetComponent<Animator>();
    }
    void Update()
    {
        InputHandler();
        AnimateCharacter();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void InputHandler()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if((verticalInput==0&&horizontalInput==0)&&moveDirection.x!=0||moveDirection.y!=0)
        {
            lastFacingDirection = moveDirection;
        }

        moveDirection = new Vector2(horizontalInput, verticalInput).normalized;
    }
    private void Move()
    {
        transform.Translate(new Vector2(moveDirection.x, moveDirection.y) * speed * Time.deltaTime);
    }
    private void AnimateCharacter()
    {
        anim.SetFloat("AnimMoveDirX", moveDirection.x);
        anim.SetFloat("AnimMoveDirY", moveDirection.y);
        //for transition from idle to running
        anim.SetFloat("AnimMoveMag", moveDirection.magnitude);
        //for make player face the direction in which he was moving for idle
        anim.SetFloat("AnimFacingDirX", lastFacingDirection.x);
        anim.SetFloat("AnimFacingDirY", lastFacingDirection.y);
    }
}

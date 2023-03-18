using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Movement
    private Vector2 _move;
    private AudioSource _sfx;
    private GameObject heart1;
    private GameObject heart2;
    private GameObject heart3;
    private float _up;
    private int lives;
    private float _down;
    private float _horizontal;
    private Rigidbody2D _rb;
    private float _newSpeed;
    private float _newJumpPower;
    public AudioClip sfx_jump;
    public AudioClip sfx_die;
    public AudioClip sfx_hurt;
    public AudioClip sfx_hold;
    public AudioClip sfx_bonus;
    public AudioClip sfx_complete;

    private bool _isGrounded;
    private bool _isJumping;
    private float _jumpCounter;
    private Vector2 _vecGravity;
    
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float speed;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpTime;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private float crateSpeedPenalty;
    [SerializeField] private float crateJumpPenalty;
    
    //Grab
    [SerializeField] private Transform grabDetect;
    [SerializeField] private Transform boxHolder;
    [SerializeField] private float rayDist;
    [SerializeField] private float xThrow;
    [SerializeField] private float yThrow;

    // FSM
    [SerializeField] private Animator _anim;
    public enum State { idle, jumping, running, death };
    private State current_state = State.idle;

    public GameObject options_menu;

    private Boolean _holding;
    private GameObject _grabbedCrate;
    private Rigidbody2D _crateRb;

    //Interact
    private Boolean _interacting;
    private ActionActivator activator;
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Grounded = Animator.StringToHash("Grounded");

    // Start is called before the first frame update
    private void Start()
    {
        _vecGravity = new Vector2(0, -Physics2D.gravity.y);
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _sfx = GetComponent<AudioSource>();
        _holding = false;
        lives=2;
        heart1=GameObject.FindWithTag("Heart1");
        heart2=GameObject.FindWithTag("Heart2");
        heart3=GameObject.FindWithTag("Heart3");
        _interacting = false;
    }

    // Update is called once per frame
    private void Update()
    {
        Movement();
        Fsm();
        Grab();
        Interact();
        OptionsMenu();
        Fall();
    }

    //Movement of the player
    private void Movement()
    {
        _up = 0;
        _down = 0;
        if (Input.GetAxis("Vertical") > 0f) _up = 1;
        if (Input.GetAxis("Vertical") < 0f) _down = -1;
        if (Input.GetAxis("Jump") > 0f) _up = Input.GetAxis("Jump");
        _horizontal = Input.GetAxis("Horizontal");
        _move = new Vector2(_horizontal, 0);
        _anim.SetFloat(Running, Mathf.Abs(_horizontal));

        _newSpeed = speed;
        if (_holding && speed > crateSpeedPenalty)
        {
            _newSpeed -= crateSpeedPenalty;
        }
        transform.position += new Vector3(_move.x, 0, 0) * (_newSpeed * Time.deltaTime);
        
        transform.Translate(_move * (_newSpeed * Time.deltaTime));
        if (_up > 0.75 && IsGrounded())
        {
            _newJumpPower = jumpPower;
            if (_holding && _newJumpPower > crateJumpPenalty) _newJumpPower -= crateJumpPenalty;
            _rb.velocity = new Vector2(_rb.velocity.x, _newJumpPower);
            _isJumping = true;
            _jumpCounter = 0;
            _sfx.PlayOneShot(sfx_jump);
        }

        if (_rb.velocity.y > 0 && _isJumping)
        {
            _jumpCounter += Time.deltaTime;
            if (_jumpCounter > jumpTime) _isJumping = false;
            _rb.velocity += _vecGravity * (jumpMultiplier * Time.deltaTime);
            _anim.SetBool(Jumping, true);
        }

        if (_rb.velocity.y < 0)
        {
            _rb.velocity -= _vecGravity * (fallMultiplier * Time.deltaTime);
            _anim.SetBool(Jumping, false);
        }

        if (_up < 1)
        {
            _isJumping = false;
        }

        if (IsGrounded()) _anim.SetBool(Grounded, true); else _anim.SetBool(Grounded, false);
        Flip();
    }

    void Fsm()
    {
        Vector2 vel = _rb.velocity;
        if (current_state != State.death)
        {
            if (vel.magnitude == 0)
            {
                current_state = State.idle;
            }
            else if (_isJumping)
            {
                current_state = State.jumping;
            }
        }
        _anim.SetInteger("State", (int)current_state);
    }

    private void OptionsMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            options_menu.SetActive(!options_menu.active);
        }
    }

    private void Flip()
    {
        if (_move.x < -0.01f) transform.localScale = new Vector3(-1, 1, 1);
        if (_move.x > 0.01f) transform.localScale = new Vector3(1, 1, 1);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.5f), CapsuleDirection2D.Horizontal,0,groundLayer); //new Vector2(0.1f, -1f)
    }

    // ReSharper disable Unity.PerformanceAnalysis
    // Grab crates
    private void Grab()
    {
        RaycastHit2D grabCheck = Physics2D.Raycast(grabDetect.position, Vector2.right * transform.localScale, rayDist);
        RaycastHit2D HoldCheck = Physics2D.Raycast(boxHolder.position, Vector2.right * boxHolder.localScale, rayDist);
        
        //Drop
        if (Input.GetButtonDown("Fire2"))
        {
            if (!(grabCheck.collider is null))
            {
                if (!_holding && grabCheck.collider.CompareTag("Crate"))
                {
                    _grabbedCrate = grabCheck.collider.gameObject;
                    _grabbedCrate.transform.position = boxHolder.position;
                    _grabbedCrate.transform.SetParent(transform);
                    _crateRb = _grabbedCrate.GetComponent<Rigidbody2D>();
                    //_crateRb.isKinematic = true;
                    _holding = true;
                    _sfx.PlayOneShot(sfx_hold);
                }
            }
            else if (_holding)
            {
                //_grabbedCrate.GetComponent<Rigidbody2D>().isKinematic = false;
                _grabbedCrate.transform.localPosition = grabDetect.transform.localPosition + new Vector3(0.6f,0);
                _grabbedCrate.transform.SetParent(null);
                _grabbedCrate = null;
                _crateRb = null;
                _holding = false;
            }

        }
        
        //Throw
        if (Input.GetButtonDown("Fire1"))
        {
            if (!(grabCheck.collider is null))
            {
                if (!_holding && grabCheck.collider.CompareTag("Crate"))
                {
                    _grabbedCrate = grabCheck.collider.gameObject;
                    _grabbedCrate.transform.position = boxHolder.position;
                    _grabbedCrate.transform.SetParent(transform);
                    _crateRb = _grabbedCrate.GetComponent<Rigidbody2D>();

                    //_crateRb.isKinematic = true;
                    _holding = true;
                }
            }
            else if (_holding) {
                //_grabbedCrate.GetComponent<Rigidbody2D>().isKinematic = false;
                _grabbedCrate.transform.SetParent(null);
                _grabbedCrate = null;
                _crateRb.velocity = new Vector2(xThrow * transform.localScale.x, yThrow);
                _crateRb = null;
                _holding = false;
            }
        }

        if (_holding)
        {
            _crateRb.velocity = _rb.velocity;
            _grabbedCrate.transform.position = boxHolder.transform.position;
        }
    }
    
    //Interact with immobile objects like lever
    private void Interact()
    {
        RaycastHit2D grabCheck = Physics2D.Raycast(grabDetect.position, Vector2.right * transform.localScale, rayDist);
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!(grabCheck.collider is null) && grabCheck.collider.CompareTag("Interactible"))
            {
                _interacting = true;
                //grabCheck.collider.GetComponent<ActionActivator>().Trigger();
            }
        }
        else
        { 
            _interacting = false;
        }
    }

    public Boolean GetInteracting()
    {
        return _interacting;
    }
    
     private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Enemy")&&lives==0){
            
            current_state = State.death;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (collision.gameObject.CompareTag("Enemy")&&lives>0){
            lives-=1;
            if(lives==1){
                 _sfx.PlayOneShot(sfx_die);
                 heart3.SetActive(false);
            }
            if(lives==0){
                 _sfx.PlayOneShot(sfx_die);
                heart2.SetActive(false);
            }
        }
       
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Life")&&lives<2){
            Destroy(collision.gameObject);
           if(lives==0){
            heart2.SetActive(true);
           }
           if(lives==1){
            heart3.SetActive(true);
           }
           lives+=1;
           _sfx.PlayOneShot(sfx_bonus);
        }
    }
    private void Fall(){
        if (transform.position.y<-10){
            _sfx.PlayOneShot(sfx_die);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Die()
    {
        _sfx.PlayOneShot(sfx_hurt);
        Thread.Sleep(300);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

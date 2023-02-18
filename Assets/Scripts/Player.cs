using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity {
    private bool _isGrounded = true;
    private PlayerInputActions _playerInputActions;
    private Animator _anim;
    [SerializeField] private float _jumpForce;
    

    [Header("Items:")]
    [SerializeField] protected GameObject weapon;

    protected override void Awake() {
        rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Jump.performed += Jump;
        _playerInputActions.Player.Attack.performed += Attack;
    }

    public void Attack(InputAction.CallbackContext context) {
        weapon.GetComponent<Weapon>().Attack();
    }

    void Start() {
        // Start is called before the first frame update
        maxSpeed = 50.0f;
        speed = 10.0f;
    }

    // Update is called once per frame
    void Update() {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();

        bool canLeft = rb.velocity.x > -maxSpeed || inputVector.x > 0;
        bool canRight = rb.velocity.x < maxSpeed || inputVector.x < 0;

        if (canLeft || canRight) {
            rb.velocity = new Vector2(inputVector.x * speed, rb.velocity.y);
            
        }
        if (inputVector != Vector2.zero && _isGrounded) {
            _anim.PlayInFixedTime("TK_Walk_Anim");
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(mousePos.x < transform.position.x) {
            transform.eulerAngles = new Vector2(0, 180);
        } else {
            transform.eulerAngles = Vector2.zero;
        }
    }

    public void Jump(InputAction.CallbackContext context) {
        if (_isGrounded) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += Vector2.up * _jumpForce;
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Floor") {
            _isGrounded = true;
        } 
        if(collision.gameObject.tag == "Barricade") {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider) ;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if(collision.gameObject.tag == "Floor") {
            _isGrounded = false;
        }
    }


}
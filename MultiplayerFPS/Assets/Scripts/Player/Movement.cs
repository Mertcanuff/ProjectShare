using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
 
public class Movement : MonoBehaviourPunCallbacks
{
    public GunSystem[] allGuns;
    private int selectedGun;
    public Camera myCam;
    
    [SerializeField] Transform playerCamera;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    public float mouseSensitivity, mouseSensitivityAfterAim = 3.5f;
    public float mouseSensitivityInAim = 1f;
    [SerializeField] float Speed = 6.0f;
    [SerializeField] float runSpeed = 6.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;  
 
    public float jumpHeight = 6f;
    public float jumpPadHeight;
    float velocityY;
    bool isGrounded;
 
    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;
    
    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector3 velocity;
    public float sprintFOV;
    public float aimFOV;
    public float normalFOV;
    public float FOVChangeSpeed;

   

    private float verticalRotStore;

 
    void Start()
    {
        
            controller = GetComponent<CharacterController>();
            SwitchGun();
            if (cursorLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
            }

            normalFOV = myCam.fieldOfView;
            sprintFOV = myCam.fieldOfView += 45;
            
        

        //Transform newTrans = SpawnManager.instance.GetSpawnPoint();
        //transform.position = newTrans.position;
        //transform.rotation = newTrans.rotation;
    }
 
    void Update()
    {
        
             if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            selectedGun++;
            if(selectedGun >= allGuns.Length)
            {
                selectedGun = 0;
            }
            SwitchGun();
        }
            else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                selectedGun--;

                if(selectedGun < 0)
                {
                    selectedGun = allGuns.Length - 1;
                }
                SwitchGun();
            }

            for(int i = 0; i < allGuns.Length; i++)
            {
                if(Input.GetKeyDown((i + 1).ToString()))
                {
                    selectedGun = i;
                    SwitchGun();
                }
            }
       
    }
   

    public void SwitchGun()
    {
        foreach(GunSystem gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }

        allGuns[selectedGun].gameObject.SetActive(true);
    }

     void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "JumpPad")
        {
            Debug.Log("Niasnd");
            velocityY = Mathf.Sqrt(jumpPadHeight * -2f * gravity);
        }
    }
 
    void UpdateMouse()
    {
        
            Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        
            currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
        
            cameraCap -= currentMouseDelta.y * mouseSensitivity;
    
            cameraCap = Mathf.Clamp(cameraCap, -80.0f, 80.0f);
    
            playerCamera.localEulerAngles = Vector3.right * cameraCap;
    
            transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
        
        
    }

   
 
    void UpdateMove()
    {
        
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);
    
            Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            targetDir.Normalize();
    
            currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);
    
            velocityY += gravity * 2f * Time.deltaTime;
    
            Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;
    
            controller.Move(velocity * Time.deltaTime);
    
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
    
            if(isGrounded! && controller.velocity.y < -1f)
            {
                velocityY = -8f;
            }
            if(Input.GetKey(KeyCode.LeftShift))
            {
                Speed = runSpeed;
                myCam.fieldOfView = Mathf.Lerp(myCam.fieldOfView, sprintFOV, FOVChangeSpeed);
            }
            else
            {
                Speed = 12;
                myCam.fieldOfView = Mathf.Lerp(myCam.fieldOfView, normalFOV, FOVChangeSpeed);
            }
        
    }

 
}
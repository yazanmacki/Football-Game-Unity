using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBall : MonoBehaviour
{
    
    Vector3 v3Force;

    [SerializeField]
    KeyCode keyShoot;

    //[SerializeField]
    float power;

    [SerializeField]
    float yRotation;

    bool isShot;

    float ballX;
    float ballZ;
    float ballToGoal;
    float zDistBalltoGoal;
    float angle;
    float angleDeg;
    float camToGoal;
    float zDistCamtoGoal;
    float xDistCamtoGoal;
    Quaternion quaternion;
    Vector3 rotationVector;
    Vector3 impulse;

    GameObject ball;

    Transform camTransform;

    float rX;
    float rY;
    float rZ;

    public int touches;
    float lerpPct;
    bool goingUp; 
    float lerpPct2;
    bool goingUp2;
    bool increaseMultiplier; 
    string state; 
    int powerIndex; 
    int bodyPowerIndex;
    int tipPowerIndex;
    bool increasing; 
    public bool goal; 
    float startRotationY;
    float xAngleDiff;

    float rotationX;

    bool timerDone;

    bool restart;

    void initVariables() {
        touches = 0;
        lerpPct = 0.0f;
        touches = 0; 
        isShot = false;
        goingUp = true; 
        goingUp2 = true;  
        power = 0.2f;
        increaseMultiplier = false; 
        powerIndex = 1;
        increasing = true; 
        goal = false; 
        xAngleDiff = 0.0f;
        restart = false;        
        ball = GameObject.Find("Ball");
        timerDone = false;
    }

    void Start() {

        GameObject.Find("Arrow").GetComponent<MeshRenderer>().enabled = true;

        for (int i = 1; i <= 88; i++) {
            GameObject.Find("Arrow/Body/Part" + i).GetComponent<Collider>().enabled = false;
            GameObject.Find("Arrow/Body/Part" + i).GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        for (int i = 89; i <= 137; i++) {
            GameObject.Find("Arrow/Body/Tip/Part" + i).GetComponent<Collider>().enabled = false;
            GameObject.Find("Arrow/Body/Tip/Part" + i).GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            GameObject.Find("Arrow/Body/Tip/Part" + i).GetComponent<MeshRenderer>().enabled = false;
        }

        for (int i = 89; i <= 137; i++) {
            GameObject.Find("Arrow/Body/Tip/Part" + i).GetComponent<MeshRenderer>().enabled = false;
        }

        for (int i = 1; i <= 88; i++) {
            string stringPath = "PowerGradient/row-1-col-" + i ;
            Material mat = Resources.Load(stringPath, typeof(Material)) as Material;
            GameObject.Find("Arrow/Body/Part" + i).GetComponent<MeshRenderer>().material = mat;
        }

        for (int i = 89; i <= 137; i++) {
            string stringPath = "PowerGradient/row-1-col-" + i ;
            Material mat = Resources.Load(stringPath, typeof(Material)) as Material;
            GameObject.Find("Arrow/Body/Tip/Part" + i).GetComponent<MeshRenderer>().material = mat;
        }
        
        initVariables();


        Physics.gravity = new Vector3(0, -45.0F, 0);
        ballX = ball.GetComponent<Transform>().position.x;
        ballZ = ball.GetComponent<Transform>().position.z;
        //0 - ballX = center of goal , will become variable when arrow implemented
        //60 Y height variable with arrow
        // 80 - ballZ constant 
    }
 
    void resetBall() {

        initVariables();

        ballX = Random.Range(-80.0f, 80.0f);
        ballZ = Random.Range(-20.0f, 50.0f);
        ball.GetComponent<Transform>().position = new Vector3(ballX, 0.5f, ballZ);
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0.0f,0.0f,0.0f);
        ball.GetComponent<Rigidbody>().angularVelocity = new Vector3(0.0f,0.0f,0.0f);

        ballToGoal = Mathf.Pow( Mathf.Pow( ballX - 0 , 2) + Mathf.Pow( ballZ - 80 , 2) , 0.5f );  //dist between ball and goal 
        zDistBalltoGoal = 80.0f - ballZ;
        angle = Mathf.Acos(zDistBalltoGoal/ballToGoal);
        angleDeg = angle * Mathf.Rad2Deg;

        camToGoal = ballToGoal + 30.0f;
        zDistCamtoGoal = Mathf.Cos(angle) * camToGoal;
        xDistCamtoGoal = Mathf.Sin(angle) * camToGoal;

        if (ballX < 0.0f) {
            xDistCamtoGoal = - xDistCamtoGoal;
        }
        if (ballX > 0.0f) {
           angleDeg = - angleDeg;
        }


        camTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
        camTransform.position = new Vector3(xDistCamtoGoal, 20.0f, 80.0f - zDistCamtoGoal);
        quaternion = Quaternion.Euler(10.0f, angleDeg, 0.0f);
        camTransform.rotation = quaternion;
         

        GameObject.Find("Arrow/Body").GetComponent<Renderer>().enabled = true;
            
        for (int i = 89; i <= 137; i++) {
            GameObject.Find("Arrow/Body/Tip/Part" + i + " (1)").GetComponent<MeshRenderer>().enabled = true;
        }

        GameObject.Find("Arrow").GetComponent<Transform>().position = new Vector3(ballX, 0.5f, ballZ);
        GameObject.Find("Arrow").GetComponent<Transform>().rotation = Quaternion.Euler(-30.0f, angleDeg, 0.0f);
        
        //rotationVector.y = rotationVector.y * 10;

        startRotationY = GameObject.Find("Arrow").GetComponent<Transform>().localEulerAngles.y;

        //GameObject.Find("Main Camera").GetComponent<Transform>().Rotate(0.0f, angle, 0.0f);
        isShot = false;

        GameObject.Find("Trail").GetComponent<TrailRenderer>().enabled = false;
    }

    void FixedUpdate()
    { 
        
        if (state == "rotateUpDown") {  
            if (goingUp) {
                lerpPct += 0.04f;
            }
            if (!goingUp) {
                lerpPct -= 0.04f;
            }
            if (lerpPct >= 1.0f) {
                goingUp = false;
            }
            if (lerpPct <= 0.0f) {
                goingUp = true;
            }
        }

        if (touches == 3) {  
            if (goingUp2) {
                lerpPct2 += 0.04f;
            }
            if (!goingUp2) {
                lerpPct2 -= 0.04f;
            }
            if (lerpPct2 >= 1.0f) {
                goingUp2 = false;
            }
            if (lerpPct2 <= 0.0f) {
                goingUp2 = true;
            }
        }

        if (increaseMultiplier) {
            //Debug.Log("Increasing = " + increasing);
            if (increasing) {
                powerIndex += 5;
                power += 0.025f;
            }
            if (!increasing) {
                powerIndex -= 5;
                power -= 0.025f;
                
                
            }
            
            if (powerIndex > 133) {
                increasing = false;
            }
            if (powerIndex <= 20) {
                increasing = true;
            }
        }

    }

    void setRestart() {
        restart = true;
    }

    void setTimer() { 
        timerDone = true;
    }

    void Update() {
        
        //Debug.Log("powerIndex = " + powerIndex);
        //Debug.Log("increasing = " + increasing);

        if (restart) {
            resetBall();
        }

        if (ball.GetComponent<Transform>().position.z > 120.0f) {
            if (!restart) {
                setRestart();
                CancelInvoke("setRestart");
            }
        }

        bodyPowerIndex = powerIndex;
        if (powerIndex > 88) {
            bodyPowerIndex = 88;
        }

        if (powerIndex > 88) {
            tipPowerIndex = powerIndex;
        } else {
            tipPowerIndex = 89;
        }

        if (!isShot) {
            for (int i = 1; i <= bodyPowerIndex; i++) {
                GameObject.Find("Arrow/Body/Part" + i).GetComponent<MeshRenderer>().enabled = true;
            }

            for (int i = bodyPowerIndex; i < 88; i++) {
                GameObject.Find("Arrow/Body/Part" + i).GetComponent<MeshRenderer>().enabled = false;
            } 

            if (powerIndex < 89) {
                GameObject.Find("Arrow/Body/Part88").GetComponent<MeshRenderer>().enabled = false;
            }
    

            for (int i = 89; i <= tipPowerIndex; i++) {
                GameObject.Find("Arrow/Body/Tip/Part" + i).GetComponent<MeshRenderer>().enabled = true;
            }

            for (int i = tipPowerIndex; i < 137; i++) {
                GameObject.Find("Arrow/Body/Tip/Part" + i).GetComponent<MeshRenderer>().enabled = false;
            } 
        }

        if (Input.touchCount > 0) 
        {
            Touch touch = Input.GetTouch(0);      
            if (touch.phase == TouchPhase.Began) {
                touches += 1;
            }
        }

        if (touches == 0) { 
            state = "rotateRightLeft"; 
        }

        if (touches == 1) {
            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended) {
                    state = "rotateUpDown";
                }
            }  
        }

        if (touches == 2) {
            state = "stopArrow";
        }

        if (touches == 3) {
            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) {
                    state = "powerArrow";
                }
                if (touch.phase == TouchPhase.Ended) {
                    state = "shootBall";
                }
            }  
        }
 

        if (state == "rotateRightLeft") {    
            Quaternion from = Quaternion.Euler(new Vector3(0.0F, startRotationY + 45.0F, 0.0F));
            Quaternion to = Quaternion.Euler(new Vector3(0.0F, startRotationY-45.0F, 0.0F));

            float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * 1.0F));
            GameObject.Find("Arrow").GetComponent<Transform>().localRotation = Quaternion.Lerp(from, to, lerp);  

            rY = GameObject.Find("Arrow").GetComponent<Transform>().localEulerAngles.y - 360;  
            if (Mathf.Abs(rY) > 300) {
                rY += 360;
            } 
             
            if (Mathf.Abs(rY - angleDeg) > 300) {
                //Debug.Log(rY - angleDeg + 360);
                xAngleDiff = (rY - angleDeg + 360);
            } else {
                //Debug.Log(rY - angleDeg); 
                xAngleDiff = (rY - angleDeg);
            }

        }
 
        if (state == "rotateUpDown") {
            rY = GameObject.Find("Arrow").GetComponent<Transform>().localEulerAngles.y - 360;  
            if (Mathf.Abs(rY) > 300) {
                rY += 360;
            }
             
            Quaternion from2 = Quaternion.Euler(new Vector3(0.0F, rY, 0.0F));
            Quaternion to2 = Quaternion.Euler(new Vector3(-65.0F, rY, 0.0F));
          
            
            GameObject.Find("Arrow").GetComponent<Transform>().localRotation = Quaternion.Lerp(from2, to2, lerpPct);  
        }
 
        if (state == "powerArrow") { 
                
            increaseMultiplier = true;      

            
            float rotationY; 

            rotationX = GameObject.Find("Arrow").GetComponent<Transform>().localEulerAngles.x - 360; 

            rotationY = GameObject.Find("Arrow").GetComponent<Transform>().localEulerAngles.y - 360;  
            if (Mathf.Abs(rotationY) > 300) {
                rotationY += 360;
            }
             
            
            if (rotationX > 0) {
                rotationX = Mathf.Pow(rotationX,1.05f);
            } else {
                rotationX = - Mathf.Pow(Mathf.Abs(rotationX),1.05f);
            }

            //rotationVector = new Vector3(rotationY, rotationX, 100 * Mathf.Pow(shotPowerMultiplier,0.2f) );  
            //Debug.Log(rotationVector);


            //Debug.Log("ang " + (xAngleDiff+angleDeg));
                
            //Debug.Log("ROTATIONX = " + rotationX);
            
           


            //Debug.Log("Power = " + power);
           
            //rotationVector.y = rotationVector.y * power;
            //rotationVector.z = rotationVector.z * power;

            //rotationVector.x = rotationY;

        }

        if (state == "shootBall") {
            if (!isShot) {
                increaseMultiplier = false;

                //Debug.Log("ROTATIONX = " + rotationX);
                
                if (Mathf.Abs(angleDeg+xAngleDiff) > 90) {
                    rotationVector = new Vector3(-(80-ballZ)*(Mathf.Tan((angleDeg+xAngleDiff)*Mathf.Deg2Rad)), -(80-ballZ)*(Mathf.Tan(Mathf.Abs(rotationX*Mathf.Deg2Rad))), ballZ-80); 
                } else {
                    rotationVector = new Vector3((80-ballZ)*(Mathf.Tan((angleDeg+xAngleDiff)*Mathf.Deg2Rad)), (80-ballZ)*(Mathf.Tan(Mathf.Abs(rotationX*Mathf.Deg2Rad))), 80-ballZ); 
                }
                
                rotationVector = rotationVector.normalized * powerIndex * 0.65f;

                impulse = (rotationVector); 
                ball.GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
                isShot = true;

                if (!restart) {
                    Invoke("setRestart", 5);
                }

                state = " ";
                
                Debug.Log("powerIndex = " + powerIndex);
                GameObject.Find("Arrow/Body").GetComponent<Renderer>().enabled = false;

                GameObject.Find("Trail").GetComponent<TrailRenderer>().enabled = true;
            
                for (int i = 89; i <= 137; i++) {
                    GameObject.Find("Arrow/Body/Tip/Part" + i + " (1)").GetComponent<MeshRenderer>().enabled = false;
                }

                for (int i = 89; i <= 137; i++) {
                    GameObject.Find("Arrow/Body/Tip/Part" + i).GetComponent<MeshRenderer>().enabled = false;
                }

                for (int i = 1; i <= 88; i++) {
                    GameObject.Find("Arrow/Body/Part" + i).GetComponent<MeshRenderer>().enabled = false;
                }

                GameObject.Find("Arrow").GetComponent<MeshRenderer>().enabled = false;

            }
        }
 
    }

}

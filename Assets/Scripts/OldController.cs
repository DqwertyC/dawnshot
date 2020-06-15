using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class OldController : MonoBehaviour
{

    public Vector2 directionalInput;

    public float baseJumpHeight = 6;
    public float baseTerminalVelocity = 30.0f;
    public float searchRadius = 8.0f;
    public float baseMass = 5000.0f;
    public float baseMoveSpeed = 5;

    public float maxFeruStorage = 65536;
    public float feruStored = 32000;

    public GameObject pointer;

    public GameObject coinTemplate;
    public MetalReserves reserves;
    public TextMeshProUGUI coinDisplay;
    public TextMeshProUGUI vialDisplay;

    public Slider healthSlider;
    public Slider feruchemySlider;
    public DiscreteSlider feruLevelSlider;

    public int coinCount;
    public int vialCount;

    public int feruchemyLevel;
    public int feruchemyRate;
    public float mass;

    float terminalVelocity;
    float gravity;
    float jumpForce;
    float jumpHeight;

    public Vector2 velocity;

    public Controller2D controller;

    Vector2 aimNormalized;
    bool spawnCoin;
    bool drinkVial;
    bool canPush;
    bool canGrab;
    bool jumping;
    bool pushing;

    MetalObject targetedObject;

    int wallDirX;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        pushing = false;
        mass = baseMass;
        gravity = 15;

        jumpForce = CalculateJumpForce(baseMass, baseJumpHeight);
        terminalVelocity = baseTerminalVelocity;


        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        feruchemyLevel = 0;
        feruchemyRate = 0;

        feruchemySlider.maxValue = maxFeruStorage;
        feruchemySlider.value = feruStored;
    }

    void Update()
    {
        if (ProjectVars.GAME_PAUSED || ProjectVars.IN_DIALOG) return;

        CalculateVelocity();
        controller.Move(velocity / Time.deltaTime, directionalInput);
        if (controller.collisions.below) velocity.y = 0;

        if (spawnCoin && coinCount > 0)
        {
            Vector2 spawnpoint = transform.position;
            spawnpoint += aimNormalized;

            GameObject newCoin = Instantiate(coinTemplate, spawnpoint, Quaternion.identity);
            newCoin.GetComponent<Rigidbody2D>().velocity = (velocity) + 2 * aimNormalized;

            coinCount--;
        }

        if (drinkVial && vialCount > 0)
        {
            reserves.AddBead();
            reserves.AddBead();
            reserves.AddBead();
            reserves.AddBead();

            vialCount--;
        }

        if (canGrab)
        {
            Vector2 size = new Vector2(2, 2);
            Collider2D[] touching = Physics2D.OverlapBoxAll(transform.position, size, 0.0f);

            for (int i = 0; i < touching.Length; i++)
            {
                if (touching[i].tag.Equals("Coin"))
                {
                    Destroy(touching[i].gameObject);
                    coinCount++;
                }
                else if (touching[i].tag.Equals("Vial"))
                {
                    Destroy(touching[i].gameObject);
                    vialCount++;
                }
            }
        }

        feruStored += feruchemyRate;

        if (feruStored >= maxFeruStorage && feruchemyRate > 0)
        {
            SetFeruchemyLevel(0);
            feruStored = maxFeruStorage;
        }

        if (feruStored <= 0 && feruchemyRate < 0)
        {
            SetFeruchemyLevel(0);
            feruStored = 0;
        }

        //Update HUD Elements
        coinDisplay.SetText(coinCount + "×");
        vialDisplay.SetText(vialCount + "×");
        feruchemySlider.value = feruStored;
    }

    void LateUpdate()
    {
        if (reserves.reserveLevel > 0)
        {
            DetectMetalInRange();
        }

        pushing = false;
        if (canPush && targetedObject != null && reserves.reserveLevel > 0)
        {
            reserves.DecreaseReserves(1);
  
            Vector2 direction = (transform.position - targetedObject.transform.position).normalized;

            float coinMass = targetedObject.rigidBody.mass;
            float anchor = targetedObject.Anchored(-direction);

            float perceivedCoinMass = coinMass;

            if (anchor > 0.1)
            {
                perceivedCoinMass = 750 * anchor;
            }

            float distance = Mathf.Max(Vector2.Distance(transform.position, targetedObject.transform.position), 4f);

            float waxMagnitude = perceivedCoinMass * (mass + baseMass / 2) / Mathf.Pow(distance, 2);
            float coinMagnitude = -coinMass * mass / Mathf.Pow(distance, 2);

            Vector2 forceOnWax = (waxMagnitude * direction);

            Force(forceOnWax);
            targetedObject.rigidBody.AddForce(coinMagnitude * direction, ForceMode2D.Force);

            if (anchor > 0.1)
            {
                targetedObject.TransferForce(coinMagnitude * direction);
            }
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void SetAim(Vector2 aim)
    {
        if (aim.magnitude > 0.1)
        {
            aimNormalized = aim.normalized;
        }


        if (pointer != null)
        {
            pointer.transform.position = (Vector2)transform.position + 2 * aimNormalized;
        }
    }

    public bool SetJump(bool canJump)
    {
        jumping = false;

        if (canJump)
        {
            velocity = new Vector2(velocity.x, 0);
            Impulse(jumpForce * Vector2.up);
            jumping = true;
            
        }

        return jumping;
    }

    public void SetPush(bool isPushing)
    {
        canPush = isPushing;
    }

    public void SetCoin(bool spawn)
    {
        spawnCoin = spawn;
    }

    public void SetVial(bool drink)
    {
        drinkVial = drink;
    }

    public void SetGrab(bool grab)
    {
        canGrab = grab;
    }

    public void SetFeru(int change)
    {
        int newLevel = Mathf.Min(3, Mathf.Max(-3, feruchemyLevel + change));

        //if ((newLevel > 0 && feruStored <= 0) || (newLevel < 0 && feruStored >= maxFeruStorage)) newLevel = 0;
        if (newLevel != feruchemyLevel) SetFeruchemyLevel(newLevel);
    }

    void SetFeruchemyLevel(int level)
    {
        feruchemyLevel = level;
        float scale = Mathf.Pow(2, level);

        SetMass(scale * baseMass);
        feruchemyRate = (int) (8 * (1 - scale));
        //if (feruchemyRate > 0) feruchemyRate *= 2;

        feruLevelSlider.setLevel(feruchemyLevel);
        terminalVelocity = baseTerminalVelocity * Mathf.Sqrt(mass / baseMass);
        jumpForce = CalculateJumpForce(mass, baseJumpHeight * Mathf.Pow((baseMass / mass),(2/3.0f)));

    }

    void DetectMetalInRange()
    {
        float minAngle = 30.0f;
        float minScore = (searchRadius * Mathf.Cos(Mathf.Deg2Rad * minAngle));
        
        Vector2 center = transform.position;
        targetedObject = null;
        MetalObject currentObject;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, searchRadius);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].GetComponentInParent<MetalObject>())
            {
                currentObject = hitColliders[i].GetComponentInParent<MetalObject>();

                Vector2 toObject = hitColliders[i].transform.position - this.transform.position;

                float angle = Vector2.Angle(aimNormalized, toObject);
                float score = (toObject - (Vector2.Dot(aimNormalized, toObject) * aimNormalized)).magnitude;

                if (angle < minAngle && score < minScore)
                {
                    targetedObject = currentObject;
                    minScore = score;
                }

                //currentObject.SetInRadius(this);
            }
        }

        if (targetedObject != null) targetedObject.SetAsSelected();
    }

    void SetMass(float newMass)
    {
        float oldMass = mass;
        mass = newMass;
        velocity = velocity * Mathf.Sqrt(oldMass / newMass);
    }

    float CalculateJumpForce(float mass, float height)
    {
        return mass * Mathf.Sqrt(2 * gravity * height);
    }

    void Impulse(Vector2 force)
    {
        Vector2 deltaV = (force / mass);
        velocity += deltaV;
    }

    void Force(Vector2 force)
    {
        Vector2 deltaV = (force / mass) * Time.deltaTime;
        velocity += deltaV;
    }

    void LateralForce(float force)
    {
        Force(new Vector2(force, 0.0f));
    }

    void VerticalForce(float force)
    {
        Force(new Vector2(0.0f, force));
    }


    void CalculateVelocity()
    {
        if (controller.collisions.below)
        {
            //Vector2 groundNormal = waxRaycasts.down.collisionNormal;
            //Vector2 groundTangent = new Vector2(groundNormal.y, -groundNormal.x);

            //Debug.DrawRay(transform.position, groundNormal, Color.cyan);
            //Debug.DrawRay(transform.position, groundTangent, Color.magenta);

            //Move a bit in the air, but not much
            Force(Vector2.right * (baseMoveSpeed * directionalInput.x * mass * controller.collisions.friction));

            //Ground-Resistance.
            Force(-Vector2.right * velocity.x * mass * controller.collisions.friction);

            //Normal Force
            Force(Vector2.up*(Vector2.up * gravity * mass).y);

        }
        else
        {
            //Move a bit in the air, but not much
            LateralForce(baseMoveSpeed * directionalInput.x * mass / 5);

            //Wind Resistance.
            Force(-(velocity / terminalVelocity) * (gravity * mass));
            
            
        }

        VerticalForce(-gravity * mass);

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(WaxInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class WaxController : MonoBehaviour
{
    [HideInInspector]
    public WaxInput input;
    [HideInInspector]
    public Rigidbody2D rigidBody;
    CapsuleCollider2D rigidCollider;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public WaxSounds playerSounds;

    float colliderW;
    float colliderH;

    public SpriteRenderer pointer;
    public GameObject coinTemplate;
    public MetalReserves reserves;
    public TextMeshProUGUI coinDisplay;
    public TextMeshProUGUI vialDisplay;

    public LayerMask collidesWith;
    public PhysicsMaterial2D movingMaterial;
    public PhysicsMaterial2D stoppedMaterial;

    public float vialCount;
    public float coinCount;
    const float steelSightRadius = 20;
    const float steelPushRadius = 16;
    public float baseMass;
    public float baseJumpHeight;
    public float baseJumpTime;
    public float moveSpeed;

    float baseJumpVel;
    float baseTermVel;

    float currentMass;
    float currentJumpVel;
    float currentTermVel;

    float aGravity;

    Vector2 fWalk;
    Vector2 fJump;
    Vector2 fGrav;
    Vector2 fAir;
    Vector2 fPushSelf;
    Vector2 fPushTarget;
    Vector2 fClimb;

    float facingDir;
    float currentSpeed;
    float speedDamping;

    Vector2 groundCheckPosition;
    Vector2 feetCheckPosition;
    Vector2 wallCheckPosition;
    Vector2 ledgeCheckPosition;
    float collisionCheckRange;
    float ledgeDir;

    const float climbTimer = 0.5f;
    float climbingTime;
    float groundSlope;
    float wallDistance;
    float groundedTime;
    GameObject groundObject;

    [HideInInspector]
    public bool grounded;
    bool pushing;
    bool facingWall;
    bool justJumped;
    bool onLedge;
    bool nearLedge;
    bool canGrabLedge;

    bool hasGravity;

    public DiscreteSlider feruchemyLevelDisplay;
    public Slider feruchemyStorageDisplay;

    int feruchemyLevel;
    const int maxFeruchemyLevel = 3; // 0.125 - 8 mass multiplier
    float feruchemyStored;
    const float maxFeruchemyStored = 60.0f; //1 minute of double weight
    float feruchemyStorageRate;

    Vector2 oldAim;

    public Slider healthDisplay;
    int health;
    const int maxHealth = 16;

    bool regenEnabled = false;
    float regenTimer = 0.0f;
    float regenTime = 0.25f;

    bool moveBody;

    MetalObject targetedMetal;

    bool jumpRising;
    float jumpMin;
    float jumpCurrent;

    public Checkpoint checkpoint;
    Vector2 spawnpoint;

    [HideInInspector]
    public bool isDead;
    [HideInInspector]
    public bool isDone;


    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<WaxInput>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        playerSounds = GetComponent<WaxSounds>();

        rigidBody.mass = baseMass;

        aGravity = 2 * baseJumpHeight / (baseJumpTime * baseJumpTime);
        baseJumpVel = aGravity * baseJumpTime;
        currentJumpVel = baseJumpVel;

        baseTermVel = 2.0f * baseJumpVel;
        currentTermVel = baseTermVel;

        canGrabLedge = true;
        facingDir = 1.0f;
        grounded = false;
        groundedTime = 0;

        fClimb = Vector2.zero;
        fGrav = Vector2.zero;
        fWalk = Vector2.zero;
        fAir = Vector2.zero;

        colliderW = rigidCollider.bounds.size.x / 2;
        colliderH = rigidCollider.bounds.size.y / 2;
        collisionCheckRange = 0.1f;

        feruchemyStored = 15;
        feruchemyStorageDisplay.maxValue = maxFeruchemyStored;
        feruchemyStorageRate = 0;
        feruchemyLevel = 0;

        health = maxHealth;
        healthDisplay.maxValue = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        DetectGround();
        DetectLedges();
        UpdateFeruchemy();
        UpdateAllomancy();
        UpdateInventory();
        CalculateForces();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        rigidBody.AddForce(fWalk, ForceMode2D.Force);
        rigidBody.AddForce(fGrav, ForceMode2D.Force);
        rigidBody.AddForce(fClimb, ForceMode2D.Force);
        rigidBody.AddForce(fAir, ForceMode2D.Force);
        rigidBody.AddForce(fPushSelf, ForceMode2D.Force);

        if (targetedMetal != null)
        {
            targetedMetal.rigidBody.AddForce(fPushTarget, ForceMode2D.Force);
        }

        rigidBody.AddForce(fJump, ForceMode2D.Impulse);
        fJump = Vector2.zero;
    }

    public void Respawn()
    {
        rigidBody.rotation = 0;
        rigidBody.freezeRotation = true;
        transform.position = spawnpoint;
        rigidBody.MovePosition(spawnpoint);
        rigidBody.velocity = Vector2.zero;

        ChangeFeruchemyLevel(0);
        health = maxHealth;
        isDead = false;
    }

    void DetectGround()
    {
        groundCheckPosition = (Vector2)rigidCollider.bounds.center +  new Vector2(0, -(colliderH - collisionCheckRange));
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPosition, Vector2.down, 2*collisionCheckRange,collidesWith);
        Debug.DrawRay(groundCheckPosition, collisionCheckRange * Vector2.down, groundHit ? Color.magenta : Color.cyan);

        bool oldGrounded = grounded;
        grounded = groundHit && groundHit.distance >= collisionCheckRange;

        if (grounded)
        {
            groundSlope = Vector2.SignedAngle(groundHit.normal, Vector2.up);
            groundObject = groundHit.collider.gameObject;
            Debug.DrawRay(transform.position, groundHit.normal, Color.green);
            Debug.DrawRay(transform.position, Quaternion.Euler(0,0,-groundSlope) * Vector2.right * facingDir, Color.red);
            groundedTime += Time.deltaTime;
        }
        else
        {
            groundedTime = 0;
            groundObject = null;
        }

        if (groundHit && groundHit.distance < collisionCheckRange && rigidBody.velocity.y < 0)
        {
            ThroughPlatform platform = groundHit.collider.GetComponent<ThroughPlatform>();

            if (platform != null)
            {
                platform.PassThrough(0.001f);
            }
        }

        if (grounded && !oldGrounded && rigidBody.velocity.y <= 0)
        {
            playerSounds.ImpactNormal();
        }
    }

    void DetectLedges()
    {
        wallCheckPosition = (Vector2)rigidCollider.bounds.center + new Vector2(facingDir * colliderW, colliderH - colliderW - 0.25f);
        ledgeCheckPosition = (Vector2)rigidCollider.bounds.center + new Vector2(facingDir * colliderW, colliderH - colliderW);
        feetCheckPosition = (Vector2)rigidCollider.bounds.center + new Vector2(facingDir * colliderW, colliderW - colliderH);

        RaycastHit2D wallHit = Physics2D.Raycast(wallCheckPosition, facingDir * Vector2.right, collisionCheckRange, collidesWith);
        RaycastHit2D inWall = Physics2D.Raycast(wallCheckPosition, -facingDir * Vector2.right, collisionCheckRange, collidesWith);
        RaycastHit2D ledgeHit = Physics2D.Raycast(ledgeCheckPosition, facingDir * Vector2.right, collisionCheckRange, collidesWith);
        RaycastHit2D feetHit = Physics2D.Raycast(feetCheckPosition, facingDir * Vector2.right, collisionCheckRange, collidesWith);
        RaycastHit2D feetInWall = Physics2D.Raycast(feetCheckPosition, -facingDir * Vector2.right, collisionCheckRange, collidesWith);

        Debug.DrawRay(wallCheckPosition, collisionCheckRange * Vector2.right * facingDir, wallHit ? Color.magenta : Color.cyan);
        Debug.DrawRay(ledgeCheckPosition, collisionCheckRange * Vector2.right * facingDir, ledgeHit ? Color.magenta : Color.cyan);
        Debug.DrawRay(feetCheckPosition, collisionCheckRange * Vector2.right * facingDir, feetHit ? Color.magenta : Color.cyan);

        if (feetHit && !wallHit && !onLedge && !(inWall || feetInWall) && rigidBody.velocity.y < 0 && canGrabLedge && !grounded)
        {
            nearLedge = true;
        }
        else if (wallHit && !ledgeHit && !onLedge && !(inWall || feetInWall) && rigidBody.velocity.y < 0 && canGrabLedge && !grounded)
        {
            onLedge = true;
            nearLedge = true;
            ledgeDir = facingDir;
            rigidBody.bodyType = RigidbodyType2D.Static;
        }
        else if (!(wallHit && !ledgeHit) || inWall || feetInWall || grounded)
        {
            onLedge = false;
            nearLedge = false;
            canGrabLedge = true;
            if (rigidBody.bodyType == RigidbodyType2D.Static) rigidBody.bodyType = RigidbodyType2D.Dynamic;
        }

        facingWall = wallHit;
        if (wallHit) wallDistance = wallHit.distance;
    }

    void CalculateForces()
    {
        bool walking = Mathf.Abs(input.moveDir.x) > 0;
        fWalk = Vector2.zero;

        //Gravity
        if (hasGravity)
        {
            fGrav = Vector2.down * rigidBody.mass * aGravity;
        }
        else
        {
            fGrav = Vector2.zero;
        }
        

        //Drag
        fAir = -rigidBody.velocity.normalized * Mathf.Pow(rigidBody.velocity.magnitude / currentTermVel, 2) * rigidBody.mass * aGravity;

        //Horizontal Player Input
        if (walking && !(isDead || isDone))
        {
            if (facingDir != Mathf.Sign(input.moveDir.x))
            {
                transform.eulerAngles = new Vector3(0, 90 - 90 * Mathf.Sign(input.moveDir.x), 0);
                facingDir = Mathf.Sign(input.moveDir.x);
            }
        }


        //Handle Walking
        if (grounded && !(isDead || isDone) && hasGravity)
        {
            if (walking)
            {
                Vector2 walkDir = Quaternion.Euler(0, 0, -groundSlope) * Vector2.right * facingDir;
                float currentSpeed = Vector2.Dot(rigidBody.velocity, walkDir.normalized / walkDir.magnitude);
                float maxSpeed = moveSpeed * Mathf.Cos(Mathf.Deg2Rad * groundSlope);
                fWalk = walkDir * ((1 - Mathf.Sign(currentSpeed) * Mathf.Pow(currentSpeed / maxSpeed, 2)) * 4 * rigidBody.mass * moveSpeed);
            }

            if (input.jumpPressed)
            {
                fJump = rigidBody.mass * (currentJumpVel - rigidBody.velocity.y) * Vector2.up;

                jumpMin = transform.position.y;
                jumpCurrent = jumpMin;
                jumpRising = true;

                playerSounds.Jump();
            }

            if (input.moveDir.y < -0.5f)
            {
                ThroughPlatform platform = groundObject.GetComponent<ThroughPlatform>();

                if (platform != null)
                {
                    platform.PassThrough(0.5f);
                }
            }
        }
        else if (hasGravity)
        {
            if (walking) 
            {
                fWalk = (Vector2.right * facingDir * rigidBody.mass * moveSpeed / 2) + 
                    (Vector2.left * rigidBody.velocity.x * rigidBody.mass / 4);
            }
            else
            {
                fWalk = Vector2.left * rigidBody.velocity.x * rigidBody.mass / 4;
            }
            
        }

        //Handle Ledges
        if (onLedge && rigidBody.bodyType == RigidbodyType2D.Static && !(isDead || isDone))
        {
            if (input.jumpPressed || input.moveDir.y > 0.5f)
            {
                nearLedge = false;
                rigidBody.bodyType = RigidbodyType2D.Dynamic;

                fJump = 0.55f * rigidBody.mass * baseJumpVel * Vector2.up;
                fClimb = ledgeDir * Vector2.right * rigidBody.mass * moveSpeed;
                climbingTime = climbTimer;
            }

            if (walking && Mathf.Sign(ledgeDir) != Mathf.Sign(facingDir))
            {
                nearLedge = false;
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
                fJump = 0.2f * facingDir * rigidBody.mass * baseJumpVel * Vector2.right;
            }

            if (input.moveDir.y < -0.5f)
            {
                nearLedge = false;
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
                canGrabLedge = false;
            }
        }

        if (climbingTime > 0.0f)
        {
            climbingTime -= Time.deltaTime;
            if (climbingTime <= 0) fClimb = Vector2.zero;
        }

        //Choose Material
        if (grounded && !walking && groundedTime > 0.1f)
        {
            rigidBody.sharedMaterial = stoppedMaterial;
        }
        else if (grounded && walking && Mathf.Abs(rigidBody.velocity.x) > 0.1f && facingDir != Mathf.Sign(rigidBody.velocity.x))
        {
            rigidBody.sharedMaterial = stoppedMaterial;
        }
        else
        {
            rigidBody.sharedMaterial = movingMaterial;
        }
    }

    void UpdateInventory()
    {
        Vector2 size = rigidCollider.bounds.size * 1.1f;
        Collider2D[] touching = Physics2D.OverlapBoxAll(transform.position, size, 0.0f);

        regenEnabled = false;

        if (!isDead)
        {
            bool itemGet = false;
            hasGravity = true;

            for (int i = 0; i < touching.Length; i++)
            {
                if (input.grabPressed && touching[i].tag.Equals("Coin"))
                {
                    Destroy(touching[i].gameObject);
                    coinCount++;
                    itemGet = true;
                    
                }
                else if (input.grabPressed && touching[i].tag.Equals("Vial"))
                {
                    Destroy(touching[i].gameObject);
                    vialCount++;
                    itemGet = true;
                }
                else if (touching[i].tag.Equals("Checkpoint"))
                {
                    spawnpoint = (Vector2)touching[i].transform.position + Vector2.up;
                    regenEnabled = true;

                    if (checkpoint != null) checkpoint.Deactivate();
                    checkpoint = touching[i].GetComponent<Checkpoint>();
                    if (checkpoint != null) checkpoint.Activate();
                }
                else if (touching[i].tag.Equals("Exit"))
                {
                    PlayerPrefs.SetInt("TutorialComplete", 1);
                    isDone = true;
                }
                else if (touching[i].tag.Equals("AntiGrav"))
                {
                    hasGravity = false;
                }
            }

            if (itemGet)
            {
                //playerSounds.ItemGet();
            }

            if (input.coinPressed && coinCount > 0)
            {
                Vector2 spawnpoint = (Vector2)transform.position + input.aimDir;
                GameObject newCoin = Instantiate(coinTemplate, spawnpoint, Quaternion.identity);
                newCoin.GetComponent<Rigidbody2D>().velocity = (rigidBody.velocity) + 2 * input.aimDir;
                coinCount--;
            }

            if (regenEnabled)
            {
                regenTimer += Time.deltaTime;
                if (regenTimer >= regenTime && health < maxHealth)
                {
                    health++;
                    regenTimer -= regenTime;
                }
            }
            else
            {
                regenTimer = 0;
            }
        }
        
        coinDisplay.SetText(coinCount + "×");
        vialDisplay.SetText(vialCount + "×");
        healthDisplay.value = health;
    }

    void UpdateFeruchemy()
    {
        int newFeruchemyLevel = feruchemyLevel + input.feruchemyChange;

        if (newFeruchemyLevel >  3) newFeruchemyLevel = 3;
        if (newFeruchemyLevel < -3) newFeruchemyLevel = -3;
        if (feruchemyStored <= 0 && newFeruchemyLevel > 0) newFeruchemyLevel = 0;
        if (feruchemyStored >= maxFeruchemyStored && newFeruchemyLevel < 0) newFeruchemyLevel = 0;
        if (newFeruchemyLevel != feruchemyLevel) ChangeFeruchemyLevel(newFeruchemyLevel);


        feruchemyStored += feruchemyStorageRate * Time.deltaTime;
        feruchemyStorageDisplay.value = feruchemyStored;

    }

    void ChangeFeruchemyLevel(int newLevel)
    {
        //Positive levels are heavier
        float multiplier = Mathf.Pow(2, newLevel);
        float oldMass = rigidBody.mass;
        feruchemyLevel = newLevel;
        feruchemyStorageRate = 1 - multiplier;

        feruchemyLevelDisplay.setLevel(feruchemyLevel);

        rigidBody.mass = baseMass * multiplier;
        currentJumpVel = baseJumpVel * (1 - (feruchemyLevel / 3.0f));
        currentTermVel = baseTermVel * (1 + (feruchemyLevel / 9.0f));

        rigidBody.velocity *= Mathf.Pow(oldMass / rigidBody.mass,0.5f);
    }

    void UpdateAllomancy()
    {
        pushing = false;

        pointer.transform.position = (Vector2) transform.position + 2 * input.aimDir;

        fPushSelf = Vector2.zero;
        fPushTarget = Vector2.zero;
        if (reserves.reserveLevel <= 0)
        {
            SteelSight(false);
        }
        else
        {
            SteelSight(true);

            if (targetedMetal != null && input.pushPressed)
            {
                pushing = true;
                AllomancyPush();
            }
        }

        if (input.vialPressed && vialCount > 0)
        {
            reserves.AddBeads(4);
            vialCount--;
            playerSounds.DrinkVial();
        }
    }

    void SteelSight(bool active)
    {
        float minAngle = 30.0f;
        float minScore = (steelSightRadius * Mathf.Cos(Mathf.Deg2Rad * minAngle));

        Vector2 center = transform.position;
        targetedMetal = null;
        MetalObject currentObject;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, steelSightRadius);

        //Check for objects in range
        for (int i = 0; i < hitColliders.Length; i++)
        {
            currentObject = hitColliders[i].GetComponentInParent<MetalObject>();

            //We only care about MetalObjects
            if (currentObject != null)
            {
                Vector2 toObject = hitColliders[i].transform.position - this.transform.position;

                if (active && toObject.magnitude < steelPushRadius)
                {
                    float angle = Vector2.Angle(input.aimDir, toObject);
                    float score = (toObject - (Vector2.Dot(input.aimDir, toObject) * input.aimDir)).magnitude;

                    //Keep track of the object closest to our aim.
                    if (angle < minAngle && score < minScore)
                    {
                        targetedMetal = currentObject;
                        minScore = score;
                    }
                }

                currentObject.SetInRadius(this,steelSightRadius,reserves.reserveLevel, active);
            }
        }

        //Change the color of the line to the closest object
        if (targetedMetal != null) targetedMetal.SetAsSelected();
    }

    void AllomancyPush()
    {
        reserves.DecreaseReserves(Time.deltaTime);

        Vector2 direction = (transform.position - targetedMetal.transform.position).normalized;

        float coinMass = targetedMetal.rigidBody.mass;
        float anchor = targetedMetal.Anchored(-direction);
        float perceivedCoinMass = (anchor > 0.1 ? 1000 : 1) * coinMass;

        float distance = Mathf.Max(Vector2.Distance(transform.position, targetedMetal.transform.position),2.0f);

        float waxMagnitude = 1.5f * perceivedCoinMass * (0.5f * (rigidBody.mass + baseMass)) / Mathf.Pow(distance, 2);
        float coinMagnitude = -1.5f * coinMass * rigidBody.mass / Mathf.Pow(distance, 2);

        fPushSelf = waxMagnitude * direction;
        fPushTarget = coinMagnitude * direction;
    }

    void UpdateAnimations()
    {
        animator.SetBool("Grounded", grounded);
        animator.SetBool("OnLedge", nearLedge);
        animator.SetBool("IsWalking", grounded && Mathf.Abs(input.moveDir.x) > 0.2f);
        animator.SetBool("Climbing", climbingTime > 0.05f);
        animator.SetFloat("VelocityY", rigidBody.velocity.y);

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D collidingBody = collision.collider.GetComponentInParent<Rigidbody2D>();
        Collider2D collidingCollider = collision.collider;

        int pointsColliding = collision.contactCount;
        float impact = Vector2.Dot(collision.relativeVelocity, collision.GetContact(0).normal);

        int damage = 0;

        if (collidingBody != null)
        {
            if (collidingBody.bodyType != RigidbodyType2D.Static)
            {
                //Projectile
                if (impact >= 10)
                {
                    //Debug.Log("Hit Projectile: " + Mathf.Round(impact));
                }
            }
        }

        if (collidingCollider.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            if (collidingCollider.gameObject.tag.Equals("Hazards")) impact = 100;

            impact += feruchemyLevel;

            //Wall, floor, or ceiling
            if (collidingCollider.gameObject.GetComponent<ThroughPlatform>() != null && rigidBody.velocity.y > 0)
            {
                impact = 0;
            }

            if (impact >= 16)
            {
                damage = ((int) Mathf.Round(impact) - 10) / 4;
                health -= damage;
            }
        }  

        if (impact > 4)
        {
            if (damage >= 1 && health > 0)
            {
                playerSounds.ImpactHurt();
            }
        }
        
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (!isDead)
        {
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            rigidBody.freezeRotation = false;
            isDead = true;
        }
    }

    public void AudioStep()
    {
        if (grounded)
        {
            playerSounds.Step();
        }
    }
}

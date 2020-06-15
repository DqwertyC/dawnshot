using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public WaxController target;
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookAheadDstY;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public Vector2 focusAreaSize;

    Vector2 shortFocusAreaSize;
    Vector2 currentFocusAreaSize;

    FocusArea focusArea;

    Vector2 lookDir;
    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    float verticalLookTime;

    bool lookAheadStopped;

    void Start()
    {
        focusArea = new FocusArea(target.GetComponent<Collider2D>().bounds, focusAreaSize);
        shortFocusAreaSize = new Vector2(focusAreaSize.x, 0.2f * focusAreaSize.y);
        currentFocusAreaSize = focusAreaSize;
        lookDir = target.transform.localScale;
    }

    void LateUpdate()
    {
        //if (target.velocity.y < -5.0f && target.transform.position.y < focusArea.centre.y )
        //{
        //    focusArea.ChangeSize(shortFocusAreaSize);
        //    currentFocusAreaSize = shortFocusAreaSize;
        //}
        //else
        //{
        //    focusArea.ChangeSize(focusAreaSize);
        //    currentFocusAreaSize = focusAreaSize;
        //}

        focusArea.Update(target.GetComponent<Collider2D>().bounds);

        lookDir = Vector2.Lerp(lookDir, target.transform.localScale, Mathf.Abs(target.input.moveDir.x)>0.1f?0.1f:0.02f);


        Vector3 targetFocus;
        targetFocus = focusArea.centre + (lookDir * lookAheadDstX) + (Vector2.up * verticalOffset);

        if (verticalLookTime > 30 && verticalLookTime < 60)
        {
            targetFocus += Vector3.up * (lookAheadDstY * target.input.moveDir.y * (verticalLookTime - 30) / 30);
        }
        else if (verticalLookTime >= 60)
        {
            targetFocus += Vector3.up * (lookAheadDstY * target.input.moveDir.y);
        }
      

        targetFocus += Vector3.forward * -10;



        transform.position = Vector3.Lerp(transform.position, targetFocus, 0.10f);
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(target.input.moveDir.y) > 0.5f)
        {
            verticalLookTime++;
        }
        else
        {
            verticalLookTime = 0;
        }
        
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = new Color(1, 0, 0, .5f);
        //Gizmos.DrawCube(focusArea.centre, currentFocusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;


        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void ChangeSize(Vector2 newSize)
        {
            left = centre.x - (newSize.x / 2);
            right = centre.x + (newSize.x / 2);
            top = centre.y + (newSize.y);
            bottom = centre.y;
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;



            float targetX = (left + right) / 2;
            float targetY = (top + bottom) / 2;


            centre = new Vector2(targetX, targetY);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

}
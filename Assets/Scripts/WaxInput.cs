using UnityEngine;
using System.Collections;

public class WaxInput : MonoBehaviour
{
    DiscreteHeldInput heldCoin;
    DiscreteHeldInput heldVial;
    DiscreteHeldInput heldFeru;
    DiscreteHeldInput heldJump;


    public Vector2 moveDir;
    public Vector2 aimDir;
    public int feruchemyChange;
    public bool jumpPressed;
    public bool pushPressed;
    public bool coinPressed;
    public bool vialPressed;
    public bool grabPressed;


    void Start()
    {
        heldCoin = new DiscreteHeldInput(0.5f, 0.5f);
        heldVial = new DiscreteHeldInput(0.3f, 0.1f);
        heldFeru = new DiscreteHeldInput(0.4f, 0.05f);
        heldJump = new DiscreteHeldInput(float.MaxValue, float.MaxValue);

        aimDir = Vector2.right;

    }

    void Update()
    {
        Vector2 directionalInput;
        Vector2 aimNormalized;
        float feru;
        float jump;
        float push;
        float coin;
        float vial;
        float grab;

        if (PlayerPrefs.GetInt("InputMode",0) == 1)
        {
            directionalInput = new Vector2(Input.GetAxisRaw("ConHori"), Input.GetAxisRaw("ConVert"));

            float aimH = Input.GetAxisRaw("ConAimH");
            float aimV = Input.GetAxisRaw("ConAimV");

            float aimMag = Mathf.Sqrt(aimH * aimH + aimV * aimV);

            if (aimMag < 0.3f)
            {
                aimH = 0;
                aimV = 0;
            }

            aimNormalized = new Vector2(aimH,aimV).normalized;
            jump = Input.GetAxisRaw("ConJump");
            push = Input.GetAxisRaw("ConPush");
            coin = Input.GetAxisRaw("ConCoin");
            vial = Input.GetAxisRaw("ConVial");
            grab = Input.GetAxisRaw("ConGrab");
            feru = Input.GetAxisRaw("ConFeru");
            feru = heldFeru.Update(feru);
        }
        else
        {
            directionalInput = new Vector2(Input.GetAxisRaw("KeyHori"), Input.GetAxisRaw("KeyVert"));
            aimNormalized = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            aimNormalized = aimNormalized / aimNormalized.magnitude;
            jump = Input.GetAxisRaw("KeyJump");
            push = Input.GetAxisRaw("KeyPush");
            coin = Input.GetAxisRaw("KeyCoin");
            vial = Input.GetAxisRaw("KeyVial");
            grab = Input.GetAxisRaw("KeyGrab");
            feru = Input.GetAxisRaw("KeyFeru");
        }

        if (!ProjectVars.GAME_PAUSED && !ProjectVars.IN_DIALOG)
        {
            this.moveDir = directionalInput;
            this.aimDir = aimNormalized.magnitude > 0.5f ? aimNormalized : this.aimDir;
            this.pushPressed = (push > 0.1f);
            this.coinPressed = (1 == heldCoin.Update(coin));
            this.vialPressed = (1 == heldVial.Update(vial));
            this.grabPressed = (grab > 0.1f);
            this.feruchemyChange = (int) feru;
            this.jumpPressed = (1 == heldJump.Update(jump));
        }
        else
        {
            heldJump.Update(1.0f);
        }
    }
}
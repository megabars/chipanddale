using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(PlatformerCharacter2D))]
public class Platformer2DUserControl : MonoBehaviour
{

    private PlatformerCharacter2D m_Character;
    private bool m_Jump;
    public bool canBox;
    public bool handBox;
    public string axis;
    private Collision2D box;

    private void Awake()
    {
        m_Character = GetComponent<PlatformerCharacter2D>();
        canBox = false;
        axis = "right";
        handBox = false;
    }

    private void Update()
    {
        if (canBox && !handBox && Input.GetKeyDown(KeyCode.Z))
            m_Character.getBox(box);

        if(handBox && Input.GetKeyDown(KeyCode.X))
        {
            m_Character.shotBox();
        }


        if (!m_Jump)
        {
            // Read the jump input in Update so button presses aren't missed.
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }
    }


    private void FixedUpdate()
    {
        // Read the inputs.
        bool crouch = Input.GetKey(KeyCode.S);

        float h = CrossPlatformInputManager.GetAxis("Horizontal");

        if (h > 0) axis = "right";
        else if (h < 0) axis = "left";
        

        // Pass all parameters to the character control script.
        m_Character.Move(h, crouch, m_Jump);
        m_Jump = false;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Box")
        {
            float deltaX = collision.transform.position.x - gameObject.transform.position.x;
            float deltaY = collision.transform.position.y - gameObject.transform.position.y;

            if ((axis == "right" && deltaX <= 0.7 && deltaY <= 0.4 && deltaY > -0.5) || (axis == "left" && deltaX >= -0.7 && deltaY <= 0.4 && deltaY > -0.5))
            {
                canBox = true;
                box = collision;

            }

        }

        if (collision.gameObject.tag == "Apple")
        {
            float deltaX = collision.transform.position.x - gameObject.transform.position.x;
            float deltaY = collision.transform.position.y - gameObject.transform.position.y;

            if ((axis == "right" && deltaX <= 0.8 && deltaY <= 0.4 && deltaY > -0.5) || (axis == "left" && deltaX >= -0.8 && deltaY <= 0.4 && deltaY > -0.5))
            {
                canBox = true;
                box = collision;

            }

        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Box" || collision.gameObject.tag == "Apple")
        {
            canBox = false;
        }
    }
}

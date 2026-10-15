using UnityEngine;

public class Crosshair : MonoBehaviour
{


    private int speed = 2;
    private float radius = 5.5f;


    // Hides cursor
    void Awake()
    {
        
        Cursor.visible = false;

    }


    void Update()
    {
        
        // Sets crosshair to follow cursor
        Vector2 mouseCursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mouseCursorPos;


        // Sets crosshair to stay within set circular range around the player
        var input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        var vectorFromParent = transform.localPosition + new Vector3(input.x, input.y, 0) * speed * Time.deltaTime;
        transform.localPosition = Vector2.ClampMagnitude(vectorFromParent, radius);


    }


}
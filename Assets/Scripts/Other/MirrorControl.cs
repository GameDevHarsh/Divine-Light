using UnityEngine;

public class MirrorControl : MonoBehaviour
{
    public float speed;
    public void RotateMirror()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.localEulerAngles += new Vector3(0, 0, -1) * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.localEulerAngles += new Vector3(0, 0, 1) * speed * Time.deltaTime;
        }

    }
}

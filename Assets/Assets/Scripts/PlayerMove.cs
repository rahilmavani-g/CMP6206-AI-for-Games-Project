using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    CharacterController cc;

    void Start() { cc = GetComponent<CharacterController>(); }

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        cc.Move(input * speed * Time.deltaTime);
    }
}
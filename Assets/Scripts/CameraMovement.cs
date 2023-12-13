using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera camera;
    public float ortSize = 5f;
    // Start is called before the first frame update

    private void Awake()
    {
        camera = GetComponent<Camera>();
        camera.orthographicSize = ortSize;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 3f;
        float moveY = Input.GetAxis("Vertical") * Time.deltaTime * 3f;

        transform.position += new Vector3(moveX, moveY, 0);

        float bound = 7f;

        if(transform.position.y > bound)
        {
            transform.position = new Vector3(transform.position.x, bound, -10f);
        }

        if (transform.position.y < -bound)
        {
            transform.position = new Vector3(transform.position.x, -bound, -10f);
        }

        if (transform.position.x > bound)
        {
            transform.position = new Vector3(bound, transform.position.y, -10f);
        }

        if (transform.position.x < -bound)
        {
            transform.position = new Vector3(-bound, transform.position.y, -10f);
        }

        float scroll = Input.mouseScrollDelta.y * Time.deltaTime * 15f;
        ortSize -= scroll;
        if(ortSize < 1f)
        {
            ortSize = 1f;
        }

        if(ortSize > 10f)
        {
            ortSize = 10f;
        }
        camera.orthographicSize = ortSize;
    }
}

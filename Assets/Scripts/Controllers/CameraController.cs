using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private float MovementSpeed;
    Camera _camera;
    private void Start()
    {
        _camera = GetComponent<Camera>();
    }
    public void MoveCamera(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, input.y, 0);
        transform.position += moveDirection * MovementSpeed * Time.deltaTime;
    }

    public void Zoom(float scrollDelta)
    {
        _camera.orthographicSize -= scrollDelta * Time.deltaTime * 50f;
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, 1f, 100f);
    }


}

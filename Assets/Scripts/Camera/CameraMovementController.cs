using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    #region Publikus változók

    public bool canMove;

    [Range(0.1f, 20f)]
    public float Speed;

    [Range(0f, 20f)]
    public float PanBorderThickness;

    [Range(1f, 1.2f)]
    public float Acceleration;
    
    [Range(0.1f, 30f)]
    public float MaxSpeed;

    [Range(100f, 1000f)]
    public float ZoomSpeed;

    [Range(1f, 100f)]
    [Tooltip("Legalább ilyen magasan legyen a föld fölött a kamera.")]
    public float MinZoomDistanceFromGround;
    
    [Tooltip("A CameraBound-ot ki kell venni belőle, mert abban a collisionben már eleve benne van, tehát mindig ütközni fog vele a ray.")]
    public LayerMask GroundLayerMask;

    #endregion

    #region Privát változók

    private float _accelerationX = 1f;
    private float _accelerationY = 1f;

    private Collider _cameraBoundCollider;

    #endregion

    #region Main

    [Command("cameraCanMove", "cameraCanMove(b) kamera mozgás letiltása/engedélyezése")]
    public void CanMove(bool b)
    {
        Debug.Log("Camera movement set to " + b);
        canMove = b;
    }

    private void Start()
    {
        GameObject cameraBound = GameObject.FindGameObjectWithTag("CameraBound");
        _cameraBoundCollider = cameraBound.GetComponent<BoxCollider>();

        if (!_cameraBoundCollider.bounds.Contains(transform.position))
        {
            Debug.LogWarning("A játékteret körülölelő collider (CameraBound) nem tartalmazza a kamera objektumot.");
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            canMove = !canMove;
        }

        if (canMove)
        {
            Pan();
            Zoom();
        }
        
    }

    #endregion

    #region Pan

    private void Pan()
    {
        var position = transform.position;
        
        // Felső és alsó szél
        if (Input.mousePosition.y >= Screen.height - PanBorderThickness)
        {
            position.z += Speed * Time.deltaTime * _accelerationY;
            _accelerationY *= Acceleration;
        }
        else if (Input.mousePosition.y <= PanBorderThickness)
        {
            position.z -= Speed * Time.deltaTime * _accelerationY;
            _accelerationY *= Acceleration;
        }
        else
        {
            _accelerationY = 1f;
        }
        
        // Jobb és bal szél
        if (Input.mousePosition.x >= Screen.width - PanBorderThickness)
        {
            position.x += Speed * Time.deltaTime * _accelerationX;
            _accelerationX *= Acceleration;
        }
        else if (Input.mousePosition.x <= PanBorderThickness)
        {
            position.x -= Speed * Time.deltaTime * _accelerationX;
            _accelerationX *= Acceleration;
        }
        else
        {
            _accelerationX = 1f;
        }

        _accelerationX = Mathf.Clamp(_accelerationX, 1f, MaxSpeed);
        _accelerationY = Mathf.Clamp(_accelerationY, 1f, MaxSpeed);

        // Ha még benne van a világ határait reprezentáló colliderben
        if (_cameraBoundCollider.bounds.Contains(position))
        {
            // Az új kiszámolt pozició alapján mozgatás
            transform.position = position;
        }
    }

    #endregion

    #region Zoom

    private void Zoom()
    {
        var position = transform.position;
        bool move = true;
        
        position.y -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * ZoomSpeed;

        if (position.y < transform.position.y)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, GroundLayerMask))
            {
                move = hit.distance > MinZoomDistanceFromGround;
            }
        }
        
        // Ha elég magasan van az alatta levő dologhoz kéepst és még benne van a világ határait reprezentáló colliderben
        if (move && _cameraBoundCollider.bounds.Contains(position))
        {
            // Az új kiszámolt pozició alapján mozgatás
            transform.position = position;
        }
    }

    #endregion
    
    
}

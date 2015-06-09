using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    private Camera cam; //  Camera component of this object.
    private GameObject[] targets;   //  Game Objects that are to be kept in the frame.

    private Vector3 targetCenter = Vector3.zero;    //  Targeted focal center for camera.
    private Rect bounds = new Rect();

    private float aspect;
    private float fov, fova;    //  Horizontal & vertical Field of View.

    public float camSpeed = 5;
    public float minDistance = 5;

	void Start ()
    {
        cam = GetComponentInChildren<Camera>();

        targets = GameObject.FindGameObjectsWithTag("Player");

        aspect = cam.aspect;
        //fov = cam.fieldOfView;
        //fova = Mathf.Atan(aspect * Mathf.Tan(fov)) * Mathf.Rad2Deg;
        fov = cam.orthographicSize;
        fova = fov / aspect;
	}
	
	void Update ()
    {
        SetBounds();

        SetPositionAndSize();
	}

    void SetBounds()    //  Create a rectangle, based on the positions of the upper-left & lower-right targets.
    {
        Vector3 min = new Vector3(Mathf.Infinity, 0, Mathf.Infinity);
        Vector3 max = new Vector3(-Mathf.Infinity, 0, -Mathf.Infinity);

        foreach (GameObject target in targets)
        {
            Vector3 tmp = target.transform.position;

            if (tmp.x < min.x)
                min.x = tmp.x;

            if (tmp.x > max.x)
                max.x = tmp.x;

            if (tmp.z < min.z)
                min.z = tmp.z;

            if (tmp.z > max.z)
                max.z = tmp.z;
        }

        bounds = new Rect(min.x, min.z, max.x - min.x, max.z - min.z);
        bounds.height *= Mathf.Sin(transform.eulerAngles.x);

        targetCenter = (min + max) / 2;
    }

    void SetPositionAndSize()
    {
        Vector3 camPos = targetCenter;
        float zoom = 0;

        if (bounds.width >= bounds.height)
        {
            zoom = bounds.width / Mathf.Atan(fov);
        }
        else
        {
            zoom = bounds.height / Mathf.Atan(fova);
        }

        if (zoom < minDistance) zoom = minDistance;

        //  Setting orthographic size.

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoom, Time.deltaTime * camSpeed);

        //  Correct position.

        camPos.y += zoom;   //  Move camera up.
        camPos.z -= zoom;   //  Move camera back;

        transform.position = camPos;
    }
}

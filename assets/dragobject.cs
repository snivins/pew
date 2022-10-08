using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragobject : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 screenPoint;
    private Vector3 offset;

    public LineRenderer line;
    public Transform origin;
    private bool dragged;
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        if (!dragged)
        {
            transform.position = Vector3.Lerp(transform.position, origin.position + origin.rotation * new Vector3(0, 3, 5), 50*Time.deltaTime);
        }
        line.SetPosition(0, origin.position + origin.rotation * new Vector3(0, 3, 0));
        line.SetPosition(1, transform.position);
    }
    void OnMouseDown()
    {
        Debug.Log("down");
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    }

    void OnMouseDrag()
    {
        Debug.Log("drag");
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        Vector3 ref_point = origin.position + origin.rotation * new Vector3(0, 3, 0);
        Vector3 ref_neutral = origin.position + origin.rotation * new Vector3(0, 3, 5);

        //transform.position = GetDistPointToLine(origin.position, origin.rotation * new Vector3(0, 3, 3), curPosition); //curPosition;
        //transform.position = NearestPointOnLine(origin.position + origin.rotation * new Vector3(0, 3, 0),  origin.rotation * new Vector3(0, 0, 1), curPosition); //curPosition;
        Vector3 posit_ = NearestPointOnLine(ref_point,  origin.rotation * new Vector3(0, 0, 1), curPosition); //curPosition;
        float movement = 0;
        if (Vector3.Distance(posit_, ref_neutral) > 5)
        {
            if (Vector3.Distance(posit_, ref_point) < Vector3.Distance(posit_, origin.position + origin.rotation * new Vector3(0, 3, 10)))
            {
                transform.position = ref_point;
                movement = -5;
            } else
            {
                transform.position = origin.position + origin.rotation * new Vector3(0, 3, 10);
                movement = 5;
            }
        } else
        {
            transform.position = posit_;
            if (Vector3.Distance(posit_, ref_point) < Vector3.Distance(posit_, origin.position + origin.rotation * new Vector3(0, 3, 10)))
            {
                movement = -Vector3.Distance(posit_, ref_neutral);
            }
            else
            {
                movement = Vector3.Distance(posit_, ref_neutral);
            }
        }
         dragged = true;
        Debug.Log(movement/5);
    }
    void OnMouseUp()
    {

        dragged = false;
    }
    static public Vector3 GetDistPointToLine(Vector3 origin, Vector3 direction, Vector3 point)
    {
        Vector3 point2origin = origin - point;
        Vector3 point2closestPointOnLine = point2origin - Vector3.Dot(point2origin, direction) * direction;
        return point2closestPointOnLine;
    }


    //linePnt - point the line passes through
    //lineDir - unit vector in direction of line, either direction works
    //pnt - the point to find nearest on line for
    public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();//this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }
}

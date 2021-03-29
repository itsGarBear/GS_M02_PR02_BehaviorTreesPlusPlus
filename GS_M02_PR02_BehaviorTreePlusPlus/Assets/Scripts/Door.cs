using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public bool isClosed = false;
    public bool isLocked = false;


    Vector3 closeRot = new Vector3(0, 0, 0);
    Vector3 openRot = new Vector3(0, 135, 0);

    private void Start()
    {
        if (isClosed)
            transform.eulerAngles = closeRot;
        else
            transform.eulerAngles = openRot;
    }
    public void ToggleClosed()
    {
        isClosed = !isClosed;

        if (isClosed)
            transform.eulerAngles = closeRot;
        else
            transform.eulerAngles = openRot;

    }
    public void ToggleLocked()
    {
        isLocked = !isLocked;
    }

    public bool Open()
    {
        if (isClosed && !isLocked)
        {
            isClosed = false;
            transform.eulerAngles = openRot;
            return true;
        }

        return false;
    }

    public bool Close()
    {
        if (!isClosed)
        {
            transform.eulerAngles = closeRot;
            isClosed = true;
        }
        return true;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Task
{
    public abstract void run();
    public bool succeeded;

    protected int eventId;
    const string EVENT_NAME_PREFIX = "FinishedTask";
    public string TaskFinished
    {
        get
        {
            return EVENT_NAME_PREFIX + eventId;
        }
    }
    public Task()
    {
        eventId = EventBus.GetEventID();
    }
}

public class IsTrue : Task
{
    bool varToTest;

    public IsTrue(bool myBool)
    {
        varToTest = myBool;
    }

    public override void run()
    {
        succeeded = varToTest;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class IsFalse : Task
{
    bool varToTest;

    public IsFalse(bool myBool)
    {
        varToTest = myBool;
    }

    public override void run()
    {
        succeeded = !varToTest;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class OpenDoor : Task
{
    Door myDoor;

    public OpenDoor(Door door)
    {
        myDoor = door;
    }

    public override void run()
    {
        succeeded = myDoor.Open();
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class BargeDoor : Task
{
    Rigidbody myDoorRB;

    public BargeDoor(Rigidbody door)
    {
        myDoorRB = door;
    }

    public override void run()
    {
        myDoorRB.AddForce(-15f, 0, 0, ForceMode.VelocityChange);
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class GoGhost : Task
{
    GameObject myEntity;

    public GoGhost(GameObject someEntity)
    {
        myEntity = someEntity;
    }

    public override void run()
    {
        myEntity.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        myEntity.GetComponent<BoxCollider>().enabled = false;
        myEntity.GetComponent<Rigidbody>().useGravity = false;
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class Jump : Task
{
    Rigidbody myRB;

    public Jump(Rigidbody rb)
    {
        myRB = rb;
    }

    public override void run()
    {
        myRB.AddForce(-1f, 20f, 0, ForceMode.Impulse);
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}
public class LiftDoor : Task
{
    Rigidbody myDoorRB;

    public LiftDoor(Rigidbody door)
    {
        myDoorRB = door;
    }

    public override void run()
    {
        myDoorRB.AddForce(0, 20f, 0, ForceMode.VelocityChange);
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class Wait : Task
{
    float myTimeToWait;

    public Wait(float time)
    {
        myTimeToWait = time;
    }

    public override void run()
    {
        succeeded = true;
        EventBus.ScheduleTrigger(TaskFinished, myTimeToWait);
    }
}

public class MoveToObject : Task
{
    Arriver myMover;
    GameObject myTarget;

    public MoveToObject(Kinematic mover, GameObject target)
    {
        myMover = mover as Arriver;
        myTarget = target;
    }

    public override void run()
    {
        myMover.OnArrived += MoverArrived;
        myMover.myTarget = myTarget;
    }
    public void MoverArrived()
    { 
        myMover.OnArrived -= MoverArrived;
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class Sequence : Task
{
    List<Task> children;
    Task currTask;
    int currTaskNdx = 0;

    public Sequence(List<Task> taskList)
    {
        children = taskList;
    }
    public override void run()
    {
        currTask = children[currTaskNdx];
        EventBus.StartListening(currTask.TaskFinished, OnChildTaskFinished);
        currTask.run();
    }

    void OnChildTaskFinished()
    {
        if (currTask.succeeded)
        {
            EventBus.StopListening(currTask.TaskFinished, OnChildTaskFinished);
            currTaskNdx++;
            if (currTaskNdx < children.Count)
            {
                this.run();
            }
            else
            {
                succeeded = true;
                EventBus.TriggerEvent(TaskFinished);
            }
        }
        else
        {
            succeeded = false;
            EventBus.TriggerEvent(TaskFinished);
        }
    }

}
public class Selector : Task
{
    List<Task> children;
    Task currTask;
    int currTaskNdx = 0;

    public Selector(List<Task> taskList)
    {
        children = taskList;
    }
    public override void run()
    {
        currTask = children[currTaskNdx];
        EventBus.StartListening(currTask.TaskFinished, OnChildTaskFinished);
        currTask.run();
    }

    void OnChildTaskFinished()
    {
        if (currTask.succeeded)
        {
            succeeded = true;
            EventBus.TriggerEvent(TaskFinished);
        }
        else
        {
            EventBus.StopListening(currTask.TaskFinished, OnChildTaskFinished);
            currTaskNdx++;
            if (currTaskNdx < children.Count)
            {
                this.run();
            }
            else
            {

                succeeded = false;
                EventBus.TriggerEvent(TaskFinished);
            }
        }
    }
}

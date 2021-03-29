using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Dropdown dropdown;
    public Door door;
    public GameObject treasure;

    bool isDoingBehavior = false;
    Task currTask;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            if(!isDoingBehavior)
            {
                isDoingBehavior = true;
                currTask = CreateTasksAndGetTreasure();
                EventBus.StartListening(currTask.TaskFinished, OnTaskFinished);
                currTask.run();
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    void OnTaskFinished()
    {
        EventBus.StopListening(currTask.TaskFinished, OnTaskFinished);
        isDoingBehavior = false;
    }


    Task CreateTasksAndGetTreasure()
    {
        List<Task> taskList = new List<Task>();

        Task checkOpenDoor = new IsFalse(door.isLocked);
        Task waitABeat = new Wait(0.5f);
        Task openDoor = new OpenDoor(door);
        taskList.Add(checkOpenDoor);
        taskList.Add(waitABeat);
        taskList.Add(openDoor);
        Sequence openUnlockedDoor = new Sequence(taskList);

        taskList = new List<Task>();
        Task isDoorClosed = new IsTrue(door.isClosed);
        taskList.Add(isDoorClosed);
        taskList.Add(waitABeat);

        if (dropdown.value == 0)
        {
            Task bargeDoor = new BargeDoor(door.transform.GetChild(0).GetComponent<Rigidbody>());
            taskList.Add(bargeDoor);
        }
        else if (dropdown.value == 1)
        {
            Task ghost = new GoGhost(this.gameObject);
            taskList.Add(ghost);
        }
        else if (dropdown.value == 2)
        {
            Task jump = new Jump(gameObject.GetComponent<Rigidbody>());
            taskList.Add(jump);
        }
        else
        {
            Task lift = new LiftDoor(door.transform.GetChild(0).GetComponent<Rigidbody>());
            taskList.Add(lift);
        }


        Sequence breakClosedDoor = new Sequence(taskList);

        taskList = new List<Task>();
        taskList.Add(openUnlockedDoor);
        taskList.Add(breakClosedDoor);
        Selector openTheDoor = new Selector(taskList);

        //
        taskList = new List<Task>();
        Task moveToDoor = new MoveToObject(this.GetComponent<Arriver>(), door.gameObject);
        Task moveToTreasure = new MoveToObject(this.GetComponent<Arriver>(), treasure.gameObject);
        taskList.Add(moveToDoor);
        taskList.Add(waitABeat);
        taskList.Add(openTheDoor);
        taskList.Add(waitABeat);
        taskList.Add(moveToTreasure);
        Sequence getTreasureBehindClosedDoor = new Sequence(taskList);

        taskList = new List<Task>();
        Task isDoorOpen = new IsFalse(door.isClosed);
        taskList.Add(isDoorOpen);
        taskList.Add(moveToTreasure);
        Sequence getTreasureBehindOpenDoor = new Sequence(taskList);

        taskList = new List<Task>();
        taskList.Add(getTreasureBehindOpenDoor);
        taskList.Add(getTreasureBehindClosedDoor);
        Selector getTreasure = new Selector(taskList);

        return getTreasure;
    }
}

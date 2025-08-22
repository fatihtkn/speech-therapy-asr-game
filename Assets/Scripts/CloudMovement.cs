
using System;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public List<CloudConfiguration> configurations = new();

    readonly List<CloudController> controllers = new();
    void Start()
    {
        configurations.ForEach((configuration) => { controllers.Add(new(configuration));});
    }

    void Update()
    {
        controllers.ForEach((controller) => { controller.MoveClouds(); });
    }
}
public class CloudController
{
    readonly Cloud cloud1;
    readonly Cloud cloud2;



    readonly List<Cloud> clouds=new();

    public CloudController(CloudConfiguration configuration)
    {
        float speed = configuration.speed;
        cloud1=new Cloud(speed, configuration.cloudRect1,configuration.exitStartXPos,configuration.exitEndXPos);
        cloud2=new Cloud(speed, configuration.cloudRect2, configuration.exitStartXPos, configuration.exitEndXPos);
        cloud1.StartMove();

        clouds.Add(cloud1);
        clouds.Add(cloud2);

        clouds.ForEach(cloud => cloud.OnStartedToExit += Cloud_OnStartedToExit); 
    }

    private void Cloud_OnStartedToExit()
    {
        foreach (Cloud cloud in clouds)
        {
            if (cloud.CheckState(Cloud.States.waiting))
            {
                cloud.StartMove();
            }
        }
    }

    public void MoveClouds()
    {
        cloud1.Move();
        
        cloud2.Move();

    }
   
}
public class Cloud
{
    private float startedToExitValue;
    private float exitValue;
    private States currentState;
    private bool canMove;
    private bool hasTriggeredExit;

    public float speed;
    public RectTransform cloud;

    public event Action OnStartedToExit;

    public Cloud(float speed, RectTransform cloud, float exitStartXPos, float exitEndXPos)
    {
        this.speed = speed;
        this.cloud = cloud;
        this.startedToExitValue = exitStartXPos;
        this.exitValue = exitEndXPos;
        currentState = States.waiting;
        hasTriggeredExit = false;
    }

    public void Move()
    {
        if (!canMove) return;

        cloud.anchoredPosition += speed * Time.deltaTime * Vector2.left;
        float x = cloud.anchoredPosition.x;

        if (x < startedToExitValue && x > exitValue && !hasTriggeredExit)
        {
            currentState = States.startedToExit;
            hasTriggeredExit = true;
            OnStartedToExit?.Invoke();
        }
        else if (x < exitValue)
        {
            currentState = States.exitCompleted;
            // Reset konumu
            cloud.anchoredPosition = new(Math.Abs(exitValue), cloud.anchoredPosition.y);
            currentState = States.waiting;
            canMove = false;
            hasTriggeredExit = false;
        }
    }

    public void StartMove()
    {
        canMove = true;
        currentState = States.moving;
        hasTriggeredExit = false;
    }

    public bool CheckState(States targetState)
    {
        return currentState == targetState;
    }

    public enum States
    {
        waiting,
        moving,
        startedToExit,
        exitCompleted,
    }
}

[Serializable]
public class CloudConfiguration
{
    public float exitStartXPos;
    public float exitEndXPos;
    public RectTransform cloudRect1;
    public RectTransform cloudRect2;
    public float speed;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    enum EnemyState
    {
        Patrol = 0,
        Investigate = 1,
        ReportBack = 2,          
        InvestigatingTogether = 3 
    }

    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _threshold = 0.5f;
    [SerializeField] private float _waitTime = 2f;
    [SerializeField] private PatrolRoute _patrolRoute;
    [SerializeField] private FieldOfView _fov;
    [SerializeField] private EnemyState _state = EnemyState.Patrol;
    [SerializeField] private GameObject otherRobot;
    private bool _moving = false;
    private Transform _currentPoint;
    private int _routeIndex = 0;
    private bool _forwardsAlongPath = true;
    private Vector3 _investigationPoint; 
    private Vector3 _otherRobotPosition;
    private bool _hasReported = false; 
    private float _waitTimer = 0f; 

    void Start()
    {
        _currentPoint = _patrolRoute.route[_routeIndex];
        _otherRobotPosition = otherRobot.transform.position;
    }

    void Update()
    {
        if (_fov.visibleObjects.Count > 0)
        {
            Vector3 playerPosition = _fov.visibleObjects[0].position;

            if (!_hasReported)
            {
                _investigationPoint = playerPosition;
                _state = EnemyState.ReportBack;  
                _agent.SetDestination(otherRobot.transform.position); 
            }
        }


        if (_state == EnemyState.Patrol)
        {
            UpdatePatrol();
        }
        else if(_state == EnemyState.Investigate)
        {
            UpdateInvestigate();
        }
        else if (_state == EnemyState.ReportBack)
        {
            UpdateReportBack();
        }
        else if (_state == EnemyState.InvestigatingTogether)
        {
            UpdateInvestigateTogether();
        }
    }

    private void UpdateReportBack()
    {
        if (Vector3.Distance(transform.position, otherRobot.transform.position) < _threshold)
        {
            _hasReported = true;
            otherRobot.GetComponent<EnemyController>().InvestigateTogether(_investigationPoint);
            InvestigateTogether(_investigationPoint); 
        }
    }

    public void InvestigateTogether(Vector3 investigatePoint)
    {
        _state = EnemyState.InvestigatingTogether;
        _investigationPoint = investigatePoint;
        _agent.SetDestination(_investigationPoint);
    }

    private void UpdateInvestigateTogether()
    {
        if (Vector3.Distance(transform.position, _investigationPoint) < _threshold)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer > _waitTime)
            {
                ReturnToPatrol();
            }
        }
    }

    public void InvestigatePoint(Vector3 investigatePoint)
    {
        _state = EnemyState.Investigate;
        _investigationPoint = investigatePoint;
        _agent.SetDestination(_investigationPoint);
    }

    private void UpdateInvestigate()
    {
        if (Vector3.Distance(transform.position, _investigationPoint) < _threshold)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer > _waitTime)
            {
                ReturnToPatrol();
            }
        }
    }

    private void ReturnToPatrol()
    {
        _state = EnemyState.Patrol;
        _waitTimer = 0;
        _moving = false;
        _hasReported = false; // Reset reporting status
    }

    private void UpdatePatrol()
    {
        if (!_moving)
        {
            NextPatrolPoint();
            _agent.SetDestination(_currentPoint.position);
            _moving = true;
        }

        if (_moving && Vector3.Distance(transform.position, _currentPoint.position) < _threshold)
        {
            _moving = false;
        }
    }

    private void NextPatrolPoint()
    {
        if (_forwardsAlongPath)
        {
            _routeIndex++;
        }
        else
        {
            _routeIndex--;
        }

        if (_routeIndex == _patrolRoute.route.Count)
        {
            if (_patrolRoute.patrolType == PatrolRoute.PatrolType.Loop)
            {
                _routeIndex = 0;
            }
            else
            {
                _forwardsAlongPath = false;
                _routeIndex-=2;
            }
        }

        if (_routeIndex == 0)
        {
            _forwardsAlongPath = true;
        }

        _currentPoint = _patrolRoute.route[_routeIndex];
    }
}

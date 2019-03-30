using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    Vector3 _prevWayPoint = Vector3.zero;
   
    //int _curIndex = 0;

    override public void Start()
    {
        //캐릭터의  이동 목적지를 세팅 (첫번째 웨이포인트)
        //순서대로 순찰
        //_curIndex = (_curIndex + 1) % _character.GetWayPointCount(); 
        //Vector3 wayPoint = _character.GetWayPoint(_curIndex);

        _prevWayPoint = _character.transform.position;
        _prevWayPoint.y = 0.0f;
        //Vector3 wayPoint = _character.GetRandomWayPoint();
        Vector3 wayPoint = _character.GetRandomWayPoint();



        if (wayPoint.Equals(_prevWayPoint))
        {
            _character.ChangeState(Character.eState.WAIT);
        }
        else
        {
            _character.SetDestination(wayPoint);
            //_character.ChangeState(Character.eState.IDLE);
            _character.ChangeState(Character.eState.WALK);

            _prevWayPoint = wayPoint;
        }
        //_character.SetDestination(wayPoint);
        //_character.ChangeState(Character.eState.WALK);
    }
}
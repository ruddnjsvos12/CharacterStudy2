using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : MonoBehaviour
{

    // 캐릭터컨트롤러를 _animator에 세팅 

    [SerializeField] AnimationController _animationController;
    [SerializeField] List<GameObject> _wayPointList;
    private void Awake()
    {
        _characterController = gameObject.GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {


        _stateDic.Add(eState.IDLE, new IdleState());
        _stateDic.Add(eState.WAIT, new WaitState());
        _stateDic.Add(eState.KICK, new KickState());
        _stateDic.Add(eState.WALK, new WalkState());
        _stateDic.Add(eState.PATROL, new PatrolState());
        _stateDic.Add(eState.WAIT2, new Wait2State());


        for (int i = 0; i < _stateDic.Count; i++)
        {
            eState state = (eState)i;
            _stateDic[state].SetCharacter(this);
        }

        //최초 상태
        ChangeState(eState.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
        UpDateMove();

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if(eState.WALK == _stateType)
        {
            ChangeState(eState.KICK);
        }
    }

    //상태(State)

    Dictionary<eState, State> _stateDic = new Dictionary<eState, State>();

    State _state = null;


    public enum eState
    {
        IDLE,
        WAIT,
        KICK,
        WALK,
        PATROL,
        WAIT2,
    }

    eState _stateType = eState.IDLE;

    public void ChangeState(eState state)
    {
        _stateType = state;
        _state = _stateDic[state];
        _state.Start();

    }

    void UpdateState()
    {
        UpDateMove();
    }

    //Animation

    public void PlayAnimation(string trigger, System.Action endCallBack)
    {
        _animationController.Play(trigger, endCallBack);
    }

    CharacterController _characterController;
    float _moveSpeed = 0.0f;
    private Vector3 _destPoint;

    //Movement

    void UpDateMove()
    {
        Vector3 moveDirection = GetMoveDirection();

        Vector3 moveVelocity = moveDirection * _moveSpeed;
        Vector3 gravityVelocity = Vector3.down * 9.8f;
        Vector3 finalVelocity = (moveVelocity + gravityVelocity) * Time.deltaTime;

        _characterController.Move(finalVelocity);

        // 현재 위치와 목적지 까지의 거리를 계산해서
        // 적절한 범위 내에 들어오면 스톱!!!!!!!!

        if (0.0f < _moveSpeed)
        {
            Vector3 charPos = transform.position;
            Vector3 curPos = new Vector3(charPos.x, 0.0f, charPos.z);
            Vector3 destPos = new Vector3(_destPoint.x, 0.0f, _destPoint.z);

            float distance = Vector3.Distance(curPos, destPos);
            if (distance < 0.5f)
            {
                _moveSpeed = 0.0f;
                ChangeState(eState.IDLE);
            }
        }
    }

    public void StartWalk(float speed)
    {
        _moveSpeed = speed;

    }

    public void StopMove()
    {
        _moveSpeed = 0.0f;
    }
    public Vector3 GetWayPoint(int index)
    {
        return _wayPointList[index].transform.position;
    }

    public int GetWayPointCount()
    {
        return _wayPointList.Count;
    }

    public void SetWaypointList(List<GameObject> wayPointList)
    {
        _wayPointList = wayPointList;
    }

    public Vector3 GetRandomWayPoint()
    {
        int index = Random.Range(0, _wayPointList.Count);
        return GetWayPoint(index);
    }

    public void SetDestination(Vector3 destPoint)
    {
        _destPoint = destPoint;
    }

    Vector3 GetMoveDirection()
    {

        //(목적 위치 - 현재 위치) 노멀라이즈
        Vector3 charPos = transform.position;
        Vector3 curPos = new Vector3(charPos.x, 0.0f, charPos.z);
        Vector3 destPos = new Vector3(_destPoint.x, 0.0f, _destPoint.z);
        Vector3 direction = (destPos - curPos).normalized;

        Vector3 lookPos = new Vector3(_destPoint.x, charPos.y, _destPoint.z);
        //transform.LookAt(lookPos);

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                      targetRotation,
                                                      180.0f * Time.deltaTime);

        return direction;
        
    }

}

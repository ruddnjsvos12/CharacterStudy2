using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fetch 체크용
public class Character : MonoBehaviour
{

    // 캐릭터컨트롤러를 _animator에 세팅 
    [SerializeField] List<GameObject> _charPrefabList;

    [SerializeField] RuntimeAnimatorController _animatorController;
    [SerializeField] List<GameObject> _wayPointList;

    /*
    [SerializeField] bool _isPlayer = false;

    PlayerModule _playerModule = null;
    NPCModule _npcModule = null;
    */
    enum CharType
    {
        Player,
        NPC
    }
    [SerializeField] CharType _charType = CharType.NPC;

    List<CharacterModule> _charModuleList = new List<CharacterModule>();
    CharacterModule _characterModule = null;

    AnimationController _animationController;

    int _meetCount = 0;

    private void Awake()
    {
        //플레이어 모듈을 생성
        _charModuleList.Add(new PlayerModule(this));
        _charModuleList.Add(new NPCModule(this));

        _characterController = gameObject.GetComponent<CharacterController>();

        int index = 0;
       
        //자동화
        {

            //★ 이 부분 조건문 없애보기

            /*
            if (CharType.Player == _charType)
            {
                index = 0;
            }
            else
            {
                index = 1;
            }*/
            
            // 프리팹 생성 및 각종 수치 초기화

           
            GameObject obj = GameObject.Instantiate<GameObject>(_charPrefabList[(int)_charType]);
            obj.transform.position = transform.position;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            obj.transform.SetParent(transform);

            if ( 0 < transform.childCount)
            {
                Transform childTransform = transform.GetChild(0);
                childTransform.gameObject.AddComponent<AnimationController>();
                _animationController = childTransform.gameObject.GetComponent<AnimationController>();

                Animator animCom = childTransform.gameObject.GetComponent<Animator>();
                animCom.runtimeAnimatorController = _animatorController;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _characterModule = _charModuleList[(int)_charType];
        _characterModule.BuildStateList();
    }

    // Update is called once per frame
    void Update()
    {
        if(eState.DEATH!= _stateType)
        {
            _characterModule.UpdateAI();

            UpdateState();
            UpDateMove();
            UpdateDeath();
            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(gameObject))
            return;


        //_meetCount++;
        /*
        if (3 <= _meetCount)
        {
            ChangeState(eState.DEATH);
            return;
        }*/
        //_lifeTime = 0.0f;

        //Debug.Log("OnTriggerEnter");
        if(eState.WALK == _stateType)
        {
            ChangeState(eState.KICK);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(gameObject))
            return;
        _meetCount--;
    }

    //상태(State)

    Dictionary<eState, State> _stateDic = new Dictionary<eState, State>();

    State _state = null;

    public Dictionary<eState, State> GetStateDic()
    {
        return _stateDic;
    }

    public enum eState
    {
        IDLE,
        WAIT,
        KICK,
        WALK,
        PATROL,
        WAIT2,
        DEATH,
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

    //Death
    float _deathTime = 12.0f;
    float _lifeTime = 0.0f;

    void UpdateDeath()
    {
        
        if(_deathTime <= _lifeTime && _charType == CharType.NPC)
        {
            ChangeState(eState.DEATH);
           
        }

        _lifeTime += Time.deltaTime;
    }

}

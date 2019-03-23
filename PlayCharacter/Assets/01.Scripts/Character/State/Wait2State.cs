using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait2State : State
{
    override public void Start()
    {
        _character.PlayAnimation("wait2", () =>
        {
            _character.ChangeState(Character.eState.PATROL);
        });
    }
}

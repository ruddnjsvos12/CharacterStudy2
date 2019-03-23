using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickState : State
{
    override public void Start()
    {
        _character.StopMove();
        _character.PlayAnimation("kick", () =>
        {
            _character.ChangeState(Character.eState.WAIT2);
        });
    }

}

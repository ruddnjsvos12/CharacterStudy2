using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : State
{
    // Start is called before the first frame update
    override public void Start()
    {

        _character.PlayAnimation("walk", null);
        _character.StartWalk(2.0f);

    }
}

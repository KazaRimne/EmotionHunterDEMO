using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Stand : IState
{
   
    private AnimeSANA _sana;

    public Stand(AnimeSANA sana)
    {
        _sana = sana;
    }

    public void Enter()
    {
        _sana.Twice = false;   
        _sana._isLooping = true;
        _sana._isReversing = false;
        _sana._playOnce = false;

        _sana.SetAnimation(_sana.frames, Datasana.Instance.StnadTicks);
    }

    public void Update()
    {
    }

    public void Exit() { }
}

public class Move : IState
{

    private AnimeSANA _sana;

    public Move(AnimeSANA sana)
    {
        _sana = sana;
    }

    public void Enter()
    {
        _sana.Twice = false;
        _sana._isLooping = true;
        _sana._isReversing = false;
        _sana._playOnce = false;

        _sana.SetAnimation(_sana.walkFrames, Datasana.Instance.walkFrameTicks);
    }

    public void Update()
    {
        _sana.UpdateSpriteAnimation();
    }

    public void Exit() { }
}

public class Jump : IState
{

    private AnimeSANA _sana;

    public Jump(AnimeSANA sana)
    {
        _sana = sana;
    }

    public void Enter()
    {
        _sana._isLooping = true;
        _sana._isReversing = false;
        _sana._playOnce = false;
        _sana.SetAnimation(_sana.jumpFrames, Datasana.Instance.jumpTicks);
    }

    public void Update()
    {
    }

    public void Exit() { }
}
public class JumpEND : IState
{

    private AnimeSANA _sana;

    public JumpEND(AnimeSANA sana)
    {
        _sana = sana;
    }

    public void Enter()
    {
        _sana._isLooping = true;
        _sana._isReversing = false;
        _sana._playOnce = false;
        _sana.SetAnimation(_sana.jumpENDFrames, Datasana.Instance.jumpENDTicks);
    }

    public void Update()
    {
    }

    public void Exit() { }
}
public class JumpTwiceState : IState
{
    private AnimeSANA _sana;
    public JumpTwiceState(AnimeSANA sana) => _sana = sana;

    public void Enter()
    {
        _sana._isLooping = true;
        _sana._isReversing = false;
        _sana._playOnce = false;

        // 2. 執行二段跳動畫邏輯
        if (!_sana.Twice)
        {
            _sana.SetAnimation(_sana.jumpTwiceFrames, Datasana.Instance.jumpTwiceTicks);
            _sana.Twice = true;
        }
    }

    public void Update()
    {
        
    }

    public void Exit() { }
}
public class Swupone : IState
{
    private AnimeSANA _sana;
    public Swupone(AnimeSANA sana) => _sana = sana;

    public void Enter()
    {
        _sana._isLooping = true;
        _sana._isReversing = false;
        _sana._playOnce = false;

        // 2. 判斷方向並播放對應動畫
        if (!_sana.Twice)
        {
           _sana.SetAnimation(_sana.Swup, Datasana.Instance.SwupTicks);
        }
    }

    public void Update()
    {
        _sana.UpdateSpriteAnimation();
    }

    public void Exit() { }
}
public class SwupTwo : IState
{
    private AnimeSANA _sana;
    public SwupTwo(AnimeSANA sana) => _sana = sana;

    public void Enter()
    {
        _sana._isLooping = true;
        _sana._isReversing = false;
        _sana._playOnce = false;

       _sana.SetAnimation(_sana.Swuptwo, Datasana.Instance.StnadTicks);
    }

    public void Update()
    {
        // 換圖心跳
        _sana.UpdateSpriteAnimation();
    }

    public void Exit() { }
}
public class ClimbState : IState
{
    private AnimeSANA _sana;
    public ClimbState(AnimeSANA sana) => _sana = sana;

    public void Enter()
    {
         _sana.SetAnimation(_sana.climbFrames, Datasana.Instance.climbTicks);
    }

    public void Update()
    {
        // 這裡是核心：即時根據按鍵更新動畫播放狀態
        bool isUp = Movesana.Instance.Up;
        bool isDown = Movesana.Instance.Down;

        if (isUp)
        {
            _sana._isLooping = true;
            _sana._playOnce = false;
            _sana._isReversing = false; // 正向播放 (向上爬)
        }
        else if (isDown)
        {
            _sana._isLooping = true;
            _sana._playOnce = false;
            _sana._isReversing = true;  // 反向播放 (向下爬)
        }
        else
        {
            // 沒按上下，掛在牆上不動
            _sana._isLooping = false;
        }

        // 執行換圖
        _sana.UpdateSpriteAnimation();
    }

    public void Exit()
    {
        // 離開爬牆狀態時，記得把倒播關掉，以免影響其他動作
        _sana._isReversing = false;
    }
}
public class ClimbENDState : IState
{
    private AnimeSANA _sana;
    public ClimbENDState(AnimeSANA sana) => _sana = sana;

    public void Enter()
    {
        
         _sana.SetAnimation(_sana.climbEND, Datasana.Instance.ETicks);
    }

    public void Update()
    {
      
    }

    public void Exit()
    {
    }
}
public class ATK : IState
{
    private AnimeSANA _sana;
    private float _exitTime; 
    private float _timer;
    public ATK(AnimeSANA sana)
    {
        _sana = sana;
    }

    public void Enter()
    {
        _sana._isLooping = true;
        _sana._isReversing = false;
        _sana._playOnce = true;
        _timer = 0f;

        // 判斷方向與連擊次數 (從 Instance 拿資料)
        bool isRight = Mouseca.Instance.mouseright;
        int comboCount = Attacksana.Instance.Count;

       
            _timer = 0f;
            if (comboCount == 0) { _sana.SetAnimation(_sana.ATK, Datasana.Instance.ATKTicks); _exitTime = 10 / 24f; }
            else if (comboCount == 1) { _sana.SetAnimation(_sana.ATKtwo, Datasana.Instance.ETicks); _exitTime = 0.5f; }
            else if (comboCount == 2) { _sana.SetAnimation(_sana.ATKthr, Datasana.Instance.ThrTicks); _exitTime = 0.5f; }
            else if (comboCount == 4) { _sana.SetAnimation(_sana.ATKEND, Datasana.Instance.ATKEndTicks); _exitTime = 0.5f; }
    }

    public void Update()
    {
        _sana.UpdateSpriteAnimation();

        // 取代原本的 Invoke("ResetToIdle")
        _timer += Time.deltaTime;
        if (_timer >= _exitTime)
        {
            _sana.stateMachine.ChangeState(new Stand(_sana));
        }
    }

    public void Exit()
    {
     
    }
}

public class Fskill : IState
{
    private AnimeSANA _sana;
    public Fskill(AnimeSANA sana) => _sana = sana;

    public void Enter()
    {
        _sana._isLooping = true;
        _sana._isReversing = false;
        _sana._playOnce = false;

        _sana.SetAnimation(_sana.FskillFrames, Datasana.Instance.FskillTicks);
    }

    public void Update()
    {
      
        _sana.UpdateSpriteAnimation();
    }

    public void Exit() { }
}

public class DamagedState : IState
{
    private AnimeSANA _sana;
    public DamagedState(AnimeSANA sana) => _sana = sana;

    public void Enter()
    {
        _sana._isLooping = true;
        _sana._isReversing = false;
        _sana._playOnce = false;

      
        _sana.SetAnimation(_sana.DamagedFrames, Datasana.Instance.StnadTicks);
    }

    public void Update()
    {

        _sana.UpdateSpriteAnimation();
    }

    public void Exit() { }
}

public class Shift : IState
{
    private AnimeSANA _sana;
    public Shift(AnimeSANA sana) => _sana = sana;

    public void Enter()
    {
        _sana._isLooping = false;
        _sana._isReversing = false;
        _sana._playOnce = true;

        _sana.SetAnimation(_sana.ShiftFrames, Datasana.Instance.ETicks);
    }

    public void Update()
    {

        _sana.UpdateSpriteAnimation();
    }

    public void Exit() { }
}
public class RATK : IState
{

    private AnimeSANA _sana;
    private float _exitTime;
    private float _timer;
    public RATK(AnimeSANA sana)
    {
        _sana = sana;
    }

    public void Enter()
    {
        _sana._isLooping = true;
        _sana._isReversing = false;
        _sana._playOnce = true;

        _timer = 0f;

        bool isRight = Mouseca.Instance.mouseright;
        int comboCount = Attacksana.Instance.Count;
        _sana.SetAnimation(_sana.RATK, Datasana.Instance.ATKTicks); _exitTime = 0.5f; ;
    }

    public void Update()
    {
        _sana.UpdateSpriteAnimation();

        // 取代原本的 Invoke("ResetToIdle")
        _timer += Time.deltaTime;
        if (_timer >= _exitTime)
        {
            _sana.stateMachine.ChangeState(new Stand(_sana));
        }
    }

    public void Exit()
    {

    }
}
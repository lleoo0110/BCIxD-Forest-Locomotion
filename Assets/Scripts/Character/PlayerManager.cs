using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class PlayerManager : MonoBehaviour
{
  public StarterAssetsInputs starterAssetsInputs;
  public GameObject PlayerAmature;
  public UDPReceiverInt udpReceiverInt;
  private bool stopFlag = false;
  
  void Start()
  {

  }

  void Update()
  {
    Move();
    Run();
    RotateR();
    Jump();

    // stopFlag切り替え判定
    if(Keyboard.current.rKey.wasPressedThisFrame)
    {
      stopFlag = !stopFlag;
    }
  }

  void Move()
  {
    if((Keyboard.current.iKey.isPressed || udpReceiverInt.receivedInt == 2) && stopFlag == true)
    {
      starterAssetsInputs.TriggerAutoMove(1);
    }
    else if(Keyboard.current.sKey.isPressed)
    {
      starterAssetsInputs.TriggerAutoMove(-1);
    }
    else
    {
      starterAssetsInputs.TriggerAutoMove(default);
    }
  }


  void Run()
  {
    if(((Keyboard.current.wKey.isPressed && Keyboard.current.leftShiftKey.isPressed)) && stopFlag == true)
    {
      starterAssetsInputs.TriggerAutoRun(1);
    }
    else if((Keyboard.current.sKey.isPressed && Keyboard.current.leftShiftKey.isPressed))
    {
      starterAssetsInputs.TriggerAutoRun(-1);
    }
    else
    {
      starterAssetsInputs.TriggerAutoRun(default);
    }
  }

   void Jump()
  {
    if((Keyboard.current.spaceKey.isPressed || udpReceiverInt.receivedInt == 3)  && stopFlag == true)
    {
      starterAssetsInputs.TriggerJump(true);
    }
    else
    {
      starterAssetsInputs.TriggerJump(false);
    }
  }

  void RotateR()
  {
    if(Keyboard.current.dKey.isPressed)
    {
      starterAssetsInputs.RotateLookDirection(1);
    }
    
    else if(Keyboard.current.aKey.isPressed)
    {
      starterAssetsInputs.RotateLookDirection(-1);
    }

    else
    {
      starterAssetsInputs.RotateLookDirection(default);
    }
  }
}

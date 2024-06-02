using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLoadScene : MonoBehaviour
{
   public void Load()
   {
      GameManager.Instance.LoadGameScene();
   }
}

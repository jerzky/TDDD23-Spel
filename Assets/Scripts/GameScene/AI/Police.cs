using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Police : Lawman
  {
      protected override void Start()
      {
          Health = 150; 
          HaltTime = 2f; 
          ShootTime = 0.25f;
          base.Start();
          CurrentState = State.FollowRoute;
          CurrentAction = ActionE.FollowPath;
          DeadHat = Resources.Load<Sprite>("Textures/guardhat");
      }

      // Update is called once per frame
      void Update()
      {

      }

      public override bool Alert(Vector2 position, AlertIntensity alertIntesity)
      {
          if (IsIncapacitated)
              return false;
          if (CurrentState == State.Pursuit)
              return true;

          CurrentAlertIntensity = alertIntesity;
          switch (CurrentAlertIntensity)
          {
              case AlertIntensity.NonHostile:
                  StartInvestigate(position);
                  break;
              case AlertIntensity.Nonexistant:
                  StartInvestigate(position);
                  break;
              case AlertIntensity.ConfirmedHostile:
                  Pursue.LastPlayerPos = position;
                  CurrentState = State.Pursuit;
                  CurrentAction = ActionE.Pursue;
                  break;

          }

          return true;
      }

      protected override void IncapacitateFailedReaction()
      {
          throw new System.NotImplementedException();
      }
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


abstract public class MovingUnitOptions : UnitOptions 
{
    
    new public enum OPTIONS : int
    {
        MoveTo=0,
        Guard=10,
        Patrol=20,
        Hide=30,
        Stay=100,

    }
    

    internal override void FocussedLeftOnGround(Vector3 worldPoint)
    {
        standardOrder = true;
        IsMovingAsGroup = true;
        SetKinematic();
        UnitState = (OPTIONS)0;
        MoveToPoint = worldPoint;
        CalculateDirection();
        gameObject.transform.position += (UNIT.MovingDirection * UNIT.Speed);
        IsAttacking = false;
        Target = null;
        standardOrder = false;
    }
    
    public float standardYPosition;
    public override Vector3 MoveToPoint
    {
        get
        {
            return base.MoveToPoint;
        }
        protected set
        {
            base.MoveToPoint = new Vector3(value.x, standardYPosition, value.z); 
        }
    }

    public override bool IsMoving
    {
        get
        {
            return base.IsMoving;
        }
        protected set
        {
            if (value) { if (!gameObject.GetComponent<Pilot>()) gameObject.AddComponent<Pilot>(); }
            else { if (gameObject.GetComponent<Pilot>()) {  Component.Destroy(gameObject.GetComponent<Pilot>()); } }
            base.IsMoving = value;
        }
    }
    public List<Vector3> WayPoints;
    protected Vector3 CalculateDirection()
    {
        return UNIT.MovingDirection = (MoveToPoint - gameObject.transform.position).normalized;
    }

  //  public Vector3 movingDirection;
    void OnCollisionExit(Collision collision)
    {
        SetKinematic();
        CalculateDirection();
    }

    protected float distance;
    public virtual float Distance
    {
        get
        {
            return Vector3.Distance(gameObject.transform.position, MoveToPoint);
        }
        protected set
        {
            if (value != distance)
            {
                if (IsMovingAsGroup)
                {
                    if (Target)
                    {
                        MoveToPoint = Target.transform.position;
                        gameObject.transform.position += (UNIT.MovingDirection * UNIT.Speed);
                    }
                    else IsGroupLeader = true;
                }
            }
        }
    }




    internal override void MoveAsGroup(GameObject leader)
    {
        Target = leader;
        MoveToPoint = leader.transform.position;
        IsMovingAsGroup = true;
        IsAttacking = false;
    }


    private OPTIONS movingUnitState;
    public int unitstateint=0;
    public override System.Enum UnitState
    {
        get
        {
            return movingUnitState;
        }
        set
        {


            OPTIONS order = (OPTIONS)value;
            if (unitstateint != (int)order)
            {

                if (!standardOrder)
                {
                    switch (order)
                    {
                        case OPTIONS.MoveTo:
                            {
                                SetKinematic();
                                WayPoints.Clear();
                                LockOnFocus();
                                MouseEvents.LEFTCLICK += MouseEvents_LEFTCLICK;
                                break;
                            }
                        case OPTIONS.Patrol:
                            {
                                SetKinematic();
                                LockOnFocus();
                                MouseEvents.LEFTCLICK += MouseEvents_LEFTCLICK;
                                MouseEvents.RIGHTCLICK += MouseEvents_RIGHTCLICK;
                                WayPoints.Add(gameObject.transform.position);
                                break;
                            }
                        case OPTIONS.Stay:
                            {
                                SetKinematic();
                                UnlockFocus(Focus.HANDLING.UnlockFocus);
                                WayPoints.Clear();
                                IsAttacking = false;
                                MoveToPoint = gameObject.transform.position;
                                break;
                            }
                        case OPTIONS.Guard:
                            {
                                SetKinematic();
                                LockOnFocus();
                                WayPoints.Clear();
                                MouseEvents.LEFTCLICK += MouseEvents_LEFTCLICK;
                                break;
                            }


                    }
                }
                unitstateint = (int)order;
                movingUnitState = order;
            }
        }
    }

    protected override void MouseEvents_LEFTCLICK(Ray qamRay, bool hold)
    {
        if (movingUnitState == OPTIONS.MoveTo)
        {
            MoveToPoint = MouseEvents.State.Position.AsWorldPointOnMap;
            CalculateDirection();
            IsMoving = true;
            MouseEvents.LEFTCLICK -= MouseEvents_LEFTCLICK;
            UnlockFocus();
        }
        else if (movingUnitState == OPTIONS.Guard)
        {
            RaycastHit Hit;
            if (Physics.Raycast(qamRay, out Hit, Camera.main.transform.position.y))
            {
                if (TargetIsAllied(Hit.collider.gameObject))
                {
                    Target = Hit.collider.gameObject.GetComponent<UnitScript>().SetInteracting(this.gameObject);
                    MoveToPoint = Target.transform.position;
                    IsMoving = true;
                }
            }
            MouseEvents.LEFTCLICK -= MouseEvents_LEFTCLICK;
            UnlockFocus(Focus.HANDLING.DestroyFocus);
        }
        else if (movingUnitState == OPTIONS.Patrol)
        {
            WayPoints.Add(MouseEvents.State.Position.AsWorldPointOnMap);
            IsMoving = true;
            MouseEvents.LEFTCLICK -= MouseEvents_LEFTCLICK;
            MouseEvents.RIGHTCLICK -= MouseEvents_RIGHTCLICK;
            UnlockFocus(Focus.HANDLING.DestroyFocus);
        }
    }

    protected override void MouseEvents_RIGHTCLICK(Ray qamRay, bool hold)
    {
        if (!hold)
        {
            if (gameObject.GetComponent<Focus>())
            {
                if (movingUnitState == OPTIONS.Patrol)
                {
                    if (WayPoints.Count >= 2) WayPoints.Insert(WayPoints.Count - 1, MouseEvents.State.Position.AsWorldPointOnMap);
                    else WayPoints.Add(MouseEvents.State.Position.AsWorldPointOnMap);
                    CalculateDirection();
                    IsMoving = true;
                }
            }
        }
    }

    virtual protected bool MoveTo()
    {

        if (gameObject.GetComponent<Pilot>()) gameObject.GetComponent<Pilot>().DoUpdate();

        if ((OPTIONS)UnitState == OPTIONS.Guard)
        {
            if (gameObject.GetComponent<Pilot>()) UnitComponent.Destroy(gameObject.GetComponent<Pilot>());
            MoveToPoint = Target.transform.position;

            if (Distance >= 20) gameObject.transform.position += (UNIT.MovingDirection * UNIT.Speed);
            else if (Distance <= 15) gameObject.transform.position -= (UNIT.MovingDirection * UNIT.Speed);
        }
        else if (IsMovingAsGroup)
        {
            if (IsGroupLeader) IsMovingAsGroup = false;
            else Distance = Vector3.Distance(gameObject.transform.position, MoveToPoint);

        }
        else if (IsAttacking)
        {
            if (Distance < UNIT.AttackRange)
            {
                UNIT.weapon.Engage(Target);
            }

            if (Distance >= UNIT.AttackRange / 2)
            {
                gameObject.transform.position += (UNIT.MovingDirection * UNIT.Speed);
                MoveToPoint = gameObject.transform.position;
                UNIT.weapon.Engage(Target);
            }
        }
        else if (Distance >= 0.5)
        {
            gameObject.transform.position += (UNIT.MovingDirection * UNIT.Speed);

        }
        else
        {
            SetKinematic();
            gameObject.transform.position = MoveToPoint;

            if (IsGroupLeader) GUIScript.main.SelectedGroup.GroupState = UnitGroup.GROUPSTATE.Waiting;
            if ((OPTIONS)UnitState == OPTIONS.Patrol)
            {
                WayPoints.RemoveAt(0);
                WayPoints.Add(gameObject.transform.position);
                MoveToPoint = WayPoints[0];
                CalculateDirection();

            }
            else { movingUnitState = OPTIONS.Stay; unitstateint = (int)movingUnitState; }

        }

        return (gameObject.transform.position != MoveToPoint);
    }

    protected void SetKinematic()
    {
        gameObject.rigidbody.isKinematic = true;
        kinematicFrames = 2;
    }
    private void checkKinematic()
    {
        if (gameObject.rigidbody.isKinematic)
            if (kinematicFrames <= 0) gameObject.rigidbody.isKinematic = false;
            else --kinematicFrames;
    }
    private int kinematicFrames;

    internal override void DoStart()
    {
        foreach (int value in System.Enum.GetValues(typeof(OPTIONS)))
            OPTIONSlist.Add(value, ((OPTIONS)value).ToString());
        standardYPosition = gameObject.transform.position.y;
        MoveToPoint = gameObject.transform.position;
        //unitstateint = 20;
        //movingUnitState = (OPTIONS)unitstateint;
        //UnitState = movingUnitState;
        //IsMoving = true;
    }

    internal override void DoUpdate()
    {
        checkKinematic();
        if (IsMoving) IsMoving = MoveTo();
    }

}

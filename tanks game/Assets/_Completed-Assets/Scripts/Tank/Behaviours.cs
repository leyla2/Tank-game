using UnityEngine;
using NPBehave;
using System.Collections.Generic;

namespace Complete
{
    /*
    Example behaviour trees for the Tank AI.  This is partial definition:
    the core AI code is defined in TankAI.cs.

    Use this file to specifiy your new behaviour tree.
     */
    public partial class TankAI : MonoBehaviour
    {
        

        private Root CreateBehaviourTree() {

            System.Random rnd = new System.Random(); //Didn't know how to use NPBehave's random so I used System's. 
            float speed = rnd.Next(-1, 1);

            switch (m_Behaviour) {

                case 9:
                    return SpinBehaviour(-0.05f, 1f);//I'm not using this.       
                case 8:
                    return TrackBehaviour(); /// I'm not using this. 
                case 0:
                    return Fun(0.3f); //Uses an argument for speed.
                case 1:
                    return MoveTowards(); //Shoots in player direction.
                case 2:
                    return Frightened(); //Enemy panics and moves in a circular motion. 
                case 3:
                    return Unpredictable(speed); //Randomly moves with a randomly generated speed used as an argument. 


                default:
                    return new Root (new Action(()=> Turn(0.1f)));
            }
        }

        /* Actions */

        private Node StopTurning() {
            return new Action(() => Turn(0));
        }

        private Node RandomFire() {
            return new Action(() => Fire(UnityEngine.Random.Range(0.0f, 1.0f)));
        }


        /* Example behaviour trees */

        // Constantly spin and fire on the spot 
        private Root SpinBehaviour(float turn, float shoot) {
            return new Root(new Sequence(
                        new Action(() => Turn(turn)),
                        new Action(() => Fire(shoot))
                    ));
        }

        private Root Fun(float move)
        {
            return new Root(
                   new Service(0.2f, UpdatePerception,
                       new Selector(
                           new BlackboardCondition("targetOffCentre",
                                                   Operator.IS_SMALLER_OR_EQUAL, 0.1f,
                                                   Stops.IMMEDIATE_RESTART,
                               new Sequence(StopTurning(), new Wait(2f), RandomFire())),
                           new BlackboardCondition("targetOnRight", Operator.IS_EQUAL, true,
                                                   Stops.IMMEDIATE_RESTART, //do next actions if statment is true
                               new Action(() => Turn(move))),new Action(() => Turn(-move))
                       )
                   )
               );
        }

        // Turn to face your opponent and fire
        private Root TrackBehaviour() {
            return new Root(
                new Service(0.2f, UpdatePerception,
                    new Selector(
                        new BlackboardCondition("targetOffCentre",
                                                Operator.IS_SMALLER_OR_EQUAL, 0.1f,
                                                Stops.IMMEDIATE_RESTART,
                            // Stop turning and fire
                            new Sequence(StopTurning(),
                                        new Wait(2f),
                                       RandomFire())),
                        new BlackboardCondition("targetOnRight",
                                                Operator.IS_EQUAL, true,
                                                Stops.IMMEDIATE_RESTART,
                            // Turn right toward target
                            new Action(() => Turn(0.2f))),
                            // Turn left toward target
                            new Action(() => Turn(-0.2f))
                    )
                )
            );
        }

        // Turn to face your opponent and fire
        private Root MoveTowards()
        {
            return new Root(
                new Service(0.2f, UpdatePerception,
                    new Selector(


                        new BlackboardCondition("targetOffCentre",
                                                Operator.IS_SMALLER_OR_EQUAL, 0.1f,
                                                Stops.IMMEDIATE_RESTART,
                            // Stop turning and fire
                            new Sequence(StopTurning(),
                                        new Wait(0.5f),
                                        new Action(() => Fire(1)))),

                        new BlackboardCondition("targetOnRight",
                                                Operator.IS_EQUAL, true,
                                                Stops.IMMEDIATE_RESTART,
                            // Turn right toward target
                            new Action(() => Turn(0.3f))),

                        new BlackboardCondition("targetOnRight",
                                                Operator.IS_EQUAL, false,
                                                Stops.IMMEDIATE_RESTART,
                            
                            new Action(() => Turn(-0.3f))),

                      new BlackboardCondition("targetDistance",
                                            Operator.IS_SMALLER_OR_EQUAL, 15.0f,
                                            Stops.IMMEDIATE_RESTART,

                        new Action(() => Move(0.3f)))
                     
                        //new Action(() => Turn(0.2f))
                        //Constantly fire.

                    )
                    )
             
                
            );
        }


        private Root Frightened()
        {
            return new Root(
                new Service(0.2f, UpdatePerception,
                    new Selector(


                        //new BlackboardCondition("targetInFront",
                        //                        Operator.IS_EQUAL, true,
                        //                        Stops.IMMEDIATE_RESTART,

                        //    new Action(() => Turn(-0.6f))),

                        //new BlackboardCondition("targetInFront",
                        //                        Operator.IS_EQUAL, false,
                        //                        Stops.IMMEDIATE_RESTART,

                        //    new Action(() => Turn(0.6f))),

                        new BlackboardCondition("targetOnLeft",
                                                Operator.IS_EQUAL, true,
                                                Stops.IMMEDIATE_RESTART,
                            // Turn left
                            new Action(() => Turn(-0.2f))),
                        new BlackboardCondition("targetOnRight",
                                                Operator.IS_EQUAL, true,
                                                Stops.IMMEDIATE_RESTART,
                            // Turn right - away
                            new Action(() => Turn(0.2f))),
                          new BlackboardCondition("targetDistance",
                                                Operator.IS_SMALLER_OR_EQUAL, 10.0f,
                                                Stops.IMMEDIATE_RESTART,

                            new Action(() => Move(-0.2f)))//Move backwards -scared. 
                           
                           // new Action(() => Turn(0.2f))
                    )
                )
            );
        }

        private Root Unpredictable(float speed)
        {
           
           
            return new Root(
                new Service(0.2f, UpdatePerception,
                    new Selector(


                         new BlackboardCondition("targetOffCentre",
                                                Operator.IS_SMALLER_OR_EQUAL, 0.1f,
                                                Stops.IMMEDIATE_RESTART,
                            // Stop turning and fire
                            new Sequence(StopTurning(),
                                        new Wait(1.0f),
                                        RandomFire())),


                        new BlackboardCondition("targetOnRight",
                                                Operator.IS_EQUAL, true,
                                                Stops.IMMEDIATE_RESTART,
                            // Turn right toward target
                            new Action(() => Turn(speed))),
                          new BlackboardCondition("targetDistance",
                                                Operator.IS_SMALLER_OR_EQUAL, 10.0f,
                                                Stops.IMMEDIATE_RESTART,

                            new Action(() => Move(speed))),
                           new BlackboardCondition("targetOffCentre",
                                               Operator.IS_GREATER_OR_EQUAL, 0.1f,
                                                Stops.IMMEDIATE_RESTART,

                            new Action(() => Turn(speed))),

                           new BlackboardCondition("targetInFront",
                                                Operator.IS_EQUAL, false,
                                                Stops.IMMEDIATE_RESTART,

                            new Action(() => Move(speed))),
                          
                            // Turn left toward target
                            new Action(() => Turn(0.2f))
                    )
                )
            );
        }


        private void UpdatePerception() {
            Vector3 targetPos = TargetTransform().position;
            Vector3 localPos = this.transform.InverseTransformPoint(targetPos);
            Vector3 heading = localPos.normalized;
            blackboard["targetDistance"] = localPos.magnitude;
            blackboard["targetInFront"] = heading.z > 0;
            blackboard["targetOnRight"] = heading.x > 0;
            blackboard["targetOffCentre"] = Mathf.Abs(heading.x);
        }

    }
}
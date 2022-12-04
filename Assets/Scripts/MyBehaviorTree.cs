using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class MyBehaviorTree : MonoBehaviour
{

	/*
	public Transform wander1;
	public Transform wander2;
	public Transform wander3;
	public GameObject participant;

	private BehaviorAgent behaviorAgent;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	protected Node ST_ApproachAndWait(Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
	}

	protected Node BuildTreeRoot()
	{
		Node roaming = new DecoratorLoop (
							new SequenceShuffle(
								this.ST_ApproachAndWait(this.wander1),
								this.ST_ApproachAndWait(this.wander2),
								this.ST_ApproachAndWait(this.wander3)));
		return roaming;
	}
	*/
	public Transform wander1;
	public Transform wander2;
	public Transform wander3;
	public Transform wander4;
	public Transform wanderFight1;
	public Transform wanderFight2;
	public Transform wanderFinal1;
	public Transform wanderFinal2;

	public GameObject participant;
	public GameObject participant2;
	public GameObject participant3;

	public GameObject doorKnob;
	public InteractionObject ikDoorKnob;
	public GameObject door;
	public InteractionObject pointPoint1;
	public InteractionObject pointPoint2;

	public FullBodyBipedEffector participantRightHand;

	private BehaviorAgent behaviorAgent;
	private Animator anim;
	private Animator anim2;

	private GameObject fighterParticipant;
	private GameObject fighterParticipant2;
	
	private Transform finalPoint;

	private float height = 1.50f;

	private static int userInput = 0;

	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	protected Node ST_ApproachAndWait(GameObject participant, Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
	}

	protected Node Node_OpenDoor() {

		return new Sequence(this.ST_ApproachAndWait(participant, wander1), 
							participant.GetComponent<BehaviorMecanim>().ST_TurnToFace(Val.V(() => doorKnob.transform.position)),
							participant.GetComponent<BehaviorMecanim>().Node_StartInteraction(participantRightHand, ikDoorKnob),
							new LeafInvoke(() => {
								anim = door.GetComponent<Animator>();
								//anim2 = doorKnob.GetComponent<Animator>();
								if (anim.GetBool("IsOpening") == false) {
								anim.SetBool("IsOpening", true);
								//anim2.SetBool("isOpened", true);
								}
							}),
							new LeafWait(1000),
							participant.GetComponent<BehaviorMecanim>().Node_StopInteraction(participantRightHand));

	}

	protected Node Node_Conversation(GameObject participantOne, GameObject participantTwo, Transform target1, Transform target2) {

		return new Sequence(new SequenceParallel(
													this.ST_ApproachAndWait(participantOne, target1),
													this.ST_ApproachAndWait(participantTwo, target2)
								),
							new Sequence(
													participantOne.GetComponent<BehaviorMecanim>().ST_TurnToFace(Val.V(() => participantTwo.transform.position)),
													participantTwo.GetComponent<BehaviorMecanim>().ST_TurnToFace(Val.V(() => participantOne.transform.position))
							));
	}

	protected Node Node_PointAndFear(GameObject participantOne, GameObject participantTwo, GameObject participantThree, 
									 FullBodyBipedEffector rightHand, InteractionObject pointPoint) {

		this.fighterParticipant = participantTwo;
		this.fighterParticipant2 = participantThree;

		// first handle ik pointing
		return new Sequence(participantOne.GetComponent<BehaviorMecanim>().ST_TurnToFace(Val.V(() => pointPoint.transform.position)),
							new LeafInvoke(() => {
								var dir = participantTwo.transform.position - participantOne.transform.position;
								dir.Normalize();
								var pos = participantOne.transform.position + dir;
								pos.y = 1.46f;
								pointPoint.transform.position = pos;
							}),
							new SequenceParallel(
								participantOne.GetComponent<BehaviorMecanim>().Node_StartInteraction(rightHand, pointPoint),
								participantTwo.GetComponent<BehaviorMecanim>().ST_TurnToFace(Val.V(() => participantOne.transform.position)),
								participantThree.GetComponent<BehaviorMecanim>().ST_TurnToFace(Val.V(() => participantOne.transform.position))
								/*
								new LeafInvoke(() => {
									Animator _anim = participantTwo.GetComponent<Animator>();
									//_anim.Play("Hand.HandGesture.StayAway", 1);
									_anim.SetTrigger("H_StayAway");
								})
								*/
								),
							new LeafWait(500),
							participantOne.GetComponent<BehaviorMecanim>().Node_StopInteraction(rightHand));
	}

	protected Node FearAndStance(GameObject participantOne, GameObject participantTwo) {

		// participant one is scared participant
		// parcticipant two is fighting participant

		return new Sequence(new LeafInvoke(() => {
												Animator _anim = participantOne.GetComponent<Animator>();
												Animator _anim2 = participantTwo.GetComponent<Animator>();
												//_anim.Play("Hand.HandGesture.StayAway", 1);
												_anim.SetTrigger("B_Idle_Fight");
												_anim2.SetTrigger("B_Idle_Fight");

												//participantOne.GetComponent<BehaviorMecanim>().Node_BodyAnimation("Idle_Fight", true);
												//participantTwo.GetComponent<BehaviorMecanim>().Node_BodyAnimation("Idle_Fight", true);
											}));

	}

	protected Node Battle(GameObject participant, GameObject fighterParticipant, Transform target1, Transform target2) {

		Animator _anim = participant.GetComponent<Animator>();
		Animator _anim2 = fighterParticipant.GetComponent<Animator>();

		return new Sequence(
						new Sequence(
								fighterParticipant.GetComponent<BehaviorMecanim>().Node_FaceAnimation("Roar", true),
								new LeafWait(1000),
								fighterParticipant.GetComponent<BehaviorMecanim>().Node_FaceAnimation("Roar", false)),
						new LeafInvoke(() => {
								//_anim = fighterParticipant.GetComponent<Animator>();
								_anim2.ResetTrigger("B_Idle_Fight");
							}),
						new SequenceParallel(
								this.ST_ApproachAndWait(participant, target1)),
						new Sequence(
								new LeafInvoke(() => {
									//_anim = participant.GetComponent<Animator>();
									_anim.SetTrigger("B_Idle_Fight");
								}),
								this.ST_ApproachAndWait(fighterParticipant, target2),
								new LeafInvoke(() => {
									//_anim = fighterParticipant.GetComponent<Animator>();
									_anim2.SetTrigger("B_Idle_Fight");
									_anim.ResetTrigger("B_Idle_Fight");
								}),
								new Sequence(
										//participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation("RoundHouse", true),
										new SequenceParallel(
													new LeafInvoke(() => {
															_anim.SetBool("B_RoundHouse", true);
															_anim2.SetBool("B_DeathFlip", true);})),
										new LeafWait(1000),
										new LeafInvoke(() => {
											_anim.SetBool("B_RoundHouse", false);
										})
										//participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation("RoundHouse", false)
								)));

	}

	protected Node Finisher(GameObject participantOne, GameObject participantTwo) {

		Animator _anim = participantOne.GetComponent<Animator>();
		Animator _anim2 = participantTwo.GetComponent<Animator>();

		return new Sequence(
							new LeafInvoke(() => {
										_anim2.SetBool("B_Idle_Fight", false);
							}),
							new LeafWait(1000),
							new Sequence(
										new LeafInvoke(() => {
												_anim2.SetBool("B_Duck", true);
										})));

	}

	protected Node UserInput(GameObject participantOne, GameObject participantTwo, Transform target1, Transform target2) {

		// target1 is the walk up position
		// target2 is the option to leave

		var input = -1;

		return new Sequence(
							//ST_ApproachAndWait(participantOne, target1),
							participantOne.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(Val.V(() => participantTwo.transform.position), Val.V(() => 1.0f)),
							participantOne.GetComponent<BehaviorMecanim>().ST_TurnToFace(Val.V(() => participantTwo.transform.position)),
							new DecoratorInvert(
								new DecoratorLoop(
									new Sequence(
										new LeafInvoke(() => {
											//input = -1;

											if (Input.GetKeyDown(KeyCode.A)) {
												input = 0;
											}
											if (Input.GetKeyDown(KeyCode.D)) {
												input = 1;
											}

											if (input >= 0 && input < 2) {

												userInput = input;

												return RunStatus.Failure;												

											} else {

												return RunStatus.Running;

											}
										}),
										new LeafInvoke(() => {
											Debug.Log("Waiting...");
										})
									)
								)
							),
							FinalArc(participantOne, participantTwo, target1, input));

	}

	protected Node FinalArc(GameObject participantOne, GameObject participantTwo, Transform target1, int userInput) {

		Animator _anim = participantOne.GetComponent<Animator>();
		Animator _anim2 = participantTwo.GetComponent<Animator>();

		Debug.Log("userInput: " + userInput);

		if (userInput == 0) {

			Debug.Log("inside this one boss, userInsput: " + userInput);

			return new Sequence(
								ST_ApproachAndWait(participantOne, target1)
			);

		}

		Debug.Log("animations aren't runnning");

		return new Sequence(
							new SequenceParallel(
										//participantOne.GetComponent<BehaviorMecanim>().Node_BodyAnimation("RoundHouse", true),
										//participantTwo.GetComponent<BehaviorMecanim>().Node_BodyAnimation("DeathFlipped", true)
										new LeafInvoke(() => {
											_anim.SetBool("B_RoundHouse", true);
											_anim2.SetBool("B_Duck", false);
											print("did it");
										}),
										new LeafInvoke(() => {
											_anim2.SetBool("B_DeathFlip", true);
										})
							),
							new LeafWait(1000),
							new LeafInvoke(() => {
									_anim.SetBool("B_RoundHouse", false);
							}),
							ST_ApproachAndWait(participantOne, target1)
		);
	}

	protected Node BuildTreeRoot()
	{
		Node roaming = new DecoratorLoop (new Sequence(
							/*
							new SequenceShuffle(
								this.ST_ApproachAndWait(this.wander1),
								this.ST_ApproachAndWait(this.wander2),
								this.ST_ApproachAndWait(this.wander3))
							*/
							//new Sequence(Node_OpenDoor(), ST_ApproachAndWait(participant, this.wander2), Node_Conversation(participant2, participant3, wander3, wander4))
							new SequenceParallel(
										new Sequence(Node_OpenDoor(), ST_ApproachAndWait(participant, this.wander2)),
										new Sequence(Node_Conversation(participant2, participant3, wander3, wander4))
							),
							new LeafWait(100),
							new SequenceShuffle(
										Node_PointAndFear(participant, participant2, participant3, participantRightHand, pointPoint1),
										Node_PointAndFear(participant, participant3, participant2, participantRightHand, pointPoint2)),
							new Sequence(
										FearAndStance(fighterParticipant, fighterParticipant2),
										Battle(participant, fighterParticipant, wanderFight1, wanderFight2),
										new LeafWait(1000),
										Finisher(participant, fighterParticipant2),
										UserInput(participant, fighterParticipant2, wanderFinal1, wanderFinal2))));

		return roaming;
	}
}

using UnityEngine;
using System.Collections;

public class MasterBehaviour : MonoBehaviour {

	public Vector3 poi { get; set; }  //point of interest that he reaches goal on
	public float health { get; set; }
	public bool seesPlayer { get; set; }
	public bool seesDeadPeople { get; set; }
	public bool hearsSomething { get; set; }
	public bool isDead { get; set; }

	public ReachGoal reachGoal { get; set; }
	private Wander wander;

	// Use this for initialization
	public void Starta (GameObject plane, GameObject swamps, float nodeSize) {

		poi = Vector3.zero;
		health = 100.0f;
		seesPlayer = false;
		seesDeadPeople = false;
		hearsSomething = false;

		reachGoal = GetComponent<ReachGoal> ();
		wander = GetComponent<Wander> ();

		reachGoal.plane = plane;
		reachGoal.swamps = swamps;
		reachGoal.nodeSize = nodeSize;
		reachGoal.goalPos = poi;
		reachGoal.Starta ();
		wander.Starta ();

//		Debug.Log (transform.name);
	}

	public void Updatea(){
		//decision tree later for different combination of senses being true
		if (isDead) {
			return;
		}
		if (!(seesPlayer || seesDeadPeople || hearsSomething)) {
//			Debug.Log ("Wander");
			wander.Updatea ();
		} else {
			Debug.Log("Update GoalPos to: " + reachGoal.goalPos);
			reachGoal.goalPos = poi;
		}
		//reaching goal is done with pathfinding which is handled by the pathfinder schedule and the masterscheduler
	}

	public bool isReachingGoal(){
		return (seesPlayer || seesDeadPeople || hearsSomething);
	}

	public void getHit(int damage) {
		if (isDead) {
			return;
		}
		reachGoal.getHit (damage);
		if (damage >= 3) {
			isDead = true;
		}
	}
}

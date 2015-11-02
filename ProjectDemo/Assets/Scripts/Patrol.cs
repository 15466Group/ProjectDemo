using UnityEngine;
using System.Collections;

public class Patrol : NPCBehaviour {
	
	public int framesTillTurnAround;
	private int frameCount;
	public Vector3 direction;

	public override void Starta () {
		base.Starta ();
		frameCount = 0;
		direction = new Vector3 (direction.x, 0.0f, direction.z).normalized * 10.0f;
	}
	
	public override void Updatea () {
		if (frameCount < framesTillTurnAround) {
			target = transform.position + direction;
		} else if (frameCount < 2 * framesTillTurnAround) {
			target = transform.position + (-1.0f) * direction;
		} else {
			frameCount = 0;
		}
		frameCount++;
		base.Updatea();
	}
}

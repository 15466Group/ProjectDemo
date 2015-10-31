using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scheduler : MonoBehaviour {
	
	public GameObject goal;
	public GameObject characters; //empty gameobject containing children of in game characters
	public GameObject plane;
	public GameObject swamps;
	public float nodeSize;

//	private Grid G;
	private Graph graph;
	private ReachGoal reachGoal;

	//each soldier has complete control for one frame 
	private int iChar;
	private int numChars;

	private State[] states;

	// Use this for initialization
	void Start () {
		iChar = 0;
		numChars = characters.transform.childCount;
		graph = new Graph (2.0f);
		states = new State[numChars];
		for (int i = 0; i < numChars; i++) {
			Transform child = characters.transform.GetChild(i);
			reachGoal = child.GetComponent<ReachGoal> ();
			reachGoal.Start();
			Grid G = new Grid(plane, goal, nodeSize, swamps);
			G.initStart();
			states[i] = new State(new List<Node> (), new List<Node> (), new Dictionary<Node, Node> (),
			                      null, null, reachGoal.swampCost, G, null, false, false);
		}
		if (swamps != null) {
			int swampCount = swamps.transform.childCount;
			for (int k = 0; k < swampCount; k++) {
				swamps.transform.GetChild (k).GetComponent<MeshCollider> ().enabled = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		//every char is moving at each frame, but at frame i % numChars,
		//	char i is moving according to new graph and everyone else is moving according to their 'current' graphs

		Transform currChar = characters.transform.GetChild (iChar);

		reachGoal = currChar.GetComponent<ReachGoal> ();
		Vector3 start = currChar.transform.position;
		Vector3 end = goal.transform.position;

		//graph sets a threshold for how many nodes to search.
		//if the threshold is hit before the goalNode is found, graphsearch returns an estimated path
		//and ongoing is set to true.
		//otherwise, ongoing is set to false, and continue searching as everything is initalized
		State s = states [iChar];
		graph.setGrid(s.sGrid);
		s.sGrid.updateGrid();
		if (!s.ongoing) {
			Vector3 startCoords = s.sGrid.getGridCoords(currChar.position);
			int startI = (int)startCoords.x;
			int startJ = (int)startCoords.z;
			s.startNode = s.sGrid.grid [startI, startJ];
			s.startNode.g = 0.0f;
			s.startNode.f = s.startNode.g + graph.weight * s.startNode.h;
			s.open.Add (s.startNode);
		}
		Vector3 endCoords = s.sGrid.getGridCoords (goal.transform.position);
		int endI = (int)endCoords.x;
		int endJ = (int)endCoords.z;
		s.endNode = s.sGrid.grid [endI, endJ];
		Vector3 charCoords = s.sGrid.getGridCoords (currChar.position);
		int charI = (int)charCoords.x;
		int charJ = (int)charCoords.z;

		//graph needs to know the current characters position
		graph.setCharNode (s.sGrid.grid [charI, charJ]);

		states[iChar] = graph.getPath (s);
		List<Node> path = states[iChar].path;
		reachGoal.assignedPath (path);

		//regardless of which character's graph search turn it is, move all of them
		for (int i = 0; i < numChars; i++) {
			State s_i = states[i];
			Transform child = characters.transform.GetChild(i);
			reachGoal = child.GetComponent<ReachGoal> ();
			reachGoal.assignGridCoords (s_i.sGrid.getGridCoords(reachGoal.next), 
			                            s_i.sGrid.getGridCoords(child.transform.position),
			                            s_i.sGrid.getGridCoords(goal.transform.position));

			//character has removed this node from its path so remove it from the dictionary
			Node r;
			r = reachGoal.nextStep ();
			if (r != null){
				foreach (Node key in states[i].dictPath.Keys){
					if (Node.Equals(states[i].dictPath[key], r)){
						states[i].dictPath.Remove(key);
						break;
					}
				}
			}
			 
		}
		iChar = (iChar + 1) % numChars;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControlScript : MonoBehaviour {
	/* This script controls a door.
	 * A door is assumed to be closed when it spawns. 
	 * If the door is actually open when it spawns, just use the opposite Direction in SetOpeningAxis()
	 */

	public enum DoorState {
		OPENING, CLOSING, OPEN, CLOSED
	};

	public enum OpenDirection {
		POS_X, NEG_X, POS_Z, NEG_Z

	};

	private Rigidbody rb;
	private DoorState state = DoorState.CLOSED;
	private Vector3 closed_position;
	private OpenDirection opening_direction; // The direction the door should move to open.
	private Vector3 open_position;
	private const float OPENING_SPEED = 1.0f; // m/s

	private void Start () {
		rb = GetComponent<Rigidbody> ();
		closed_position = transform.position;
		ComputeOpenPosition ();
	}

	private void Update () {
		if (state == DoorState.OPENING) {
			Vector3 difference = open_position - transform.position;
			if (difference.magnitude <= OPENING_SPEED * Time.deltaTime) {
				rb.MovePosition (open_position);
				state = DoorState.OPEN;
			} else {
				rb.MovePosition (transform.position + GetDirectionVector (opening_direction) * Time.deltaTime);
			}

		} else if (state == DoorState.CLOSING) {
			Vector3 difference = closed_position - transform.position;
			if (difference.magnitude <= OPENING_SPEED * Time.deltaTime) {
				rb.MovePosition (closed_position);
				state = DoorState.CLOSED;
			} else {
				rb.MovePosition (transform.position - GetDirectionVector(opening_direction) * Time.deltaTime);
			}

		}
	}

	public void SetOpeningAxis (OpenDirection direction) {
		opening_direction = direction;
		ComputeOpenPosition ();
	}

	// Initiates the process of opening the door. 
	public void OpenDoor () {
		if (opening_direction != null) {
			state = DoorState.OPENING;
		} else {
			Debug.Log ("OpenDoor() called before opening_direction was set.");
		}
	}

	// Initiates the process of closing the door.
	public void CloseDoor () {
		if (opening_direction != null) {
			state = DoorState.CLOSING;
		} else {
			Debug.Log ("CloseDoor() called before opening_direction was set.");
		}
	}

	private static Vector3 GetDirectionVector (OpenDirection d) {
		if (d == OpenDirection.NEG_X)
			return Vector3.left;
		else if (d == OpenDirection.POS_X)
			return Vector3.right;
		else if (d == OpenDirection.NEG_Z)
			return Vector3.back;
		else if (d == OpenDirection.POS_Z)
			return Vector3.forward;
		else
			throw new UnityException ("Bad OpenDirection: " + d);
	}

	private void ComputeOpenPosition () {
		if (opening_direction == OpenDirection.POS_X || opening_direction == OpenDirection.NEG_X)
			open_position = closed_position + GetDirectionVector(opening_direction) * transform.localScale.x;
		else if (opening_direction == OpenDirection.POS_Z || opening_direction == OpenDirection.NEG_Z) {
			open_position = closed_position + GetDirectionVector(opening_direction) * transform.localScale.z;
		}
	}
}

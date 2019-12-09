using UnityEngine;
using System.Collections;

public class CharacterTrail : MonoBehaviour
{
	public bool directMode = true;

	public Character character = null;
	public CharacterTrail previousTrail = null;

	public float spacing = 0.25f;
	public float targetDistSq = 0.1f;
	public Vector3 targetPos = Vector3.zero;

	private void Awake()
	{
		if (character != null)
			targetPos = previousTrail != null ? previousTrail.transform.position : character.transform.position;
	}

	public void UpdatePosition()
	{
		if (character == null)
		{
			Destroy(gameObject);
			return;
		}

		if (directMode || previousTrail == null)
		{
			Vector3 precursorPos = previousTrail != null ? previousTrail.transform.position : character.transform.position;
			Vector3 targetDir = (precursorPos - transform.position).normalized;
			Vector3 newPos = precursorPos - targetDir * spacing;
			
			transform.position = newPos;
		}
		else
		{
			float distSq = Vector3.SqrMagnitude(transform.position - targetPos);
			if (distSq < targetDistSq)
			{
				targetPos = previousTrail.targetPos;
			}
			Vector3 targetDir = (targetPos - transform.position).normalized;
			Vector3 newPos = transform.position + targetDir * character.speed * Time.deltaTime;

			transform.position = newPos;
		}
	}

	public void InvokeDeath()
	{
		Debug.Log($"Ouch! {name}");
		if (character != null) character.CutTrail(this);
	}
}

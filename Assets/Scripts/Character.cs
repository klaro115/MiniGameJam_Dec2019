using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
	[Serializable]
	public struct CharSprite
	{
		public Sprite sprite;
		public Vector3 dir;
	}

	[Header("Character Settings")]
	public World world = null;

	public float headRadius = 0.25f;
	public float speed = 3.0f;
	protected Vector3 curMoveDir = Vector3.up;

	[Range(0, 16)]
	public int trailStartLength = 5;
	public CharacterTrail trailPrefab = null;
	public List<CharacterTrail> trail = new List<CharacterTrail>();

	public CharSprite[] sprites = new CharSprite[4]
	{
		new CharSprite() { dir = Vector3.up },
		new CharSprite() { dir = Vector3.down },
		new CharSprite() { dir = Vector3.left },
		new CharSprite() { dir = Vector3.right }
	};
	public SpriteRenderer spriteRenderer = null;

	protected AudioSource audioSrc = null;


	public int Length => trail != null ? trail.Count + 1 : 1;


	protected virtual void Start()
	{
		if (world == null) world = World.Main;
		if (trail == null) trail = new List<CharacterTrail>(10);
		audioSrc = GetComponent<AudioSource>();
		if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>(false);

		if (trailPrefab != null && trailStartLength > 0)
		{
			CharacterTrail prevTrail = null;
			for (int i = 0; i < trailStartLength; ++i)
			{
				CharacterTrail ct = Instantiate<CharacterTrail>(trailPrefab, world.transform);
				ct.character = this;
				ct.previousTrail = prevTrail;
				ct.transform.position = transform.position - curMoveDir * i * ct.spacing;
				trail.Add(ct);
				prevTrail = ct;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, headRadius);
	}

	void Update()
	{
		if (world == null) return;

		UpdateBehaviour();

		UpdateMovement();
	}

	protected abstract void UpdateBehaviour();

	protected virtual void UpdateMovement()
	{
		transform.Translate(curMoveDir * Time.deltaTime * speed);

		foreach (CharacterTrail ct in trail)
		{
			ct.UpdatePosition();
		}

		if (spriteRenderer != null && sprites != null)
		{
			float bestDot = -1;
			Sprite sprite = null;
			foreach (CharSprite cs in sprites)
			{
				if (cs.sprite != null)
				{
					float dot = Vector3.Dot(curMoveDir, cs.dir);
					if (dot > bestDot)
					{
						sprite = cs.sprite;
						bestDot = dot;
					}
				}
			}
			spriteRenderer.sprite = sprite;
		}
	}

	public virtual void InvokeDeath()
	{
		if (trail != null)
		{
			foreach (CharacterTrail ct in trail.ToArray())
			{
				Destroy(ct.gameObject);
			}
			trail.Clear();
		}

		Destroy(gameObject);
	}

	public void CutTrail(CharacterTrail trailPart)
	{
		if (trail == null || trailPart == null) return;

		int index = trail.IndexOf(trailPart);
		if (index < 0 || index >= trail.Count) return;

		int restCount = trail.Count - index;
		CharacterTrail[] rest = trail.GetRange(index, restCount).ToArray();
		trail.RemoveRange(index, restCount);
		if (rest != null)
		{
			foreach (CharacterTrail ct in rest)
			{
				Destroy(ct.gameObject);
			}
		}

		if (Length < 1) InvokeDeath();
	}
}

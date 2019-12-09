using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
	private Camera cam = null;
	private Collider2D coll = null;

	public Transform gun = null;
	public SpriteRenderer laser = null;
	public AudioClip[] laserSfx = new AudioClip[1] { null };

	public float gunRotationOffset = 90.0f;
	public int laserFrameDuration = 3;
	private int laserLifetime = 0;
	

	protected override void Start()
    {
		base.Start();

		cam = Camera.main;
		coll = GetComponent<Collider2D>();
	}

	private void OnDrawGizmos()
	{
		Vector3 fireDir = gun.up;
		Vector3 firePos = gun.position + fireDir * headRadius;

		Gizmos.DrawSphere(firePos, 0.15f);
		Gizmos.DrawLine(firePos, fireDir * 20);
	}

	private bool GetAxis(KeyCode a, KeyCode b)
	{
		return Input.GetKeyDown(a) || Input.GetKeyDown(b);
	}

	protected override void UpdateBehaviour()
    {
		if (GetAxis(KeyCode.UpArrow, KeyCode.W)) curMoveDir = Vector3.up;
		else if (GetAxis(KeyCode.DownArrow, KeyCode.S)) curMoveDir = Vector3.down;
		else if (GetAxis(KeyCode.LeftArrow, KeyCode.A)) curMoveDir = Vector3.left;
		else if (GetAxis(KeyCode.RightArrow, KeyCode.D)) curMoveDir = Vector3.right;

		if (laser != null && laser.enabled)
		{
			laserLifetime++;
			if (laserLifetime > laserFrameDuration)
			{
				laser.enabled = false;
			}
		}
		if (gun != null)
		{
			Vector3 mousePos = Input.mousePosition;
			Vector3 aimPos = cam.ScreenToWorldPoint(mousePos);
			aimPos.z = 0.0f;
			Vector3 aimDir = (aimPos - gun.position).normalized;
			float gunAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg + gunRotationOffset;
			gun.eulerAngles = new Vector3(0, 0, gunAngle);
			
			if (Input.GetMouseButtonDown(0)) Fire();
		}
    }

	protected override void UpdateMovement()
	{
		base.UpdateMovement();

		if(world.IsOutsideBorders(transform.position))
		{
			InvokeDeath();
		}
	}

	public void Fire()
	{
		const float laserRange = 20.0f;

		Vector3 fireDir3 = gun.up;
		Vector3 firePos3 = gun.position + fireDir3 * headRadius;
		Vector2 fireDir = new Vector2(fireDir3.x, fireDir3.y);
		Vector2 firePos = new Vector2(firePos3.x, firePos3.y);
		RaycastHit2D hit = Physics2D.Raycast(firePos, fireDir, laserRange);

		Vector3 laserPos = firePos + fireDir * laserRange * 0.5f;
		float laserLength = laserRange;

		if (hit.collider != null && hit.collider != coll)
		{
			hit.collider.SendMessage("InvokeDeath", SendMessageOptions.DontRequireReceiver);
			laserPos = 0.5f * (firePos3 + new Vector3(hit.point.x, hit.point.y, 0));
			laserLength = hit.distance;
		}

		if (laser != null)
		{
			Transform trans = laser.transform;
			trans.position = laserPos;
			//trans.localScale = new Vector3(1, laserLength, 1);
			trans.up = fireDir;
			laser.enabled = true;
			laser.size = new Vector2(1, laserLength);
			laserLifetime = 0;
		}
		if (laserSfx != null && audioSrc != null)
		{
			AudioClip laserSound = laserSfx[Random.Range(0, laserSfx.Length)];
			audioSrc.PlayOneShot(laserSound);
		}
	}
}

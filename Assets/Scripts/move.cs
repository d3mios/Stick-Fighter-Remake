using System;
using System.Linq;
using UnityEngine;
using Characters;
using PlayerProfiles;
using System.Collections.Generic;

namespace Moves {
	public class Hitbox : MonoBehaviour {
		protected int height;
		protected int width;
		protected int duration;
		protected Vector3 position;
		protected character user;
		protected List<character> hitList;
		protected bool active;
		protected float alpha = 0.5f;

		public virtual void Initialize(int height_, int width_, int duration_, Vector3 position_, character user_, List<character> hitList_ = null) {
			height = height_;
			width = width_;
			duration = duration_;
			position = position_;
			active = true;
			user = user_;
			if (hitList != null) hitList = hitList_;
			else hitList = new List<character>();
			GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
			var texture = (Texture2D)Resources.Load("Sprites/solidWhite");
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0f, 0f, 1, 1), new Vector2(0.5f, 0.5f));
			GetComponent<SpriteRenderer>().color = new Color(1, 0, 1, alpha);
			GetComponent<SpriteRenderer>().enabled = true;
			transform.localScale = new Vector3(width, height, 1);
			transform.position = position + user.transform.position;
			hitboxList.list.Add(this);
		}
		public virtual bool isProjectile() { return false; }
		public virtual void hit(ref float hp, ref float hsp, ref float vsp, ref int hitlag, bool shielded, character charac, ref bool hardHit) {
			hitList.Add(charac);
			hardHit = false;
		}

		public bool isMine(character charac) { return (charac == user); }

		public void updatePosition(Vector3 position_) {
			position = position_;
		}

		public void deactivate() {
			active = false;
			GetComponent<Renderer>().enabled = false;
		}

		public void activate() {
			active = true;
			GetComponent<Renderer>().enabled = true;
		}

		public void delete() {
			hitboxList.list.Remove(this);
			DestroyImmediate(this.gameObject);
		}

		void FixedUpdate() {
			transform.position = position + user.transform.position;
			duration--;
			if (duration == 0) {
				delete();
			}
		}
	}
	
	public class HitboxDmg : Hitbox {
		protected int dmg;
		protected int stunFrames, shieldStun, priority;
		protected float baseKb, angle;
		protected bool projectile, destroyOnHit;
		protected int hitRate;
		protected int refreshCounter;
		protected character hitCharac;
		protected bool doSideEffect;
		protected Action<character> sideEffect;
		protected Action onDestroy;
		protected bool destroy;
		protected int facing;
		protected bool hardHit;

		public void Initialize(int height_, int width_, int dmg_, float baseKb_, float angle_, int duration_, int stunFrames_, int shieldStun_, bool projectile_, Vector3 position_, character user_, int priority_, Action<character> sideEffect_ = null, int hitRate_ = -1, bool destroyOnHit_ = false, List<character> hitList_ = null) {
			height = height_;
			width = width_;
			dmg = dmg_;
			baseKb = baseKb_;
			angle = angle_;
			duration = duration_;
			stunFrames = stunFrames_;
			shieldStun = shieldStun_;
			projectile = projectile_;
			position = position_;
			active = true;
			hardHit = false;
			user = user_;
			priority = priority_;
			sideEffect = sideEffect_;
			hitRate = hitRate_;
			destroyOnHit = destroyOnHit_;
			destroy = false;
			facing = user.getFacing();
			if (hitList_ != null) hitList = hitList_;
			else hitList = new List<character>();
			GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
			var texture = (Texture2D)Resources.Load("Sprites/solidWhite");
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0f, 0f, 1, 1), new Vector2(0.5f, 0.5f));
			GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, alpha);
			GetComponent<SpriteRenderer>().enabled = true;
			transform.localScale = new Vector3(width, height, 1);
			transform.position = position + user.transform.position;
			hitboxList.list.Add(this);
		}

		public override void hit(ref float hp, ref float hsp, ref float vsp, ref int hitlag, bool shielded, character charac, ref bool hardHit) {
			if (hitList.Contains(charac)) return;
			hardHit = this.hardHit;
			if (shielded) {
				hp -= dmg;
				if (baseKb != 0) {
					hsp = baseKb * facing * (float)Math.Cos(angle*((float)Math.PI / 180f));
					vsp = baseKb * (float)Math.Sin(angle*((float)Math.PI / 180f));
					hsp *= 0.2f;
					vsp *= 0.2f;
					hardHit = false;
				}
				if (shieldStun != 0) hitlag = shieldStun;
			}
			else {
				hp -= dmg;
				if (baseKb != 0) {
					hsp = baseKb * facing * (float)Math.Cos(angle*((float)Math.PI / 180f));
					vsp = baseKb * (float)Math.Sin(angle*((float)Math.PI / 180f));
				}
				if (stunFrames != 0) hitlag = stunFrames * (int) ((Math.Abs(hsp) + Math.Abs(vsp)) / 6);
			}
			if (sideEffect != null) {
				doSideEffect = true;
				hitCharac = charac;
			}
			if (destroyOnHit) destroy = true;
			charac.getHit();
			hitList.Add(charac);
		}

		public override bool isProjectile() { return projectile; }

		void FixedUpdate () {
			transform.position = new Vector3(user.transform.position.x, user.transform.position.y, 0) + position;
			duration--;
			if (hitRate != -1) {
				refreshCounter++;
				if (refreshCounter == hitRate) {
					refreshCounter = 0;
					hitList.Clear();
				}
			}
			if (doSideEffect) {
				sideEffect.Invoke(hitCharac);
				doSideEffect = false;
			}
			if (duration == 0 || destroy) {
				delete();
			}
		}
	}

	public class FSpecialHitbox : HitboxDmg {
		private float strength;
		public void Initialize(int height_, int width_, int dmg_, float baseKb_, float angle_, int duration_, int stunFrames_, int shieldStun_, bool projectile_, Vector3 position_, character user_, int priority_, Action<character> sideEffect_ = null, int hitRate_ = -1, bool destroyOnHit_ = false, List<character> hitList_ = null) {
			height = height_;
			width = width_;
			dmg = dmg_;
			strength = 1;
			baseKb = baseKb_;
			angle = angle_;
			duration = duration_;
			stunFrames = stunFrames_;
			shieldStun = shieldStun_;
			projectile = projectile_;
			position = position_;
			active = true;
			hardHit = true;
			user = user_;
			priority = priority_;
			sideEffect = sideEffect_;
			hitRate = hitRate_;
			destroyOnHit = destroyOnHit_;
			destroy = false;
			facing = user.getFacing();
			if (hitList_ != null) hitList = hitList_;
			else hitList = new List<character>();
			GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
			var texture = (Texture2D)Resources.Load("Sprites/solidWhite");
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0f, 0f, 1, 1), new Vector2(0.5f, 0.5f));
			GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, alpha);
			GetComponent<SpriteRenderer>().enabled = true;
			transform.localScale = new Vector3(width, height, 1);
			transform.position = position + user.transform.position;
			hitboxList.list.Add(this);
		}
		public override void hit(ref float hp, ref float hsp, ref float vsp, ref int hitlag, bool shielded, character charac, ref bool hardHit) {
			if (hitList.Contains(charac)) return;
			hardHit = this.hardHit;
			if (shielded) {
				hp -= dmg*strength;
				if (baseKb != 0) {
					hsp = strength*baseKb * facing * (float)Math.Cos(angle*((float)Math.PI / 180f));
					vsp = strength*baseKb * (float)Math.Sin(angle*((float)Math.PI / 180f));
					hsp *= 0.2f;
					vsp *= 2.5f;
				}
				if (shieldStun != 0) hitlag = shieldStun;
			}
			else {
				hp -= dmg*strength;
				if (baseKb != 0) {
					hsp = strength*baseKb * facing * (float)Math.Cos(angle*((float)Math.PI / 180f));
					vsp = strength*baseKb * (float)Math.Sin(angle*((float)Math.PI / 180f));
				}
				if (stunFrames != 0) hitlag = stunFrames * (int) ((Math.Abs(hsp) + Math.Abs(vsp)) / 6);
			}
			if (sideEffect != null) {
				doSideEffect = true;
				hitCharac = charac;
			}
			if (destroyOnHit) destroy = true;
			charac.getHit();
			hitList.Add(charac);
		}
		void FixedUpdate () {
			transform.position = new Vector3(user.transform.position.x, user.transform.position.y, 0) + position;
			duration--;
			strength -= 0.02f;
			if (hitRate != -1) {
				refreshCounter++;
				if (refreshCounter == hitRate) {
					refreshCounter = 0;
					hitList.Clear();
				}
			}
			if (doSideEffect) {
				sideEffect.Invoke(hitCharac);
				doSideEffect = false;
			}
			if (duration == 0 || destroy) {
				delete();
			}
		}
	}

	public class HitboxProjectile : HitboxDmg {
		protected Projectile follow;
		public void Initialize(int height_, int width_, int dmg_, float baseKb_, float angle_, int duration_, int stunFrames_, int shieldStun_, bool projectile_, Vector3 position_, character user_, Projectile follow_, int priority_, Action<character> sideEffect_ = null, int hitRate_ = -1, bool destroyOnHit_ = false, List<character> hitList_ = null) {
			doSideEffect = false;
			hitCharac = null;
			height = height_;
			width = width_;
			dmg = dmg_;
			baseKb = baseKb_;
			angle = angle_;
			duration = duration_;
			stunFrames = stunFrames_;
			shieldStun = shieldStun_;
			projectile = projectile_;
			position = position_;
			active = true;
			hardHit = false;
			user = user_;
			follow = follow_;
			priority = priority_;
			sideEffect = sideEffect_;
			refreshCounter = 0;
			hitRate = hitRate_;
			destroyOnHit = destroyOnHit_;
			destroy = false;
			facing = follow.getFacing();
			if (hitList_ != null) hitList = hitList_;
			else hitList = new List<character>();
			GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
			var texture = (Texture2D)Resources.Load("Sprites/solidWhite");
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0f, 0f, 1, 1), new Vector2(0.5f, 0.5f));
			GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, alpha);
			GetComponent<SpriteRenderer>().enabled = true;
			transform.localScale = new Vector3(width, height, 1);
			transform.position = position;
			hitboxList.list.Add(this);
		}

		void FixedUpdate () {
			transform.position = follow.transform.position;
			
			duration--;
			if (hitRate != -1) {
				refreshCounter++;
				if (refreshCounter == hitRate) {
					refreshCounter = 0;
					hitList.Clear();
				}
			}
			if (doSideEffect) {
				sideEffect.Invoke(hitCharac);
				doSideEffect = false;
			}
			if (duration == 0 || destroy) {
				delete();
				if (destroy && follow != null) follow.delete();
			}
		}
	}
	
	public class Projectile : MonoBehaviour {
		protected float vsp, hsp;
		protected int duration;
		protected int facing;
		protected GameObject hb1;
		protected Action onDestroy;
		protected character followPlayer;
		protected Vector3 offset;
		public Animator animator;
		public RuntimeAnimatorController animController;
		public void Initialize(int height_, int width_, int dmg_, float baseKb_, float angle_, int duration_, int stunFrames_, int shieldStun_, float vsp_, float hsp_, Vector3 position_, Vector3 offset_, character user_, int priority_, Action<character> sideEffect_ = null, int facing_ = 0, Action onDestroy_ = null, int hitRate_ = -1, bool destroyOnHit_ = false, character followPlayer_ = null) {
			if (facing_ != 0) facing = facing_;
			else facing = user_.getFacing();
			vsp = vsp_;
			hsp = hsp_ * facing;
			duration = duration_;
			offset = offset_;
			followPlayer = followPlayer_;
			if (followPlayer != null) transform.position = position_ + new Vector3(followPlayer.getFacing() * -offset.x, offset.y, offset.z);
			else transform.position = position_ + offset;
			onDestroy = onDestroy_;

			if (gameObject.GetComponent<SpriteRenderer>() == null) gameObject.AddComponent<SpriteRenderer>();
			gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;

			hb1 = new GameObject();
            hb1.AddComponent<SpriteRenderer>();
            var hb1Script = hb1.AddComponent<HitboxProjectile>();
            hb1Script.Initialize(height_, width_, dmg_, baseKb_, angle_, duration_, stunFrames_, shieldStun_, true, position_ + offset_, user_, this, 1, sideEffect_: sideEffect_, hitRate_: hitRate_, destroyOnHit_: destroyOnHit_);
		}

		public void delete() {
			if (onDestroy != null) onDestroy.Invoke();
			if (hb1 != null) hb1.GetComponent<HitboxProjectile>().delete();
			DestroyImmediate(this.gameObject);
		}

		public int getFacing() { return facing; }
		
		protected void FixedUpdate () {
			if (followPlayer != null && followPlayer.died()) {
				delete();
				return;
			}
			if (followPlayer == null) transform.position += new Vector3(hsp * Time.fixedDeltaTime, vsp * Time.fixedDeltaTime, 0f);
			else transform.position = followPlayer.transform.position + new Vector3(followPlayer.getFacing() * -offset.x, offset.y, offset.z);
			duration--;
			if (duration == 0) {
				delete();
			}
		}
	}
	
	public static class hitboxList {
		public static List<Hitbox> list = new List<Hitbox>();
		public static void deleteMine(character charac) {
			List<Hitbox> toDelete = list.Where(x => x.isMine(charac) && !x.isProjectile()).ToList();
			list.RemoveAll(x=> x.isMine(charac) && !x.isProjectile());
			foreach (var hb in toDelete) {
				UnityEngine.Object.DestroyImmediate(hb.gameObject);
			}
		}
	}

	public class Move {
		private Action<List<character>> moveFunction;
		private List<character> hitList;
		public Move(Action<List<character>> moveFunction_) {
			this.moveFunction = moveFunction_;
			hitList = new List<character>();
		}
		public void execMove() {
			moveFunction.Invoke(hitList);
		}
	}

	public class Moveset {
		private Move[] moves;
		public Moveset(Move[] moves_) {
			moves = moves_;
		}

		public void Normal(input dir) {
			if (dir.down) moves[0].execMove();
			else if (dir.left || dir.right) moves[1].execMove();
			else moves[2].execMove();
		}

		public void Aereal(input dir) {
			if (dir.up) moves[3].execMove();
			else if (dir.down) moves[4].execMove();
			else moves[5].execMove();
		}

		public void Special(input dir) {
			if (dir.down) moves[6].execMove();
			else if (dir.left || dir.right) moves[7].execMove();
			else moves[8].execMove();
		}
	}
}


using UnityEngine;
using Hurtbox;
using PlayerProfiles;
using System.Collections.Generic;
using Moves;
using System;

namespace Characters {
    public class character : MonoBehaviour
    {
        private int facing, airdashCounter, frameCounter;
        private bool airdashing, blocking, specialCancel, hardHit;
        public character otherPlayer;
        public string profile;
        private float ground = -1.5f;
        private float hp;
        private float hsp, vsp;
        private Action<input> move;
        private input moveInput;
        private Moveset moveset;
        private hurtbox hurtbox;
        private controller player;
        private input playerInput;
        private State state;
        private enum State {
            Ground,
            Air,
            AttackAir,
            AttackGround,
            Hit,
            LandingLag
        }

        public int getFacing() { return facing; }

        public float getX() { return transform.position.x; }

        public bool died() { return hp <= 0; }

        private void Jab(List<character> hitList) {
            if (frameCounter == 3) {
                GameObject hb1 = new GameObject();
                hb1.AddComponent<SpriteRenderer>();
                var hb1Script = hb1.AddComponent<HitboxDmg>();
                hb1Script.Initialize(100, 100, 5, 10, 80, 10, 3, 10, false, new Vector3(facing*1f, 0.5f, 0), this, 1);
            }
            if (frameCounter == 15) frameCounter = 0;
        }
        
        private void FTilt(List<character> hitList) {
            if (frameCounter == 3) {
                GameObject hb1 = new GameObject();
                hb1.AddComponent<SpriteRenderer>();
                var hb1Script = hb1.AddComponent<HitboxDmg>();
                hb1Script.Initialize(100, 100, 5, 10, 80, 10, 3, 10, false, new Vector3(facing*1f, 0.5f, 0), this, 1);
            }
            if (frameCounter == 15) frameCounter = 0;
        }

        private void DTilt(List<character> hitList) {
            if (frameCounter == 3) {
                GameObject hb1 = new GameObject();
                hb1.AddComponent<SpriteRenderer>();
                var hb1Script = hb1.AddComponent<HitboxDmg>();
                hb1Script.Initialize(100, 100, 5, 10, 80, 10, 3, 10, false, new Vector3(facing*1f, 0.5f, 0), this, 1);
            }
            if (frameCounter == 15) frameCounter = 0;
        }

        private void NAir(List<character> hitList) {
            if (frameCounter == 3) {
                GameObject hb1 = new GameObject();
                hb1.AddComponent<SpriteRenderer>();
                var hb1Script = hb1.AddComponent<HitboxDmg>();
                hb1Script.Initialize(100, 100, 5, 10, 80, 10, 3, 10, false, new Vector3(facing*1f, 0.5f, 0), this, 1);
            }
            if (frameCounter == 15) frameCounter = 0;
        }

        private void DAir(List<character> hitList) {
            if (frameCounter == 3) {
                GameObject hb1 = new GameObject();
                hb1.AddComponent<SpriteRenderer>();
                var hb1Script = hb1.AddComponent<HitboxDmg>();
                hb1Script.Initialize(100, 100, 5, 10, 80, 10, 3, 10, false, new Vector3(facing*1f, 0.5f, 0), this, 1);
            }
            if (frameCounter == 15) frameCounter = 0;
        }

        private void UAir(List<character> hitList) {
            if (frameCounter == 3) {
                GameObject hb1 = new GameObject();
                hb1.AddComponent<SpriteRenderer>();
                var hb1Script = hb1.AddComponent<HitboxDmg>();
                hb1Script.Initialize(100, 100, 5, 10, 80, 10, 3, 10, false, new Vector3(facing*1f, 0.5f, 0), this, 1);
            }
            if (frameCounter == 15) frameCounter = 0;
        }

        private void DSpecial(List<character> hitList) {
            if (frameCounter == 3) {
                GameObject hb1 = new GameObject();
                hb1.AddComponent<SpriteRenderer>();
                var hb1Script = hb1.AddComponent<HitboxDmg>();
                hb1Script.Initialize(100, 100, 5, 10, 80, 10, 3, 10, false, new Vector3(facing*1f, 0.5f, 0), this, 1);
            }
            if (frameCounter == 15) frameCounter = 0;
        }

        private void FSpecial(List<character> hitList) {
            if (moveInput.left) hsp = -7;
            else if (moveInput.right) hsp = 7;
            if (frameCounter == 3) {
                GameObject hb1 = new GameObject();
                hb1.AddComponent<SpriteRenderer>();
                var hb1Script = hb1.AddComponent<FSpecialHitbox>();
                hb1Script.Initialize(200, 400, 30, 15, 40, 30, 3, 10, false, new Vector3(0, -0.5f, 0), this, 1);
            }
            if (frameCounter == 45) frameCounter = 0;
        }

        private void NSpecial(List<character> hitList) {
            if (frameCounter == 3) {
                GameObject hb1 = new GameObject();
                hb1.AddComponent<SpriteRenderer>();
                var hb1Script = hb1.AddComponent<Projectile>();
                hb1Script.Initialize(200, 200, 5, 10, 80, 300, 3, 10, 0, 4, transform.position, new Vector3(0, 0, 0), this, 1, destroyOnHit_: true);
                hb1Script.animator = hb1.AddComponent<Animator>();
                hb1Script.animController = Resources.Load<RuntimeAnimatorController>("Animations/projectile_C");
                hb1Script.animator.runtimeAnimatorController = hb1Script.animController;
            }
            if (frameCounter == 15) frameCounter = 0;
        }
        
        private void checkHit() {
            foreach (var hb in hitboxList.list) {
                if (!hb.isMine(this) && hb.GetComponent<Renderer>().bounds.Intersects(hurtbox.getBounds())) {
                    if (blocking) hb.hit(ref hp, ref hsp, ref vsp, ref frameCounter, true, this, ref hardHit);
                    else hb.hit(ref hp, ref hsp, ref vsp, ref frameCounter, false, this, ref hardHit);
                }
            }
        }

        public void getHit() {
            Debug.Log("aqui");
            /*if (hp > 0) */state = State.Hit;
        }

        private void hitUpdate() {
            blocking = false;
            vsp -= 0.85f;
            frameCounter--;
            if (frameCounter <= 0) {
                if (transform.position.y > ground) state = State.Air;
                else state = State.Ground;
            }
            if (!hardHit) {
                if (hsp > 0) hsp = Math.Max(hsp - 0.5f, 0);
                else if (hsp < 0) hsp = Math.Min(hsp + 0.5f, 0);
            }
            else {
                if (hsp > 0) hsp = Math.Max(hsp - 0.2f, 0);
                else if (hsp < 0) hsp = Math.Min(hsp + 0.2f, 0);
            }
        }

        private void landingLagUpdate() {
            frameCounter--;
            if (frameCounter == 0) state = State.Ground;
        }
        private void groundUpdate() {
            vsp = 0;
            if (!airdashing) {
                if (playerInput.left && facing == -1)  hsp = -4f;
                else if (playerInput.right && facing == 1) hsp = 4f;
                else if (playerInput.left && facing == 1)  hsp = -2.85f;
                else if (playerInput.right && facing == -1) hsp = 2.85f;
                else hsp = 0;
            }

            if (playerInput.down) {
                blocking = true;
                hsp *= 0.2f;
                if (!playerInput.right && !playerInput.left) hurtbox.updateSize(new Bounds(new Vector3(0.1f*facing, -0.75f, 0), new Vector3(175, 365, 1)));
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/blockIdle");
            }
            else {
                blocking = false;
                hurtbox.updateSize(new Bounds(new Vector3(0f, -0.45f, 0), new Vector3(150, 425, 1)));
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/idle");
            }

            if (playerInput.up) {
                vsp = 20;
                blocking = false;
                hurtbox.updateSize(new Bounds(new Vector3(0f, -0.45f, 0), new Vector3(150, 425, 1)));
            }
            else if (playerInput.right || playerInput.left) hurtbox.updateSize(new Bounds(new Vector3(0f, -0.45f, 0), new Vector3(150, 425, 1)));
            if (playerInput.normal) {
                move = moveset.Normal;
                moveInput = playerInput;
                state = State.AttackGround;
                blocking = false;
                if (playerInput.down) hurtbox.updateSize(new Bounds(new Vector3(0.1f*facing, -0.75f, 0), new Vector3(175, 365, 1)));
            }
            else if (playerInput.special) {
                move = moveset.Special;
                moveInput = playerInput;
                state = State.AttackGround;
                blocking = false;
                hurtbox.updateSize(new Bounds(new Vector3(0f, -0.45f, 0), new Vector3(150, 425, 1)));
            }
        }
        private void airUpdate() {
            if (vsp >= 0) GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/airRising");
            else GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/airFalling");
            if (!airdashing) {
                vsp -= 0.85f;
                if (playerInput.left && facing == -1)  hsp = -5f;
                else if (playerInput.right && facing == 1) hsp = 5f;
                else if (playerInput.left && facing == 1)  hsp = -4f;
                else if (playerInput.right && facing == -1) hsp = 4f;
                else hsp = 0;
            }

            if (playerInput.normal) {
                specialCancel = true;
                move = moveset.Aereal;
                moveInput = playerInput;
                state = State.AttackAir;
            }
            else if (playerInput.special) {
                move = moveset.Special;
                moveInput = playerInput;
                state = State.AttackAir;
            }

            if (playerInput.airdash && !airdashing) {
                airdashing = true;
                if (playerInput.left) {
                    hsp = -10;
                    vsp = -12.75f;
                }
                else if (playerInput.right) {
                    hsp = 10;
                    vsp = -12.75f;
                }
                else vsp = -18.7f;
                airdashCounter = 12;
            }
        }
        private void attackAirUpdate() {
            if (!airdashing) vsp -= 0.85f;
            if (playerInput.left && facing == -1)  hsp = -5f;
            else if (playerInput.right && facing == 1) hsp = 5f;
            else if (playerInput.left && facing == 1)  hsp = -4f;
            else if (playerInput.right && facing == -1) hsp = 4f;
            else hsp = 0;

            frameCounter++;
            move.Invoke(moveInput);
            if (frameCounter == 0) {
                if (!died()) state = State.Air;
                move = null;
            }

            if (playerInput.airdash && !airdashing) {
                airdashing = true;
                if (playerInput.left) {
                    hsp = -10;
                    vsp = -12.75f;
                }
                else if (playerInput.right) {
                    hsp = 10;
                    vsp = -12.75f;
                }
                else vsp = -18.7f;
                airdashCounter = 12;
            }
        }
        private void attackGroundUpdate() {
            if (!airdashing) hsp = 0;

            frameCounter++;
            move.Invoke(moveInput);
            if (frameCounter == 0) {
                if (!died()) state = State.Ground;
                move = null;
            }
        }

        void stateMachine() {
            switch (state) {
                case State.Ground: 
                    groundUpdate();
                break;
                case State.Air:
                    airUpdate();
                break;
                case State.AttackAir:
                    attackAirUpdate();
                break;
                case State.AttackGround:
                    attackGroundUpdate();
                break;
                case State.Hit:
                    hitUpdate();
                break;
                case State.LandingLag:
                    landingLagUpdate();
                break;
            }
        }

        void checkState() {
            if (transform.position.y > ground) {
                if (state != State.AttackAir && state != State.Hit) state = State.Air;
            }
            else {
                if (state == State.Hit && (!hardHit || vsp > 0)) return;
                if (state == State.AttackAir) {
                    if (specialCancel && move == moveset.Special) {
                        state = State.Ground;
                        frameCounter = 0;
                        hitboxList.deleteMine(this);
                    }
                    else if (move != moveset.Special) {
                        state = State.LandingLag;
                        hitboxList.deleteMine(this);
                    }
                    else state = State.AttackGround;
                }
                else if (state == State.AttackGround) state = State.AttackGround;
                else state = State.Ground;
                specialCancel = false;
            }
        }

        void moveChar() {
            if (transform.position.x + hsp*Time.fixedDeltaTime < -8.1f) {
                hsp = 0;
                transform.position = new Vector3(-8.1f, transform.position.y, 0);
            }
            else if (transform.position.x + hsp*Time.fixedDeltaTime > 8.1f) {
                hsp = 0;
                transform.position = new Vector3(8.1f, transform.position.y, 0);
            }
            transform.position += new Vector3(hsp*Time.fixedDeltaTime, vsp*Time.fixedDeltaTime, 0);
            if (transform.position.y <= ground) transform.position = new Vector3(transform.position.x, ground, 0);
        }

        void Update() {
            playerInput = player.getInput(playerInput);
        }
        void FixedUpdate()
        {
            Debug.Log(state);
            if (airdashCounter > 0) airdashCounter--;
            if (airdashCounter == 0) airdashing = false;
            stateMachine();
            checkHit();
            moveChar();
            checkState();
            if (transform.position.y <= ground) facing = (otherPlayer.getX() > transform.position.x) ? 1 : -1;
            transform.localScale = new Vector3(facing, 1, 1);
            hurtbox.updatePosition(transform.position);
            playerInput = controller.setFalse(playerInput);
        }

        void Start()
        {
            player = new controller(playerProfiles.getProfile(profile));
            facing = (int)transform.localScale.x;
            hp = 360;
            GameObject hb = new GameObject("hurtbox");
            hurtbox = hb.AddComponent<hurtbox>();
            hurtbox.Initialize(new Bounds(new Vector3(0f, -0.45f, 0), new Vector3(150, 425, 1)), this, transform.position + new Vector3(0f, -0.45f, 0));
            Move[] moves = new Move[9];
            moves[0] = new Move(DTilt);
            moves[1] = new Move(FTilt);
            moves[2] = new Move(Jab);
            moves[3] = new Move(UAir);
            moves[4] = new Move(DAir);
            moves[5] = new Move(NAir);
            moves[6] = new Move(DSpecial);
            moves[7] = new Move(FSpecial);
            moves[8] = new Move(NSpecial);
            moveset = new Moveset(moves);
        }
    }
}

using UnityEngine;

public class AttackController_QWE : MonoBehaviour
{
    public Animator anim;

    public string meleeTrigger = "ATTACK_Melle";   // Q
    public string rangeTrigger = "ATTACK_Throw";   // W
    public string magicTrigger = "ATTACK_Special"; // E

    public KeyCode meleeKey = KeyCode.Q;
    public KeyCode rangeKey = KeyCode.W;
    public KeyCode magicKey = KeyCode.E;

    public float meleeCooldown = 0.25f;
    public float rangeCooldown = 0.4f;
    public float magicCooldown = 0.8f;

    float meleeTimer;
    float rangeTimer;
    float magicTimer;

    void Reset()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (anim == null) return;

        if (meleeTimer > 0f) meleeTimer -= Time.deltaTime;
        if (rangeTimer > 0f) rangeTimer -= Time.deltaTime;
        if (magicTimer > 0f) magicTimer -= Time.deltaTime;

        if (Input.GetKeyDown(meleeKey) && meleeTimer <= 0f)
        {
            meleeTimer = meleeCooldown;
            anim.SetTrigger(meleeTrigger);
        }

        if (Input.GetKeyDown(rangeKey) && rangeTimer <= 0f)
        {
            rangeTimer = rangeCooldown;
            anim.SetTrigger(rangeTrigger);
        }

        if (Input.GetKeyDown(magicKey) && magicTimer <= 0f)
        {
            magicTimer = magicCooldown;
            anim.SetTrigger(magicTrigger);
        }
    }
}

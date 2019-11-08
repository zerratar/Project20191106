using UnityEngine;
using System.Collections;

public class MotherBase : BuildingController
{
    public int ShoeCost = 10;
    public float ShoeGenerateTime = 10f;

    private float shoeGenerateCooldown;
    private PlayerController player;

    private bool generatingShoe;
    // Use this for initialization
    void Start()
    {
        player = this.transform.root.GetComponent<PlayerController>();
        shoeGenerateCooldown = ShoeGenerateTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (generatingShoe)
        {
            shoeGenerateCooldown -= Time.deltaTime;
            if (shoeGenerateCooldown <= 0f)
            {
                ++player.Shoes;
                shoeGenerateCooldown = ShoeGenerateTime;
                generatingShoe = false;
            }
        }

        if (player.Money >= ShoeCost && !generatingShoe)
        {
            player.Money -= ShoeCost;
            generatingShoe = true;
        }
    }
}


//public class CraftShoe : Ability
//{
//    protected override void UseAbility()
//    {

//    }

//    protected override bool CanUseAbility()
//    {
//        return false;
//    }
//}


//public abstract class Ability : ScriptableObject
//{
//    public float Cooldown;

//    private float useTimer;

//    public bool CanUse()
//    {
//        return CanUseAbility() && useTimer <= 0f;
//    }

//    public void Use()
//    {
//        if (CanUse())
//        {
//            UseAbility();
//            useTimer = Cooldown;
//        }
//    }

//    protected abstract void UseAbility();

//    protected virtual bool CanUseAbility()
//    {
//        return true;
//    }
//}

//public class AbilityUser : MonoBehaviour
//{

//    public void Use(Ability ability)
//    {
//        //....
//    }
//}

using UnityEngine;
using System;

[Serializable]
public struct CharacterSet
{
    public string name;
    public RuntimeAnimatorController controller;
    public Sprite shotgunSprite;
}

public class PlayerAnimatorComp : MonoBehaviour
{
    public Animator animator;

    public SpriteRenderer shotgunSprite;

    // To add a new character, plug values in the editor
    [SerializeField]
    public CharacterSet[] characterSets;

    bool switchInput { set; get; }
    int currentSkin = 0;

    PlayerControllerComp playerController;

    void Start()
    {
        playerController = GetComponent<PlayerControllerComp>();
    }

    void Update()
    {
        updateInputs();

        animate(playerController.velocity, playerController.isGrounded);
        switchSkin();
    }

    public void updateInputs()
    {
        switchInput = Input.GetButtonDown("SwitchSkin");
    }

    public void applyCharacterSet(CharacterSet set)
    {
        animator.runtimeAnimatorController = set.controller;
        shotgunSprite.sprite = set.shotgunSprite;
    }

    void switchSkin()
    {
        //allows for swapping skins
        if (switchInput)
        {
            currentSkin += 1;
            currentSkin = currentSkin % 3;
        }

        
        applyCharacterSet(characterSets[currentSkin]);
    }

    void animate(Vector3 velocity, bool isGrounded)
    {
        //sets variables to trigger animations :thumbs_up:
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("HSpeed", velocity.y); //this is horizontal speed, animates jump/fall
    }
}

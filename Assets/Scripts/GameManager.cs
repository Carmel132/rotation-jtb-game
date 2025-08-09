using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class GameManager : MonoBehaviour
{

    public static GameManager gm;

    // player sfx
    public EventReference
        playerFootstep,
        playerDash,
        playerJump,
        playerShoot;

    public EventReference button;
    public EventReference coin;
    public EventReference lizard;
    public EventReference titleScreen;
    private EventInstance titleScreenInstance;

    public int frameCap;

    private void Awake()
    {
        if (gm)
        {
            // I am not Gerald
            Destroy(gameObject);
        }
        else
        {
            gm = this;
            DontDestroyOnLoad(gameObject);
            titleScreenInstance = RuntimeManager.CreateInstance(titleScreen);
            // titleScreenInstance.start();   uncomment when actually title screen exists
        }
    }

    private void Start()
    {
        // function to limit fps
        // Application.targetFrameRate = frameCap;
    }

    // sfx functions
    public void PlayerFootstepSFX()
    {
        RuntimeManager.PlayOneShot(playerFootstep);
    }

    public void PlayerDashSFX()
    {
        RuntimeManager.PlayOneShot(playerDash);
    }

    public void PlayerJumpSFX()
    {
        RuntimeManager.PlayOneShot(playerJump);
    }

    public void PlayerShootSFX()
    {
        RuntimeManager.PlayOneShot(playerShoot);
    }

    public void LizardSFX()
    {
        RuntimeManager.PlayOneShot(lizard);
    }

    public void ButtonSFX()
    {
        RuntimeManager.PlayOneShot(button);
    }

    public void CoinSFX()
    {
        RuntimeManager.PlayOneShot(coin);
    }
}

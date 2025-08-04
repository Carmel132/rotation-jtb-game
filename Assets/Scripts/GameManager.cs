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

    public  EventReference titleScreen;
    private EventInstance titleScreenInstance;

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
            titleScreenInstance.start();
        }
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
}

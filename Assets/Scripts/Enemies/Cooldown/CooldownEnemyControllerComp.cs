using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CooldownEnemyControllerComp : MonoBehaviour
{
    internal enum States
    {
        SurveyLeft,
        SurveyRight,
        SurveyWait,
        Attack,
        Search
    }

    Rigidbody2D rigidBody;

    StateMachine<States> stateMachine;
    [SerializeField]
    FixedTimer surveyTimer;
    [SerializeField]
    FixedTimer surveyWaitTimer;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float runSpeed;
    bool surveyLastRight = false;
    [SerializeField]
    float searchRadius = 10;
    [SerializeField]
    FixedTimer searchTimer;
    [SerializeField]
    FixedTimer attackTimer;

    [SerializeField]
    public GameObject bulletPrefab;


    Collider2D player;
    Vector3 lastSeen;
    void Start()
    {
        buildStateMachine();
        surveyTimer.reset();
        surveyWaitTimer.reset();
        searchTimer.reset();
        attackTimer.reset();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        stateMachine.loop();
        Debug.Log(stateMachine.getCurrentNode());
    }

    void buildStateMachine()
    {
        stateMachine = new();

        stateMachine.addNode(States.SurveyRight, () =>
        {
            surveyTimer.reset();
        }, () => surveyLastRight = true, () =>
        {
            rigidBody.linearVelocityX = moveSpeed;
        });

        stateMachine.addNode(States.SurveyLeft, () =>
        {
            surveyTimer.reset();
        }, () => surveyLastRight = false, () =>
        {
            rigidBody.linearVelocityX = -moveSpeed;
        });

        stateMachine.addNode(States.SurveyWait, () =>
        {
            surveyWaitTimer.reset();
        }, null, null);

        stateMachine.addNode(States.Attack, () => attackTimer.reset(), null, () =>
        {
            foundPlayer();
            attack();
        });

        stateMachine.addNode(States.Search, () =>
        {
            searchTimer.reset();
        }, null, runTowardsLastSeenPlayer);

        stateMachine.addArrow(States.SurveyWait, States.SurveyLeft, () => surveyWaitTimer.timePassed && surveyLastRight);
        stateMachine.addArrow(States.SurveyWait, States.SurveyRight, () => surveyWaitTimer.timePassed && !surveyLastRight);
        stateMachine.addArrow(States.SurveyLeft, States.SurveyWait, () => surveyTimer.timePassed);
        stateMachine.addArrow(States.SurveyRight, States.SurveyWait, () => surveyTimer.timePassed);

        stateMachine.addArrow(States.SurveyLeft, States.Attack, foundPlayer);
        stateMachine.addArrow(States.SurveyRight, States.Attack, foundPlayer);
        stateMachine.addArrow(States.SurveyWait, States.Attack, foundPlayer);

        stateMachine.addArrow(States.Attack, States.Search, () => !foundPlayer());
        stateMachine.addArrow(States.Search, States.Attack, foundPlayer);

        stateMachine.addArrow(States.Search, States.SurveyWait, () => searchTimer.timePassed);

        stateMachine.setCurrentNode(States.SurveyWait);
    }

    bool foundPlayer()
    {
        List<Collider2D> colliders = new();
        ContactFilter2D contactFilter = new();
        contactFilter.NoFilter();
        Physics2D.OverlapCircle(transform.position, searchRadius, contactFilter, colliders);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                player = collider;
                lastSeen = player.transform.position;
                return true;
                
            }
        }
        return false;
    }

    void attack() {
        if (attackTimer.timePassed)
        {
            createBullet(Vector3.right, Quaternion.identity);
            createBullet(Vector3.up, Quaternion.Euler(0, 0, 90));
            createBullet(Vector3.right, Quaternion.Euler(0, 0, 180));
            attackTimer.reset();
        }
    }

    void createBullet(Vector3 offset, Quaternion rotation)
    {
        var bullet = Instantiate(bulletPrefab, transform.position + offset, rotation);
        var comp = bullet.GetComponent<CooldownEnemyHomingMissileControllerComp>();
        comp.target = lastSeen;
    }

    void runTowardsLastSeenPlayer()
    {
        rigidBody.linearVelocityX = Mathf.Min(runSpeed, Mathf.Abs(lastSeen.x - transform.position.x)) * Mathf.Sign(lastSeen.x - transform.position.x);
    }
}

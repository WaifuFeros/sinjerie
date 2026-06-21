using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private GameObject blocker;

    private bool _lockTransition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        blocker.SetActive(false);
    }

    public void PlayClosing()
    {
        transitionAnimator.SetTrigger("Close");
    }

    public void PlayOpening()
    {
        transitionAnimator.SetTrigger("Open");
    }

    public void TransitionWithAction(Action action)
    {
        //if (_lockTransition)
        //    return;

        StartCoroutine(RoutineTransition(action));
    }

    private IEnumerator RoutineTransition(Action action)
    {
        blocker.SetActive(true);
        _lockTransition = true;
        PlayClosing();
        yield return StartCoroutine(WaitEndTransition());
        action.Invoke();
        yield return new WaitForSeconds(0.1f);
        PlayOpening();
        yield return StartCoroutine(WaitEndTransition());
        _lockTransition = false;
        blocker.SetActive(false);
    }

    private IEnumerator WaitEndTransition()
    {
        yield return new WaitForEndOfFrame();
        AnimatorStateInfo stateInfo = transitionAnimator.GetCurrentAnimatorStateInfo(0);
        float dureeReelle = stateInfo.length;
        yield return new WaitForSeconds(dureeReelle);
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private Animator transitionAnimator;

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
        StartCoroutine(RoutineTransition(action));
    }

    private IEnumerator RoutineTransition(Action action)
    {
        PlayClosing();
        yield return StartCoroutine(WaitEndTransition());
        action.Invoke();
        yield return new WaitForSeconds(0.1f);
        PlayOpening();
        yield return StartCoroutine(WaitEndTransition());
    }

    private IEnumerator WaitEndTransition()
    {
        yield return new WaitForEndOfFrame();
        AnimatorStateInfo stateInfo = transitionAnimator.GetCurrentAnimatorStateInfo(0);
        float dureeReelle = stateInfo.length;
        yield return new WaitForSeconds(dureeReelle);
    }
}
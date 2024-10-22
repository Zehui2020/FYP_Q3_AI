using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneGroup : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private float lerpSpeed;

    [Header("Player Weight")]
    [SerializeField] private float playerDefaultWeight;
    [SerializeField] private float playerCustsceneWeight;

    [Header("Target Weight")]
    [SerializeField] private float targetDefaultWeight;
    [SerializeField] private float targetCutsceneWeight;

    [Header("Events")]
    public UnityEvent CutsceneStart;
    public UnityEvent CutsceneEnd;

    private Coroutine cutsceneRoutine;

    public void EnterCutscene()
    {
        cutsceneRoutine = StartCoroutine(EnterCustsceneRoutine());
        CutsceneStart?.Invoke();
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Map);
    }

    public void ExitCutscene() 
    {
        if (cutsceneRoutine != null)
            StopCoroutine(cutsceneRoutine);

        cutsceneRoutine = StartCoroutine(ExitCutsceneRoutine());
        CutsceneEnd?.Invoke();
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Movement);
    }

    private IEnumerator EnterCustsceneRoutine()
    {
        while (Mathf.Abs(targetGroup.m_Targets[0].weight - playerCustsceneWeight) > 0.1f &&
            Mathf.Abs(targetGroup.m_Targets[1].weight - targetCutsceneWeight) > 0.1f)
        {
            targetGroup.m_Targets[0].weight = Mathf.Lerp(targetGroup.m_Targets[0].weight, playerCustsceneWeight, Time.deltaTime * lerpSpeed);
            targetGroup.m_Targets[1].weight = Mathf.Lerp(targetGroup.m_Targets[1].weight, targetCutsceneWeight, Time.deltaTime * lerpSpeed);

            yield return null;
        }

        targetGroup.m_Targets[0].weight = playerCustsceneWeight;
        targetGroup.m_Targets[1].weight = targetCutsceneWeight;

        cutsceneRoutine = null;
    }

    private IEnumerator ExitCutsceneRoutine()
    {
        while (Mathf.Abs(targetGroup.m_Targets[0].weight - playerDefaultWeight) > 0.1f &&
            Mathf.Abs(targetGroup.m_Targets[1].weight - playerDefaultWeight) > 0.1f)
        {
            targetGroup.m_Targets[0].weight = Mathf.Lerp(targetGroup.m_Targets[0].weight, playerDefaultWeight, Time.deltaTime * lerpSpeed);
            targetGroup.m_Targets[1].weight = Mathf.Lerp(targetGroup.m_Targets[1].weight, targetDefaultWeight, Time.deltaTime * lerpSpeed);

            yield return null;
        }

        targetGroup.m_Targets[0].weight = playerDefaultWeight;
        targetGroup.m_Targets[1].weight = targetDefaultWeight;

        cutsceneRoutine = null;
    }
}
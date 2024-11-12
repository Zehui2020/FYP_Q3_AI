using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComfyPropsManager : ComfyManager
{
    [System.Serializable]
    public struct PropData
    {
        public string fileName;
        public string prompt;
        public string referenceImage;
    }

    [SerializeField] private int propsRecieved = 0;
    [SerializeField] private List<PropData> propDatas;
    [SerializeField] private bool queueOnStart;

    public UnityEvent OnFinishAllProps;

    private void Start()
    {
        InitManager();

        propsRecieved = 0;
        if (queueOnStart)
            QueuePropPrompt();
    }

    public void QueuePropPrompt()
    {
        if (propsRecieved >= propDatas.Count)
            return;

        GameData.Instance.EnqueueLoading(propDatas[propsRecieved].fileName + "_Prop", false);
        promptCtr.QueuePromptWithControlNet(propDatas[propsRecieved].prompt, propDatas[propsRecieved].referenceImage);
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        Debug.Log(propsRecieved);
        Debug.Log(propDatas.Count);

        if (propsRecieved >= propDatas.Count)
        {
            OnFinishAllProps?.Invoke();
            Destroy(gameObject);
            return false;
        }

        fileName = propDatas[propsRecieved].fileName;
        Debug.Log("RECIEVED " + fileName);

        if (base.OnRecieveImage(promptID, texture))
        {
            propsRecieved++;
            QueuePropPrompt();
            return true;
        }

        return false;
    }
}
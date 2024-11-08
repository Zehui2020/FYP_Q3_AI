using System.Collections.Generic;
using UnityEngine;

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

    public void QueuePropPrompt()
    {
        if (propsRecieved >= propDatas.Count)
            return;

        GameData.Instance.currentlyLoadingImage.Enqueue(propDatas[propsRecieved] + "_Prop");
        promptCtr.QueuePromptWithControlNet(propDatas[propsRecieved].prompt, propDatas[propsRecieved].referenceImage);
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        if (propsRecieved >= propDatas.Count)
        {
            Destroy(gameObject);
            return false;
        }

        fileName = propDatas[propsRecieved].prompt;
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
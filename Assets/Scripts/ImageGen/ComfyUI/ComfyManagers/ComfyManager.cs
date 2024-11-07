using UnityEngine;

public class ComfyManager : MonoBehaviour
{
    public ComfyWebsocket comfyWebsocket;
    public ComfyPromptCtr promptCtr;
    public ComfyImageCtr imageCtr;
    public ImageSaver imageSaver;

    protected string promptID;
    [SerializeField] protected string fileName;

    public virtual void InitManager()
    {
        DontDestroyOnLoad(this);
        comfyWebsocket.InitWebsocket();

        promptCtr.OnQueuePrompt.AddListener(SetCurrentPromptID);
        imageCtr.OnRecieveImage.AddListener((promptID, texture) => { OnRecieveImage(promptID, texture); });
    }

    public virtual void SetCurrentPromptID(string promptID)
    {
        this.promptID = promptID;
    }

    public virtual bool OnRecieveImage(string promptID, Texture2D texture)
    {
        if (promptID.Equals(this.promptID))
        {
            imageSaver.SaveImageToLocalDisk(texture, fileName);
            Debug.Log("Obtained: " + fileName);
            return true;
        }

        return false;
    }
}
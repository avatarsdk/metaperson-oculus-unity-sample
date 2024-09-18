using AvatarSDK.MetaPerson.Loader;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OculusSampleSceneHandler : MonoBehaviour
{
    public Button button;
    public MetaPersonLoader loader;
    public GameObject dstObject;
    public Text progressText;
    public AudioSource audioSource;
    public GameObject existingAvatar;

    public OVRLipSyncContextMorphTarget headContextMorphTarget;
    public OVRLipSyncContextMorphTarget teethContextMorphTarget;
    public OVRLipSyncContext lipSyncContext;

    const string avatarUri = "https://metaperson.avatarsdk.com/avatars/b255d298-7644-48ec-85ef-4a2200668458/model.glb";
    // Start is called before the first frame update
    void Start()
    {
        progressText.gameObject.SetActive(false);
        button.onClick.AddListener(OnButtonClick);
    }
    void ProgressReport(float progress)
    {
        progressText.text = string.Format("Downloading avatar: {0}%", (int)(progress * 100));
    }
    async void OnButtonClick()
    {
        button.gameObject.SetActive(false);
        progressText.gameObject.SetActive(true);

        await loader.LoadModelAsync(avatarUri, ProgressReport);
        progressText.gameObject.SetActive(false);

        

        /*headContextMorphTarget.blendshapeScale = teethContextMorphTarget.blendshapeScale = GetMaxBlendshapesValue(loader.avatarObject);
        headContextMorphTarget.skinnedMeshRenderer = headMesh;
        teethContextMorphTarget.skinnedMeshRenderer = teethLowerMesh;*/

        AvatarSdkOculusTools.Configure(loader.avatarObject, dstObject);


        MetaPersonUtils.ReplaceAvatar(loader.avatarObject, existingAvatar);
    }
    // Update is called once per frame
    void Update()
    {
        
    }


}

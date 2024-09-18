using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class AvatarSdkOculusTools
{
    static int[] headBlendshapes = new int[]{
                65,55,54,58,52,61,51,57,62,56,59,53,60,63,64
            };
    static int[] teethBlendshapes = new int[]{
                18,8,7,11,5,14,4,10,15,9,12,6,13,16,17
            };
    public static void Configure(GameObject avatarObj, GameObject parentObj, AudioClip audioClip = null)
    {
        if (avatarObj == null || parentObj == null)
        {
            return;
        }
        var audioComponent = parentObj.GetComponent<AudioSource>();
        if (audioComponent == null)
        {
            audioComponent = parentObj.AddComponent<AudioSource>();
            audioComponent.loop = true;
            audioComponent.playOnAwake = true;
            audioComponent.clip = audioClip;
            parentObj.AddComponent<AudioSource>();
        }
        var context = parentObj.GetComponent<OVRLipSyncContext>();
        if (context == null)
        {
            context = parentObj.AddComponent<OVRLipSyncContext>();
            context.audioSource = audioComponent;
            context.enableAcceleration = true;
            context.audioLoopback = true;
            context.gain = 1.0f;
        }

        SkinnedMeshRenderer[] meshRenderes = avatarObj.GetComponentsInChildren<SkinnedMeshRenderer>();
        var headMesh = meshRenderes.FirstOrDefault(loader => loader.name == "AvatarHead");
        var teethLowerMesh = meshRenderes.FirstOrDefault(loader => loader.name == "AvatarTeethLower");

        var contextMorphTargets = parentObj.GetComponents<OVRLipSyncContextMorphTarget>();
        OVRLipSyncContextMorphTarget headMorphTargets, teethMorphTargets;
        if (contextMorphTargets.Count() == 0)
        {
            headMorphTargets = parentObj.AddComponent<OVRLipSyncContextMorphTarget>();
            teethMorphTargets = parentObj.AddComponent<OVRLipSyncContextMorphTarget>();
            headMorphTargets.blendshapeScale = teethMorphTargets.blendshapeScale = GetMaxBlendshapesValue(avatarObj);
            headMorphTargets.skinnedMeshRenderer = headMesh;
            teethMorphTargets.skinnedMeshRenderer = teethLowerMesh;
            
            for(int i = 0; i < teethMorphTargets.visemeToBlendTargets.Count(); i++)
            {
                teethMorphTargets.visemeToBlendTargets[i] = teethBlendshapes[i];
                headMorphTargets.visemeToBlendTargets[i] = headBlendshapes[i];
            }           
        }
        else
        {
            headMorphTargets = contextMorphTargets.FirstOrDefault(c => c.skinnedMeshRenderer.name.Contains("AvatarHead"));
            teethMorphTargets = contextMorphTargets.FirstOrDefault(c => c.skinnedMeshRenderer.name.Contains("AvatarTeethLower"));
            headMorphTargets.blendshapeScale = teethMorphTargets.blendshapeScale = GetMaxBlendshapesValue(avatarObj);
            headMorphTargets.skinnedMeshRenderer = headMesh;
            teethMorphTargets.skinnedMeshRenderer = teethLowerMesh;
        }
    }

    private static float GetMaxBlendshapesValue(GameObject gameObject)
    {
        SkinnedMeshRenderer[] meshRenderes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        var headMesh = meshRenderes.FirstOrDefault(loader => loader.name == "AvatarHead");
        if (headMesh == null)
        {
            return 100.0f;
        }
        int blenshapeIdx = headMesh.sharedMesh.GetBlendShapeIndex("FF");
        if (blenshapeIdx > -1)
        {
            var res = meshRenderes.FirstOrDefault(mr => mr.name == "AvatarHead").sharedMesh.GetBlendShapeFrameWeight(blenshapeIdx, 0);
            return res;
        }
        return 100.0f;
    }
}


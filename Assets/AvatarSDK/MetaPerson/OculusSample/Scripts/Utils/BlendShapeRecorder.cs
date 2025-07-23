/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, July 2025
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AvatarSDK.MetaPerson.OculusLipSync
{
	[System.Serializable]
	public class BlendShapeFrame
	{
		public float timestamp;
		public float[] weights;
	}

	[System.Serializable]
	public class BlendShapeAnimationData
	{
		public List<string> blendShapeNames;
		public List<BlendShapeFrame> frames = new List<BlendShapeFrame>();
	}

	public class BlendShapeRecorder : MonoBehaviour
	{
		public AudioSource audioSource;
		public SkinnedMeshRenderer skinnedMeshRenderer;
		public List<string> blendShapeNames = new List<string>();
		public string outputJsonFile;

		private BlendShapeAnimationData animationData = new BlendShapeAnimationData();
		private List<int> blendShapeIndices = new List<int>();
		private bool isRecording = false;
		private float nextSampleTime;
		private const float sampleInterval = 1f / 30f; // 30 FPS

		void Start()
		{
			if (skinnedMeshRenderer == null)
			{
				Debug.LogError("SkinnedMeshRenderer not assigned!");
				return;
			}

			foreach (string shapeName in blendShapeNames)
			{
				int index = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(shapeName);
				if (index == -1)
				{
					Debug.LogError($"Blend shape '{shapeName}' not found on mesh!");
				}
				else
				{
					blendShapeIndices.Add(index);
				}
			}

			animationData.blendShapeNames = new List<string>(blendShapeNames);
		}

		void Update()
		{
			if (audioSource == null) return;

			if (audioSource.isPlaying && !isRecording)
			{
				StartRecording();
			}
			else if (!audioSource.isPlaying && isRecording)
			{
				StopRecording();
			}

			if (isRecording && Time.time >= nextSampleTime)
			{
				RecordFrame();
				nextSampleTime = Time.time + sampleInterval;
			}
		}

		void StartRecording()
		{
			isRecording = true;
			animationData.frames.Clear();
			RecordFrame();
			nextSampleTime = Time.time + sampleInterval;
			Debug.Log("Recording started");
		}

		void RecordFrame()
		{
			var frame = new BlendShapeFrame
			{
				timestamp = audioSource.time,
				weights = new float[blendShapeIndices.Count]
			};

			for (int i = 0; i < blendShapeIndices.Count; i++)
			{
				frame.weights[i] = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndices[i]);
			}

			animationData.frames.Add(frame);
		}

		void StopRecording()
		{
			isRecording = false;
			SaveToJson();
			Debug.Log("Recording stopped. Data saved.");
		}

		void SaveToJson()
		{
			if (animationData.frames.Count == 0)
			{
				Debug.LogWarning("No frames recorded. Skipping save.");
				return;
			}

			if (string.IsNullOrEmpty(outputJsonFile))
			{
				Debug.LogWarningFormat("Output JSON file isn't provided");
				return;
			}

			string jsonData = JsonUtility.ToJson(animationData, true);
			File.WriteAllText(outputJsonFile, jsonData);
			Debug.Log($"Saved {animationData.frames.Count} frames to: {outputJsonFile}");
		}
	}
}
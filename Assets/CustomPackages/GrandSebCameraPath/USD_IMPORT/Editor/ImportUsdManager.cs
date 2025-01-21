using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace Assets.USD_IMPORT.Editor {

    public class ImportUsdManager {
        
        string pathFile;
        AnimationClip clip;

        public ImportUsdManager(string pathUsdFile) {
            pathFile = pathUsdFile;
        }

        public void TransformToAnimation() {
            clip = new AnimationClip();
            clip.legacy = true;

            // Read USD Keys
            List<UsdKey> Keys = ReadUsdFile();

            // create a curve to move the GameObject and assign to the clip
            CreateCurves(clip, Keys);

            // Serialize clip
            DateTime dt = DateTime.Now;
            string AnimationFileName = $"{Path.GetFileNameWithoutExtension(pathFile)}.{dt.Hour}_{dt.Minute}.anim";
            AssetDatabase.CreateAsset(clip, $"Assets/{AnimationFileName}");
        }

        private void CreateCurves(AnimationClip clip, List<UsdKey> keys) {

            Keyframe[] keysX = new Keyframe[keys.Count];
            Keyframe[] keysY = new Keyframe[keys.Count];
            Keyframe[] keysZ = new Keyframe[keys.Count];

            Keyframe[] keysQX = new Keyframe[keys.Count];
            Keyframe[] keysQY = new Keyframe[keys.Count];
            Keyframe[] keysQZ = new Keyframe[keys.Count];
            Keyframe[] keysQW = new Keyframe[keys.Count];            

            int count = 0;
            foreach (UsdKey key in keys) {
                // Position
                keysX[count] = new Keyframe(key.time, key.position.x);
                keysY[count] = new Keyframe(key.time, key.position.y);
                keysZ[count] = new Keyframe(key.time, key.position.z);

                // Rotation
                keysQX[count] = new Keyframe(key.time, key.rotation.x);
                keysQY[count] = new Keyframe(key.time, key.rotation.y);
                keysQZ[count] = new Keyframe(key.time, key.rotation.z);
                keysQW[count] = new Keyframe(key.time, key.rotation.w);

                count++;
            }

            clip.SetCurve("", typeof(Transform), "localPosition.x", new AnimationCurve(keysX));
            clip.SetCurve("", typeof(Transform), "localPosition.y", new AnimationCurve(keysY));
            clip.SetCurve("", typeof(Transform), "localPosition.z", new AnimationCurve(keysZ));

            clip.SetCurve("", typeof(Transform), "localRotation.x", new AnimationCurve(keysQX));
            clip.SetCurve("", typeof(Transform), "localRotation.y", new AnimationCurve(keysQY));
            clip.SetCurve("", typeof(Transform), "localRotation.z", new AnimationCurve(keysQZ));
            clip.SetCurve("", typeof(Transform), "localRotation.w", new AnimationCurve(keysQW));
        }

        private List<UsdKey> ReadUsdFile() {
            string StartLine = "matrix4d xformOp:transform.timeSamples = {";
            string EndLine = "}";

            bool Started = false;
            bool Ended = false;

            List<UsdKey> Keys = new List<UsdKey>();

            List<String> Lines = File.ReadAllLines(pathFile).ToList();
            for (int iLine = 0; (iLine < Lines.Count) && !Ended; iLine++) {

                String currentLine = Lines[iLine];
                if (!Started) {
                    if (currentLine.Contains(StartLine)) {
                        Started = true;
                        continue;
                    }
                } else { // Started
                    if (currentLine.Contains(EndLine)) {
                        Ended = true;
                        continue;
                    }

                    // Parse current line into UsdKey
                    // 1.386592984199524: ( (4.930350303649902, 0.21411895751953125, 3.891984462738037, 0), (-0.6169515252113342, 6.239311695098877, 0.4382939338684082, 0), (-3.8487348556518555, -0.7258676886558533, 4.915496349334717, 0), (-0.637768030166626, 1.2840969562530518, -0.21879366040229797, 1) ),
                    String[] Parts = currentLine.Split(':');
                    float time = float.Parse(Parts[0].Replace(" ", ""), new CultureInfo("en-US"));

                    String values = Parts[1]
                        .Replace("(", "")
                        .Replace(")", "")
                        .Replace(" ", "");

                    List<string> matrixValues =  values.Split(',').ToList();
                    List<float> matValues = new List<float>();
                    foreach (string item in matrixValues) {
                        if (item == "") continue;
                        matValues.Add(float.Parse(item, new CultureInfo("en-US")));
                    }

                    Vector4 v1 = new Vector4(matValues[0], matValues[1], matValues[2], matValues[3]);
                    Vector4 v2 = new Vector4(matValues[4], matValues[5], matValues[6], matValues[7]);
                    Vector4 v3 = new Vector4(matValues[8], matValues[9], matValues[10], matValues[11]);
                    Vector4 v4 = new Vector4(matValues[12], matValues[13], matValues[14], matValues[15]);

                    //Vector4 v1 = new Vector4(matValues[0], matValues[4], matValues[8], matValues[12]);
                    //Vector4 v2 = new Vector4(matValues[1], matValues[5], matValues[9], matValues[13]);
                    //Vector4 v3 = new Vector4(matValues[2], matValues[6], matValues[10], matValues[14]);
                    //Vector4 v4 = new Vector4(matValues[3], matValues[7], matValues[11], matValues[15]);

                    UsdKey key = new UsdKey(time, v1, v2, v3, v4);

                    Keys.Add(key);
                }
            }

            return Keys;
        }

    }

}
using System.Collections;
using UnityEngine;

namespace Assets.USD_IMPORT.Editor {

    public class UsdKey {

        public UsdKey(float time, Vector4 v1, Vector4 v2, Vector4 v3, Vector4 v4) {
            this.time = time;
            matrix = new Matrix4x4(v1, v2, v3, v4);

            matrix = UnityTypeConverter.ChangeBasis(matrix);
        }

        public float time;

        public Matrix4x4 matrix;

        public Vector3 position {
            get {
                Vector3 position;

                position.x = matrix.m03;
                position.y = matrix.m13;
                position.z = matrix.m23;

                return position;
            }
        }

        public Quaternion rotation
        {
            get {
                Vector3 forward;
                forward.x = matrix.m02;
                forward.y = matrix.m12;
                forward.z = matrix.m22;

                Vector3 upwards;
                upwards.x = matrix.m01;
                upwards.y = matrix.m11;
                upwards.z = matrix.m21;

                return Quaternion.LookRotation(forward, upwards);
            }
        }

        public Vector3 scale {
            get {
                Vector3 scale;

                scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
                scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
                scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;

                return scale;
            }
        }


    }
}


public class UnityTypeConverter
{
    /// <summary>
    /// Configurable matrix used for change of basis from USD to Unity and vice versa (change handedness).
    /// </summary>
    /// <remarks>
    /// Allows global configuration of the change of basis matrix, which e.g. is used to make the USD importer conform
    /// to the legacy FBX import convention in Unity, swapping the X-axis instead of the Z-axis.
    /// By default this matrix is set to change handedness by swapping the Z-axis.
    /// </remarks>
    public static UnityEngine.Matrix4x4 basisChange = UnityEngine.Matrix4x4.Scale(new UnityEngine.Vector3(1, 1, -1));
    public static UnityEngine.Matrix4x4 inverseBasisChange = UnityEngine.Matrix4x4.Scale(new UnityEngine.Vector3(1, 1, -1));

    /// <summary>
    /// Converts to and from the USD transform space.
    /// This method should be applied to all Unity matrices before being written and all USD
    /// matrices after being read, unless the USD file is stored in the Unity transform space
    /// (though doing so will result in a non-standard USD file).
    /// </summary>
    static public UnityEngine.Matrix4x4 ChangeBasis(UnityEngine.Matrix4x4 input)
    {
        // Furthermore, this could be simplified to multiplying -1 by input elements [2,6,8,9,11,14].
        return UnityTypeConverter.basisChange * input * UnityTypeConverter.inverseBasisChange;
    }
}
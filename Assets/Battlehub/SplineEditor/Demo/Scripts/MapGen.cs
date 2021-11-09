using UnityEngine;
namespace Battlehub.SplineEditor
{
    public class MapGen : MonoBehaviour
    {
        public GameObject BuildingPrefab;
        public int Rows = 10;
        public int Cols = 10;
        public float Density = 0.2f;
        public float Width = 2;
        public float Length = 2;
        public float MinHeight = 8;
        public float MaxHeight = 16;

        public void Generate()
        {
            for(int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            for(int i = 0; i < Rows; ++i)
            {
                for(int j = 0; j < Cols; ++j)
                {
                    if(Random.value < Density)
                    {
                        float x = (-Rows * Width / 2.0f) + i * Width;
                        float z = (-Cols * Length / 2.0f) + j * Length;
                        float height = MinHeight + Random.value * (MaxHeight - MinHeight);

                        GameObject building = (GameObject)Instantiate(BuildingPrefab, new Vector3(x, 0, z), Quaternion.identity);
                        building.transform.localScale = new Vector3(Width / 2, height, Length / 2);
                        building.transform.SetParent(transform, true);
                    }
                }
            }
        }
    }
}


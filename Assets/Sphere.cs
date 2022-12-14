using UnityEngine;

public class Sphere : MonoBehaviour
{
    private bool _hit;

    private Vector3[] vertices;
    private Vector3[] axes;

    Color[] colors;
    Vector3[] verts;

    public Transform Transform
    {
        get
        {
            return gameObject.transform;
        }
    }

    public bool Hit
    {
        get
        {
            return _hit;
        }
        set
        {
            if (_hit != value)
            {
                _hit = value;

                Tint();
            }
        }
    }


    public void Start()
    {


    }


    Vector3[] removeDuplicates(Vector3[] dupArray)
    {

        for (int j = 0; j < dupArray.Length; j++)
        {
            dupArray[j] = transform.TransformPoint(dupArray[j]);
        }

        Vector3[] newArray = new Vector3[8];  //change 8 to a variable dependent on shape
        bool isDup = false;
        int newArrayIndex = 0;
        for (int i = 0; i < dupArray.Length; i++)
        {
            for (int j = 0; j < newArray.Length; j++)
            {
                if (dupArray[i] == newArray[j])
                {
                    isDup = true;
                }
            }
            if (!isDup)
            {
                newArray[newArrayIndex] = dupArray[i];
                newArrayIndex++;
                isDup = false;
            }
        }
        return newArray;
    }

    public Vector3[] GetVertices()
    {
        vertices = GetComponent<MeshFilter>().mesh.vertices;
        verts = removeDuplicates(vertices);
        return verts;
    }

    public Vector3[] GetAxes()
    {
        //Unity에서 제공하는 기능 말고 rotation으로 axis를 구하는 방법은 없을까?

        Quaternion rotation = this.transform.rotation;
        Vector3 right = new Vector3(1f, 0f, 0f);
        Vector3 up = new Vector3(0f, 1f, 0f);
        Vector3 forward = new Vector3(0f, 0f, 1f);

        Vector3[] axesTest = new[]
        {
            (rotation* right ),
            (rotation* up ),
            (rotation* forward )
        };

        axes = new[]
        {
            (this.transform.right),
            (this.transform.up),
            (this.transform.forward)
        };
        return axes;

    }

    private void Tint()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Hit ? Color.red : Color.white);
    }

}
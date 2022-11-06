using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets
{
    public class CollisionUtil
    {

        public static bool CheckCollision(Cube a, Cube b, CollisionInfo collisionInfo)
        {
            Vector3[] aAxes;
            Vector3[] bAxes;
            Vector3[] AllAxes;
            Vector3[] aVertices;
            Vector3[] bVertices;

            aAxes = a.GetAxes();
            bAxes = b.GetAxes();

            Vector3 a_xx = Vector3.Cross(aAxes[0], bAxes[0]);
            Vector3 a_xy = Vector3.Cross(aAxes[0], bAxes[1]);
            Vector3 a_xz = Vector3.Cross(aAxes[0], bAxes[2]);

            Vector3 a_yx = Vector3.Cross(aAxes[1], bAxes[0]);
            Vector3 a_yy = Vector3.Cross(aAxes[1], bAxes[1]);
            Vector3 a_yz = Vector3.Cross(aAxes[1], bAxes[2]);

            Vector3 a_zx = Vector3.Cross(aAxes[2], bAxes[0]);
            Vector3 a_zy = Vector3.Cross(aAxes[2], bAxes[1]);
            Vector3 a_zz = Vector3.Cross(aAxes[2], bAxes[2]);

            AllAxes = new Vector3[]
            {
            aAxes[0],
            aAxes[1],
            aAxes[2],
            bAxes[0],
            bAxes[1],
            bAxes[2],
            a_xx,
            a_xy,
            a_xz,

            a_yx,
            a_yy,
            a_yz,

            a_zx,
            a_zy,
            a_zz
            };

            Debug.DrawRay(a.transform.position, aAxes[0] * 2f, Color.red);
            Debug.DrawRay(a.transform.position, aAxes[1] * 2f, Color.cyan);
            Debug.DrawRay(a.transform.position, aAxes[2] * 2f, Color.blue);

            Debug.DrawRay(b.transform.position, bAxes[0] * 2f, Color.red);
            Debug.DrawRay(b.transform.position, bAxes[1] * 2f, Color.cyan);
            Debug.DrawRay(b.transform.position, bAxes[2] * 2f, Color.blue);

            aVertices = a.GetVertices();
            bVertices = b.GetVertices();

            int aVertsLength = aVertices.Length;
            int bVertsLength = bVertices.Length;

            bool hasOverlap = false;

            if (ProjectionHasOverlap(AllAxes, bVertices, aVertices, collisionInfo))
            {
                collisionInfo.isFromA = true;
                hasOverlap = true;
            }
            else if (ProjectionHasOverlap(AllAxes, aVertices, bVertices, collisionInfo))
            {
                collisionInfo.isFromA = false;
                hasOverlap = true;
            }

            return hasOverlap;
        }

        public static bool CheckCollision(Sphere a, float radius, Cube b, CollisionInfo collisionInfo)
        {
            Vector3[] cubeAxes = b.GetAxes();
            Vector3[] sphereAxes = a.GetAxes();
            Vector3[] cubeVertices = b.GetVertices();

            Vector3 distanceVec = a.transform.position - b.transform.position;
            distanceVec = Vector3.Normalize(distanceVec);
            
            Vector3[] allAxes = new Vector3[]
            {
                cubeAxes[0],
                cubeAxes[1],
                cubeAxes[2],
                sphereAxes[0],
                sphereAxes[1],
                sphereAxes[2],
                distanceVec
            };

            Debug.DrawRay(a.transform.position, sphereAxes[0] * 2f, Color.red);
            Debug.DrawRay(a.transform.position, sphereAxes[1] * 2f, Color.cyan);
            Debug.DrawRay(a.transform.position, sphereAxes[2] * 2f, Color.blue);

            Debug.DrawRay(b.transform.position, cubeAxes[0] * 2f, Color.red);
            Debug.DrawRay(b.transform.position, cubeAxes[1] * 2f, Color.cyan);
            Debug.DrawRay(b.transform.position, cubeAxes[2] * 2f, Color.blue);

            bool hasOverlap = false;

          
            if (ProjectionHasOverlapSphereAndCube(a.Transform.position, b.Transform.position, allAxes, cubeVertices, radius, collisionInfo))
            {
                collisionInfo.isFromA = true;
                hasOverlap = true;
            }
            //else if (ProjectionHasOverlapSphereAndCube(b.Transform.position, a.Transform.position, allAxes, cubeVertices, radius, collisionInfo))
            //{
            //    //2번 체크할 이유가 있나?
            //    collisionInfo.isFromA = false;
            //    hasOverlap = true;
            //}

            return hasOverlap;
        }

        //TODO : Sphere와 cube 의 collision 체크
        private static bool ProjectionHasOverlapSphereAndCube(
            Vector3 spherePos,
            Vector3 cubePos,

            Vector3[] allAxes,
            Vector3[] cubeVertices,

            float radius,
            CollisionInfo collisionInfo
            )
        {
            float minOverlap = float.PositiveInfinity;
            Vector3 minAxis = new Vector3();

            for (int i = 0; i < allAxes.Length; i++)
            {

                float cubeProjMin = float.MaxValue;
                float cubeProjMax = float.MinValue;

                Vector3 axis = allAxes[i];
                Debug.DrawRay(cubePos, axis * 2f, Color.black);

                // Handles the cross product = {0,0,0} case
                if (axis == Vector3.zero)
                {
                    continue;
                };

                if (axis == new Vector3(1f, 0f, 0f))
                {
                    Debug.Log("the fuck");
                }

                for (int j = 0; j < cubeVertices.Length; j++)
                {
                    float val = FindScalarProjection((cubeVertices[j]), axis);

                    if (val < cubeProjMin)
                    {
                        cubeProjMin = val;
                    }

                    if (val > cubeProjMax)
                    {
                        cubeProjMax = val;
                    }
                }

                float sphereProj = FindScalarProjection(spherePos, axis);

                float overlap = FindOverlap(sphereProj - (radius/2), sphereProj + (radius/2), cubeProjMin, cubeProjMax);

                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    collisionInfo.depth = minOverlap;
                    collisionInfo.normal = axis;
                }

           

                if (overlap <= 0)
                {
                    return false;
                }
            }

            //디버깅을 위해 가장 가까운 axis를 찾자
            Debug.DrawRay(cubePos, minAxis * 2f, Color.black);

            return true; // A penetration has been found
        }

        /// Detects whether or not there is overlap on all separating axes.
        private static bool ProjectionHasOverlap(
            Vector3[] aAxes,
            Vector3[] bVertices,
            Vector3[] aVertices,

            CollisionInfo collisionInfo
            )
        {

            int aAxesLength = aAxes.Length;
            int aVertsLength = bVertices.Length;
            int bVertsLength = bVertices.Length;


            float minOverlap = float.PositiveInfinity;

            for (int i = 0; i < aAxesLength; i++)
            {


                float bProjMin = float.MaxValue, aProjMin = float.MaxValue;
                float bProjMax = float.MinValue, aProjMax = float.MinValue;

                Vector3 axis = aAxes[i];

                // Handles the cross product = {0,0,0} case
                if (aAxes[i] == Vector3.zero) return true;

                for (int j = 0; j < bVertsLength; j++)
                {
                    float val = FindScalarProjection((bVertices[j]), axis);

                    if (val < bProjMin)
                    {
                        bProjMin = val;
                    }

                    if (val > bProjMax)
                    {
                        bProjMax = val;
                    }
                }

                for (int j = 0; j < aVertsLength; j++)
                {
                    float val = FindScalarProjection((aVertices[j]), axis);

                    if (val < aProjMin)
                    {
                        aProjMin = val;
                    }

                    if (val > aProjMax)
                    {
                        aProjMax = val;
                    }
                }

                float overlap = FindOverlap(aProjMin, aProjMax, bProjMin, bProjMax);

                // seperating axis를 찾지 못하는 케이스라면 
                // minOverlap이 collision 발생 지점

                if (overlap < minOverlap)
                {
                    // overlap이 모두 양수라면(즉, seperating axix를 찾지 못 했다면) 가장 적게 overlap 하는 axis가 collision 발생한 axis 이다
                    minOverlap = overlap;
                    collisionInfo.depth = minOverlap;
                    collisionInfo.normal = axis;
                }

                //FindOverlap 의 리턴 값이 0이라면(즉, 두 object가 overlapping 하지 않고 seperating 하는 중이라면 두 object colliding 하는 상태가 아니다
                if (overlap <= 0)
                {
                    // Separating Axis Found Early Out, So there is no collision
                    return false;
                }
            }

            return true; // A penetration has been found
        }


        /// Calculates the scalar projection of one vector onto another, assumes normalised axes
        private static float FindScalarProjection(Vector3 point, Vector3 axis)
        {
            return Vector3.Dot(point, axis);
        }

        /// Calculates the amount of overlap of two intervals.
        private static float FindOverlap(float astart, float aend, float bstart, float bend)
        {
            if (astart < bstart)
            {
                if (aend < bstart)
                {
                    return 0f;
                }

                return aend - bstart;
            }

            if (bend < astart)
            {
                return 0f;
            }

            return bend - astart;
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;

public class SeparatingAxisTest : MonoBehaviour {

	// References
	// Getting the Right Axes to Test with
	//https://gamedev.stackexchange.com/questions/44500/how-many-and-which-axes-to-use-for-3d-obb-collision-with-sat/

	//Unity Code, that nearly worked, but registered collisions incorrectly in some cases
	//http://thegoldenmule.com/blog/2013/12/supercolliders-in-unity/

	[SerializeField]
	private Cube _playerCube;

	[SerializeField]
	private Sphere _player;

	[SerializeField]
	private Cube _testCube;

	[SerializeField]
	private Cube _cubeB;

	[SerializeField]
	private Cube[] _objectList;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		Vector3 force = new Vector3();

		//STEP 1 : 방향키 force
		float playerMoveSpeed = 0.01f;
		if (Input.GetKey("right"))
		{
			_player.transform.position += new Vector3(playerMoveSpeed, 0.0f, 0.0f);
		}

		if (Input.GetKey("left"))
		{
			_player.transform.position += new Vector3(-playerMoveSpeed, 0.0f, 0.0f);

		}


		if (Input.GetKey("up"))
		{
			_player.transform.position += new Vector3(0f, 0f, playerMoveSpeed);

		}


		if (Input.GetKey("down"))
		{
			_player.transform.position += new Vector3(0f, 0f, -playerMoveSpeed);
		}

		//중력 force 추가
		//_player.transform.position += new Vector3(0.0f, -playerMoveSpeed, 0.0f);


		//Sphere to Cube Coliision test
		CollisionInfo collisionInfo = new CollisionInfo();

		float radius = _player.GetComponent<Renderer>().bounds.extents.magnitude;
		if(CollisionUtil.CheckCollision(_player, radius, _testCube, collisionInfo))
        {
			Debug.DrawRay(_player.transform.position, collisionInfo.normal * 5f, Color.yellow);
			_player.Hit = true;
		} else
        {
			_player.Hit = false;
		}

		//new collision check
		//for (int i = 0; i < _objectList.Length; i++)
  //      {
		//	Cube gameObject = _objectList[i]; 
		//	CollisionInfo collisionInfo = new CollisionInfo();
		//	if (CollisionUtil.CheckCollision(_player, gameObject,collisionInfo))
		//	{

		//		//just for debugging
		//		Vector3 penetrationNormal = collisionInfo.normal;
		//		float penetrationDepth = collisionInfo.depth;
		//		//normal.x *= collisionDepth;
		//		//normal.y *= collisionDepth;
		//		//normal.z *= collisionDepth;

		//		//이렇게 하면 작동은 하는데 .. 이유는 아직 명확하지 않다
		//		//rotation의 방향 떄문에 그러는듯 ..?
		//		if(penetrationNormal.y < 0.0f)
  //              {
		//			penetrationNormal.x *= -1;
		//			penetrationNormal.y *= -1;
		//		}
		//		Vector3 fixPosition = penetrationNormal * penetrationDepth;
		//		Debug.DrawRay(_player.transform.position, penetrationNormal * 5f, Color.yellow);

		//		//Collision Projection Method
		//		//_player.transform.position += fixPosition;

		//		_player.Hit = gameObject.Hit = true;
		//	}
		//	else
		//	{
		//		_player.Hit = gameObject.Hit = false;
		//	}
		//}
	}

	
}
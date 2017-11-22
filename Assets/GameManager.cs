using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    private GameObject m_hCubeOriginal;
    private GameObject m_hSpherePrefab;

    private void Awake() {
        Instance = this;
        m_hSpherePrefab = Resources.Load("Sphere") as GameObject;
        m_hCubeOriginal = Resources.Load("Cube") as GameObject;
        Resources.UnloadUnusedAssets();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            CubeController hCube = GlobalFactory.GetInstance<CubeController>(m_hCubeOriginal, c => c.transform.position = Vector3.zero);
        }
        if (Input.GetMouseButtonDown(1)) {
            SphereController hSphere = GlobalFactory.GetInstance<SphereController>(m_hSpherePrefab, c => c.transform.position = Vector3.zero);
        }
    }
}

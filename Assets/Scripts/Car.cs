using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {
    private Vector3 initPos;
    // Use this for initialization
    void Start() {
        initPos = gameObject.transform.position;
    }

    public void Init() {
        gameObject.transform.position = initPos;
        EnemyMove gm = GetComponent<EnemyMove>();
        if (gm != null) {
            gm.InitializeEnemy();
        }

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) {
            pc.ResetDestination();
        }
    }
}

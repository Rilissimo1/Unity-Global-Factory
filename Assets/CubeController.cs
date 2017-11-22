using UnityEngine;
using System.Collections;
using System;

public class CubeController : MonoBehaviour, Pool.IPoolable {
    public Pool Pool { get; set; }
    private Rigidbody m_hRb;

    public void OnGet() {
        this.gameObject.SetActive(true);
    }

    public void OnRecycle() {
        m_hRb.velocity = Vector3.zero;
        this.gameObject.SetActive(false);
    }

    private void Awake () {
        m_hRb = GetComponent<Rigidbody>();
    }

	private void Update () {
	    if (this.transform.position.y < -50) {
	        Pool.Recycle(this);
	    }
	}
}

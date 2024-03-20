using UnityEngine;

public class CollisionPainter : MonoBehaviour{
    public Color paintColor;
    
    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;

    public PaintCoverageTracker tracker;
    public Team team;

    void Start()
    {
        tracker = PaintCoverageTracker.Instance;
        if (tracker == null)
        {
            Debug.LogError("PaintCoverageTracker not found in the scene.");
        }
    }

    // When the player comes in contact with the paintable floor, apply paint
    private void OnCollisionStay(Collision other)
    {
        Paintable p = other.collider.GetComponent<Paintable>();
        if (p != null)
        {
            Vector3 pos = other.contacts[0].point;
            PaintManager.instance.paint(p, pos, radius, hardness, strength, paintColor);
        }

        if (other.gameObject.CompareTag("PaintableFloor"))
        {
            PaintCoverageTracker.Instance.PaintArea(transform.position, team);
        }
    }
}

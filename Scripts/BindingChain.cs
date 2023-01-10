using System.Collections;
using System.Collections.Generic;
using UnboundLib;
using UnityEngine;

public class BindingChain : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private ParticleSystem particles;

    [SerializeField]
    private Transform rightSpear, leftSpear;

    [SerializeField]
    private float lifeTime;

    private AnchorPoint rightAnchor, leftAnchor;
    
    private Vector2 rVel, lVel;
    
    private float
        distancePerSecond = 100f,
        baseEmissionRate = 0;

    private float lifeTimeCounter = 0;

    class AnchorPoint
    {
        private Vector2 offset;
        private Transform target;

        public AnchorPoint(Vector2 position, Transform target)
        {
            this.target = target;

            if (target != null)
            {
                this.offset = position - (Vector2)target.position;
            }
            else
            {
                this.offset = position;
            }
        }

        public Vector2 GetPosition()
        {
            if (target == null) return offset;
            return (Vector2)target.position + offset;
        }
    }

    private void Start()
    {
        rightAnchor = GetAnchorPointFrom(transform.right);
        leftAnchor = GetAnchorPointFrom(-transform.right);

        baseEmissionRate = particles.emission.rateOverTime.constant;

        lineRenderer.SetPositions(new []{ transform.position, transform.position });
    }

    private void Update()
    {
        lifeTimeCounter += TimeHandler.deltaTime;

        bool retracting = lifeTimeCounter >= lifeTime;

        var fromLeftPos = retracting ? leftAnchor.GetPosition() : (Vector2)transform.position;
        var fromRightPos = retracting ? rightAnchor.GetPosition() : (Vector2)transform.position;

        var targetLeftPos = retracting ? (Vector2)transform.position : leftAnchor.GetPosition();
        var targetRightPos = retracting ? (Vector2)transform.position : rightAnchor.GetPosition();

        var leftPos = Vector2.SmoothDamp(lineRenderer.GetPosition(0), targetLeftPos, ref lVel, Vector3.Distance(targetLeftPos, fromLeftPos) / (distancePerSecond * (retracting ? 2 : 1)));
        var rightPos = Vector2.SmoothDamp(lineRenderer.GetPosition(1), targetRightPos, ref rVel, Vector3.Distance(targetRightPos, fromRightPos) / (distancePerSecond * (retracting ? 2 : 1)));

        // set line end points
        lineRenderer.SetPosition(0, leftPos);
        lineRenderer.SetPosition(1, rightPos);

        // set spear locations
        leftSpear.transform.position = leftPos;
        rightSpear.transform.position = rightPos;

        // set spear rotations
        leftSpear.transform.right = (leftPos - rightPos).normalized;
        rightSpear.transform.right = (rightPos - leftPos).normalized;

        var emission = particles.emission;
        var rateOverTime = emission.rateOverTime;

        rateOverTime.constant = baseEmissionRate * Vector3.Distance(leftPos, rightPos);

        emission.rateOverTime = rateOverTime;

        if (retracting && Vector3.Distance(leftPos, rightPos) <= 10f)
        {
            var removeAfter = gameObject.GetOrAddComponent<RemoveAfterSeconds>();
            removeAfter.shrink = true;
            removeAfter.seconds = 0;
        }
    }

    private AnchorPoint GetAnchorPointFrom(Vector3 direction)
    {
        var hit = Physics2D.Raycast(transform.position, direction, 50f, ArcanaCardsPlugin.floorMask);
        if (hit.collider == null)
        {
            return new AnchorPoint(transform.position + direction.normalized * 50f, null);
        }
        else
        {
            return new AnchorPoint(hit.point, hit.transform);
        }
    }
}

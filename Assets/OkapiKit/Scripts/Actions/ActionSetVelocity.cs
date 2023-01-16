using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ActionSetVelocity : Action
{
    [SerializeField] private enum VelocityOperation { Set, PercentageModify };

    [SerializeField] 
    private VelocityOperation   operation = VelocityOperation.Set;
    [SerializeField, ShowIf("isPercentageModify")]
    private Vector2             percentageValue = new Vector2(1.0f, 1.0f);
    [SerializeField, ShowIf("isSet")] 
    private bool                useRotation = false;
    [SerializeField, ShowIf("isSet")]
    private bool                useRandom = false;
    [SerializeField, ShowIf("isSet")]
    private float               startAngle = 0.0f;
    [SerializeField, ShowIf("isSet")]
    private float               endAngle = 360.0f;
    [SerializeField, ShowIf("isSet")]
    private Vector2             speedRange = new Vector2(100, 100);
    [SerializeField, ShowIf("isSetNotRandom")]
    private Vector2             minVelocity = new Vector2(100, 100);
    [SerializeField, ShowIf("isSetNotRandom")]
    private Vector2             maxVelocity = new Vector2(100, 100);

    private bool isSet => (operation == VelocityOperation.Set) && (useRandom);
    private bool isSetNotRandom => (operation == VelocityOperation.Set) && (!useRandom);
    private bool isPercentageModify => (operation == VelocityOperation.PercentageModify);

    public override string GetRawDescription(string ident)
    {
        if (operation == VelocityOperation.Set)
        {
            if (useRandom)
            {
                var desc = $"Select a random angle between {startAngle} and {endAngle} and set the velocity of this object towards that direction, with a magnitude between {speedRange.x} and {speedRange.y}";
                if (useRotation) desc += "; Angles are relative to the object rotation.";
                return desc;
            }
            return $"Select a random velocity between {minVelocity} and {maxVelocity} and set it to this object";
        }
        else if (operation == VelocityOperation.PercentageModify) 
        {
            return $"Changes the current velocity of this object by a percentage in the [{percentageValue.x},{percentageValue.y}] range.";
        }

        return "Unknown Operation";
    }

    public override void Execute()
    {
        if (!enableAction) return;

        XYMovement mxy = GetComponent<XYMovement>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 velocity = Vector2.zero;

        if (operation == VelocityOperation.Set)
        {
            if (useRandom)
            {
                float angle = Random.Range(Mathf.Min(startAngle, endAngle), Mathf.Max(startAngle, endAngle)) * Mathf.Deg2Rad;
                float speed = Random.Range(speedRange.x, speedRange.y);

                velocity = new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));
            }
            else
            {
                velocity = new Vector2(Random.Range(minVelocity.x, maxVelocity.y), Random.Range(minVelocity.y, maxVelocity.y));
            }

            if (useRotation)
            {
                velocity = velocity.x * transform.right + velocity.y * transform.up;
            }
        }
        else if (operation == VelocityOperation.PercentageModify)
        {
            if (mxy)
            {
                velocity = mxy.GetSpeed();
            }
            else if (rb)
            {
                velocity = rb.velocity;
            }

            float r = Random.Range(percentageValue.x, percentageValue.y);

            velocity = velocity + velocity * r;
        }

        if (mxy)
        {
            mxy.SetSpeed(velocity);
        }
        else if (rb)
        {
            rb.velocity = velocity;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static bool LineOfSight(Vector2 from, Vector2 to, LayerMask layerMask, string compareTag, float distance = Mathf.Infinity, float offsetDistance = 0f)
    {
        return LineOfSight(from, to, layerMask, distance, offsetDistance, null, compareTag);
    }
    public static bool LineOfSight(Vector2 from, GameObject target, LayerMask layerMask, float distance = Mathf.Infinity, float offsetDistance = 0f)
    {
        return LineOfSight(from, target.transform.position, layerMask, distance, offsetDistance, target);
    }
    public static bool LineOfSight(Vector2 from, Vector2 to, LayerMask layerMask, float distance = Mathf.Infinity, float offsetDistance = 0f, GameObject target = null, string compareTag = "")
    {
        Vector2 dir = to - from;
        var hit = Physics2D.Raycast(from + dir * offsetDistance, dir, distance, layerMask);
        if (hit.collider == null)
            return false;

        if (target != null && hit.collider.gameObject.GetInstanceID() != target.GetInstanceID())
            return false;
        else if (target != null && hit.collider.gameObject.GetInstanceID() == target.GetInstanceID())
            return true;

        if (target == null && !hit.collider.CompareTag(compareTag))
            return false;
        else if (target == null && hit.collider.CompareTag(compareTag))
            return true;

        throw new System.Exception("CODE SHOULD NOT REACH THIS");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuScreen
{
    protected string name;
    protected Vector2 limit;
    protected GameObject holder;
    protected Vector2 position;
    protected List<KeyCode> monitoredKeys = new List<KeyCode> { };
    public List<KeyCode> MonitoredKeys { get => monitoredKeys; }
    public bool IsActive { get { return holder.activeSelf; } }
    public bool SkipFrame { get; set; }
    public MenuScreen(GameObject holder, string name)
    {
        this.holder = holder;
        this.name = name;
    }

    protected bool WithinLimit(Vector2 pos)
    {
        return Mathf.Clamp(pos.x, 0f, limit.x) == pos.x && Mathf.Clamp(pos.y, 0f, limit.y) == pos.y;
    }

    protected abstract void ActivateMenuObject(Vector2 pos);
    protected abstract void DeactivateMenuObject(Vector2 pos);

    public virtual void ResetPosition()
    {
        MoveTo(new Vector2(0, 0));
    }
    public virtual void MoveTo(Vector2 newPos)
    {
        if (!WithinLimit(newPos))
            return;

        DeactivateMenuObject(position);
        ActivateMenuObject(newPos);
        position = newPos;
    }
    public virtual void Move(Vector2 dir)
    {
        if (!WithinLimit(position - dir))
            return;

        DeactivateMenuObject(position);
        position -= dir;
        ActivateMenuObject(position);
    }

    public virtual string Activate()
    {
        holder.SetActive(true);
        ResetPosition();
        position = Vector2.zero;
        SkipFrame = true;
        return name;
    }

    public virtual void DeActivate()
    {
        holder.SetActive(false);
    }

    public abstract void KeyPressed(KeyCode key);

    public virtual void OnGUI() { }
}

